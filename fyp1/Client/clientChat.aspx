<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="clientChat.aspx.cs" Inherits="fyp1.Client.clientChat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" rel="stylesheet" />
    <style>
        .text-left {
            justify-content: flex-start;
        }

        .text-right {
            justify-content: flex-end;
        }

        .flex {
            display: flex;
        }

        .mb-2 {
            margin-bottom: 0.5rem;
        }

        .bg-blue-100 {
            background-color: #cce5ff;
            color: #004085;
        }

        .bg-green-100 {
            background-color: #d4edda;
            color: #155724;
        }

        .rounded-lg {
            border-radius: 0.5rem;
        }

        .max-w-md {
            max-width: 28rem;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="chatForm" runat="server">
        <asp:ScriptManager runat="server" />
        <div class="flex h-screen">
            <!-- Left Side: Doctor List -->
            <div class="w-1/4 bg-white p-4 border-r mt-20">
                <h2 class="text-2xl font-semibold text-gray-700">Chats</h2>
                <asp:UpdatePanel ID="UpdatePanelDoctorList" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Repeater ID="RepeaterDoctorList" runat="server" OnItemCommand="DoctorSelected">
                            <ItemTemplate>
                                <asp:LinkButton
                                    ID="lnkSelectDoctor"
                                    runat="server"
                                    CommandName="SelectDoctor"
                                    CommandArgument='<%# Eval("doctorID") %>'
                                    CssClass="block">
                                    <div class="flex items-center w-full text-left text-lg p-3 bg-gray-200 rounded hover:bg-gray-300 mt-3">
                                        <asp:Image ID="imgDoctorPhoto" runat="server" ImageUrl='<%# Eval("ImageUrl") %>' CssClass="w-10 h-10 rounded-full mr-3" />
                                        <span class="mt-1"><%# Eval("name") %></span>
                                    </div>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
            <!-- Right Side -->
            <div class="flex-1 bg-white p-4 mt-10">
                <div class="flex flex-col h-full">
                    <!-- Chat Messages Section -->
                    <div id="chatMessages" class="flex-1 p-4 space-y-4">
                        <asp:UpdatePanel ID="UpdatePanelChat" runat="server">
                            <ContentTemplate>
                                <div id="chatContainer" class="flex overflow-y-auto flex-col-reverse max-h-[calc(100vh-11rem)] h-full">
                                    <asp:Repeater ID="RepeaterMessages" runat="server">
                                        <ItemTemplate>
                                            <div class='<%# Eval("alignmentClass") %> flex mb-2 mr-6'>
                                                <div class='<%# Eval("messageClass") %> p-3 rounded-lg max-w-md'>
                                                    <%# Eval("content") %>
                                                    <div class="text-sm text-gray-500"><%# Eval("timestamp") %></div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnSend" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <asp:Timer ID="TimerRefresh" runat="server" Interval="7000" OnTick="TimerRefresh_Tick" />
                    </div>

                    <div id="chatInput" class="bg-gray-100 p-4 flex items-center justify-between mt-auto w-full">
                        <asp:TextBox ID="txtMessage" runat="server" placeholder="Type your message..." CssClass="w-full p-3 rounded-lg border border-gray-300"></asp:TextBox>
                        <asp:Button
                            ID="btnSend"
                            runat="server"
                            CssClass="bg-blue-600 text-white p-3 rounded-full hover:bg-blue-700 flex items-center justify-center ml-2"
                            OnClick="btnSend_Click"
                            Text="Send"
                            UseSubmitBehavior="false" />
                        <asp:Button
                            ID="btnVideoCall"
                            runat="server"
                            CssClass="bg-green-600 text-white p-3 rounded-full hover:bg-green-700 flex items-center justify-center ml-2"
                            OnClick="btnVideoCall_Click"
                            Text="Video Call"
                            UseSubmitBehavior="false" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
