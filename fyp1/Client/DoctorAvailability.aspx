<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="DoctorAvailability.aspx.cs" Inherits="fyp1.Client.DoctorAvailability" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form runat="server">
    <!-- Page Container -->
    <div class="max-w-7xl mx-auto p-6">

        <!-- Page Header -->
        <div class="text-center mb-8">
            <h1 class="text-3xl font-bold text-gray-800">Doctor Availability</h1>
            <p class="text-gray-600">Check the availability of doctors by department</p>
        </div>

        <!-- Department Filter Dropdown -->
        <div class="mb-8 flex justify-center">
            <select id="ddlDepartment" runat="server" class="block w-1/3 p-2 rounded-lg shadow bg-white border border-gray-300">
                <option value="">Select Department</option>
                <!-- Options populated from code-behind -->
            </select>
        </div>

        <!-- Doctor Availability Grid -->
        <div id="doctorGrid" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <asp:Repeater ID="rptDoctors" runat="server">
                <ItemTemplate>
                    <!-- Doctor Card -->
                    <div class="bg-white p-6 rounded-lg shadow-lg hover:shadow-xl transition duration-300 ease-in-out">
                        <!-- Doctor Photo -->
                        <div class="flex justify-center mb-4">
                            <asp:Image ID="imgDoctorPhoto" runat="server" CssClass="w-24 h-24 rounded-full" ImageUrl='<%# Eval("ImageUrl") %>' />
                        </div>
                        <!-- Doctor Details -->
                        <div class="text-center">
                            <h2 class="text-xl font-semibold text-gray-800"><%# Eval("Name") %></h2>
                            <p class="text-gray-500"><%# Eval("Role") %></p>
                            <p class="text-gray-600"><%# Eval("ContactInfo") %></p>
                            <p class="mt-2 text-green-500 font-semibold"><%# Eval("Availability") %></p>
                        </div>
                        <!-- Book Appointment Button -->
                        <div class="mt-4 text-center">
                            <asp:Button ID="btnBook" runat="server" CssClass="bg-blue-500 text-white py-2 px-4 rounded-full hover:bg-blue-600"
                                Text="Book Appointment" CommandArgument='<%# Eval("DoctorID") %>' OnClick="btnBook_Click" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</form>
</asp:Content>
