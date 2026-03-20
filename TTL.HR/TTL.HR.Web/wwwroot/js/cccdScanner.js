window.cccdScanner = {
    initWebcam: async function (id) {
        console.log("[CCCD_SCANNER-v2] Initializing webcam for element ID:", id);
        
        // Check for secure context or localhost
        if (window.location.protocol !== 'https:' && window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
            console.warn("[CCCD_SCANNER] Potentially insecure context detected. Camera may be blocked.");
        }

        if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
            let errorMsg = "Browser does not support camera access (getUserMedia). Ensure you use HTTPS or localhost.";
            console.error("[CCCD_SCANNER]", errorMsg);
            alert("Security Error: " + errorMsg);
            return false;
        }

        let v = document.getElementById(id);
        if (!v) {
            console.error("[CCCD_SCANNER] Video element not found in DOM:", id);
            return false;
        }
        
        console.log("[CCCD_SCANNER] Video element found. Display state:", v.style.display, "OffsetSize:", v.offsetWidth, "x", v.offsetHeight);
        
        const constraints = { 
            video: { 
                facingMode: { ideal: "environment" }, 
                width: { ideal: 1920 }, 
                height: { ideal: 1080 } 
            } 
        };

        try {
            console.log("[CCCD_SCANNER] Requesting user media with constraints:", constraints);
            const s = await navigator.mediaDevices.getUserMedia(constraints);
            console.log("[CCCD_SCANNER] SUCCESS: Stream received. Tracks:", s.getVideoTracks().map(t => t.label));
            
            v.srcObject = s; 
            
            // Critical for iOS and some modern browsers
            v.setAttribute('autoplay', '');
            v.setAttribute('muted', '');
            v.setAttribute('playsinline', '');
            
            try {
                await v.play();
                console.log("[CCCD_SCANNER] Video PLAYING successfully.");
            } catch (playError) {
                console.error("[CCCD_SCANNER] Play failed. Trying muted play...", playError);
                v.muted = true;
                await v.play();
                console.log("[CCCD_SCANNER] Video PLAYING after manual mute.");
            }
            return true;
        } catch (e) { 
            console.error("[CCCD_SCANNER] Primary camera failed. Error Name:", e.name, "Message:", e.message);
            
            try {
                console.log("[CCCD_SCANNER] Attempting fallback to ANY camera...");
                const s = await navigator.mediaDevices.getUserMedia({ video: true });
                console.log("[CCCD_SCANNER] Fallback SUCCESS.");
                v.srcObject = s;
                await v.play();
                return true;
            } catch (e2) {
                console.error("[CCCD_SCANNER] Hard failure. No camera accessible:", e2);
                alert("Hardware Error: Could not connect to any camera. Details: " + e2.name + " - " + e2.message);
                return false; 
            }
        }
    },
    captureFrame: function (id) {
        let v = document.getElementById(id);
        if (!v || v.readyState !== 4) return "";
        let c = document.createElement("canvas");
        c.width = v.videoWidth; c.height = v.videoHeight;
        c.getContext("2d").drawImage(v, 0, 0);
        return c.toDataURL("image/jpeg", 0.85);
    },
    getBrightness: function (id) {
        let v = document.getElementById(id);
        if (!v || v.readyState !== 4) return 127;
        let c = document.createElement("canvas");
        c.width = 100; c.height = 100;
        let ctx = c.getContext("2d");
        ctx.drawImage(v, 0, 0, 100, 100);
        let d = ctx.getImageData(0, 0, 100, 100).data;
        let s = 0;
        for (let i = 0; i < d.length; i += 4) s += (d[i] + d[i+1] + d[i+2]) / 3;
        return s / 10000;
    },
    stopWebcam: function (id) {
        let v = document.getElementById(id);
        if (v && v.srcObject) {
            v.srcObject.getTracks().forEach(t => t.stop());
            v.srcObject = null;
        }
    }
};
