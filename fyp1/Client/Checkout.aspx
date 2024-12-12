<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="fyp1.Client.Checkout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://www.paypal.com/sdk/js?client-id=<%= PayPalClientID %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="container mx-auto mb-12 p-6 max-w-md bg-white rounded-lg shadow-md">
         <div class="mt-24">
        <h1 class="text-2xl font-bold text-center mb-6">PayPal Checkout</h1>
        <p class="text-gray-600 text-center mb-6">
            Total Amount: <span class="font-semibold text-gray-800">MYR 30.00</span>
        </p>
        <div id="paypal-button-container" class="flex justify-center"></div>
    </div>
         </div>
    <script>
        paypal.Buttons({
            createOrder: function (data, actions) {
                return fetch('Checkout.aspx/CreateOrder', {
                    method: 'POST',
                })
                    .then(response => response.json())
                    .then(data => data.id);
            },
            onApprove: function (data, actions) {
                return fetch('Checkout.aspx/CaptureOrder', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ orderId: data.orderID }),
                })
                    .then(response => response.json())
                    .then(details => {
                        alert('Payment completed successfully!');
                    });
            }
,
        }).render('#paypal-button-container');
    </script>
</asp:Content>
