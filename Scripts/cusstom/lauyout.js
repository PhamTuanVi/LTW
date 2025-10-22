document.addEventListener('DOMContentLoaded', () => {

    const body = document.body;
    const themeToggle = document.getElementById('themeToggle');
    const mobileMenuToggle = document.getElementById('mobileMenuToggle');
    const headerNav = document.querySelector('.header-nav');
    const mobileOverlay = document.getElementById('mobileOverlay');
    const avatarToggle = document.getElementById('user-avatar-toggle');
    const userMenu = document.getElementById('user-menu');
    const searchInput = document.querySelector('.search-box input');
    const searchBtn = document.querySelector('.search-btn');

   
    if (themeToggle) {
        const themeIcon = themeToggle.querySelector('i');

        const updateThemeIcon = (theme) => {
            if (!themeIcon) return;
            themeIcon.className = 'fas'; 
            switch (theme) {
                case 'light':
                    themeIcon.classList.add('fa-sun');
                    break;
                case 'blue':
                    themeIcon.classList.add('fa-droplet');
                    break;
                default:
                    themeIcon.classList.add('fa-moon');
                    break;
            }
        };

        const savedTheme = localStorage.getItem('theme') || 'dark';
        body.setAttribute('data-theme', savedTheme);
        updateThemeIcon(savedTheme);

        themeToggle.addEventListener('click', () => {
            const themes = ['dark', 'light', 'blue'];
            const currentTheme = body.getAttribute('data-theme');
            const nextTheme = themes[(themes.indexOf(currentTheme) + 1) % themes.length];

            body.setAttribute('data-theme', nextTheme);
            localStorage.setItem('theme', nextTheme);
            updateThemeIcon(nextTheme);
        });
    }

    const closeMobileMenu = () => {
        if (!headerNav || !mobileOverlay || !mobileMenuToggle) return;
        headerNav.classList.remove('active');
        mobileOverlay.classList.remove('active');
        const icon = mobileMenuToggle.querySelector('i');
        if (icon) {
            icon.classList.remove('fa-times');
            icon.classList.add('fa-bars');
        }
    };

    if (mobileMenuToggle) {
        mobileMenuToggle.addEventListener('click', () => {
            headerNav.classList.toggle('active');
            mobileOverlay.classList.toggle('active');
            const icon = mobileMenuToggle.querySelector('i');
            if (icon) {
                icon.classList.toggle('fa-bars');
                icon.classList.toggle('fa-times');
            }
        });
    }

    if (mobileOverlay) {
        mobileOverlay.addEventListener('click', closeMobileMenu);
    }


    const allNavLinks = document.querySelectorAll('.header-nav .nav-link');
    allNavLinks.forEach(link => {
        link.addEventListener('click', () => {
            if (window.innerWidth <= 768) {
                closeMobileMenu();
            }
        });
    });

  
    if (avatarToggle && userMenu) {
        avatarToggle.addEventListener('click', (e) => {
            e.stopPropagation();
            userMenu.classList.toggle('active');
        });

        document.addEventListener('click', (e) => {
            if (!userMenu.contains(e.target) && !avatarToggle.contains(e.target)) {
                userMenu.classList.remove('active');
            }
        });
    }

   
    const triggerSearch = () => {
        const query = searchInput.value.trim();
        if (query) {
            console.log('Searching for:', query);
           
        }
    };

    if (searchBtn) {
        searchBtn.addEventListener('click', triggerSearch);
    }
    if (searchInput) {
        searchInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                triggerSearch();
            }
        });
    }

   
    const handleResize = () => {
        if (window.innerWidth > 768) {
            closeMobileMenu();
            if (userMenu) userMenu.classList.remove('active');
        }
    };

    window.addEventListener('resize', handleResize);

    console.log('Layout initialized successfully');

});
