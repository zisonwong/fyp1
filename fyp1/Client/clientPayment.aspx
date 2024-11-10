<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clientPayment.aspx.cs" Inherits="fyp1.Client.clientPayment" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Payment</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
</head>
<body class="bg-gray-100">
    <form runat="server">
        <div class="container mx-auto py-10">
            <div class="bg-white shadow-lg rounded-lg p-8">
                <!-- Header -->
                <h1 class="text-2xl font-bold text-gray-700 mb-4">Complete the Payment</h1>
                <p class="text-gray-500 mb-6">Complete your appointment booking by choosing a payment method below.</p>

                <!-- Appointment Summary -->
                <div class="mb-6">
                    <h2 class="text-xl font-semibold text-gray-600">Appointment Summary</h2>
                    <p>
                        <strong>Doctor:</strong>
                        <asp:Label ID="lblDoctorName" runat="server" CssClass="text-gray-700"></asp:Label>
                    </p>
                    <p>
                        <strong>Date:</strong>
                        <asp:Label ID="lblAppointmentDate" runat="server" CssClass="text-gray-700"></asp:Label>
                    </p>
                    <p>
                        <strong>Time:</strong>
                        <asp:Label ID="lblAppointmentTime" runat="server" CssClass="text-gray-700"></asp:Label>
                    </p>
                    <p class="text-lg font-bold text-gray-700 mt-4">Consultation Fee: RM<asp:Label ID="lblConsultationFee" runat="server" CssClass="text-gray-700"></asp:Label></p>
                </div>

                <!-- Payment Methods -->
                <div class="mb-6">
                    <h2 class="text-xl font-semibold text-gray-600">Payment Method</h2>

                    <!-- Bank Transfer -->
                    <label class="flex items-center mt-4">
                        <input type="radio" name="paymentMethod" value="Bank Transfer" id="radioBankTransfer" runat="server" class="form-radio h-5 w-5 text-blue-600" />
                        <span class="ml-2 text-gray-700">Bank Transfer</span>
                    </label>
                    <input type="text" placeholder="Bank Name" id="txtBankName" runat="server" class="mt-2 block w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400" />

                    <!-- Credit/Debit Card -->
                    <label class="flex items-center mt-4">
                        <input type="radio" name="paymentMethod" value="Credit Card" id="radioCreditCard" runat="server" class="form-radio h-5 w-5 text-blue-600" />
                        <span class="ml-2 text-gray-700">Credit/Debit Card</span>
                    </label>
                    <input type="text" placeholder="Card Number" id="txtCardNumber" runat="server" class="mt-2 block w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400" />
                    <div class="flex mt-2">
                        <input type="text" placeholder="Expiration Date (MM/YY)" id="txtExpiryDate" runat="server" class="w-1/2 px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400" />
                        <input type="text" placeholder="CVV" id="txtCVV" runat="server" class="w-1/2 px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 ml-2" />
                    </div>
                    <input type="text" placeholder="Name on Card" id="txtCardName" runat="server" class="mt-2 block w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400" />

                    <!-- Cash Payment -->
                    <label class="flex items-center mt-4">
                        <input type="radio" name="paymentMethod" value="Cash" id="radioCash" runat="server" class="form-radio h-5 w-5 text-blue-600" />
                        <span class="ml-2 text-gray-700">Cash Payment (Pay at Clinic)</span>
                    </label>

                </div>

                <!-- Terms and Conditions -->
                <div class="flex items-center mb-4">
                    <input type="checkbox" class="form-checkbox h-5 w-5 text-blue-600">
                    <span class="ml-2 text-gray-700">I agree to the terms and conditions of payment.</span>
                </div>

                <!-- Confirm Payment Button -->
                <asp:Button runat="server" ID="btnConfirmPayment" class="w-full bg-blue-600 text-white py-2 rounded-lg font-semibold hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500" onclick="btnConfirmPayment_Click" Text="Confirm Payment">
                </asp:Button>
                <!-- Success or Error Messages -->
                <div class="mt-4 text-center">
                    <asp:Label ID="lblMessage" runat="server" CssClass="text-green-600 font-semibold"></asp:Label>
                    <asp:Label ID="lblError" runat="server" CssClass="text-red-600 font-semibold"></asp:Label>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
