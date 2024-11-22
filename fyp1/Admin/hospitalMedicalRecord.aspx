﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalMedicalRecord.aspx.cs" Inherits="fyp1.Admin.hospitalMedicalRecord" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../layout/PageStyle.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content">
        <div class="col-xxl-12 col-lg-12">
            <div class="mb-2">
                <asp:LinkButton CssClass="btn btn-primary fw-bold"
                    ID="lbAddMedicalRecord" runat="server" OnClick="lbAddMedicalRecord_Click">
                    <i class="bi bi-file-medical" style="color:white;"></i>
                    Add Medical Record
                </asp:LinkButton>
                <div class="mt-5 d-flex">
                    <div class="me-auto">
                        <h3>Medical Record</h3>
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
                                OnItemCommand="lvMedicalRecord_ItemCommand"
                                ID="lvMedicalRecord" runat="server">
                                <LayoutTemplate>
                                    <table class="table table-responsive-md table-hover">
                                        <thead>
                                            <tr>
                                                <th>
                                                    <asp:LinkButton runat="server" CssClass="sortable-header">Record ID</asp:LinkButton></th>
                                                <th>
                                                    <asp:LinkButton runat="server" CssClass="sortable-header">Patient ID</asp:LinkButton></th>
                                                <th>
                                                    <asp:LinkButton runat="server" CssClass="sortable-header">Doctor ID</asp:LinkButton></th>
                                                <th>
                                                    <asp:LinkButton runat="server" CssClass="sortable-header">Department</asp:LinkButton></th>
                                                <th>
                                                    <asp:LinkButton runat="server" CssClass="sortable-header">Date</asp:LinkButton></th>
                                                <th>Action</th>
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
                                            CommandName="SelectRecord"
                                            CommandArgument='<%# Eval("recordID") %>'
                                            CssClass="d-none" />
                                        <td><%# Eval("recordID") %></td>
                                        <td><%# Eval("patientID") %></td>
                                        <td><%# Eval("doctorID") %></td>
                                        <td><%# Eval("department") %></td>
                                        <td><%# Eval("recordDate", "{0:dd/MM/yyyy}") %></td>
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
                                            </asp:LinkButton></td>
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