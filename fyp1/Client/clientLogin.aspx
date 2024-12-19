<%@ Page Title="Login" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="clientLogin.aspx.cs" Inherits="fyp1.Client.clientLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../CSS/login.css" rel="stylesheet" />
    <link href="../CSS/home.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <div class="page-container">
            <div class="login-container">
                <h1><strong>Login</strong></h1>
                <div class="inputGroup">
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="inputText" Placeholder="Username/Email" AutoCompleteType="Disabled"></asp:TextBox>
                </div>
                <div class="inputGroup">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="inputText" TextMode="Password" Placeholder="Password" AutoCompleteType="Disabled"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:CheckBox ID="chkRememberMe" runat="server" CssClass="cbRememberMe" Text="Remember Me"/>
                </div>
                <asp:Label ID="lblErrorMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
                <br>
                <br>
                <div class="form-group">
                    <asp:Button CssClass="btnLogin" ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click"/>
                </div>
                <div class="form-group forget-password">
                    <a class="forgetPW" href="../Client/clientForgotPassword.aspx">Forgot Password?</a>
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
