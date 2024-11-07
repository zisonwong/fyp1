<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="DoctorAvailability.aspx.cs" Inherits="fyp1.Client.DoctorAvailability" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form runat="server">
        <!-- Page Container -->
        <div class="max-w-7xl mx-auto p-6 grid grid-cols-1 md:grid-cols-3 gap-6">

            <!-- Sidebar for Doctor Details -->
            <div class="col-span-1 bg-white p-6 rounded-lg shadow-lg mt-6">
                <asp:Panel ID="pnlDoctorDetails" runat="server">
                    <h2 class="text-2xl font-semibold text-gray-800">Doctor Details</h2>
                    <div class="mt-4">
                        <asp:Image ID="imgDoctorPhoto" runat="server" CssClass="w-24 h-24 rounded-full mb-4" />
                        <h3 class="text-xl font-semibold text-gray-800">
                            <asp:Literal ID="litDoctorName" runat="server" />
                        </h3>
                        <p class="text-gray-500">
                            <asp:Literal ID="litDoctorRole" runat="server" />
                        </p>
                        <p class="text-gray-600">
                            <asp:Literal ID="litDoctorContact" runat="server" />
                        </p>
                    </div>
                </asp:Panel>
            </div>

            <!-- Right Side for Availability -->
            <div class="col-span-2">
                <h1 class="text-3xl font-bold text-gray-800 text-center mb-6">Doctor Availability</h1>
                <div id="doctorAvailabilityGrid" class="grid grid-cols-1 lg:grid-cols-2 gap-6">
                    <asp:Repeater ID="rptAvailability" runat="server">
                        <ItemTemplate>
                            <div class="bg-white p-6 rounded-lg shadow-lg hover:shadow-xl transition duration-300 ease-in-out">
                                <p class="text-lg font-semibold"><%# Eval("availableDate", "{0:dddd, MMMM dd, yyyy}") %></p>
                                <p>From: <%# Eval("availableFrom", "{0:hh\\:mm}") %> <%# DateTime.Parse(Eval("availableFrom").ToString()).ToString("tt") %></p>
                                <p>To: <%# Eval("availableTo", "{0:hh\\:mm}") %> <%# DateTime.Parse(Eval("availableTo").ToString()).ToString("tt") %></p>

                                <asp:Button ID="btnSelectTime" runat="server" Text="Select Time"
                                    CssClass="bg-blue-500 text-white py-2 px-4 rounded-full mt-4"
                                    CommandArgument='<%# Eval("availableDate") + " " + Eval("availableFrom") %>'
                                    OnClick="btnSelectTime_Click" />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </form>
</asp:Content>

