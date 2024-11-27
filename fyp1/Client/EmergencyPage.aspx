<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="EmergencyPage.aspx.cs" Inherits="fyp1.Client.EmergencyPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <div class="bg-white p-8 rounded-xl shadow-lg max-w-3xl mx-auto border border-gray-200">
            <!-- Title Section -->
            <asp:Label ID="lblTitle" runat="server" Text="Emergency Assistance" 
                CssClass="text-3xl font-bold text-blue-700 text-center mb-6 block mt-20"></asp:Label>

            <!-- Nearest Branch Section -->
            <div class="mb-6">
                <asp:Label ID="lblNearestBranch" runat="server" CssClass="text-xl font-semibold text-blue-600 block" Text="Nearest Branch: Not Available"></asp:Label>
                <asp:Label ID="lblBranchAddress" runat="server" 
                    CssClass="text-lg text-gray-700 block" Text="Address: Fetching..."></asp:Label>
            </div>

            <!-- Ambulance Arrival Time -->
            <div class="mb-6">
                <asp:Label ID="lblAmbulanceTime" runat="server" 
                    CssClass="text-lg text-green-600 font-medium block" Text="Estimated Ambulance Arrival Time: Calculating..."></asp:Label>
            </div>

            <!-- Message Section -->
            <div class="mb-6">
                <asp:Label ID="lblMessage" runat="server" 
                    CssClass="text-lg text-red-500 font-medium block" Text="No active emergency message."></asp:Label>
            </div>
        </div>
        <!-- Google Maps API -->
        <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAbFkinyyHf8XwboZ1KHr7yupFq2yo_ufo&callback=initMap" async defer></script>
    </form>
</asp:Content>
