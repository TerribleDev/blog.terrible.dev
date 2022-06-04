//Add this below content to your HTML page, or add the js file to your page at the very top to register sercie worker
if (navigator && navigator.serviceWorker && navigator.serviceWorker.controller) {
    console.log('[PWA Builder] active service worker found, no need to register')
} else if (navigator && navigator.serviceWorker) {

    //Register the ServiceWorker
    navigator.serviceWorker.register('/sw.min.js', {
        scope: '/'
    }).then(function (reg) {
        console.log('Service worker has been registered for scope:' + reg.scope);
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

// setTimeout(function() {
//     console.log('trigger')
//     const nav = document.querySelector('.navHero');
// nav.outerHTML = `<picture class="navHero">
// <source srcset="/content/tommyAvatar4.jpg.webp" loading="lazy" type="image/webp" alt="An image of TerribleDev" class="round" />
// <img src="/content/tommyAvatar4.jpg" loading="lazy" alt="An image of TerribleDev" class="round" />
// </picture>`
// }, 3000)

// const hero = document.querySelector('.navHero');
// hero.outerHTML = `<svg class="navHero round" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 200 200" preserveAspectRatio="none"><filter id="a" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB"><feGaussianBlur stdDeviation="20 20" edgeMode="duplicate"/><feComponentTransfer><feFuncA type="discrete" tableValues="1 1"/></feComponentTransfer></filter><image filter="url(#a)" x="0" y="0" height="100%" width="100%" xlink:href="data:image/jpeg;base64,/9j/2wBDAAYEBQYFBAYGBQYHBwYIChAKCgkJChQODwwQFxQYGBcUFhYaHSUfGhsjHBYWICwgIyYnKSopGR8tMC0oMCUoKSj/2wBDAQcHBwoIChMKChMoGhYaKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCj/wAARCAAoACgDASIAAhEBAxEB/8QAGgAAAgMBAQAAAAAAAAAAAAAAAAYDBQcECP/EAC4QAAEDAwMBBgYDAQAAAAAAAAECAwQABREGEiFBIjEyUWGBBxMUFSNSQpGxwf/EABkBAAIDAQAAAAAAAAAAAAAAAAMEAAECBf/EAB0RAAMBAAIDAQAAAAAAAAAAAAABAhEDIQQSMWH/2gAMAwEAAhEDEQA/AN2QBjmsg+LfxaTY5siw6fKTcW+w/IJH4lfqkdVDqelawl2vO7ulbfdfijqx69QkyGmpRLe8nB3c54PPFZrovjn3eCLD1XqduemZDvUxD+48/UKUCc9QSQR7V6K0Dqy7XNMWJqeG2zKks/OjSWcbHwBlSSP4rA5x3EZ8qprDpfQCHmowtEV98eEFS1DI7x4sE+h5q+n3S33J21/ZglKYE1oFKE7QlPgIHscVnQ9cOJtjavHlRUalZHrRRMFCIuYzSNe4ioN/dnpR2JOAcDgkf95ptW92SRnApP1UiRJfYkt5/FlAT+oOOT7gf3UqG1oz42qvw6JsuHGXCTEiBpYcDikhQST6gGr2K+1IKQiIhKN4WHUjG7qPfOKzdrU13iXdyPMtT70cAfKU23vJPpinrTEydcHZq5zS2nUqSEMHvbRj/T30KG6roZ5nkvoZEqymiokK49aKMc05ZDTnyFBK8qIyOlVhmJjodV9smPkcEJCBkem4iiinWPyWtslWcpQFLQlZ5SHOypPmOeoqKKWLbKmyBvdU+reEI8RAGAAPOiihRCVai7WPDqjO/Wx0vBt1oqydrqdqx6EUUUUVxL7Yu5W/D//Z"/></svg>`
