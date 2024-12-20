<%@ Page Title="Emergency" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="EmergencyPage.aspx.cs" Inherits="fyp1.Client.EmergencyPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="icon" href="Images/tabLogo.svg"/>
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAbFkinyyHf8XwboZ1KHr7yupFq2yo_ufo&libraries=places"></script>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <header class="w-full bg-red-500 py-4 shadow-md">
            <div class="container mx-auto text-center mt-24">
                <h1 class="text-2xl font-bold text-white">Emergency Assistance</h1>
            </div>
        </header>

        <main class="flex flex-col items-center w-full px-4 py-6">
            <div class="w-full max-w-4xl bg-white shadow-md rounded-lg p-6 mb-6">
                <h2 class="text-xl font-semibold text-gray-800 border-b pb-3 mb-4">Nearest Branch Details</h2>
                <div class="flex flex-col gap-4">
                    <div>
                        <h3 class="text-lg font-bold text-gray-700">Branch Name:</h3>
                        <asp:Label ID="lblBranchName" runat="server" CssClass="text-gray-600"/>
                    </div>
                    <div>
                        <h3 class="text-lg font-bold text-gray-700">Branch Address:</h3>
                        <asp:Label ID="lblBranchAddress" runat="server" CssClass="text-gray-600"/>
                    </div>
                    <div>
                        <h3 class="text-lg font-bold text-gray-700">Distance:</h3>
                        <asp:Label ID="lblDistance" runat="server" CssClass="text-gray-600"/>
                    </div>
                    <div>
                        <h3 class="text-lg font-bold text-gray-700">Estimated Time:</h3>
                        <asp:Label ID="lblTime" runat="server" CssClass="text-gray-600"/>
                    </div>
                </div>
            </div>
    </main>
    </form>

</asp:Content>
