window.vCashWebcam = {
    videoStream: null,

    start: async function (videoElementId) {
        try {
            const video = document.getElementById(videoElementId);
            if (!video) return;

            const stream = await navigator.mediaDevices.getUserMedia({ video: true });
            video.srcObject = stream;
            video.play();
            this.videoStream = stream;
        } catch (err) {
            console.error("Error accessing webcam: ", err);
            alert("Could not access webcam. Please check permissions and connection.");
        }
    },

    capture: function (videoElementId) {
        console.log("vCashWebcam: Capture called for " + videoElementId);
        const video = document.getElementById(videoElementId);
        if (!video) {
            console.error("vCashWebcam: Video element not found!");
            return null;
        }

        if (video.videoWidth === 0 || video.videoHeight === 0) {
            console.error("vCashWebcam: Video dimensions are 0 (not ready?)");
            return null;
        }

        console.log(`vCashWebcam: Dimensions ${video.videoWidth}x${video.videoHeight}`);

        const canvas = document.createElement("canvas");
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        const context = canvas.getContext("2d");
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        const dataUrl = canvas.toDataURL("image/png");
        console.log("vCashWebcam: Captured DataURL length: " + dataUrl.length);
        return dataUrl;
    },

    stop: function (videoElementId) {
        const video = document.getElementById(videoElementId);
        if (video && video.srcObject) {
            const tracks = video.srcObject.getTracks();
            tracks.forEach(track => track.stop());
            video.srcObject = null;
        }
        this.videoStream = null;
    },

    generateIdCard: async function (details) {
        // Create canvas for ID Card (Standard CR80 size ratio)
        const canvas = document.createElement("canvas");
        canvas.width = 500;
        canvas.height = 315;
        const ctx = canvas.getContext("2d");

        // Background
        ctx.fillStyle = "#f0f0f0";
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        // Header
        ctx.fillStyle = "#0d47a1"; // Blue
        ctx.fillRect(0, 0, canvas.width, 50);
        ctx.fillStyle = "white";
        ctx.font = "bold 24px Arial";
        ctx.fillText((details.state || "US") + " DRIVER LICENSE", 20, 35);

        // Photo Placeholder or Real Photo
        ctx.fillStyle = "#ccc";
        ctx.fillRect(20, 70, 120, 150);

        if (details.photoUrl) {
            const img = new Image();
            img.src = details.photoUrl;
            await new Promise(r => img.onload = r);
            ctx.drawImage(img, 20, 70, 120, 150);
        } else {
            ctx.fillStyle = "#555";
            ctx.font = "14px Arial";
            ctx.fillText("No Photo", 45, 150);
        }

        // Text Data
        ctx.fillStyle = "black";
        ctx.font = "bold 18px Arial";
        ctx.fillText("LN: " + (details.lastName || "").toUpperCase(), 160, 90);
        ctx.fillText("FN: " + (details.firstName || "").toUpperCase(), 160, 115);

        ctx.font = "16px Arial";
        ctx.fillText(details.address || "", 160, 150);
        ctx.fillText((details.city || "") + ", " + (details.state || "") + " " + (details.zip || ""), 160, 175);

        ctx.fillStyle = "#d32f2f"; // Red for critical info
        ctx.font = "bold 16px Arial";
        ctx.fillText("DOB: " + (details.dob ? new Date(details.dob).toLocaleDateString() : ""), 160, 210);
        ctx.fillText("SEX: " + (details.sex || ""), 350, 210);

        ctx.fillStyle = "black";
        ctx.fillText("DL: " + (details.dl || ""), 160, 240);

        // Barcode Mock
        ctx.fillStyle = "black";
        ctx.fillRect(20, 250, 460, 40);

        return canvas.toDataURL("image/png");
    }
};
