﻿<%@ Page Title="Doctor" Language="C#" MasterPageFile="~/Admin/adminSideBar.Master" AutoEventWireup="true" CodeBehind="hospitalDoctor.aspx.cs" Inherits="fyp1.Admin.hospitalDoctor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content container">
        <div class="col-xxl-12 col-lg-12">
            <div class="mb-2">
                <asp:LinkButton CssClass="btn btn-primary fw-bold"
                    ID="lbAddDoctor" runat="server" OnClick="lbAddDoctor_Click">
    <svg xmlns="http://www.w3.org/2000/svg" height="20" width="17.5" viewBox="0 0 448 512">
        <path fill="#ffffff" d="M224 256A128 128 0 1 0 224 0a128 128 0 1 0 0 256zm-96 55.2C54 332.9 0 401.3 
            0 482.3C0 498.7 13.3 512 29.7 512l388.6 0c16.4 0 29.7-13.3 29.7-29.7c0-81-54-149.4-128-171.1l0 50.8c27.6 
            7.1 48 32.2 48 62l0 40c0 8.8-7.2 16-16 16l-16 0c-8.8 0-16-7.2-16-16s7.2-16 16-16l0-24c0-17.7-14.3-32-32-32s-32 
            14.3-32 32l0 24c8.8 0 16 7.2 16 16s-7.2 16-16 16l-16 0c-8.8 0-16-7.2-16-16l0-40c0-29.8 20.4-54.9 
            48-62l0-57.1c-6-.6-12.1-.9-18.3-.9l-91.4 0c-6.2 0-12.3 .3-18.3 .9l0 65.4c23.1 6.9 40 28.3 40 53.7c0 30.9-25.1 
            56-56 56s-56-25.1-56-56c0-25.4 16.9-46.8 40-53.7l0-59.1zM144 448a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/></svg>
    Add Doctor
</asp:LinkButton>
                <asp:Button ID="btnExportToExcel" runat="server" CssClass="btn btn-success fw-bold"
            Text="Export to Excel" OnClick="btnExportToExcel_Click" />
            </div>
            <div class="mt-5">
                <h3>Doctor</h3>
            </div>
            <asp:ListView ID="lvStaff" runat="server" ItemPlaceholderID="itemPlaceholder">
                <LayoutTemplate>
                    <div class="row">
                        <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="col-md-4 col-lg-3 mb-4">
                        <asp:LinkButton ID="lbDoctorDetails" runat="server" CssClass="card shadow-sm staff-card rounded"
                            CommandArgument='<%# Eval("doctorID") %>'
                            OnCommand="lbDoctorDetails_Command"
                            Style="text-decoration: none; color: inherit;">
                            <img src='<%# Eval("doctorPhoto") %>' alt="Profile Image" class="card-img-top profile-image rounded-circle mx-auto mt-3" style="width: 100px; height: 100px; object-fit: cover;" />
                            <div class="card-body text-center">
                                <h5 class="card-title"><%# Eval("name") %></h5>
                                <p class="card-text text-muted"><%# Eval("role") %></p>
                                <p class="card-text"><%# Eval("email") %></p>
                                <asp:Button ID="btnDelete" CssClass="btn btn-danger me-2" runat="server" Text="Delete"
                                    CommandArgument='<%# Eval("doctorID") %>' OnCommand="btnDelete_Command"
                                    OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                <asp:Button ID="btnEdit" CssClass="btn btn-primary" runat="server" Text="Edit"
                                    CommandArgument='<%# Eval("doctorID") %>' OnCommand="btnEdit_Command" />
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
