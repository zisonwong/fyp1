<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SymptomChecker.aspx.cs" Inherits="fyp1.Client.SymptomChecker" Async="true"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet"/>
    <title>Symptom Checker</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }
        .result-box {
            background-color: #f4f4f4;
            border: 1px solid #ddd;
            padding: 15px;
            margin-top: 20px;
            border-radius: 5px;
        }
        .disclaimer {
            color: #666;
            font-style: italic;
            margin-top: 10px;
        }
    </style>
</head>
<body class="bg-gray-100 text-gray-800 font-sans">
    <form id="form1" runat="server" class="max-w-lg mx-auto mt-10 p-6 bg-white shadow-md rounded-lg">
        <div>
            <h1 class="text-2xl font-bold text-center text-blue-600 mb-6">Symptom Checker</h1>
            
            <asp:Label ID="lblInstructions" runat="server" Text="Enter your symptoms (comma-separated):"
                class="block text-gray-700 font-medium mb-2"></asp:Label>
            <asp:TextBox ID="txtSymptoms" runat="server" 
                class="w-full p-3 border border-gray-300 rounded-md focus:outline-none focus:ring focus:ring-blue-300"
                TextMode="MultiLine" Rows="3"></asp:TextBox>
            
            <div class="flex justify-between mt-4">
                <asp:Button ID="btnCheckSymptoms" runat="server" Text="Check Symptoms" 
                    CssClass="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-md shadow-md" OnClick="btnCheckSymptoms_Click" />
                <asp:Button ID="btnClearText" runat="server" Text="Clear Text" 
                    CssClass="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-md shadow-md" OnClick="btnClearText_Click"/>
                <asp:Button ID="BackToHomeBtn" runat="server" Text="Back To Home" 
                    CssClass="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-md shadow-md" OnClick="BackToHomeBtn_Click"/>
            </div>

            <asp:Panel ID="pnlResults" runat="server" 
                class="mt-6 p-4 bg-gray-100 border border-gray-300 rounded-md shadow-md" Visible="false">
                <h2 class="text-xl font-semibold text-gray-800">Symptom Analysis Results</h2>
                <asp:Literal ID="litResults" runat="server"></asp:Literal>
                <asp:Label ID="lblDisclaimer" runat="server" 
                    CssClass="block mt-4 text-sm text-gray-600 italic"></asp:Label>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
