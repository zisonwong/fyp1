<%@ Page Title="Reset your password" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="clientResetPassword.aspx.cs" Inherits="fyp1.Client.clientResetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="icon" href="Images/tabLogo.svg"/>
    <link href="../layout/bootstrap.min.css" rel="stylesheet" />
    <script src="../layout/bootstrap.bundle.min.js"></script>
    <style>
        main {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
            background-color: #f8f9fa;
            text-align: center;
        }

        .reset-password-container {
            width: 100%;
            max-width: 700px;
            padding: 20px;
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            text-align: center;
        }

            .reset-password-container h1 {
                font-size: 28px;
                margin-bottom: 20px;
                color: #333;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <main>
            <div class="reset-password-container text-center">
                <h1 class="mb-3 fw-bold">Reset account password</h1>
                <div class="mb-3">
                    <asp:TextBox runat="server" ID="txtVerificationCode" CssClass="form-control w-75 mx-auto"
                        AutoCompleteType="Disabled" placeholder="Enter verification code" MaxLength="6"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <asp:LinkButton ID="btnSendVerificationCode" runat="server" CssClass="btn btn-primary w-25 mx-auto"
                        OnClick="btnSendVerificationCode_Click" Text="Send Code"></asp:LinkButton>
                </div>
                <p class="text-muted mb-4">Enter a new password</p>
                <div class="mb-3">
                    <asp:TextBox runat="server" ID="txtNewPassword" CssClass="form-control w-75 mx-auto"
                        TextMode="Password" placeholder="New Password"></asp:TextBox>
                </div>
                <div class="mb-3">
                    <asp:TextBox runat="server" ID="txtReEnterPassword" CssClass="form-control w-75 mx-auto"
                        TextMode="Password" placeholder="ReEnter Password"></asp:TextBox>
                </div>
                <asp:LinkButton ID="btnResetPassword" runat="server" CssClass="btn btn-primary w-25 mx-auto"
                    OnClick="btnResetPassword_Click" Text="Reset password"></asp:LinkButton>
            </div>
        </main>
    </form>
</asp:Content>
