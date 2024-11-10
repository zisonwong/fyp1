<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="DoctorAvailability.aspx.cs" Inherits="fyp1.Client.DoctorAvailability" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form runat="server">
        <!-- Page Container -->
        <div class="max-w-7xl mx-auto p-6 grid grid-cols-1 md:grid-cols-3 gap-6">

            <!-- Sidebar for Doctor Details -->
            <div class="col-span-1 bg-white p-6 rounded-lg shadow-lg mt-16">
                <asp:Panel ID="pnlDoctorDetails" runat="server">
                    <h2 class="text-2xl font-semibold text-gray-800">Doctor Details</h2>
                    <div class="mt-4">
                        <asp:Image ID="imgDoctorPhoto" runat="server" CssClass="w-24 h-24 rounded-full mb-4" />
                        <h3 class="text-xl font-semibold text-gray-800">
                            <asp:Literal ID="litDoctorName" runat="server" />
                        </h3>
                        <p class="text-gray-500">
                            <asp:Literal ID="litDoctorDepartment" runat="server" />
                        </p>
                        <p class="text-gray-600">
                            <asp:Literal ID="litDoctorContact" runat="server" />
                        </p>
                    </div>
                </asp:Panel>
            </div>

            <div class="col-span-2">
                <h1 class="text-3xl font-bold text-gray-800 text-center mb-6">Doctor Availability</h1>

                <!-- Date Dropdown -->
                <asp:DropDownList ID="ddlAvailableDates" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlAvailableDates_SelectedIndexChanged" CssClass="mb-4 bg-gray-200 p-2 rounded">
                </asp:DropDownList>

                <div id="doctorAvailabilityGrid" class="grid grid-cols-1 lg:grid-cols-2 gap-6">
                    <asp:Repeater ID="rptAvailability" runat="server" OnItemCommand="rptAvailability_ItemCommand">
                        <ItemTemplate>
                            <div class="col-span-1 bg-white p-6 rounded-lg shadow-lg">
                                Date: <%# Eval("availableDate", "{0:dd-MM-yyyy}") %>
                                <br />
                                From: <%# Convert.ToDateTime("00:00").Add((TimeSpan)Eval("availableFrom")).ToString("hh:mm tt") %>
                                <br />
                                To: <%# Convert.ToDateTime("00:00").Add((TimeSpan)Eval("availableTo")).ToString("hh:mm tt") %>
                                <br />

                                <asp:Button ID="btnSelectTime" runat="server"
                                    Text="Select Time" CssClass="bg-blue-500 text-white py-2 px-6 rounded-full hover:bg-blue-600"
                                    CommandName="SelectTime"
                                    CommandArgument='<%# Eval("availabilityID") %>' />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </div>
        </div>
    </form>
</asp:Content>

