window.LayoutHelper = {
    toggleSidebar: function () {
        console.log("Toggle Sidebar clicked");
        const body = document.body;
        const attributeName = 'data-kt-app-sidebar-minimize';
        const isMinimized = body.getAttribute(attributeName) === 'on';

        if (isMinimized) {
            body.setAttribute(attributeName, 'off');
            localStorage.setItem('kt_app_sidebar_minimize', 'off');
        } else {
            body.setAttribute(attributeName, 'on');
            localStorage.setItem('kt_app_sidebar_minimize', 'on');
        }
    },
    initSidebar: function () {
        const savedState = localStorage.getItem('kt_app_sidebar_minimize');
        if (savedState) {
            document.body.setAttribute('data-kt-app-sidebar-minimize', savedState);
        }
    }
};
