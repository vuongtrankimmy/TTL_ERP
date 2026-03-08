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
    },
    getCoordinatesFromAddress: async function (address) {
        if (!address) return null;
        try {
            const response = await fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}&limit=1`, {
                headers: {
                    'Accept-Language': 'vi-VN,vi;q=0.9,en-US;q=0.8,en;q=0.7'
                }
            });
            const data = await response.json();
            if (data && data.length > 0) {
                return {
                    lat: parseFloat(data[0].lat),
                    lon: parseFloat(data[0].lon)
                };
            }
        } catch (error) {
            console.error("Geocoding error:", error);
        }
        return null;
    },
    downloadFile: function (fileName, contentType, content) {
        const blazorStream = new Uint8Array(content);
        const file = new File([blazorStream], fileName, { type: contentType });
        const exportUrl = URL.createObjectURL(file);
        const a = document.createElement("a");
        document.body.appendChild(a);
        a.href = exportUrl;
        a.download = fileName;
        a.target = "_self";
        a.click();
        URL.revokeObjectURL(exportUrl);
        document.body.removeChild(a);
    },
    scrollToElement: function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.focus();
            element.scrollIntoView({ behavior: 'smooth', block: 'center' });
            
            // For TTLInput which uses custom layout, we might need to highlight the parent fv-row
            const container = element.closest('.fv-row') || element.closest('.row');
            if (container) {
                container.classList.add('pulse', 'pulse-danger');
                setTimeout(() => container.classList.remove('pulse', 'pulse-danger'), 3000);
            }
        }
    }
};
