const CACHE_NAME = 'bms-bridge-cache-v1';
const urlsToCache = [
    '/',
    '/static/style.css',
    '/static/js/app.js',
    '/libs/pdfjs/build/pdf.mjs',
    '/libs/pdfjs/build/pdf.worker.mjs'
];

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                console.log('Service Worker: Caching app shell');
                return cache.addAll(urlsToCache);
            })
    );
});

self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                if (response) {
                    return response;
                }

                return fetch(event.request);
            })
    );
});