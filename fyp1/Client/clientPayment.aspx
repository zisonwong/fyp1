<%@ Page Title="Payment Success" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="clientPayment.aspx.cs" Inherits="fyp1.Client.clientPayment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
            padding-top: 50px;
        }
        .success-message {
            color: green;
            font-size: 24px;
            font-weight: bold;
            margin-top: 20px;
        }
        .appointment-details {
            margin-top: 30px;
            font-size: 18px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="text-center bg-white p-10 rounded-lg shadow-lg max-w-md w-full">
        <h2 class="text-2xl font-bold text-green-600 mb-4">Payment Successful</h2>
        <p class="text-lg text-gray-700 mb-6">Thank you! Your payment has been confirmed.</p>
        <div class="text-gray-700">
            <p>Your appointment has been successfully booked.</p>
            <p><strong>Appointment Date:</strong> <asp:Label ID="lblAppointmentDate" runat="server" class="font-semibold"></asp:Label></p>
            <p><strong>Appointment Time:</strong> <asp:Label ID="lblAppointmentTime" runat="server" class="font-semibold"></asp:Label></p>
            <p><strong>Doctor:</strong> <asp:Label ID="lblDoctorName" runat="server" class="font-semibold"></asp:Label></p>
        </div>
    </div>
</asp:Content>
