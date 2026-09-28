// service-worker.js - Кэширование статики и офлайн-режим PWA

// Имя кэша (меняется при обновлении — тогда старый кэш удаляется)
const CACHE_NAME = 'luxdust-v1';

// Файлы, которые кэшируются при установке Service Worker
const urlsToCache = [
    '/',
    '/css/site.css',
    '/css/pages.css',
    '/css/home.css',
    '/js/quiz.js',
    '/js/site.js',
    '/manifest.json'
];

// Установка Service Worker: открываю кэш и складываю туда все файлы
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => cache.addAll(urlsToCache))
    );
    // Активирую нового Service Worker сразу, не дожидаясь закрытия вкладок
    self.skipWaiting();
});

// Удаляю старые кэши (если версия CACHE_NAME изменилась)
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.filter(name => name !== CACHE_NAME).map(name => caches.delete(name))
            );
        })
    );
    // Беру под контроль все открытые вкладки
    self.clients.claim();
});

// Перехват запросов: сначала кэш, потом сеть
self.addEventListener('fetch', event => {
    // Кэширую только GET-запросы
    if (event.request.method !== 'GET') return;

    // Кэширую только запросы с моего домена
    if (!event.request.url.startsWith(self.location.origin)) return;

    event.respondWith(
        caches.match(event.request).then(response => {
            // Если файл есть в кэше — отдаю его, если нет — иду в сеть, если сеть недоступна — отдаю главную страницу из кэша
            return response || fetch(event.request).catch(() => caches.match('/'));
        })
    );
});