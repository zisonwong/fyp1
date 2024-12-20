﻿<%@ Page Title="Branch" Language="C#" MasterPageFile="~/Admin/adminSideBar.Master" AutoEventWireup="true" CodeBehind="hospitalBranch.aspx.cs" Inherits="fyp1.Admin.hospitalBranch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="layout/time.js"></script>

    <script type="text/javascript">
        var openingDDLId = '<%= ddlFilterOpeningTime.ClientID %>';
        var closingDDLId = '<%= ddlFilterClosingTime.ClientID %>';
    </script>
    <link href="~/layout/PageStyle.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <script src="../layout/time.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content container">
        <div class="col-xxl-12 col-lg-12">
            <div class="mb-2">
                <asp:LinkButton CssClass="btn btn-primary fw-bold"
                    ID="lbAddBranch" runat="server" OnClick="lbAddBranch_Click">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-hospital" viewBox="0 0 16 16">
  <path d="M8.5 5.034v1.1l.953-.55.5.867L9 7l.953.55-.5.866-.953-.55v1.1h-1v-1.1l-.953.55-.5-.866L7 7l-.953-.55.5-.866.953.55v-1.1zM13.25 9a.25.25 0 0 0-.25.25v.5c0 .138.112.25.25.25h.5a.25.25 0 0 0 .25-.25v-.5a.25.25 0 0 0-.25-.25zM13 11.25a.25.25 0 0 1 .25-.25h.5a.25.25 0 0 1 .25.25v.5a.25.25 0 0 1-.25.25h-.5a.25.25 0 0 1-.25-.25zm.25 1.75a.25.25 0 0 0-.25.25v.5c0 .138.112.25.25.25h.5a.25.25 0 0 0 .25-.25v-.5a.25.25 0 0 0-.25-.25zm-11-4a.25.25 0 0 0-.25.25v.5c0 .138.112.25.25.25h.5A.25.25 0 0 0 3 9.75v-.5A.25.25 0 0 0 2.75 9zm0 2a.25.25 0 0 0-.25.25v.5c0 .138.112.25.25.25h.5a.25.25 0 0 0 .25-.25v-.5a.25.25 0 0 0-.25-.25zM2 13.25a.25.25 0 0 1 .25-.25h.5a.25.25 0 0 1 .25.25v.5a.25.25 0 0 1-.25.25h-.5a.25.25 0 0 1-.25-.25z"/>
  <path d="M5 1a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v1a1 1 0 0 1 1 1v4h3a1 1 0 0 1 1 1v7a1 1 0 0 1-1 1H1a1 1 0 0 1-1-1V8a1 1 0 0 1 1-1h3V3a1 1 0 0 1 1-1zm2 14h2v-3H7zm3 0h1V3H5v12h1v-3a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1zm0-14H6v1h4zm2 7v7h3V8zm-8 7V8H1v7z"/>
