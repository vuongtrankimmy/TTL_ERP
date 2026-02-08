let scale = 1, pointX = 0, pointY = 20, isDragging = false, startX = 0, startY = 0;

window.initOrgChartPanZoom = function() {
    const orgTree = document.querySelector('.org-tree');
    const orgContainer = document.querySelector('.org-container');
    if (!orgTree || !orgContainer) return;

    function setTransform() {
        orgTree.style.transform = `translate(${pointX}px, ${pointY}px) scale(${scale})`;
    }

    orgContainer.onmousedown = (e) => {
        if (e.target.closest('.org-node')) return;
        isDragging = true;
        orgContainer.style.cursor = 'grabbing';
        startX = e.clientX - pointX;
        startY = e.clientY - pointY;
    };

    window.onmouseup = () => {
        isDragging = false;
        orgContainer.style.cursor = 'grab';
    };

    window.onmousemove = (e) => {
        if (!isDragging) return;
        pointX = e.clientX - startX;
        pointY = e.clientY - startY;
        setTransform();
    };

    orgContainer.onwheel = (e) => {
        e.preventDefault();
        const rect = orgContainer.getBoundingClientRect();
        const mouseX = e.clientX - rect.left;
        const mouseY = e.clientY - rect.top;
        const xs = (mouseX - pointX) / scale;
        const ys = (mouseY - pointY) / scale;
        const delta = -e.deltaY;
        if (delta > 0) scale *= 1.1; else scale /= 1.1;
        scale = Math.min(Math.max(0.2, scale), 3);
        pointX = mouseX - xs * scale;
        pointY = mouseY - ys * scale;
        setTransform();
    };
    
    // Initial set
    setTransform();
};

window.zoomAtCenter = function(zoomIn) {
    const orgContainer = document.querySelector('.org-container');
    if (!orgContainer) return;
    const rect = orgContainer.getBoundingClientRect();
    const centerX = rect.width / 2;
    const centerY = rect.height / 2;
    const xs = (centerX - pointX) / scale;
    const ys = (centerY - pointY) / scale;
    if (zoomIn) scale *= 1.2; else scale /= 1.2;
    scale = Math.min(3, Math.max(0.2, scale));
    pointX = centerX - xs * scale;
    pointY = centerY - ys * scale;
    
    const orgTree = document.querySelector('.org-tree');
    if (orgTree) {
        orgTree.style.transform = `translate(${pointX}px, ${pointY}px) scale(${scale})`;
    }
};

window.resetPanZoom = function() {
    scale = 1; pointX = 0; pointY = 20;
    const orgTree = document.querySelector('.org-tree');
    if (orgTree) {
        orgTree.style.transform = `translate(${pointX}px, ${pointY}px) scale(${scale})`;
    }
};

window.openEmployeeDetail = function() {
    const drawerElement = document.querySelector('#kt_drawer_employee_detail');
    if (typeof KTDrawer !== 'undefined') {
        const drawer = KTDrawer.getInstance(drawerElement);
        if (drawer) drawer.show();
    }
};
