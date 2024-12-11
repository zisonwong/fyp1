<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="doctorAvailability.aspx.cs" Inherits="fyp1.Client.doctorAvailability" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server" class="container mx-auto px-4 py-8">
        <div class="max-w-4xl mx-auto bg-white shadow-md rounded-lg p-8">
            <div class="mb-6">
                <h1 class="text-3xl font-bold text-gray-800 mb-4">Available Appointment Slots</h1>

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
                            <asp:ListItem Text="Walk-in" Value="WalkIn" />
                            <asp:ListItem Text="Online Consultation" Value="Online" />
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
                    <HeaderTemplate>
                        <div class="space-y-4">
                            <div class="grid grid-cols-6 bg-gray-100 p-3 font-bold text-gray-700 rounded-t-lg">
                                <div>Branch</div>
                                <div>Department</div>
                                <div>Date</div>
                                <div>Time</div>
                                <div>Consultation Type</div>
                                <div>Actions</div>
                            </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="grid grid-cols-6 items-center bg-white p-3 border-b hover:bg-gray-50 transition-colors">
                            <div><%# Eval("BranchName") %></div>
                            <div><%# Eval("DepartmentName") %></div>
                            <div><%# Eval("AvailableDate", "{0:dd MMM yyyy}") %></div>
                            <div><%# Eval("AvailableTime") %></div>
                            <div><%# Eval("ConsultationType") %></div>
                            <div>
                                <asp:Button ID="btnSelectSlot" runat="server"
                                    Text="Select"
                                    CommandName="SelectSlot"
                                    CommandArgument='<%# Eval("AvailabilityID") %>'
                                    CssClass="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2" />
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                        <asp:Label ID="lblNoSlots" runat="server"
                            Text="No available slots match your selection."
                            CssClass='<%# ((Repeater)Container.NamingContainer).Items.Count == 0 ? "block text-center text-gray-500 p-4 bg-gray-50 rounded-b-lg" : "hidden" %>' />
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="bg-gray-50 shadow-md rounded-lg p-6 mt-6">
                <h2 class="text-xl font-semibold text-gray-800 mb-4">Selected Appointment</h2>
                <asp:Label
                    ID="lblSelectedAppointment"
                    runat="server"
                    Text="No appointment selected yet."
                    CssClass="block text-lg text-gray-600">
    </asp:Label>
            </div>
        </div>
    </form>


</asp:Content>
