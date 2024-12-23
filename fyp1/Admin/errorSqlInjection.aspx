<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="errorSqlInjection.aspx.cs" Inherits="fyp1.Admin.errorSqlInjection" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error: SQL Injection Detected</title>
    <style>
        @import url("https://fonts.googleapis.com/css?family=Share+Tech+Mono|Montserrat:700");

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            background-color: #121212;
            color: #f5f5f5;
            font-family: "Share Tech Mono", monospace;
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            text-align: center;
        }

        #messageContainer {
            background: rgba(255, 255, 255, 0.1);
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 0 15px rgba(255, 255, 255, 0.1);
            width: 90%;
            max-width: 600px;
        }

        h1 {
            font-size: 2em;
            color: red;
            margin-bottom: 20px;
        }

        p {
            margin-bottom: 15px;
            line-height: 1.5;
            font-size: 1.1em;
        }

        .highlight {
            color: #00e676;
            font-weight: bold;
        }

        .error-description {
            color: #ff5722;
        }

        .homepage, .contact-support {
            color: #00bcd4;
            text-decoration: none;
            font-weight: bold;
            transition: color 0.3s;
        }

        .homepage:hover, .contact-support:hover {
            color: #ff9800;
        }

        footer {
            margin-top: 20px;
            font-size: 0.9em;
            color: #aaa;
        }
    </style>
</head>
<body>
    <div id="messageContainer" aria-label="Error Message">
        <h1>SQL Injection Detected</h1>
        <p>
            <span class="highlight">Error Code:</span> SQL_INJECTION
        </p>
        <p>
            <span class="highlight">Message:</span> Access Denied. Suspicious activity detected. 
            Your account is temporarily locked to prevent unauthorized access.
        </p>
        <p class="error-description">
            <span class="highlight">Description:</span> A potential SQL injection attempt was identified, involving malicious input. This activity violates security protocols.
        </p>
        <p>
            <a class="homepage" href="../Client/clientHome.aspx">Return to Home Page</a> or 
            <a class="contact-support" href="mailto:yhchan6@gmail.com">Contact Support</a>
        </p>
    </div>

    <script>
        const messageContainer = document.getElementById("messageContainer");
        const originalContent = messageContainer.innerHTML;
        let index = 0;
        messageContainer.innerHTML = "";

        function typeEffect() {
            messageContainer.innerHTML = originalContent.slice(0, index) + "|";
            index++;

            if (index > originalContent.length) {
                messageContainer.innerHTML = originalContent;
                return;
            }

            setTimeout(typeEffect, 50);
        }

        typeEffect();
    </script>

    <footer>
        If you believe this is an error, please <a href="mailto:yhchan6@gmail.com" class="contact-support">contact our support team</a>.
    </footer>
</body>
</html>
