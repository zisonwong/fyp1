<%@ Page Title="Doctor" Language="C#" MasterPageFile="~/Admin/adminSideBar.Master" AutoEventWireup="true" CodeBehind="hospitalDoctorDetails.aspx.cs" Inherits="fyp1.Admin.hospitalDoctorDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content container">
        <div class="col-xxl-12 col-lg-12">
            <div class="mb-2">
                <div class="d-flex">
                    <div class="me-auto">
                        <h3>
                            <asp:Label ID="lblDoctorName" runat="server" Text="Label" /></h3>
                    </div>
                    <asp:Button ID="btnEdit" CssClass="btn btn-primary fw-bold" runat="server" Text="Edit"
                         OnCommand="btnEdit_Command" />
                </div>
            </div>
            <div class="card border-0 shadow mb-4">
                <div class="card-body p-3">
                    <!-- Avatar Upload Section -->
                    <div class="d-flex align-items-center mb-4">
                        <asp:Image ID="imgAvatar" runat="server" CssClass="rounded-circle justify-content-center" Width="100px" Height="100px" ImageUrl="~/hospitalImg/defaultAvatar.jpg" />
                    </div>

                    <!-- Personal Information Section -->
                    <h5>Personal Information</h5>
                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label for="txtEmployeeId" class="form-label">Employee ID</label>
                            <asp:TextBox ID="txtEmployeeId" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtFirstName" class="form-label">First Name</label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtLastName" class="form-label">Last Name</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtDateOfBirth" class="form-label">Date of Birth</label>
                            <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtIc" class="form-label">IC Number</label>
                            <asp:TextBox ID="txtIc" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtGender" class="form-label">Gender</label>
                            <asp:TextBox ID="txtGender" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Contact Information Section -->
                    <h5 class="mt-4">Contact Information</h5>
                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label for="txtContactInfo" class="form-label">Phone</label>
                            <asp:TextBox ID="txtContactInfo" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtEmailProvince" class="form-label">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtPassword" class="form-label">Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" ReadOnly="true" TextMode="Password"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Employment Information Section -->
                    <h5 class="mt-4">Employment Information</h5>
                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label for="txtDepartmentId" class="form-label">Department</label>
                            <asp:TextBox ID="txtDepartmentId" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtRole" class="form-label">Role</label>
                            <asp:TextBox ID="txtRole" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtStatus" class="form-label">Status</label>
                            <asp:TextBox ID="txtStatus" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </main>
</asp:Content>
