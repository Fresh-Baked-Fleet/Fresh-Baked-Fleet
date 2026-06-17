// Added this in week 5 for slide show 
// Initialize Bootstrap carousels
function initializeCarousels() {
    const carouselElements = document.querySelectorAll('[id$="Carousel"]');
    carouselElements.forEach(carouselEl => {
        const carousel = new bootstrap.Carousel(carouselEl, {
            interval: 3000,
            ride: 'carousel'
        });
    });
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    initializeCarousels();
});

// Reinitialize after Blazor updates
document.addEventListener('blazor:navigated', function() {
    initializeCarousels();
});

const cartStorageKey = 'freshBakedFleetCart';

window.orderCart = {
    get: () => {
        try {
            return JSON.parse(localStorage.getItem(cartStorageKey) || '[]');
        } catch {
            return [];
        }
    },
    set: (items) => {
        localStorage.setItem(cartStorageKey, JSON.stringify(items ?? []));
    },
    clear: () => {
        localStorage.removeItem(cartStorageKey);
    }
};
