<%@ Page Title="Chat" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalDoctorChat.aspx.cs" Inherits="fyp1.Admin.hospitalDoctorChat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" rel="stylesheet" />
    <link href="../layout/bootstrap.min.css" rel="stylesheet" />
    <style>
        .text-left {
            justify-content: flex-start;
        }

        .text-right {
            justify-content: flex-end;
        }

        .chat-container {
            max-height: calc(100vh - 24rem);
            overflow-y: auto;
        }

        .bg-blue-100 {
            background-color: #cce5ff;
            color: #004085;
        }

        .bg-green-100 {
            background-color: #d4edda;
            color: #155724;
        }

        .chat-message {
            max-width: 28rem;
        }

        .doctor-list-item:hover {
            background-color: #e9ecef;
        }

        .chat-section {
            height: 80vh;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex chat-section">
        <!-- Left Side: Doctor List -->
        <div class="col-3 bg-white p-4 border-end">
            <h2 class="fs-2 fw-semibold text-secondary">Chats</h2>
            <asp:UpdatePanel ID="UpdatePanelPatientList" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Repeater ID="RepeaterPatientList" runat="server" OnItemCommand="PatientSelected">
                        <ItemTemplate>
                            <asp:LinkButton
                                ID="lnkSelectPatient"
                                runat="server"
                                CommandName="SelectPatient"
                                CommandArgument='<%# Eval("patientID") %>'
                                CssClass="text-decoration-none d-block">
                                <div class="d-flex align-items-center w-100 text-start fs-5 p-3 bg-light rounded doctor-list-item mt-3">
                                    <asp:Image ID="imgDoctorPhoto" runat="server" ImageUrl='<%# Eval("ImageUrl") %>' CssClass="rounded-circle me-3" Width="40" Height="40" />
                                    <span class="mt-1"><%# Eval("name") %></span>
                                </div>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Right Side -->
        <div class="col bg-white p-4">
            <div class="d-flex flex-column h-100">
                <!-- Chat Messages Section -->
                <div id="chatMessages" class="flex-grow-1 p-4">
                    <asp:UpdatePanel ID="UpdatePanelChat" runat="server">
                        <ContentTemplate>
                            <div id="chatContainer" class="d-flex flex-column-reverse chat-container h-100">
                                <asp:Repeater ID="RepeaterMessages" runat="server">
                                    <ItemTemplate>
                                        <div class='<%# Eval("alignmentClass") %> d-flex mb-2 me-4'>
                                            <div class='<%# Eval("messageClass") %> p-3 rounded chat-message'>
                                                <%# Eval("content") %>
                                                <div class="small text-muted"><%# Eval("timestamp") %></div>
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

                <div id="chatInput" class="bg-light p-4 d-flex align-items-center justify-content-between mt-auto w-100">
                    <asp:TextBox ID="txtMessage" runat="server" placeholder="Type your message..." onkeydown="checkEnter(event)" 
                        CssClass="form-control me-2"></asp:TextBox>
                    <asp:Button
                        ID="btnSend"
                        runat="server"
                        CssClass="btn btn-primary rounded-circle d-flex align-items-center justify-content-center"
                        OnClick="btnSend_Click"
                        Text="Send"
                        UseSubmitBehavior="false" />
                </div>
            </div>
        </div>
    </div>
   <script type="text/javascript">
       function checkEnter(event) {
           if (event.key === "Enter" || event.keyCode === 13) {
               event.preventDefault(); 
               document.getElementById('<%= btnSend.ClientID %>').click(); 
           }
       }
   </script>
</asp:Content>
