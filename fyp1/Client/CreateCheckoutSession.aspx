<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateCheckoutSession.aspx.cs" Inherits="fyp1.Client.CreateCheckoutSession" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Appointment Payment</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
            padding-top: 50px;
        }

        #checkout-button {
            background-color: #6772e5;
            color: #fff;
            border: none;
            padding: 10px 20px;
            font-size: 18px;
            border-radius: 5px;
            cursor: pointer;
        }

            #checkout-button:hover {
                background-color: #5469d4;
            }

        .message {
            color: red;
            margin-top: 20px;
        }
    </style>
</head>
<body class="bg-gray-100 flex items-center justify-center h-screen">
    <form id="form1" runat="server">
         <div class="text-center bg-white p-10 rounded-lg shadow-lg max-w-md w-full">
        <h2 class="text-2xl font-bold text-gray-700 mb-4">Confirm Appointment Payment</h2>
        <p class="text-gray-600 mb-6">Please click the button below to proceed with your payment.</p>
        <button id="checkout-button" class="bg-blue-600 text-white px-6 py-2 rounded-full font-semibold hover:bg-blue-500 transition duration-200">
            Pay Now
        </button>
        <p class="text-red-500 mt-4" id="lblError"></p>
    </div>

    <script type="text/javascript">
        document.getElementById("checkout-button").addEventListener("click", function () {
            fetch("/Client/CreateCheckoutSession.aspx?paymentID=<%=Request.QueryString["paymentID"]%>")
                .then(response => response.json())
                .then(session => {
                    return Stripe('your-public-key-here').redirectToCheckout({ sessionId: session.id });
                })
                .then(result => {
                    if (result.error) {
                        document.getElementById("lblError").innerText = result.error.message;
                    }
                })
                .catch(error => {
                    document.getElementById("lblError").innerText = "An error occurred while initiating payment.";
                });
        });
    </script>
    </form>
</body>
</html>
