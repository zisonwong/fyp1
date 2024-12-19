<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="fyp1.Client.Checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mx-auto mb-12 p-6 max-w-md bg-white rounded-lg shadow-md">
        <div class="mt-24">
            <h1 class="text-2xl font-bold text-center mb-6">PayPal Checkout</h1>
            <p class="text-gray-600 text-center mb-6">
                Total Amount: <span class="font-semibold text-gray-800">MYR 30.00</span>
            </p>
            <div class="flex justify-center">
                <button id="payButton" class="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none">
                    Pay with PayPal
   
                </button>
            </div>
        </div>
    </div>

    <script>
        function redirectToPayPal() {
            fetch('Checkout.aspx/CreateOrder', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.approvalUrl) {
                        // Redirect user to PayPal approval page
                        window.location.href = data.approvalUrl;
                    } else {
                        console.error("Error:", data.error || "No approval URL found.");
                        alert("Failed to create PayPal order. Please try again.");
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                    alert("An error occurred while processing your payment.");
                });
        }

        // Call the redirect function on button click
        document.addEventListener("DOMContentLoaded", function () {
            document.getElementById("payButton").addEventListener("click", redirectToPayPal);
        });
    </script>
</asp:Content>
