<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="clientForgotPassword.aspx.cs" Inherits="fyp1.Client.clientForgotPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../layout/bootstrap.bundle.min.js"></script>
    <link href="../layout/bootstrap.min.css" rel="stylesheet" />
    <style>
        main {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
            background-color: #f8f9fa;
        }

        .forgot-password-form {
            width: 100%;
            max-width: 800px; 
            padding: 30px;
            background: white;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            text-align: center; 
        }

        .forgot-password-form h1 {
            font-size: 28px;
            margin-bottom: 20px;
            color: #333;
        }

        .forgot-password-form h3 {
            font-size: 20px;
            margin-bottom: 20px;
        }

        .btn-primary {
            background-color: #007bff;
            border-color: #007bff;
        }

            .btn-primary:hover {
                background-color: #0056b3;
                border-color: #0056b3;
            }

        .form-text {
            font-size: 14px;
            color: #6c757d;
            margin-top: 10px;
        }
        .form-control, .btn {
            display: block;
            margin-left: auto;
            margin-right: auto;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <main>
            <div class="forgot-password-form">
                <h1 class="fw-bold">Forgot Password</h1>
                <div class="mb-3">
                    <h3>We will send a link to your email that can reset your password if you have forgotten it.</h3>
                    <label for="emailInput" class="form-label">Please enter your email address below</label>
                    <asp:TextBox ID="txt_fp_email" runat="server" CssClass="form-control w-75 mx-auto"
                        AutoCompleteType="Disabled"  placeholder="example@gmail.com" required="true"></asp:TextBox>
                    <div class="form-text">We'll send a verification link to your email.</div>
                </div>
                <asp:Button ID="btnSendVerification" runat="server" Text="Send Verification" CssClass="btn btn-primary w-25 mx-auto" OnClick="btnSendVerification_Click" />
            </div>
        </main>
    </form>
</asp:Content>
