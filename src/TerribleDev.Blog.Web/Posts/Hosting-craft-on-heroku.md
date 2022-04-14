title: Hosting Craft CMS on Heroku
date: 2022-02-24 07:19
tags:
- craftcms
- cms
- heroku
---

So, like most early startups, [Quala](https://www.quala.io) (where I currently work) bought into a Wordpress site to sell our product, probably before it really existed. Flash forward, we have customers, and we're on a path to building a platform to change the game on customer management. The Wordpress site was terrible for performance, and [core web vitals](https://web.dev/vitals/). None of us know Wordpress, and barely know any php. We had huge drive to rebrand ourselves, but to do that we needed to edit the Wordpress theme 😬 or use something else.

<!-- more -->

*tl;dr you can use this sweet [deploy to heroku button](https://github.com/oof-bar/craft-heroku) that [oof.studio](https://oof.studio/) made. Most of this post is inspired by their implementation*

## Why Craft?

I was introduced to [CraftCMS](https://craftcms.com/) 2 years ago. Back then my first instinct was *eww php*, might also still be my primary reaction 🤣. At that time, and still today, I love the headless CMS ([Contentful](https://www.contentful.com/), [Sanity](https://www.sanity.io/)) + [Gatsby](https://www.gatsbyjs.com/) strategy. However, we are a startup. For us, every dollar counts. The license for Craft is $300/year. Most of the other GraphQL CMS' we looked at were more expensive. We have a developer that's used craft, and I know some other [big brain craft people](https://www.johnlamb.me/).

## Craft + Heroku

So, Heroku is a Platform to host webapps. They have good postgres support, and we've used them in the past. Apps on Heroku need to be [12 factor apps](https://12factor.net/). Heroku has an ephemeral file system, scales horizontally, and logs stdout/stderr streams.

Craft is based on the yii php framework. You'll need to use the official `php` buildpack for craft to work, and any libraries for yii will work with Craft. When we started looking into this, I found a [deploy to heroku button](https://github.com/oof-bar/craft-heroku) that [oof.studio](https://oof.studio/) built. We had to fork this, and update it. However, since then they've updated it (almost exactly how we did), so you may want to use their deploy button to get started. I didn't have much experience with craft, so much of this writing you can attribute to me reverse engineering their configs and updating it to the newest version of craft.

## Configuring Craft

Craft configurations sit in an `app.php` file. This file will need to add redis for sessions, and cache (the cache for the cache tags). Also, using [codemix's logstream](https://github.com/codemix/yii2-streamlog), piping the stream to stdout.

```php
    'production' => [
        'components' => [
            'redis' => [
                'class' => yii\redis\Connection::class,
                'hostname' => parse_url(App::env('REDIS_URL'), PHP_URL_HOST),
                'port' => parse_url(App::env('REDIS_URL'), PHP_URL_PORT),
                'password' => parse_url(App::env('REDIS_URL'), PHP_URL_PASS)
            ],
            'session' => [
                'class' => yii\redis\Session::class,
                'as session' => [
                    'class' => \craft\behaviors\SessionBehavior::class
                ]
            ],
            'cache' => [
                'class' => yii\redis\Cache::class,
                'defaultDuration' => 86400
            ],
            'log' => [
                'targets' => [
                    [
                        'class' => codemix\streamlog\Target::class,
                        'url' => 'php://stderr',
                        'levels' => ['error', 'warning'],
                        'logVars' => []
                    ]
                ]
            ]
        ]
    ]
```

There is also a file to set the db configuration in `db.php`. That must have the following, which will use heroku's `DATABASE_URL` environment variable in prod, and [nitro's](https://craftcms.com/docs/nitro/2.x/) set of environment variables locally. You'll need a `bootstrap.php` file to setup the environment properly (including license keys).

```php

<?php
define('CRAFT_BASE_PATH', __DIR__);
define('CRAFT_VENDOR_PATH', CRAFT_BASE_PATH . '/vendor');

require_once CRAFT_VENDOR_PATH . '/autoload.php';

// Load dotenv?
if (class_exists('Dotenv\Dotenv')) {
    Dotenv\Dotenv::createUnsafeImmutable(CRAFT_BASE_PATH)->safeLoad();
}

define('CRAFT_ENVIRONMENT', getenv('ENVIRONMENT') ?: 'production');
define('CRAFT_LICENSE_KEY', getenv('CRAFT_LICENSE_KEY'));
define('CRAFT_STORAGE_PATH', getenv('CRAFT_STORAGE_PATH') ?: '../storage');
define('CRAFT_STREAM_LOG', true);
```

## S3

In our case, the button didn't provide any support for uploaded files. We went for S3. I added the Craft s3 plugin, and configured it to read the apikeys, and bucket names from environment variables. Then I registered those variables in the environment variables in heroku.

## Other important files

Heroku requires a Procfile to launch apps.

```shell
web: vendor/bin/heroku-php-nginx -C nginx_app.conf web
worker: ./craft queue/listen --verbose
release: ./bin/release.sh
```
`release.sh` will run a db migration
```shell
if /usr/bin/env php /app/craft install/check
then
    /usr/bin/env php /app/craft up --interactive=0
fi
```

A `nginx_app.conf` nginx config for heroku's php buildpack.

```nginx
if ($http_x_forwarded_proto != "https") {
  return 301 https://$host$request_uri;
}

if ($host ~ ^www\.(.+)) {
  return 301 https://$1$request_uri;
}

location / {
  # try to serve file directly, fallback to rewrite
  try_files $uri @rewriteapp;
}

location @rewriteapp {
  # rewrite all to index.php
  rewrite ^(.*)$ /index.php?p=$1 last;
}

location ~ ^/(index)\.php(/|$) {
  fastcgi_pass heroku-fcgi;
  fastcgi_split_path_info ^(.+\.php)(/.*)$;
  include fastcgi_params;
  fastcgi_param SCRIPT_FILENAME $document_root$fastcgi_script_name;
  fastcgi_param HTTPS on;
}

# Global Config
client_max_body_size 20M;
```

## Anything else?

Nope, not really. You need to be aware that you need to treat craft's configuration as entirely immutable. Any changes to configuration such as plugins, twig templates, etc. Will need to be changed in dev and pushed to Heroku. Nothing can be mutated in production, other than the authoring of the site. Even file uploads! 