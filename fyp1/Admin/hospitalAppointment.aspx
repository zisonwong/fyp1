<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalAppointment.aspx.cs" Inherits="fyp1.Admin.hospitalAppointment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../layout/PageStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content container">
        <div class="col-xxl-12 col-lg-12">
            <div class="d-flex">
                <div class="me-auto">
                    <h3>Appointment</h3>
                </div>
                <div class="d-flex py-2">
                    <asp:DropDownList ID="ddlFilter" AutoPostBack="true" runat="server">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="card border-0 shadow mb-4">
                <div class="card-body p-3">
                    <div class="table-responsive-md">
                        <asp:ListView
                            OnItemCommand="lvAppointment_ItemCommand"
                            ID="lvAppointment" runat="server">
                            <LayoutTemplate>
                                <table class="table table-responsive-md table-hover">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Appointment ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Patient ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Doctor ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Time</asp:LinkButton></th>
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
                                    <td><%# Eval("patientID") %></td>
                                    <td><%# Eval("doctorID") %></td>
                                    <td><%# Eval("time", "{0:hh\\:mm\\:ss}") %></td>
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
