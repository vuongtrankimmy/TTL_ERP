/**
 * Helper for Tesseract.js OCR in Blazor WASM
 * True Local OCR - Runs in the Browser (Offline)
 */
window.OcrHelper = {
    recognize: async (imageBuffer, language = 'vie') => {
        console.log('--- [AI LOCAL] Starting Tesseract.js Recognition ---');
        console.log('Language:', language);
        
        try {
            // Load Tesseract (Inject Script if not present)
            if (!window.Tesseract) {
                console.log('Loading Tesseract.js script...');
                await new Promise((resolve, reject) => {
                    const script = document.createElement('script');
                    script.src = 'https://cdn.jsdelivr.net/npm/tesseract.js@5/dist/tesseract.min.js';
                    script.onload = resolve;
                    script.onerror = reject;
                    document.head.appendChild(script);
                });
            }

            // Create Worker
            const worker = await Tesseract.createWorker(language, 1, {
                logger: m => console.log('[AI PROGRESS]', (m.progress * 100).toFixed(2) + '%', m.status),
                workerBlobURL: false // Ensure it works on some local setups
            });

            // Process Image (imageBuffer is Uint8Array from Blazor)
            const blob = new Blob([imageBuffer], { type: 'image/jpeg' });
            const url = URL.createObjectURL(blob);
            
            console.log('Running AI OCR analysis...');
            const { data: { text, confidence } } = await worker.recognize(url);
            
            console.log('--- [AI RESULT] Recognition Complete ---');
            console.log('Text Length:', text.length);
            console.log('Confidence Score:', confidence.toFixed(2) + '%');
            
            await worker.terminate();
            URL.revokeObjectURL(url);
            
            return {
                text: text,
                confidence: confidence,
                success: true
            };
        } catch (error) {
            console.error('[AI ERROR] Local OCR Failed:', error);
            return {
                success: false,
                error: error.message
            };
        }
    }
};
