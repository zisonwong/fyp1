<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="DoctorAvailability.aspx.cs" Inherits="fyp1.Client.DoctorAvailability" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form runat="server">
        <div class="max-w-7xl mx-auto p-6">
            <!-- Department Filter Dropdown -->
            <div class="mb-4 flex justify-center">
                <asp:DropDownList ID="ddlDepartment" runat="server" 
                    CssClass="block w-1/3 p-2 rounded-lg shadow bg-white border border-gray-300" 
                    AutoPostBack="True" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            
            <!-- Doctor Filter Dropdown -->
            <div class="mb-8 flex justify-center">
                <asp:DropDownList ID="ddlDoctor" runat="server" 
                    CssClass="block w-1/3 p-2 rounded-lg shadow bg-white border border-gray-300" 
                    AutoPostBack="True" OnSelectedIndexChanged="ddlDoctor_SelectedIndexChanged">
                    <asp:ListItem Text="Select Doctor" Value=""></asp:ListItem>
                </asp:DropDownList>
            </div>

            <!-- Doctor Availability Grid -->
            <div id="doctorAvailability" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <asp:Repeater ID="rptAvailability" runat="server">
                    <ItemTemplate>
                        <div class="bg-white p-6 rounded-lg shadow-lg hover:shadow-xl transition duration-300 ease-in-out">
                            <p class="text-lg font-semibold"><%# Eval("availableDate", "{0:dddd, MMMM dd, yyyy}") %></p>
                            <p>From: <%# Eval("availableFrom", "{0:hh\\:mm tt}") %></p>
                            <p>To: <%# Eval("availableTo", "{0:hh\\:mm tt}") %></p>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </form>
</asp:Content>

