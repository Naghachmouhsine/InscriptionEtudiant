// sidebar.js - small native JS to toggle admin sidebar
document.addEventListener('DOMContentLoaded', function () {
    var toggle = document.getElementById('sidebarToggleBtn');
    var closeBtn = document.getElementById('sidebarCloseBtn');
    var sidebar = document.getElementById('adminSidebar');

    function toggleSidebar() {
        if (!sidebar) return;
        sidebar.classList.toggle('collapsed');
    }

    if (toggle) {
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            toggleSidebar();
        });
    }

    if (closeBtn) {
        closeBtn.addEventListener('click', function (e) {
            e.preventDefault();
            toggleSidebar();
        });
    }

    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function (e) {
        if (!sidebar) return;
        if (window.innerWidth <= 767.98 && !sidebar.classList.contains('collapsed')) {
            var target = e.target;
            if (!sidebar.contains(target) && !target.closest('#sidebarToggleBtn')) {
                sidebar.classList.add('collapsed');
            }
        }
    });

    // Handle submenu chevron rotation using Bootstrap collapse events
    try {
        var collapses = sidebar ? sidebar.querySelectorAll('.collapse') : [];
        collapses.forEach(function (c) {
            c.addEventListener('shown.bs.collapse', function (ev) {
                var parent = ev.target.closest('.nav-item');
                if (!parent) return;
                var toggle = parent.querySelector('.submenu-toggle .bi-chevron-down');
                if (toggle) toggle.style.transform = 'rotate(180deg)';
            });

            c.addEventListener('hidden.bs.collapse', function (ev) {
                var parent = ev.target.closest('.nav-item');
                if (!parent) return;
                var toggle = parent.querySelector('.submenu-toggle .bi-chevron-down');
                if (toggle) toggle.style.transform = '';
            });
        });
    } catch (e) {
        // silent fallback if Bootstrap events aren't available
    }
});
