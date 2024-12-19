<%@ Page Title="Medical Record Details" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalRecordDetails.aspx.cs" Inherits="fyp1.Admin.hospitalRecordDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content container">
        <div class="col-xxl-12 col-lg-12">
            <div class="d-flex mb-2">
                <div class="me-auto">
                    <h3>Record Details</h3>
                </div>
                 <% if (Session["Role"].ToString() == "Doctor" || Session["Role"].ToString() == "Admin")
                    { %>
                <asp:LinkButton CssClass="btn btn-primary fw-bold"
                    ID="btnEditRecord" runat="server" OnClick="btnEditRecord_Click">
                Edit
                </asp:LinkButton>
                 <% } %>
            </div>
            <div class="card -border-0 shadow">
                <div class="card-body">
                    <div class="row">
                        <div class="col-xxl-4 col-lg-4">
                            <div class="mb-2">
                                <p class="mb-0">Patient Name</p>
                                <asp:TextBox ID="txtName" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Gender</p>
                                <asp:TextBox ID="txtGender" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Blood Type</p>
                                <asp:TextBox ID="txtBloodType" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Age</p>
                                <asp:TextBox ID="txtAge" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Date</p>
                                <asp:TextBox ID="txtCreateDate" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xxl-8 col-lg-8">
                            <div class="mb-2">
                                <p class="mb-0">Problems</p>
                                <asp:TextBox ID="txtProblems" CssClass="w-100 form-control resize height" ReadOnly="true" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Diagnosis</p>
                                <asp:TextBox ID="txtDiagnosis" CssClass="w-100 form-control resize height" ReadOnly="true" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Treatment</p>
                                <asp:TextBox ID="txtTreatment" CssClass="w-100 form-control resize height" ReadOnly="true" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="d-flex mb-2">
                <h3>Prescription Details</h3>
            </div>
            <div class="card border-0 shadow mb-4">
                <div class="card-body p-3">
                    <div class="table-responsive-md">
                        <asp:ListView
                            OnItemDeleting="lvPrescriptionDetails_ItemDeleting"
                            DataKeyNames="prescriptionID"
                            ID="lvPrescriptionDetails" runat="server">
                            <LayoutTemplate>
                                <table class="table table-responsive-md table-hover">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Prescription ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Medicine Name</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Medicine Type</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Quantity</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Details</asp:LinkButton></th>
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
                                    <td><%# Eval("prescriptionID") %></td>
                                    <td><%# Eval("medicineName") %></td>
                                    <td><%# Eval("medicineType") %></td>
                                    <td><%# Eval("quantity") %></td>
                                    <td><%# Eval("details") %></td>
                                    <td>
                                        <asp:LinkButton ID="btnDelete" runat="server"
                                            CommandName="Delete" 
                                            CommandArgument='<%# Eval("prescriptionID") %>'
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
