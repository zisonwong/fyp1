<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/adminSideBar.Master" AutoEventWireup="true" CodeBehind="hospitalNurseEdit.aspx.cs" Inherits="fyp1.Admin.hospitalNurseEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/layout/PageStyle.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content">
        <div class="col-xxl-12 col-lg-12">
            <div class="mb-2">
                <div class="d-flex">
                    <div class="me-auto">
                        <h3>Edit Nurse</h3>
                    </div>
                    <asp:LinkButton CssClass="btn btn-primary fw-bold"
                        ID="btnConfirmEditNurse" runat="server" OnClick="btnConfirmEditNurse_Click"
                        OnClientClick="return confirm('Are you sure you want to edit this record?');">
                 Edit Nurse
                    </asp:LinkButton>
                </div>
            </div>
            <div class="card border-0 shadow mb-4">
                <div class="card-body p-3">
                    <!-- Avatar Upload Section -->
                    <div class="d-flex align-items-center mb-4">
                        <asp:Image ID="imgAvatar" runat="server" CssClass="rounded-circle" Width="100px" Height="100px" ImageUrl="~/hospitalImg/defaultAvatar.jpg" />
                        <div class="ps-4">
                            <asp:FileUpload ID="FileUploadAvatar" runat="server" CssClass="d-none" onchange="previewImage();" />
                            <asp:LinkButton ID="btnUploadAvatar" runat="server" CssClass="btn btn-primary fw-bold" OnClientClick="triggerFileUpload(); return false;">Upload Avatar</asp:LinkButton>
                            <asp:LinkButton ID="btnClearImage" runat="server" CssClass="text-danger ml-3"
                                OnClick="btnClearImage_Click">Clear selected Image</asp:LinkButton>
                            <p class="text-muted mt-2 mb-0">Please upload a .jpg, .png or .pneg file with a minimum dimension of 400w x 400h not exceeding 5MB</p>
                        </div>
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
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" Placeholder="First Name"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtLastName" class="form-label">Last Name</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" Placeholder="Last Name"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtDateOfBirth" class="form-label">Date of Birth</label>
                            <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtIc" class="form-label">IC Number</label>
                            <asp:TextBox ID="txtIc" runat="server" CssClass="form-control" Placeholder="0123456-78-9012"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="ddlGender" class="form-label">Gender</label>
                            <div class="d-flex position-relative">
                                <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="- Select -" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <i class="bi bi-chevron-down dropdown-icon"></i>
                            </div>
                        </div>
                    </div>

                    <!-- Contact Information Section -->
                    <h5 class="mt-4">Contact Information</h5>
                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label for="txtContactInfo" class="form-label">Phone</label>
                            <asp:TextBox ID="txtContactInfo" runat="server" CssClass="form-control" Placeholder="0123456789" TextMode="Phone"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtEmailProvince" class="form-label">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="example@gmail.com" TextMode="Email"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtPassword" class="form-label">Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" Placeholder="Password" TextMode="Password" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Employment Information Section -->
                    <h5 class="mt-4">Employment Information</h5>
                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label for="ddlBranchId" class="form-label">Branch</label>
                            <div class="d-flex position-relative">
                                <asp:DropDownList ID="ddlBranchId" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="- Select -" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <i class="bi bi-chevron-down dropdown-icon"></i>
                            </div>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="txtRole" class="form-label">Role</label>
                            <asp:TextBox ID="txtRole" runat="server" CssClass="form-control" PlaceHolder="Nurse" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="col-md-4 mb-3">
                            <label for="ddlStatus" class="form-label">Status</label>
                            <div class="d-flex position-relative">
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="- Select -" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <i class="bi bi-chevron-down dropdown-icon"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </main>
    <script type="text/javascript">
        function triggerFileUpload() {
            var fileUpload = document.getElementById('<%= FileUploadAvatar.ClientID %>');
            fileUpload.click();
        }
        function previewImage() {
            var fileUpload = document.getElementById('<%= FileUploadAvatar.ClientID %>');
            var imgAvatar = document.getElementById('<%= imgAvatar.ClientID %>');
            if (fileUpload.files && fileUpload.files[0]) {
                var file = fileUpload.files[0];
                var fileType = file.type.toLowerCase();
                var fileSize = file.size;
                var reader = new FileReader();
                // Validate file type
                if (fileType === "image/jpeg" || fileType === "image/jpg" || fileType === "image/png") {
                    // Validate file size (5MB max)
                    if (fileSize <= 5 * 1024 * 1024) { // 5 MB in bytes
                        var image = new Image();
                        image.onload = function () {
                            // Validate image dimensions
                            if (this.width >= 400 && this.height >= 400) {
                                // File is valid
                                reader.onload = function (e) {
                                    imgAvatar.src = e.target.result;
                                };
                                reader.readAsDataURL(file);
                            } else {
                                alert('Image dimensions must be at least 400x400 pixels.');
                            }
                        };
                        // Load the image to validate dimensions
                        image.src = URL.createObjectURL(file);
                    } else {
                        alert('File size must not exceed 5MB.');
                    }
                } else {
                    alert('File type only allowed PNG, JPG and JPEG.');
                }
            } else {
                alert('Please select a file to upload.');
            }
        }
    </script>
</asp:Content>
