<%@ Page Title="Emergency Alert" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalEmergencyAlert.aspx.cs" Inherits="fyp1.Admin.hospitalEmergencyAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/layout/PageStyle.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content container">
        <div class="col-xxl-12 col-lg-12">
            <div class="d-flex">
                <div class="me-auto">
                    <h3>Emergency Alert</h3>
                </div>
                <div class="d-flex py-2 position-relative dropdown-size">
                    <asp:DropDownList CssClass="form-control " ID="ddlFilterStatus" OnSelectedIndexChanged="ddlFilterStatus_SelectedIndexChanged" 
                        AutoPostBack="true" runat="server">
                    </asp:DropDownList>
                    <i class="bi bi-chevron-down dropdown-icon"></i>
                </div>
            </div>
            <div class="card border-0 shadow mb-4">
                <div class="card-body p-3">
                    <div class="table-responsive-md">
                        <asp:ListView
                            OnItemEditing="lvEmergency_ItemEditing"
                            OnItemUpdating="lvEmergency_ItemUpdating"
                            OnItemCanceling="lvEmergency_ItemCanceling"
                            ID="lvEmergency" runat="server">
                            <LayoutTemplate>
                                <table class="table table-responsive-md table-hover">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Alert ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Patient ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Patient name</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Location</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Time</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Status</asp:LinkButton></th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("alertID") %></td>
                                    <td><%# Eval("patientID") %></td>
                                    <td><%# Eval("patientName") %></td>
                                    <td><%# Eval("address") %></td>
                                    <td><%# Eval("time") %></td>
                                    <td><%# Eval("status") %></td>
                                    <td>
                                        <asp:LinkButton ID="lbEdit" runat="server"
                                            CommandName="Edit">
     <svg xmlns="http://www.w3.org/2000/svg" height="28" width="28" viewBox="0 0 512 512">
         <path fill="#000000" d="M441 58.9L453.1 71c9.4 9.4 9.4 24.6 0 33.9L424 134.1 
             377.9 88 407 58.9c9.4-9.4 24.6-9.4 33.9 0zM209.8 256.2L344 121.9 390.1 
             168 255.8 302.2c-2.9 2.9-6.5 5-10.4 6.1l-58.5 16.7 16.7-58.5c1.1-3.9 3.2-7.5 
             6.1-10.4zM373.1 25L175.8 222.2c-8.7 8.7-15 19.4-18.3 31.1l-28.6 100c-2.4 8.4-.1 
             17.4 6.1 23.6s15.2 8.5 23.6 6.1l100-28.6c11.8-3.4 22.5-9.7 31.1-18.3L487 138.9c28.1-28.1 
             28.1-73.7 0-101.8L474.9 25C446.8-3.1 401.2-3.1 373.1 25zM88 64C39.4 64 0 103.4 0 152L0 424c0 
             48.6 39.4 88 88 88l272 0c48.6 0 88-39.4 88-88l0-112c0-13.3-10.7-24-24-24s-24 10.7-24 24l0 
             112c0 22.1-17.9 40-40 40L88 464c-22.1 0-40-17.9-40-40l0-272c0-22.1 17.9-40 40-40l112 0c13.3 
             0 24-10.7 24-24s-10.7-24-24-24L88 64z"/></svg>
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblAlertId" runat="server" Text='<%# Eval("alertID") %>'></asp:Label></td>
                                    <td>
                                        <asp:Label ID="lblPatientID" runat="server" Text='<%# Eval("patientID") %>'></asp:Label></td>
                                    <td>
                                        <asp:Label ID="lblPatientName" runat="server" Text='<%# Eval("patientName") %>'></asp:Label></td>
                                    <td>
                                        <asp:Label ID="lblAddress" runat="server" Text='<%# Eval("address") %>'></asp:Label></td>
                                    </td>
                                     <td>
                                         <asp:Label ID="lblTime" runat="server" Text='<%# Eval("time") %>'></asp:Label></td>
                                    </td>
                                     <td>
                                         <asp:TextBox CssClass="px-2 w-50 form-control" ID="txtStatus" runat="server" Text='<%# Eval("status") %>'></asp:TextBox></td>
                                    <td>
                                        <asp:Button CssClass="btn btn-primary" ID="btnAdd" runat="server"
                                            CommandArgument="none"
                                            OnClientClick="return confirm('Are you sure you want to update this record?');"
                                            Text="Update" CommandName="Update" />
                                        <asp:Button ID="btnCancel" CssClass="btn btn-danger"
                                            OnClientClick="return confirm('Are you sure you want to discard to update?');"
                                            CommandArgument="none" runat="server" CommandName="Cancel" Text="Cancel" />
                                    </td>
                                </tr>
                            </EditItemTemplate>
                            <EmptyDataTemplate>
                                <tr>
                                    <td colspan="6" class="text-center">No data here...</td>
                                </tr>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </div>
                </div>
            </div>
            <%-- <div class="pagination-container">
                <asp:DataPager ID="dpEmergency" runat="server" PagedControlID="lvEmergency" PageSize="10" class="pagination">
                    <Fields>
                        <asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="False" ShowNextPageButton="False" ShowPreviousPageButton="True" PreviousPageText="<" />
                        <asp:NumericPagerField CurrentPageLabelCssClass="active" ButtonCount="5" />
                        <asp:NextPreviousPagerField ButtonType="Button" ShowLastPageButton="False" ShowNextPageButton="True" ShowPreviousPageButton="False" NextPageText=">" />
                    </Fields>
                </asp:DataPager>
            </div>--%>
        </div>
    </main>
</asp:Content>
