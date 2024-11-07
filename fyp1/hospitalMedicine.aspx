﻿<%@ Page Title="" Language="C#" MasterPageFile="~/adminSideBar.Master" AutoEventWireup="true" CodeBehind="hospitalMedicine.aspx.cs" Inherits="fyp1.hospitalMedicine" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .sortable-header {
            color: inherit;
            text-decoration: none;
            cursor: pointer;
        }

            .sortable-header:hover {
                color: inherit;
                text-decoration: none;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content">
        <div class="col-xxl-12 col-lg-12">
            <div class="mb-2">
                <asp:LinkButton CssClass="btn btn-primary fw-bold"
                    ID="lbAddMedicine" runat="server" OnClick="lbAddMedicine_Click">
                <svg xmlns="http://www.w3.org/2000/svg" height="20" width="22.5" viewBox="0 0 576 512">
                    <path fill="#ffffff" d="M112 96c-26.5 0-48 21.5-48 48l0 112 96 0 0-112c0-26.5-21.5-48-48-48zM0 144C0 
                        82.1 50.1 32 112 32s112 50.1 112 112l0 224c0 61.9-50.1 112-112 112S0 429.9 0 368L0 144zM554.9 
                        399.4c-7.1 12.3-23.7 13.1-33.8 3.1L333.5 214.9c-10-10-9.3-26.7 3.1-33.8C360 167.7 387.1 160 416 
                        160c88.4 0 160 71.6 160 160c0 28.9-7.7 56-21.1 79.4zm-59.5 59.5C472 472.3 444.9 480 416 480c-88.4 
                        0-160-71.6-160-160c0-28.9 7.7-56 21.1-79.4c7.1-12.3 23.7-13.1 33.8-3.1L498.5 425.1c10 10 9.3 26.7-3.1 
                        33.8z"/></svg>
                Add Medicine
            </asp:LinkButton>
            </div>
            <div class="mt-5 d-flex">
                <div class="me-auto">
                    <h3>Medicine</h3>
                </div>
                <div class="d-flex py-2">
                    <asp:DropDownList ID="ddlFilterMedicineType" AutoPostBack="true" runat="server" 
                        OnSelectedIndexChanged="ddlFilterMedicineType_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="card border-0 shadow mb-4">
                <div class="card-body p-3">
                    <div class="table-responsive-md">
                        <asp:ListView
                            OnItemInserting="lvMedicine_ItemInserting"
                            OnItemCanceling="lvMedicine_ItemCanceling"
                            OnItemEditing="lvBranch_ItemEditing"
                            OnItemUpdating="lvMedicine_ItemUpdating"
                            OnItemDataBound="lvMedicine_ItemDataBound"
                            ID="lvMedicine" runat="server">
                            <LayoutTemplate>
                                <table class="table table-responsive-md">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Medicine ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Name</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Quantity</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Type</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Description</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Dosage</asp:LinkButton></th>
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
                                    <td><%# Eval("medicineID") %></td>
                                    <td><%# Eval("name") %></td>
                                    <td><%# Eval("quantity") %></td>
                                    <td><%# Eval("type") %></td>
                                    <td><%# Eval("description") %></td>
                                    <td><%# Eval("dosage") %></td>
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
                            <InsertItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblMedicineId" runat="server" Text="-"></asp:Label></td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75" ID="txtMedicineName" runat="server"></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75" ID="txtQuantity" runat="server"></asp:TextBox></td>
                                    <td>
                                        <asp:DropDownList ID="ddlMedicineType" CssClass="px-2" AutoPostBack="true" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75" ID="txtDescription" runat="server"></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75" ID="txtDosage" runat="server"></asp:TextBox></td>
                                    <td>
                                        <asp:Button CssClass="bg-success text-white" ID="btnAdd" runat="server"
                                            OnClientClick="return confirm('Are you sure you want to add this record?');"
                                            Text="Add" CommandName="Insert" />
                                        <asp:Button ID="btnCancel" CssClass="bg-success bg-danger mt-2 text-white"
                                            OnClientClick="return confirm('Are you sure you want to discard to add?');"
                                            CommandArgument="none" runat="server" CommandName="Cancel" Text="Cancel" />
                                    </td>
                                </tr>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblMedicineId" runat="server" Text='<%# Eval("MedicineID") %>'></asp:Label></td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75" ID="txtEditMedicineName" runat="server" Text='<%# Eval("Name") %>'></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75" ID="txtEditQuantity" runat="server" Text='<%# Eval("Quantity") %>'></asp:TextBox></td>
                                    <td>
                                        <asp:DropDownList CssClass="px-2" ID="ddlEditMedicineType" AutoPostBack="true" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75" ID="txtEditDescription" runat="server" Text='<%# Eval("Description") %>'></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox CssClass="px-2 w-75" ID="txtEditDosage" runat="server" Text='<%# Eval("Dosage") %>'></asp:TextBox></td>
                                    <td>
                                        <asp:Button CssClass="bg-success text-white" ID="btnAdd" runat="server"
                                            CommandArgument="none"
                                            OnClientClick="return confirm('Are you sure you want to update this record?');"
                                            Text="Update" CommandName="Update" />
                                        <asp:Button ID="btnCancel" CssClass="bg-success bg-danger mt-2 text-white"
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
        </div>
    </main>
</asp:Content>
