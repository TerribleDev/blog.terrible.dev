//Add this below content to your HTML page, or add the js file to your page at the very top to register sercie worker
if (navigator.serviceWorker.controller) {
    console.log('[PWA Builder] active service worker found, no need to register')
} else {

    //Register the ServiceWorker
    navigator.serviceWorker.register('/sw.min.js', {
        scope: './'
    }).then(function(reg) {
        console.log('Service worker has been registered for scope:'+ reg.scope);
    });
}
var fetched = [];
if(fetch){
    document.querySelectorAll('a').forEach( a=> {
        if(a.href.includes('http') || a.href.includes('https') || a.href.includes('mailto') || a.href.includes('#') || fetched.includes(a.href)) {
            return;
        }
        fetched.push(item.href);
        fetch(item.href);
    })
}
Promise.resolve(fetched);
