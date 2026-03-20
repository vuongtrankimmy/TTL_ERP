window.LayoutHelper = {
    startWebcam: async (elementId) => {
        try {
            const video = document.getElementById(elementId);
            if (!video) return false;
            
            // Try environment first, but fallback to any camera if it fails
            const constraints = {
                video: {
                    facingMode: 'environment', // Try back camera
                    width: { ideal: 1920 },
                    height: { ideal: 1080 }
                }
            };
            
            try {
                const stream = await navigator.mediaDevices.getUserMedia(constraints);
                video.srcObject = stream;
                video.play();
                return true;
            } catch (innerErr) {
                console.warn("Back camera fail, trying default camera...", innerErr);
                const stream = await navigator.mediaDevices.getUserMedia({ video: true });
                video.srcObject = stream;
                video.play();
                return true;
            }
        } catch (err) {
            console.error("Webcam Error:", err);
            return false;
        }
    },
    stopWebcam: async () => {
        try {
            const videos = document.querySelectorAll('video');
            videos.forEach(video => {
                if (video.srcObject) {
                    video.srcObject.getTracks().forEach(track => track.stop());
                    video.srcObject = null;
                }
            });
            return true;
        } catch (err) { return false; }
    },
    captureSnapshot: async (elementId) => {
        try {
            const video = document.getElementById(elementId);
            if (!video) return null;
            const canvas = document.createElement('canvas');
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            canvas.getContext('2d').drawImage(video, 0, 0);
            return canvas.toDataURL('image/jpeg', 0.9);
        } catch (err) { return null; }
    },
    checkImageQuality: async (elementId) => {
        try {
            const video = document.getElementById(elementId);
            if (!video) return { Brightness: 0, Rating: "ERROR" };
            
            const canvas = document.createElement('canvas');
            canvas.width = 100; // Small sample is enough
            canvas.height = 100;
            const ctx = canvas.getContext('2d');
            ctx.drawImage(video, 0, 0, 100, 100);
            
            const imageData = ctx.getImageData(0, 0, 100, 100);
            const data = imageData.data;
            let total = 0;
            for (let i = 0; i < data.length; i += 4) {
                total += (data[i] + data[i+1] + data[i+2]) / 3;
            }
            const avg = total / (data.length / 4);
            
            let rating = "GOOD";
            if (avg < 40) rating = "DARK";
            if (avg > 210) rating = "BRIGHT";
            
            return { Brightness: Math.round(avg), Rating: rating };
        } catch { return { Brightness: 0, Rating: "ERROR" }; }
    },
    toggleTheme: () => {
        const themeMode = document.documentElement.getAttribute("data-bs-theme") === "dark" ? "light" : "dark";
        document.documentElement.setAttribute("data-bs-theme", themeMode);
        localStorage.setItem("data-bs-theme", themeMode);
    },
    toggleSidebar: () => {
        const sidebar = document.body.getAttribute('data-kt-app-sidebar-minimize');
        const newState = sidebar === 'on' ? 'off' : 'on';
        document.body.setAttribute('data-kt-app-sidebar-minimize', newState);
        localStorage.setItem('kt_app_sidebar_minimize', newState);
    },
    initSidebar: () => {
        const themeMode = localStorage.getItem("data-bs-theme") || "light";
        document.documentElement.setAttribute("data-bs-theme", themeMode);
        
        const savedState = localStorage.getItem('kt_app_sidebar_minimize');
        if (savedState) {
            document.body.setAttribute('data-kt-app-sidebar-minimize', savedState);
        }
    }
};

window.OcrHelper = {
    recognize: async (imageBytes) => {
        try {
            // Wait for OpenCV if needed
            if (typeof cv === 'undefined' || !cv.Mat) {
                await new Promise(resolve => setTimeout(resolve, 500));
            }

            const blob = new Blob([new Uint8Array(imageBytes)], { type: 'image/jpeg' });
            const imageUrl = URL.createObjectURL(blob);
            
            const img = await new Promise((resolve) => {
                const i = new Image();
                i.onload = () => resolve(i);
                i.src = imageUrl;
            });

            const procCanvas = document.createElement('canvas');
            const ctx = procCanvas.getContext('2d');
            procCanvas.width = img.width;
            procCanvas.height = img.height;
            ctx.drawImage(img, 0, 0);

            let wasOptimized = false;
            if (typeof cv !== 'undefined' && cv.imread) {
                try {
                    console.log("AI CV Optimizer: Start");
                    let src = cv.imread(procCanvas);
                    let dst = new cv.Mat();
                    cv.cvtColor(src, src, cv.COLOR_RGBA2GRAY, 0);
                    cv.adaptiveThreshold(src, dst, 255, cv.ADAPTIVE_THRESH_GAUSSIAN_C, cv.THRESH_BINARY, 11, 2);
                    cv.imshow(procCanvas, dst);
                    src.delete(); dst.delete();
                    wasOptimized = true;
                    console.log("AI CV Optimizer: Success");
                } catch (e) { console.warn("CV Optimization error:", e); }
            }
            
            const processedUrl = wasOptimized ? procCanvas.toDataURL('image/png') : imageUrl;
            const result = await Tesseract.recognize(processedUrl, 'vie+eng', {
                logger: m => console.log("AI Engine:", m.status, Math.round(m.progress * 100) + "%")
            });
            
            return {
                Success: true,
                Text: result.data.text,
                Confidence: result.data.confidence
            };
        } catch (error) {
            console.error("OCR Error:", error);
            return { Success: false, Error: error.message };
        }
    }
};
