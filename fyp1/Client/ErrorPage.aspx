<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="fyp1.Client.ErrorPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" href="Images/tabLogo.svg"/>
    <title>Error - Something Went Wrong</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet" />
</head>
<body class="bg-gray-100 flex items-center justify-center min-h-screen">
    <form id="form1" runat="server">
        <div class="bg-white p-8 rounded-lg shadow-lg max-w-lg w-full text-center">
            <div class="text-6xl font-bold text-red-600">
                <strong>404</strong>
            </div>
            <h1 class="text-4xl font-semibold text-gray-800 mt-4">Oops! Something Went Wrong</h1>
            <p class="text-lg text-gray-600 mt-4">We couldn't find the page you were looking for. Please try again later or go back to the homepage.</p>
            <a href="clientHome.aspx" class="mt-6 inline-block px-6 py-3 bg-blue-500 text-white rounded-lg text-lg transition duration-300 ease-in-out hover:bg-blue-600">
                Go to Homepage
            </a>
        </div>
    </form>
</body>
</html>
