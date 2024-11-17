<%@ Page Title="Login as Staff" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="hospitalStaffLogin.aspx.cs" Inherits="fyp1.Admin.hospitalStaffLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../CSS/login.css" rel="stylesheet" />
    <link href="../CSS/home.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <div class="page-container">
            <div class="login-container">
                <h1><strong>Login</strong><span style="font-size:20px"> as Staff</span></h1>
                <div class="inputGroup">
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="inputText" Placeholder="Email" AutoCompleteType="Disabled"></asp:TextBox>
                </div>
                <div class="inputGroup">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="inputText" TextMode="Password" Placeholder="Password" AutoCompleteType="Disabled"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:CheckBox ID="chkRememberMe" runat="server" CssClass="cbRememberMe" Text="Remember Me" />
                </div>
                <asp:Label ID="lblErrorMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
                <br>
                <br>
                <div class="form-group">
                    <asp:Button CssClass="btnLogin" ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
                </div>
                <div class="form-group forget-password">
                    <a class="forgetPW" href="forgetPassword.aspx">Forgot Password?</a>
                </div>
                <br>
                <br>
                <hr class="seperator">
                <br>
                <div class="signUp">
                    <p>Don't have an account?</p>
                    <a href="clientSignUp.aspx">Sign up now</a>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
