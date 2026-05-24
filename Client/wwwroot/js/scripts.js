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

window.startRecording = async () => {
    try {
        const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
        mediaRecorder = new MediaRecorder(stream);
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
            const audioBlob = new Blob(audioChunks, { type: 'audio/webm' });
            const reader = new FileReader();
            reader.readAsDataURL(audioBlob);
            reader.onloadend = () => {
                // החזרה של מחרוזת ה-Base64 ל-Blazor
                const base64String = reader.result.split(',')[1];
                resolve(base64String);
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