title: Speeding up CraftCMS on Heroku
date: 2022-04-13 07:55
tags:
- nginx
- craftcms
- craft
---

So, I previously [blogged about how we hosted CraftCMS](/Hosting-craft-on-heroku/) on Heroku. When we built the marketing site for [Quala](https://www.quala.io) the twig templates were built for maximum authoring flexibility at the cost of some TTFB problems. We knew this going into the project. In an ideal world we would use GatsbyJS to build the frontend, but we very limited in time. When we went live, we saw a dramatic improvement to First Contentful paint, but a huge decrease to Time To First Byte, averaging at 1.3 seconds.

<!-- more -->

The initial thinking that was bounced around was *we just need caching* as our previous wordpress site had cached all renderings in memory. However, we wanted to start rendering CSRF tokens to the browser, and collecting form data. Furthermore, I struggled to come to terms with this being a solution. Simply put, I'm not a fan of PHP, and I know that the Yii framework is known to be slow even in the PHP community, but I couldn't believe that it should be *that* slow. We did sprinkle some cache tags around our twig templates, and it did improve things, but not enough to brag about. So I started digging into the docs of Heroku, Nginx, and FastCGI.

## Heroku's buildpack

So [Heroku's buildpack docs](https://devcenter.heroku.com/articles/php-support#php-fpm-configuration) contains a lot of very good information. Props to them for docs! I ran into this one quote.

> PHP-FPM is set up to automatically spawn a suitable number of worker processes depending on dyno size and the configured PHP memory_limit

This made me go look at another article I found by them regarding [php concurrency](https://devcenter.heroku.com/articles/php-concurrency). This article boils down to, different dynos have different memory limits. They allocate 128mb to a php process and divide that by the total memory on a machine and that is used to determine how many threads to have on a single dyno. They also look for a `.user.ini` file if you want to override the memory defaults. So first I realized our `.user.ini` file had specified `memory_limit = 256M` which was causing us to have half as many processes per dyno, so I set this back to 128. Ok great, this did improve things a little. I then read that you could override the concurrency default behavior by setting the environment variable `WEB_CONCURRENCY` to be whatever you wanted. This did come with a warning.

> When setting WEB_CONCURRENCY manually, make sure that its value multiplied by your memory_limit does not exceed the amount of RAM available on your dyno type.

Now I started doing some load testing of my own, and while it would over subscribe the dyno, I gave us 10 on a 2x dyno which theoretically would cause us to OOM but with some basic load testing didnt seem like it would happen. This gave us some boost, but not as much as we hoped. I was still very stuck, and I had a suspicion that maybe there was some problem between PHP and Nginx which was slowing things down. I used the craft diagnostic tools, and I couldn't find more than 400ms being wasted in sql queries which didn't account for the almost 1 second page load I still had.

## Nginx configs

Ok, so I started looking around, and I found a [TON of great CraftCMS content by nystudio107](https://nystudio107.com/). I don't quite remember which article, but I stumbled across several that implied I needed better fastcgi settings in Nginx. So, I [forked the heroku buildpack](https://github.com/qualaio/heroku-buildpack-php) and got to work. I ended up making these settings.

```nginx
fastcgi_buffers 256 16k;
fastcgi_buffer_size 128k;
fastcgi_connect_timeout 10s;
fastcgi_send_timeout 120s;
fastcgi_read_timeout 120s;
fastcgi_busy_buffers_size 256k;
fastcgi_temp_file_write_size 256k;
reset_timedout_connection on;
```

## Brotli

While I was in the config, I decided *what the hell, lets get brolti working*. [Brotli](https://github.com/google/brotli) is a compression format that is more compact than gzip. Over the wire assets are usually 5-10% smaller than gzipped. So, sending brotli if the browser supports it, is a big win. Turns out there is an [issue filed in 2019 with heroku](https://github.com/heroku/heroku-buildpack-php/issues/356) to add it, but its not gone anywhere. Ultimately, I found someone else [figured out how to add it](https://github.com/seyallin/heroku-brotli-nginx). I made some changes and added it to our fork. You can view all of our changes in [github's compare view](https://github.com/heroku/heroku-buildpack-php/compare/main...qualaio:main#diff-ff7b43f722c67a80d4c82bf656918b3bf96f553a5ad1f62ef185dff16582f033R24-R31).

## Results

So the results was a **huge** drop in TTFB, which overall improved our ligthhouse score by 30 points. The other thing that's great is, we're moderately fast without caches, which means caches can only improve the situation further. 

![A graph showing a drop in response time from over 1 second to less than one](1.png)
