<%@ Page Title="Add Appointment" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalAppointment.aspx.cs" Inherits="fyp1.Admin.hospitalAppointment" %>

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
            </div>
            <div class="card border-0 shadow mb-4">
                <div class="card-body p-3">
                    <div class="table-responsive-md">
                        <asp:ListView
                            OnSorting="lvAppointment_Sorting"
                            OnItemCommand="lvAppointment_ItemCommand"
                            OnPagePropertiesChanging="lvAppointment_PagePropertiesChanging"
                            ID="lvAppointment" runat="server">
                            <LayoutTemplate>
                                <table class="table table-responsive-md table-hover">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="appointmentID" CssClass="sortable-header">Appointment ID</asp:LinkButton>
                                            </th>
                                            <th>
                                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="patientID" CssClass="sortable-header">Patient ID</asp:LinkButton>
                                            </th>
                                            <th>
                                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="doctorID" CssClass="sortable-header">Doctor ID</asp:LinkButton>
                                            </th>
                                            <th>
                                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="time" CssClass="sortable-header">Time</asp:LinkButton>
                                            </th>
                                            <th>
                                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="date" CssClass="sortable-header">Date</asp:LinkButton>
                                            </th>
                                            <th>
                                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="status" CssClass="sortable-header">Status</asp:LinkButton>
                                            </th>

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
            <div class="pagination-container">
                <asp:DataPager ID="dpAppointment" runat="server" PagedControlID="lvAppointment" PageSize="10" class="pagination">
                    <Fields>
                        <asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="False" ShowNextPageButton="False" ShowPreviousPageButton="True" PreviousPageText="<" />
                        <asp:NumericPagerField CurrentPageLabelCssClass="active" ButtonCount="5" />
                        <asp:NextPreviousPagerField ButtonType="Button" ShowLastPageButton="False" ShowNextPageButton="True" ShowPreviousPageButton="False" NextPageText=">" />
                    </Fields>
                </asp:DataPager>
            </div>
        </div>
    </main>
</asp:Content>
