<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/adminSideBar.Master" AutoEventWireup="true" CodeBehind="hospitalNurse.aspx.cs" Inherits="fyp1.Admin.hospitalNurse" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <main class="content">
        <div class="col-xxl-12 col-lg-12">
            <div class="mb-2">
                <asp:LinkButton CssClass="btn btn-primary fw-bold"
                    ID="lbAddNurse" runat="server" OnClick="lbAddNurse_Click">
    <svg xmlns="http://www.w3.org/2000/svg" height="20" width="17.5" viewBox="0 0 448 512">
        <path fill="#ffffff" d="M96 128l0-57.8c0-13.3 8.3-25.3 20.8-30l96-36c7.2-2.7 15.2-2.7 
            22.5 0l96 36c12.5 4.7 20.8 16.6 20.8 30l0 57.8-.3 0c.2 2.6 .3 5.3 .3 8l0 40c0 70.7-57.3 
            128-128 128s-128-57.3-128-128l0-40c0-2.7 .1-5.4 .3-8l-.3 0zm48 48c0 44.2 35.8 80 80 80s80-35.8 
            80-80l0-16-160 0 0 16zM111.9 327.7c10.5-3.4 21.8 .4 29.4 8.5l71 75.5c6.3 6.7 17 6.7 23.3 
            0l71-75.5c7.6-8.1 18.9-11.9 29.4-8.5C401 348.6 448 409.4 448 481.3c0 17-13.8 30.7-30.7 30.7L30.7 
            512C13.8 512 0 498.2 0 481.3c0-71.9 47-132.7 111.9-153.6zM208 48l0 16-16 0c-4.4 0-8 3.6-8 8l0 
            16c0 4.4 3.6 8 8 8l16 0 0 16c0 4.4 3.6 8 8 8l16 0c4.4 0 8-3.6 8-8l0-16 16 0c4.4 0 8-3.6 
            8-8l0-16c0-4.4-3.6-8-8-8l-16 0 0-16c0-4.4-3.6-8-8-8l-16 0c-4.4 0-8 3.6-8 8z"/></svg>
    Add Nurse
</asp:LinkButton>
            </div>
            <div class="mt-5">
                <h3>Nurse</h3>
            </div>
            <asp:ListView ID="lvStaff" runat="server" ItemPlaceholderID="itemPlaceholder">
                <LayoutTemplate>
                    <div class="row">
                        <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="col-md-4 col-lg-3 mb-4">
                        <asp:LinkButton ID="lbNurseDetails" runat="server" CssClass="card shadow-sm staff-card rounded"
                            CommandArgument='<%# Eval("nurseID") %>'
                            OnCommand="lbNurseDetails_Command"
                            Style="text-decoration: none; color: inherit;">
                            <img src='<%# Eval("nursePhoto") %>' alt="Profile Image" class="card-img-top profile-image rounded-circle mx-auto mt-3" style="width: 100px; height: 100px; object-fit: cover;" />
                            <div class="card-body text-center">
                                <h5 class="card-title"><%# Eval("name") %></h5>
                                <p class="card-text text-muted"><%# Eval("role") %></p>
                                <p class="card-text"><%# Eval("email") %></p>
                                <asp:Button ID="btnDelete" CssClass="btn btn-danger me-2" runat="server" Text="Delete"
                                    CommandArgument='<%# Eval("nurseID") %>' OnCommand="btnDelete_Command"
                                    OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                <asp:Button ID="btnEdit" CssClass="btn btn-primary" runat="server" Text="Edit"
                                    CommandArgument='<%# Eval("nurseID") %>' OnCommand="btnEdit_Command" />
                            </div>
                        </asp:LinkButton>
                    </div>
                </ItemTemplate>
                <EmptyDataTemplate>
                    No data...
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
    </main>
</asp:Content>
