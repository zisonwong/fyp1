<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="clientAppointment.aspx.cs" Inherits="fyp1.Client.clientAppointment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <div class="max-w-7xl mx-auto p-6 grid grid-cols-1 lg:grid-cols-4 gap-6">
            <!-- Sidebar for Doctor Details -->
            <div class="col-span-1 bg-white p-6 rounded-lg shadow-lg mt-20">
                <h2  class="text-xl font-semibold text-gray-700 mb-4">Doctor Information</h2>
                <asp:Image ID="imgDoctorPhoto" runat="server" CssClass="w-32 h-32 rounded-full mb-4 mt-6" />
                <asp:Label ID="lblDoctorName" runat="server" class="text-xl font-semibold text-gray-800"></asp:Label>
                <asp:Label ID="lblDoctorContact" runat="server" class="text-gray-600"></asp:Label>
                <br/>
                <asp:Label ID="lblDepartmentName" runat="server" class="mt-2 text-gray-700"></asp:Label>
            </div>

            <div class="col-span-1 lg:col-span-3 bg-white p-6 rounded-lg shadow-lg mt-20">
                <h1 class="text-2xl font-bold mb-6 text-gray-800">Confirm Your Appointment </h1>

                <!-- Patient Details -->
                <div class="mb-4">
                    <h3 class="text-xl font-semibold text-gray-700 mb-4">Your Information</h3>
                    <p>
                        <strong>Name:</strong>
                        <asp:Literal ID="litPatientName" runat="server"></asp:Literal>
                    </p>
                    <p>
                        <strong>NRIC:</strong>
                        <asp:Literal ID="litPatientNRIC" runat="server"></asp:Literal>
                    </p>
                    <p>
                        <strong>Contact:</strong>
                        <asp:Literal ID="litPatientContact" runat="server"></asp:Literal>
                    </p>
                </div>

                <!-- Appointment Details -->
                <div class="mb-6">
                    <h3 class="text-xl font-semibold text-gray-700 mb-4">Appointment Details</h3>
                    <p>
                        <strong>Date:</strong>
                        <asp:Label ID="lblAppointmentDate" runat="server"></asp:Label>
                    </p>
                    <p>
                        <strong>Time:</strong>
                        <asp:Label ID="lblAppointmentTime" runat="server"></asp:Label>
                    </p>
                </div>
                <!-- Confirmation Button -->
                <div class="text-center">
                    <asp:Button ID="btnConfirmAppointment" runat="server" Text="Confirm Appointment"
                        CssClass="bg-blue-500 text-white py-2 px-6 rounded-full hover:bg-blue-600"
                        OnClick="btnConfirmAppointment_Click" />
                </div>

                <!-- Success and Error Messages -->
                <div class="mt-4">
                    <asp:Label ID="lblDateTime" runat="server"></asp:Label>
                    <asp:Label ID="lblMessage" runat="server" CssClass="text-center text-green-600 font-semibold text-lg"></asp:Label>
                    <asp:Label ID="lblError" runat="server" CssClass="text-center text-red-600 font-semibold text-lg"></asp:Label>
                </div>
            </div>
        </div>
    </form>

</asp:Content>
