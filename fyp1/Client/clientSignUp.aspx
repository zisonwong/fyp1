<%@ Page Title="Sign Up" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="clientSignUp.aspx.cs" Inherits="fyp1.Client.clientSignUp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="icon" href="Images/tabLogo.svg"/>
    <link href="../CSS/signup.css" rel="stylesheet" />
    <link href="../CSS/home.css" rel="stylesheet" />
    <link href="../CSS/footer.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <div class="page-container">
            <div class="signup-container">
                <h1><strong>Sign Up</strong></h1>

                <div class="form-group">
                    <div class="inputGroup">
                        <asp:TextBox ID="identityCard" runat="server" CssClass="input-ic" MaxLength="12" onkeyup="validateAndFillDOB()" />
                        <label for="identityCard">IC Number</label>
                    </div>
                </div>
                <div class="form-group">
                    <div class="inputGroup">
                        <asp:TextBox ID="txtDOB" CssClass="input-dob" runat="server"></asp:TextBox>
                        <label class="dob" for="dob">Date of Birth</label>
                    </div>
                </div>
                <div class="form-group">
                    <div class="inputGroup">
                        <asp:TextBox ID="txtUsername" CssClass="input-username" runat="server"></asp:TextBox>
                        <label for="txtUsername">Username</label>
                    </div>
                </div>
                <div class="form-group">
                    <div class="inputGroup">
                        <asp:TextBox ID="txtEmail" CssClass="input-email" TextMode="Email" runat="server"></asp:TextBox>
                        <label for="txtEmail">Email</label>
                    </div>
                </div>
                <div class="form-group">
                    <div class="inputGroup">
                        <asp:TextBox ID="txtAddress1" CssClass="input-address1" runat="server"></asp:TextBox>
                        <label for="address1">Address 1</label>
                    </div>
                </div>
                <div class="form-group">
                    <div class="inputGroup">
                        <asp:TextBox ID="txtAddress2" runat="server"></asp:TextBox>
                        <label for="address2">Address 2</label>
                    </div>
                </div>
                <div class="form-group">
                    <div class="inputGroup">
                        <asp:TextBox ID="txtPostalCode" CssClass="input-postcode" runat="server" MaxLength="5"></asp:TextBox>
                        <label for="txtPostalCode">Postal Code</label>
                    </div>
                </div>
                <div class="form-group">
                    <div class="inputGroup">
                        <asp:TextBox ID="txtPassword" CssClass="input-password" TextMode="Password" runat="server"></asp:TextBox>
                        <label for="password">Password</label>
                    </div>
                </div>
                <div class="form-group">
                    <div class="inputGroup">
                        <asp:TextBox ID="txtConfirmPassword" CssClass="input-confirm-password" TextMode="Password" runat="server"></asp:TextBox>
                        <label for="txtConfirmPassword">Confirm Password</label>
                    </div>
                </div>
                <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-label"></asp:Label>
                <div class="form-group">
                    <asp:Button ID="btnSignup" CssClass="btnSignup" runat="server" Text="Sign Up" OnClick="btnSignup_Click" />
                </div>
                <div class="form-group login-link">
                    <p>Already have an account?</p>
                    <a href="clientLogin.aspx">Log in here</a>
                </div>
            </div>
        </div>
    </form>
    <script>
        function validateAndFillDOB() {
            const icInput = document.getElementById("<%= identityCard.ClientID %>");
            const dobInput = document.getElementById("<%= txtDOB.ClientID %>");

            if (icInput.value.length >= 6) {
                const year = icInput.value.substring(0, 2);
                const month = icInput.value.substring(2, 4);
                const day = icInput.value.substring(4, 6);
                const fullYear = parseInt(year, 10) > 24 ? `19${year}` : `20${year}`;
                const dob = `${fullYear}-${month}-${day}`;

                if (Date.parse(dob)) {
                    dobInput.value = dob;
                    dobInput.labels[0].classList.add("move-up");
                } else {
                    dobInput.value = "";
                    dobInput.labels[0].classList.remove("move-up");
                }
            } else {
                dobInput.value = "";
                dobInput.labels[0].classList.remove("move-up");
            }
        }


        function enforceMaxLength(element) {
            if (element.value.length > 12) {
                element.value = element.value.slice(0, 12); // Limit to 12 characters
            }
        }

        const dobInput = document.getElementById("<%= txtDOB.ClientID %>");
        dobInput.addEventListener("keydown", function (e) {
            e.preventDefault();
        });

        function validateForm(event) {
            let errorMessage = "";

            // 1. IC Number: 12 digits
            const icNumber = document.getElementById("identityCard").value;
            if (!/^\d{12}$/.test(icNumber)) {
                errorMessage += "IC Number must be exactly 12 digits.\n";
            }

            // 2. Username: 3-20 alphanumeric characters
            const username = document.getElementById("txtUsername").value;
            if (!/^[a-zA-Z0-9]{3,20}$/.test(username)) {
                errorMessage += "Username must be 3-20 characters and can only contain letters and numbers.\n";
            }

            // 3. Email: Valid format
            const email = document.getElementById("txtEmail").value;
            if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email)) {
                errorMessage += "Invalid email format.\n";
            }

            // 4. Address 1: Minimum 5 characters
            const address1 = document.getElementById("txtAddress1").value;
            if (address1.length < 5) {
                errorMessage += "Address 1 must be at least 5 characters.\n";
            }

            // 5. Password: At least 8 characters, with uppercase, lowercase, digit, and special character
            const password = document.getElementById("txtPassword").value;
            if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test(password)) {
                errorMessage += "Password must be at least 8 characters long and contain an uppercase letter, a lowercase letter, a digit, and a special character.\n";
            }

            // 6. Confirm Password: Match with Password
            const confirmPassword = document.getElementById("txtConfirmPassword").value;
            if (password !== confirmPassword) {
                errorMessage += "Passwords do not match.\n";
            }

            // Display error messages if any validation fails
            if (errorMessage) {
                alert(errorMessage);
                event.preventDefault(); // Prevent form submission
                return false;
            }

            return true; // Allow form submission if validation passes
        }

        // Attach the validation function to the form's submit event
        document.getElementById("form1").addEventListener("submit", validateForm);

    </script>
</asp:Content>
