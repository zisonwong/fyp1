<%@ Page Title="Staff Settings" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalStaffSettings.aspx.cs" Inherits="fyp1.Admin.hospitalStaffSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../layout/bootstrap.bundle.min.js"></script>
    <link href="../layout/bootstrap.min.css" rel="stylesheet" />
    <link href="../layout/PageStyle.css" rel="stylesheet" />
    <style>
        .bg-body2 {
            background-color: #f4f7f9 !important;
        }

        #password-strength-container {
            margin-top: 10px;
            height: 10px;
            width: 100%;
            background-color: #e0e0e0;
            border-radius: 5px;
        }

        #password-strength-bar {
            height: 100%;
            width: 0;
            border-radius: 5px;
        }

        #password-strength-text {
            margin-top: 5px;
            font-size: 14px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container content">
        <div class="row">
            <div class="col-md-12">
                <!-- General Section -->
                <h4>General</h4>
                <p>View and update your general account information.</p>
                <div class="card">
                    <div class="card-body">
                        <div class="list-group list-group-flush">
                            <div class="list-group-item d-flex align-items-center">
                                <div class="flex-fill">
                                    <div class="fw-bold">Name</div>
                                    <asp:Label ID="txtName" runat="server" Text="Name"></asp:Label>
                                </div>
                                <div class="w-100px">
                                    <a href="#modalEditName" id="modalEditNameLink" data-bs-toggle="modal" class="btn btn-primary w-100px" runat="server">Edit</a>
                                </div>
                            </div>
                            <div class="list-group-item d-flex align-items-center">
                                <div class="flex-fill">
                                    <div class="fw-bold">IC Number</div>
                                    <asp:Label ID="txtIC" runat="server" Text="Name"></asp:Label>
                                </div>
                                <div>
                                    <a href="#modalEditIC" data-bs-toggle="modal" class="btn btn-primary w-100px">Edit</a>
                                </div>
                            </div>
                            <div class="list-group-item d-flex align-items-center">
                                <div class="flex-fill">
                                    <div class="fw-bold">Phone</div>
                                    <asp:Label ID="txtPhone" runat="server" Text="Name"></asp:Label>
                                </div>
                                <div>
                                    <a href="#modalEditPhone" data-bs-toggle="modal" class="btn btn-primary w-100px">Edit</a>
                                </div>
                            </div>
                            <div class="list-group-item d-flex align-items-center">
                                <div class="flex-fill">
                                    <div class="fw-bold">Email address</div>
                                    <asp:Label ID="txtEmail" runat="server" Text="Name"></asp:Label>
                                </div>
                                <div>
                                    <a href="#modalEditEmail" data-bs-toggle="modal" class="btn btn-primary w-100px">Edit</a>
                                </div>
                            </div>
                            <div class="list-group-item d-flex align-items-center">
                                <div class="flex-fill">
                                    <div class="fw-bold">Image</div>
                                </div>
                                <div>
                                    <a href="#modalEditImage" data-bs-toggle="modal" class="btn btn-primary w-100px">Edit</a>
                                </div>
                            </div>
                            <div class="list-group-item d-flex align-items-center">
                                <div class="flex-fill">
                                    <div class="fw-bold">Password</div>
                                </div>
                                <div>
                                    <a href="#modalEditPassword" data-bs-toggle="modal" class="btn btn-primary w-100px">Edit</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal fade" id="modalEditName" style="display: none;" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Edit name</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3">
                                    <div class="row">
                                        <div class="col-6">
                                            <label class="form-label">First Name</label>
                                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" Placeholder="First Name"></asp:TextBox>
                                        </div>
                                        <div class="col-6">
                                            <label class="form-label">Last Name</label>
                                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" Placeholder="Last Name"></asp:TextBox>
                                        </div>
                                        <div class="col-4">
                                        </div>
                                    </div>
                                </div>
                                <div class="alert bg-body2">
                                    <b>Please note:</b> If you change your name, you can't change it again for 60 days. 
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
                                <asp:LinkButton ID="btnSaveName" CssClass="btn btn-primary" OnClick="btnSaveName_Click" runat="server">Save changes</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal fade" id="modalEditPassword" style="display: none;" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Edit password</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3">
                                    <label class="form-label">Current Password</label>
                                    <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Password"></asp:TextBox>
                                </div>

                                <div class="mb-4">
                                    <label class="form-label">New Password</label>
                                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="New Password"></asp:TextBox>
                                    <div id="password-strength-container">
                                        <div id="password-strength-bar"></div>
                                        <span id="password-strength-text"></span>
                                    </div>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Confirm Password</label>
                                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Confirm Password"></asp:TextBox>
                                </div>
                                <div class="alert bg-body2">
                                    <b>Please note:</b> Please make sure New password must contain at least 8 characters, one uppercase letter, one lowercase letter, one number and one symbol.
                                </div>
                            </div>

                            <div class="modal-footer">
                                <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
                                <asp:LinkButton ID="btnSavePassword" CssClass="btn btn-primary" OnClick="btnSavePassword_Click" runat="server">Save changes</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal fade" id="modalEditIC" style="display: none;" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Edit IC number</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3">
                                    <label class="form-label">IC Number</label>
                                    <div class="row">
                                        <div class="col-12">
                                            <asp:TextBox ID="txtEditICNumber" runat="server" CssClass="form-control" Placeholder="123456-11-1111"></asp:TextBox>
                                        </div>
                                        <div class="col-4">
                                        </div>
                                    </div>
                                </div>
                                <div class="alert bg-body2">
                                    <b>Please note:</b> Please make sure the IC number you enter matches your actual, real-world IC number. 
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
                                <asp:LinkButton ID="btnSaveIC" CssClass="btn btn-primary" OnClick="btnSaveIC_Click" runat="server">Save changes</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal fade" id="modalEditPhone" style="display: none;" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Edit phone</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3">
                                    <label class="form-label">Phone</label>
                                    <div class="row">
                                        <div class="col-12">
                                            <asp:TextBox ID="txtEditPhone" runat="server" CssClass="form-control" Placeholder="0123456789"></asp:TextBox>
                                        </div>
                                        <div class="col-4">
                                        </div>
                                    </div>
                                </div>
                                <div class="alert bg-body2">
                                    <b>Please note:</b> Please ensure that your phone is capable of receiving calls and is in working condition.
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
                                <asp:LinkButton ID="btnSavePhone" CssClass="btn btn-primary" OnClick="btnSavePhone_Click" runat="server">Save changes</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal fade" id="modalEditEmail" style="display: none;" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Edit email</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3">
                                    <label class="form-label">Email</label>
                                    <div class="row">
                                        <div class="col-12">
                                            <asp:TextBox ID="txtEditEmail" runat="server" CssClass="form-control" Placeholder="example@gmail.com"></asp:TextBox>
                                        </div>
                                        <div class="col-4">
                                        </div>
                                    </div>
                                </div>
                                <div class="alert bg-body2">
                                    <b>Please note:</b>
                                    Please make sure the email address you provide is valid and active and that you can access it to receive important communications.
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
                                <asp:LinkButton ID="btnSaveEmail" CssClass="btn btn-primary" OnClick="btnSaveEmail_Click" runat="server">Save changes</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal fade" id="modalEditImage" style="display: none;" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Edit Photo</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3 text-center">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <!-- Center the image and give it a fixed size -->
                                            <asp:Image ID="imgAvatar" runat="server" CssClass="rounded-circle" Width="150px" Height="150px" ImageUrl="~/hospitalImg/defaultAvatar.jpg" />

                                            <!-- File upload and buttons horizontally aligned -->
                                            <div class="d-flex justify-content-center mt-4">
                                                <div class="me-2">
                                                    <asp:FileUpload ID="FileUploadAvatar" runat="server" CssClass="d-none" onchange="previewImage();" />
                                                    <asp:LinkButton ID="btnUploadAvatar" runat="server" CssClass="btn btn-primary fw-bold" OnClientClick="triggerFileUpload(); return false;">Upload Avatar</asp:LinkButton>
                                                </div>
                                                <div>
                                                    <asp:LinkButton ID="btnClearImage" runat="server" OnClick="btnClearImage_Click" CssClass="btn btn-danger fw-bold">Clear Avatar</asp:LinkButton>
                                                </div>
                                            </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <div class="alert bg-body2 mt-3">
                                        <b>Please note:</b> Please upload a .jpg, .png or .pneg file with a minimum dimension of 400w x 400h not exceeding 5MB.
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
                                    <asp:LinkButton ID="btnSavePhoto" CssClass="btn btn-primary" OnClick="btnSavePhoto_Click" runat="server">Save changes</asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
    </main>
    <script>
        function checkPasswordStrength() {
            const passwordInput = document.querySelector('input[name$="txtNewPassword"]');
            const strengthBar = document.getElementById('password-strength-bar');
            const strengthText = document.getElementById('password-strength-text');

            if (!passwordInput || !strengthBar || !strengthText) {
                console.error('One or more required elements not found');
                return;
            }

            const password = passwordInput.value;

            if (password === "") {
                strengthBar.style.width = '0';
                strengthText.innerText = '';
                return;
            }

            const regexVeryWeak = /^(?=.*[a-zA-Z]+$|^\d+$).{6,}$/;  // only letters or only digits
            const regexWeak = /^(?=.*[a-zA-Z])(?=.*\d).{6,}$/;  // letters + digits
            const regexModerate = /^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()]).{8,}$/; //letters + digits + special characters
            const regexStrong = /^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()]).{12,}$/;  // letters + digits + special characters + length

            let strength = 0;

            if (regexVeryWeak.test(password)) {
                strength = 0; // very weak 
            } else if (regexStrong.test(password)) {
                strength = 3; // strong
            } else if (regexModerate.test(password)) {
                strength = 2; // moderate
            } else if (regexWeak.test(password)) {
                strength = 1; // weak
            }

            switch (strength) {
                case 3:
                    strengthBar.style.width = '100%';
                    strengthBar.style.backgroundColor = 'green';
                    strengthText.innerText = 'Strong';
                    break;
                case 2:
                    strengthBar.style.width = '60%';
                    strengthBar.style.backgroundColor = 'orange';
                    strengthText.innerText = 'Moderate';
                    break;
                case 1:
                    strengthBar.style.width = '30%';
                    strengthBar.style.backgroundColor = 'red';
                    strengthText.innerText = 'Weak';
                    break;
                case 0:
                    strengthBar.style.width = '10%';
                    strengthBar.style.backgroundColor = 'gray';
                    strengthText.innerText = 'Very Weak';
                    break;
                default:
                    strengthBar.style.width = '0';
                    strengthText.innerText = '';
                    break;
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            const passwordInput = document.querySelector('input[name$="txtNewPassword"]');
            if (passwordInput) {
                passwordInput.addEventListener('keyup', checkPasswordStrength);
            }
        });

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