</svg>
                    Add Branch
                </asp:LinkButton>
            </div>
            <div class="mt-5 d-flex">
                <div class="me-auto">
                    <h3>Branch List</h3>
                </div>
                <div class="d-flex py-2 w-25">
                    <div class="pe-2 d-flex position-relative w-100">
                        <asp:DropDownList CssClass="form-control" ID="ddlFilterOpeningTime" OnSelectedIndexChanged="ddlFilterOpeningTime_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
                        <i class="bi bi-chevron-down dropdown-icon"></i>
                    </div>
                    <div class="d-flex position-relative w-100">
                        <asp:DropDownList CssClass="form-control" ID="ddlFilterClosingTime" OnSelectedIndexChanged="ddlFilterClosingTime_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
                        <i class="bi bi-chevron-down dropdown-icon"></i>
                    </div>
                </div>
            </div>

            <div class="card borer-0 shadow mb-4">
                <div class="card-body p-3">
                    <div class="table-responsive-md">
                        <asp:ListView
                            ID="lvBranch" runat="server"
                            OnItemDataBound="lvBranch_ItemDataBound"
                            OnItemCanceling="lvBranch_ItemCanceling"
                            OnItemInserting="lvBranch_ItemInserting"
                            OnItemUpdating="lvBranch_ItemUpdating"
                            OnItemEditing="lvBranch_ItemEditing"
                            OnPagePropertiesChanging="lvBranch_PagePropertiesChanging"
                            OnDataBound="lvBranch_DataBound"
                            OnItemCommand="lvBranch_ItemCommand">
                            <LayoutTemplate>
                                <table class="table table-responsive-md table-hover">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Branch ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Name</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Address</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Open Time</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Close Time</asp:LinkButton></th>
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
                                    <td><%# Eval("branchID") %></td>
                                    <td><%# Eval("name") %></td>
                                    <td><%# Eval("address") %></td>
                                    <td><%# Eval("openTime") %></td>
                                    <td><%# Eval("closeTime") %></td>
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
                                        <asp:LinkButton ID="lbDelete" runat="server"
                                            CommandName="Unactivate"
                                            CommandArgument='<%# Eval("branchID") %>'
                                            OnClientClick="return confirm('Are you sure you want to delete this record?');">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 16 16">
                                        <path d="M6.5 1h3a.5.5 0 0 1 .5.5v1H6v-1a.5.5 0 0 1 .5-.5M11 2.5v-1A1.5 1.5 0 0 0 9.5 0h-3A1.5 1.5 0 0 
                                            0 5 1.5v1H1.5a.5.5 0 0 0 0 1h.538l.853 10.66A2 2 0 0 0 4.885 16h6.23a2 2 0 0 0 1.994-1.84l.853-10.66h.538a.5.5 
                                            0 0 0 0-1zm1.958 1-.846 10.58a1 1 0 0 1-.997.92h-6.23a1 1 0 0 1-.997-.92L3.042 3.5zm-7.487 1a.5.5 0 0 1 .528.47l.5 
                                            8.5a.5.5 0 0 1-.998.06L5 5.03a.5.5 0 0 1 .47-.53Zm5.058 0a.5.5 0 0 1 .47.53l-.5 8.5a.5.5 0 1 1-.998-.06l.5-8.5a.5.5 
                                            0 0 1 .528-.47M8 4.5a.5.5 0 0 1 .5.5v8.5a.5.5 0 0 1-1 0V5a.5.5 0 0 1 .5-.5"/>
                                        </svg>
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <InsertItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblBranchId" runat="server" Text="-"></asp:Label></td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75 form-control" ID="txtBranchName" runat="server"></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75 form-control" ID="txtBranchAddress" runat="server"></asp:TextBox></td>
                                    <td>
                                        <div class="d-flex position-relative">
                                            <asp:DropDownList CssClass="px-2 form-control" ID="ddlBranchOpeningTime" runat="server">
                                            </asp:DropDownList>
                                            <i class="bi bi-chevron-down dropdown-icon"></i>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="d-flex position-relative">
                                            <asp:DropDownList CssClass="px-2 form-control" ID="ddlBranchClosingTime" runat="server">
                                            </asp:DropDownList>
                                            <i class="bi bi-chevron-down dropdown-icon"></i>
                                        </div>
                                    </td>
                                    <td>
                                        <asp:Button CssClass="btn btn-primary" ID="btnAdd" runat="server"
                                            OnClientClick="return confirm('Are you sure you want to add this record?');"
                                            Text="Add" CommandName="Insert" />
                                        <asp:Button ID="btnCancel" CssClass="btn btn-danger"
                                            OnClientClick="return confirm('Are you sure you want to discard to add?');"
                                            CommandArgument="none" runat="server" CommandName="Cancel" Text="Cancel" />
                                    </td>
                                </tr>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblBranchId" runat="server" Text='<%# Eval("branchID") %>'></asp:Label></td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75 form-control" ID="txtEditBranchName" runat="server" Text='<%# Eval("name") %>'></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75 form-control" ID="txtEditBranchAddress" runat="server" Text='<%# Eval("address") %>'></asp:TextBox></td>
                                    <td>
                                        <div class="d-flex position-relative">
                                            <asp:DropDownList CssClass="px-2 form-control" ID="ddlEditBranchOpeningTime" runat="server">
                                            </asp:DropDownList>
                                            <i class="bi bi-chevron-down dropdown-icon"></i>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="d-flex position-relative">
                                            <asp:DropDownList CssClass="px-2 form-control" ID="ddlEditBranchClosingTime" runat="server">
                                            </asp:DropDownList>
                                            <i class="bi bi-chevron-down dropdown-icon"></i>
                                        </div>
                                    </td>
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
            <div class="pagination-container">
                <asp:DataPager ID="dpBranch" runat="server" PagedControlID="lvBranch" PageSize="10" class="pagination">
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
