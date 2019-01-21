//Add this below content to your HTML page, or add the js file to your page at the very top to register sercie worker
if (navigator && navigator.serviceWorker && navigator.serviceWorker.controller) {
    console.log('[PWA Builder] active service worker found, no need to register')
} else if (navigator && navigator.serviceWorker) {

    //Register the ServiceWorker
    navigator.serviceWorker.register('/sw.min.js', {
        scope: './'
    }).then(function (reg) {
        console.log('Service worker has been registered for scope:' + reg.scope);
    });
}
var fetched = [];
if (fetch) {
    document.querySelectorAll('a').forEach(a => {
        if (a.href.includes('http') || a.href.includes('https') || a.href.includes('mailto') || a.href.includes('#') || fetched.includes(a.href)) {
            return;
        }
        fetched.push(item.href);
        fetch(item.href);
    })
}
Promise.resolve(fetched);
var triggerLazyImages = function () {
    document.querySelectorAll('.lazy').forEach(a => {
        var src = a.getAttribute('data-src');
        if (src) {
            a.src = src;
        }
    });
}

var toggleNav = function () {
    var nav = document.getElementById('navBar');
    if (!nav) {
        return;
    }
    var hidden = nav.classList.contains('hide');
    if (hidden) {
        nav.classList.remove('hide');
        triggerLazyImages();
    }
    else {
        nav.classList.add('hide');
    }
}
function attachNavToggle(elementId) {
    var menuButton = document.getElementById(elementId);
    if (menuButton) {
        menuButton.addEventListener('click', function () {
            toggleNav();
        });
    }
}
attachNavToggle('menuBtn');
attachNavToggle('closeNav');
document.addEventListener("readystatechange", function () {
    var nav = document.getElementById('navBar');
    if (!nav) {
        return;
    }
    var computedNav = window.getComputedStyle(nav);
    if (computedNav.width && computedNav.width !== "0px") {
        triggerLazyImages();
    }
});
