<%@ Page Title="Select Branch and Doctor" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="BranchDoctorSelection.aspx.cs" Inherits="fyp1.Client.BranchDoctorSelection" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <div class="flex h-screen">
            <!-- Sidebar for Search and Branch Selection -->
            <div class="w-1/6 bg-gray-100 p-6 border-r border-gray-200 mt-16">
                <div class="mb-8">
                    <h2 class="text-xl font-bold text-gray-700">Search and Filter</h2>
                </div>
                <!-- Branch Selection -->
                <div class="mb-6">
                    <asp:Label ID="lblBranch" runat="server" Text="Select Branch:" CssClass="block text-gray-700 font-semibold mb-2"></asp:Label>
                    <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" CssClass="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"></asp:DropDownList>
                </div>
                <!-- Doctor Search Filter -->
                <div class="mb-6">
                    <asp:Label ID="lblDoctorSearch" runat="server" Text="Search Doctor:" CssClass="block text-gray-700 font-semibold mb-2"></asp:Label>
                    <asp:TextBox ID="txtDoctorSearch" runat="server" CssClass="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="mt-3 w-full inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" />
                </div>
            </div>

            <!-- Right Side for Doctor Cards -->
            <div class="w-5/6 p-6 overflow-y-auto mt-20">
                <!-- Main Doctor List -->
                <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
                    <asp:Repeater ID="rptDoctors" runat="server" OnItemDataBound="rptDoctors_ItemDataBound">
                        <ItemTemplate>
                            <div class="bg-white shadow-md rounded-lg overflow-hidden border border-gray-200">
                                <div class="p-6">
                                    <%-- Doctor Photo (optional) --%>
                                    <asp:Image ID="imgDoctorPhoto" runat="server" CssClass="w-24 h-24 rounded-full mx-auto mb-4"
                                        ImageUrl='<%# ResolveUrl("~/Images/DoctorPhotos/" + Eval("photo")) %>' />

                                    <h3 class="text-lg font-semibold text-gray-900 mb-2"><%# Eval("name") %></h3>
                                    <p class="text-sm text-gray-600 mb-1"><%# Eval("role") %></p>
                                    <p class="text-sm text-gray-500 mb-4"><%# Eval("contactInfo") %></p>
                                    <div class="flex justify-between">
                                        <asp:Button ID="btnMakeAppointment" runat="server" Text="Make Appointment" CommandArgument='<%# Eval("doctorID") %>' OnClick="btnMakeAppointment_Click" CssClass="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500" />
                                        <asp:Button ID="btnChatOnline" runat="server" Text="Chat Online" CommandArgument='<%# Eval("doctorID") %>' OnClick="btnChatOnline_Click" CssClass="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500" />
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <!-- Recommended Doctors Panel -->
                <asp:Panel ID="pnlRecommendations" runat="server" CssClass="mb-8" Visible="false">
                    <h3 class="text-lg font-semibold text-gray-700 mb-4">No exact matches found. Recommended doctors:</h3>
                    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                        <asp:Repeater ID="rptRecommendations" runat="server">
                            <ItemTemplate>
                                <div class="bg-white shadow-md rounded-lg overflow-hidden border border-gray-200">
                                    <div class="p-6">
                                        <h3 class="text-lg font-semibold text-gray-900 mb-2"><%# Eval("name") %></h3>
                                        <p class="text-sm text-gray-600 mb-1"><%# Eval("role") %></p>
                                        <div class="flex justify-end mt-4">
                                            <asp:Button ID="btnMakeAppointment" runat="server" Text="Make Appointment" CommandArgument='<%# Eval("doctorID") %>' OnClick="btnMakeAppointment_Click" CssClass="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500" />
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:Panel>

                <asp:Label ID="lblError" runat="server"></asp:Label>
            </div>
        </div>
    </form>
</asp:Content>
