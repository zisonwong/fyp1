<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VideoCall.aspx.cs" Inherits="fyp1.Client.VideoCall" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet" />
    <style>
        .speech-detected {
            border: 4px solid green;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 255, 0, 0.8);
        }
    </style>
</head>
<body class="bg-gray-100">
    <form id="videoCallForm" runat="server">
        <div class="flex flex-col h-screen items-center justify-center">
            <div class="bg-white p-3 rounded-lg shadow-lg max-w-lg text-center">
                <h1 class="text-2xl font-bold mb-4">Video Call</h1>
                    <p>You</p>
                    <video id="localVideo" autoplay="autoplay" muted="muted" class="border rounded-lg mb-4 w-full"></video>
                    <p>Doctor</p>
                    <video id="remoteVideo" autoplay="autoplay" class="border rounded-lg w-full mb-4"></video>

                    <button id="btnStartCall" class="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 mr-2">Start Call</button>
                    <button id="btnEndCall" class="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600 mr-2">End Call</button>
                    <button id="btnMuteAudio" class="bg-yellow-500 text-white px-4 py-2 rounded hover:bg-yellow-600 mr-2">Mute Audio</button>
                    <button id="btnToggleVideo" class="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 mr-2">Disable Video</button>
            </div>
        </div>
    </form>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/peerjs/1.3.2/peerjs.min.js"></script>
    <script>
        const localVideo = document.getElementById('localVideo');
        const remoteVideo = document.getElementById('remoteVideo');
        const startCallButton = document.getElementById('btnStartCall');
        const endCallButton = document.getElementById('btnEndCall');
        const muteAudioButton = document.getElementById('btnMuteAudio');
        const toggleVideoButton = document.getElementById('btnToggleVideo');

        let localStream;
        let peer;
        let isAudioMuted = false;
        let isVideoDisabled = false;

        // Speech Detection Variables
        let audioContext;
        let analyser;
        let dataArray;

        startCallButton.addEventListener('click', async () => {
            console.log("Start Call button clicked.");
            try {
                console.log("Requesting local media...");
                localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
                console.log("Local media stream obtained.");
                localVideo.srcObject = localStream;

                console.log("Initializing Web Audio API...");
                audioContext = new (window.AudioContext || window.webkitAudioContext)();
                const audioSource = audioContext.createMediaStreamSource(localStream);
                analyser = audioContext.createAnalyser();
                analyser.fftSize = 512;
                dataArray = new Uint8Array(analyser.frequencyBinCount);
                audioSource.connect(analyser);

                console.log("Speech detection started.");
                detectSpeech(localVideo);

                console.log("Initializing PeerJS...");
                peer = new Peer();
                peer.on('call', (call) => {
                    console.log("Incoming call received.");
                    call.answer(localStream);
                    call.on('stream', (remoteStream) => {
                        console.log("Remote stream received.");
                        remoteVideo.srcObject = remoteStream;
                        detectSpeech(remoteVideo);
                    });
                });
            } catch (error) {
                console.error("Error accessing media devices:", error);
            }
        });


        muteAudioButton.addEventListener('click', () => {
            if (localStream) {
                const audioTracks = localStream.getAudioTracks();
                isAudioMuted = !isAudioMuted;
                audioTracks.forEach(track => track.enabled = !isAudioMuted);
                muteAudioButton.textContent = isAudioMuted ? 'Unmute Audio' : 'Mute Audio';
            }
        });

        toggleVideoButton.addEventListener('click', () => {
            if (localStream) {
                const videoTracks = localStream.getVideoTracks();
                isVideoDisabled = !isVideoDisabled;
                videoTracks.forEach(track => track.enabled = !isVideoDisabled);
                toggleVideoButton.textContent = isVideoDisabled ? 'Enable Video' : 'Disable Video';
            }
        });

        endCallButton.addEventListener('click', () => {
            if (peer) peer.disconnect();
            if (localStream) localStream.getTracks().forEach(track => track.stop());
            localVideo.srcObject = null;
            remoteVideo.srcObject = null;

            // Stop audio context
            if (audioContext) audioContext.close();
        });

        function detectSpeech(videoElement) {
            const threshold = 50; // Adjust sensitivity level
            function analyze() {
                analyser.getByteFrequencyData(dataArray);
                const volume = dataArray.reduce((a, b) => a + b) / dataArray.length;

                // Apply border based on volume
                if (volume > threshold) {
                    videoElement.classList.add('speech-detected');
                } else {
                    videoElement.classList.remove('speech-detected');
                }

                requestAnimationFrame(analyze);
            }
            analyze();
        }
    </script>
</body>
</html>
