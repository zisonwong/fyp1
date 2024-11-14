<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalAppointmentDetails.aspx.cs" Inherits="fyp1.Admin.hospitalAppointmentDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <link href="../layout/PageStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content">
        <div class="col-xxl-12 col-lg 12">
            <h3>Appointment Details</h3>
            <div class="d-flex justify-content-between">
                <div class="col-xxl-3 col-lg-3">
                    <div class="card border-0 shadow">
                        <div class="card-body p-2">
                            <h5>Doctor Info</h5>
                            <div class="d-flex align-items-center mb-2">
                                <i class="bi bi-person-vcard" style="font-size: 20px"></i>
                                <asp:Label ID="txtDoctorName" runat="server" Text="Name" CssClass="ps-2"></asp:Label>
                            </div>
                            <div class="d-flex align-items-center mb-2">
                                <i class="bi bi-envelope" style="font-size: 20px"></i>
                                <asp:Label ID="txtDoctorEmail" runat="server" Text="Name" CssClass="ps-2"></asp:Label>
                            </div>
                            <div class="d-flex align-items-center mb-2">
                                <i class="bi bi-telephone" style="font-size: 20px"></i>
                                <asp:Label ID="txtDoctorPhone" runat="server" Text="Contact" CssClass="ps-2"></asp:Label>
                            </div>
                            <div class="d-flex align-items-center">
                                <i class="bi bi-building" style="font-size: 20px"></i>
                                <asp:Label ID="txtDoctorDepartment" runat="server" Text="Department" CssClass="ps-2"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-xxl-3 col-lg-3">
                    <div class="card border-0 shadow">
                        <div class="card-body p-2">
                            <h5>Patient Info</h5>
                            <div class="d-flex align-items-center mb-2">
                                <i class="bi bi-person-vcard" style="font-size: 20px"></i>
                                <asp:Label ID="txtPatientName" runat="server" Text="Name" CssClass="ps-2"></asp:Label>
                            </div>
                            <div class="d-flex align-items-center mb-2">
                                <i class="bi bi-envelope" style="font-size: 20px"></i>
                                <asp:Label ID="txtPatientEmail" runat="server" Text="Name" CssClass="ps-2"></asp:Label>
                            </div>
                            <div class="d-flex align-items-center mb-2">
                                <i class="bi bi-telephone" style="font-size: 20px"></i>
                                <asp:Label ID="txtPatientPhone" runat="server" Text="Contact" CssClass="ps-2"></asp:Label>
                            </div>
                            <div class="d-flex align-items-center">
                                <i class="bi bi-geo-alt" style="font-size: 20px"></i>
                                <asp:Label ID="txtPatientAddress" runat="server" Text="Address" CssClass="ps-2"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-xxl-3 col-lg-3">
                    <div class="card border-0 shadow">
                        <div class="card-body p-2">
                            <h5>Payment Info</h5>
                            <div class="d-flex align-items-center mb-2">
                                <i class="bi bi-cash" style="font-size: 20px"></i>
                                <asp:Label ID="txtPaymentAmount" runat="server" Text="Amount" CssClass="ps-2"></asp:Label>
                            </div>
                            <div class="d-flex align-items-center mb-2">
                                <i class="bi bi-credit-card" style="font-size: 20px"></i>
                                <asp:Label ID="txtPaymentMethod" runat="server" Text="Method" CssClass="ps-2"></asp:Label>
                            </div>
                            <div class="d-flex align-items-center mb-2">
                                <i class="bi bi-calendar" style="font-size: 20px"></i>
                                <asp:Label ID="txtPaymentDate" runat="server" Text="Date" CssClass="ps-2"></asp:Label>
                            </div>
                            <div class="d-flex align-items-center">
                                <i class="bi bi-patch-question" style="font-size: 20px"></i>
                                <asp:Label ID="txtPaymentStatus" runat="server" Text="Status" CssClass="ps-2"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card border-0 shadow mb-4">
                <div class="card-body p-3">
                    <div class="table-responsive-md">
                        <asp:ListView
                            OnItemCommand="lvAppointmentHistory_ItemCommand"
                            ID="lvAppointmentHistory" runat="server">
                            <LayoutTemplate>
                                <table class="table table-responsive-md">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Appointment ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Doctor ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Start Time</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">End Time</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Date</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Status</asp:LinkButton></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr style="cursor: pointer;"
                                    onclick="document.getElementById('<%# ((Control)Container).FindControl("btnRowClick").ClientID %>').click();">
                                    <asp:LinkButton
                                        ID="btnRowClick"
                                        runat="server"
                                        CommandName="SelectAppointment"
                                        CommandArgument='<%# Eval("appointmentID") %>'
                                        CssClass="d-none" />
                                    <td><%# Eval("appointmentID") %></td>
                                    <td><%# Eval("doctorID") %></td>
                                    <td><%# Eval("startTime", "{0:hh\\:mm\\:ss}") %></td>
                                    <td><%# Eval("endTime", "{0:hh\\:mm\\:ss}") %></td>
                                    <td><%# Eval("date", "{0:dd/MM/yyyy}") %></td>
                                    <td><%# Eval("status") %></td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <tr>
                                    <td colspan="6" class="text-center">No data here...</td>
                                </tr>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </div>
                </div>
            </div>
        </div>
    </main>
</asp:Content>
