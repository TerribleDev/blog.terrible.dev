//Add this below content to your HTML page, or add the js file to your page at the very top to register sercie worker
if (navigator && navigator.serviceWorker && navigator.serviceWorker.controller) {
} else if (navigator && navigator.serviceWorker) {

    //Register the ServiceWorker
    navigator.serviceWorker.register('/sw.min.js', {
        scope: '/'
    }).then(() => {
        console.log('SW');
    });
}

function toggleNav () {
    var nav = document.getElementById('navBar');
    if (!nav) {
        return;
    }
    var hidden = nav.classList.contains('hide');
    if (hidden) {
        nav.classList.remove('hide');
    }
    else {
        nav.classList.add('hide');
    }
}