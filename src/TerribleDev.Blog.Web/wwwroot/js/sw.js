//This is the service worker with the combined offline experience (Offline page + Offline copy of pages)

//Install stage sets up the offline page in the cache and opens a new cache

self.addEventListener('install', function (event) {
    setTimeout(function() {
        event.waitUntil(preLoad());
    }, 5000);
});

var preLoad = function () {
    return caches.open('pwabuilder-offline').then(function (cache) {
        return cache.addAll(['/offline/', '/', '/404.html', '/index.html']);
    });
}

self.addEventListener('fetch', function (event) {
    event.respondWith(checkResponse(event.request).catch(function () {
        return returnFromCache(event.request)
    }
    ));
    event.waitUntil(addToCache(event.request));
});

var checkResponse = function (request) {
    return new Promise(function (fulfill, reject) {
        fetch(request).then(function (response) {
            if (response.status !== 404) {
                fulfill(response)
            } else {
                reject()
            }
        }, reject)
    });
};

var addToCache = function (request) {
    return caches.open('pwabuilder-offline').then(function (cache) {
        return fetch(request).then(function (response) {
            return cache.put(request, response);
        });
    });
};

var returnFromCache = function (request) {
    return caches.open('pwabuilder-offline').then(function (cache) {
        return cache.match(request, {
            ignoreSearch: true
        }).then(function (matching) {
            if (!matching) {
                return cache.match('/offline/')
            } else if (matching.status == 404) {
                return cache.match('/404.html');
            } else {
                return matching
            }
        });
    });
};