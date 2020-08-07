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

if(window.IntersectionObserver) {
  var lazyImages = [].slice.call(document.querySelectorAll(".lazy"));
    var lazyImageObserver = new IntersectionObserver(function(entries, observer) {
      entries.forEach(function(entry) {
        if (entry.isIntersecting) {
          var lazyImage = entry.target;
          if(lazyImage.dataset.src) {
            lazyImage.src = lazyImage.dataset.src;
          }
          if(lazyImage.dataset.srcset) {
            lazyImage.srcset = lazyImage.dataset.srcset;
          }

          lazyImage.classList.remove("lazy");
          lazyImageObserver.unobserve(lazyImage);
        }
      });
    });

    lazyImages.forEach(function(lazyImage) {
      lazyImageObserver.observe(lazyImage);
    });
} else {
    var lazyImages = [].slice.call(document.querySelectorAll(".lazy"));
    lazyImages.forEach(function(image) {
        if(image.dataset.srcset) {
          image.srcset = image.dataset.srcset;
        }
        if(image.dataset.src) {
          image.src = image.dataset.src;
        }
    });
}
