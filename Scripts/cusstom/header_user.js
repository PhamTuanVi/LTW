
document.addEventListener('DOMContentLoaded', () => {
   
    const cartToggle = document.getElementById('cart-toggle');
    const cartMenu = document.getElementById('cart-menu');

    if (cartToggle && cartMenu) {
        cartToggle.addEventListener('click', (e) => {
            e.stopPropagation();
            cartMenu.classList.toggle('active');
            
            closeMenu('user-menu');
            closeMenu('notification-menu');
        });
    }

    
    document.addEventListener('click', (e) => {
       
        if (!e.target.closest('.cart-container')) {
            closeMenu('cart-menu');
        }
    });

    
    window.addEventListener('resize', () => {
        
        closeMenu('cart-menu');
    });

   
});



function closeMenu(menuId) {
    const menu = document.getElementById(menuId);
    if (menu) {
        menu.classList.remove('active');
    }
}

