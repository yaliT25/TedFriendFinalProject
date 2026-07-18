// פונקציה לגלילה
window.scrollToEnd = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

// --- תוספת עבור המיקרופון (Classi Audio) ---
let mediaRecorder;
let audioChunks = [];
let recordedMimeType = 'audio/webm';

window.startRecording = async () => {
    try {
        const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
        
        recordedMimeType = 'audio/webm';
        let options = {};
        
        if (typeof MediaRecorder.isTypeSupported === 'function') {
            if (MediaRecorder.isTypeSupported('audio/webm')) {
                recordedMimeType = 'audio/webm';
                options = { mimeType: 'audio/webm' };
            } else if (MediaRecorder.isTypeSupported('audio/mp4')) {
                recordedMimeType = 'audio/mp4';
                options = { mimeType: 'audio/mp4' };
            }
        }
        
        mediaRecorder = new MediaRecorder(stream, options);
        audioChunks = [];

        mediaRecorder.ondataavailable = e => {
            audioChunks.push(e.data);
        };

        mediaRecorder.start();
    } catch (err) {
        console.error("Error accessing microphone:", err);
    }
};

window.stopRecording = () => {
    return new Promise(resolve => {
        mediaRecorder.onstop = async () => {
            const actualMime = mediaRecorder.mimeType || recordedMimeType;
            const audioBlob = new Blob(audioChunks, { type: actualMime });
            const reader = new FileReader();
            reader.readAsDataURL(audioBlob);
            reader.onloadend = () => {
                // החזרת ה-Data URL המלא (כולל ה-Mime Type) ל-Blazor
                resolve(reader.result);
            };
        };
        mediaRecorder.stop();
    });
};

window.currentAudio = null;

window.playBase64Audio = (base64String) => {
    if (window.currentAudio !== null) {
        window.currentAudio.pause();
        window.currentAudio.currentTime = 0;
    }

    const audioUrl = "data:audio/mpeg;base64," + base64String;
    window.currentAudio = new Audio(audioUrl);

    window.currentAudio.onended = () => {
        window.currentAudio = null;
    };

    window.currentAudio.play().catch(error => {
        console.error("Error playing audio:", error);
        window.currentAudio = null;
    });
};