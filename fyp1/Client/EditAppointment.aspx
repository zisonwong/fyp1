<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="EditAppointment.aspx.cs" Inherits="fyp1.Client.EditAppointment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@3.3.2/dist/tailwind.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server" class="container mx-auto px-4 py-8">
        <div class="max-w-4xl mx-auto bg-white shadow-md rounded-lg p-8">
            <div class="mb-6">
                <h1 class="text-3xl font-bold text-gray-800 mb-4">Edit The Appointment</h1>

                <div class="grid grid-cols-4 gap-4 mb-6 bg-gray-50 p-4 rounded-lg">
                    <div>
                        <label class="block text-sm font-medium text-gray-700">Branch</label>
                        <asp:DropDownList ID="ddlBranch" runat="server"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged"
                            CssClass="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50">
                        </asp:DropDownList>
                    </div>

                    <div>
                        <label class="block text-sm font-medium text-gray-700">Department</label>
                        <asp:DropDownList ID="ddlDepartment" runat="server"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged"
                            CssClass="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50">
                        </asp:DropDownList>
                    </div>

                    <div>
                        <label class="block text-sm font-medium text-gray-700">Consultation Type</label>
                        <asp:DropDownList ID="ddlConsultationType" runat="server"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlConsultationType_SelectedIndexChanged"
                            CssClass="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50">
                            <asp:ListItem Text="Walk-in" Value="Walk-in" />
                            <asp:ListItem Text="Online Consultation" Value="Online Consultation" />
                        </asp:DropDownList>
                    </div>

                    <div>
                        <label class="block text-sm font-medium text-gray-700">Date</label>
                        <asp:TextBox ID="txtSelectedDate" runat="server"
                            TextMode="Date"
                            AutoPostBack="true"
                            OnTextChanged="txtSelectedDate_TextChanged"
                            CssClass="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50">
                        </asp:TextBox>
                    </div>

                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </div>

                <!-- Doctor Details Card -->
                <div class="bg-gray-50 p-6 rounded-lg mb-6">
                    <h2 class="text-xl font-semibold text-gray-800 mb-4">Doctor Details</h2>
                    <div class="grid grid-cols-3 gap-4">
                        <div>
                            <asp:Image ID="imgDoctor" runat="server"
                                CssClass="w-24 h-24 object-cover rounded-full shadow-md ml-5" />
                        </div>
                        <div class="col-span-2">
                            <asp:Label ID="lblDoctorName" runat="server"
                                CssClass="block text-lg font-bold text-gray-700" />
                            <asp:Label ID="lblDepartment" runat="server"
                                CssClass="block text-md text-gray-600 mb-2" />
                            <asp:Label ID="lblBranch" runat="server"
                                CssClass="block text-md text-gray-600" />
                        </div>
                    </div>
                </div>

                <!-- Available Slots Repeater -->
                <asp:Repeater ID="rptAvailableSlots" runat="server" OnItemCommand="rptAvailableSlots_ItemCommand">
                    <ItemTemplate>
                        <div class="slot flex items-center justify-between bg-white border rounded-lg shadow-md p-4 mb-3 hover:shadow-lg transition-shadow">
                            <div>
                                <p class="text-lg font-semibold text-gray-800">
                                    <span class="text-blue-600">Time:</span>
                                    <%# Eval("AvailableFromTime") %> - <%# Eval("AvailableToTime") %>
                                </p>
                            </div>
                            <asp:Button
                                ID="btnSelectSlot"
                                runat="server"
                                Text="Select"
                                CommandName="SelectSlot"
                                CommandArgument='<%# Eval("availabilityID") %>'
                                CssClass="btn bg-blue-500 hover:bg-blue-600 text-white font-medium py-2 px-4 rounded-lg shadow-md transition-all" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>


            </div>
            <div class="bg-gray-50 shadow-md rounded-lg p-6 mt-6">
                <h2 class="text-xl font-semibold text-gray-800 mb-4">Selected Appointment</h2>
                <asp:Label
                    ID="lblSelectedAppointment"
                    runat="server"
                    Text="No appointment selected yet."
                    CssClass="block text-lg text-gray-600 mb-4">
 </asp:Label>
                <asp:Button
                    ID="btnEditAppointment"
                    runat="server"
                    Text="Edit Appointment"
                    OnClick="btnEditAppointment_Click"
                    Visible="false"
                    CssClass="btn btn-primary bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"></asp:Button>
            </div>
        </div>
    </form>


</asp:Content>
