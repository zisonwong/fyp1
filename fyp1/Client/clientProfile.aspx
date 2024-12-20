<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clientProfile.aspx.cs" Inherits="fyp1.Client.clientProfile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" href="Images/tabLogo.svg"/>
    <link rel="preconnect" href="https://fonts.gstatic.com/" crossorigin="" />

    <title>Profile</title>
    <link rel="icon" type="image/x-icon" href="data:image/x-icon;base64," />
    <link href="../CSS/clientProfile.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet" />

    <style>
        html, body {
            height: 100%;
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        .tab-link {
            background: none;
            border: none;
            color: #4a5568;
            font-size: 1.05rem;
            font-weight: 500;
            text-align: left;
            cursor: pointer;
            padding: 8px 0;
            padding-left: 5px;
        }

            .tab-link:hover {
                color: #3182ce;
                background-color: #e2e8f0;
            }

            .tab-link:focus {
                outline: none;
            }

        .logout-btn {
            background: none;
            border: none;
            color: #4a5568;
            font-size: 1.05rem;
            font-weight: 500;
            text-align: left;
            cursor: pointer;
            padding: 8px 0;
            padding-left: 5px;
        }

            .logout-btn:hover {
                color: #e53e3e;
                background-color: #edabab;
            }

            .logout-btn:focus {
                outline: none;
            }

        .settings-panel {
            background-color: #f9fafb;
        }

        .settings-card {
            transition: box-shadow 0.3s ease;
        }

            .settings-card:hover {
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            }

        .settings-button {
            color: #1d4ed8;
        }

        .form-row {
            display: flex;
            align-items: center;
            margin-bottom: 12px;
        }

        .label {
            width: 150px;
            color: #4a5568;
            font-weight: 500;
        }

        .checkbox {
            margin-left: 8px;
        }

    </style>
</head>

<body class="bg-gray">
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" />
        <div class="flex h-full w-screen">

            <!-- Sidebar -->
            <aside class="lg:w-1/6 bg-white shadow-lg p-6">
                <div class="text-center mb-8">
                    <asp:Image ID="imgProfilePicture" runat="server" CssClass="rounded-full w-24 h-24 mx-auto mt-6" />
                    <h2 class="text-2xl font-bold mt-4 text-gray-800" runat="server" id="lblName"></h2>
                    <asp:Label ID="lblIC" runat="server" CssClass="text-gray-600 mt-1"></asp:Label>
                </div>

                <aside class="bg-gray-50 py-4 px-4 rounded-lg shadow-lg">
                    <div class="space-y-4">
                        <asp:Button ID="btnHome" Text="Back to Home" CssClass="tab-link text-left w-full px-4 py-2 hover:bg-blue-200 transition ease-in-out duration-200" OnClick="btnHome_Click" runat="server" />
                        <asp:Button ID="btnProfile" Text="Profile Management" CssClass="tab-link text-left w-full px-4 py-2 hover:bg-blue-200 transition ease-in-out duration-200" OnClientClick="showTab('panelProfile', this); return false;" CausesValidation="false" runat="server" />
                        <asp:Button ID="btnAppointment" Text="Appointment Scheduling" CssClass="tab-link text-left w-full px-4 py-2 hover:bg-blue-200 transition ease-in-out duration-200" OnClientClick="showTab('panelAppointment', this); return false;" CausesValidation="false" runat="server" />
                        <asp:Button ID="btnEmergency" Text="Emergency Contact" CssClass="tab-link text-left w-full px-4 py-2 hover:bg-blue-200 transition ease-in-out duration-200" OnClientClick="showTab('panelEmergency', this); return false;" CausesValidation="false" runat="server" />
                        <asp:Button ID="btnPayment" Text="Payment" CssClass="tab-link text-left w-full px-4 py-2 hover:bg-blue-200 transition ease-in-out duration-200" OnClientClick="showTab('panelPayment', this); return false;" CausesValidation="false" runat="server" />
                        <asp:Button ID="btnSettings" Text="Settings" CssClass="tab-link text-left w-full px-4 py-2 hover:bg-blue-200 transition ease-in-out duration-200" OnClientClick="showTab('panelSettings', this); return false;" CausesValidation="false" runat="server" />

                        <asp:Button ID="btnLogout" Text="Logout" CssClass="logout-btn text-left w-full px-4 py-2 hover:text-red-600 transition ease-in-out duration-200" OnClick="signOutBtn_Click" runat="server" />
                    </div>
                </aside>
            </aside>

            <!-- Main Content -->
            <main class="flex-grow bg-gray-100 p-8">
                <div class="bg-white shadow-lg rounded-xl p-8 ">
                    <!-- Profile Management Panel -->
                    <asp:Panel ID="panelProfile" runat="server" CssClass="tab-content">
                        <div class="flex items-center mb-6">
                            <h3 class="text-2xl font-semibold text-gray-800">Profile Management</h3>
                        </div>
                        <div class="bg-white shadow-md rounded-lg p-6">
                            <!-- Name Field -->
                            <div class="mb-4">
                                <label class="font-bold text-gray-700" for="txtName">Name:</label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="border rounded w-full p-2 text-gray-600" ReadOnly="true" />
                            </div>
                            <!-- Date of Birth Field -->
                            <div class="mb-4">
                                <label class="font-bold text-gray-700" for="txtDOB">Date of Birth:</label>
                                <asp:TextBox ID="txtDOB" runat="server" ReadOnly="true" CssClass="border rounded w-full p-2 text-gray-600" />
                            </div>
                            <!-- Email Field -->
                            <div class="mb-4">
                                <label class="font-bold text-gray-700" for="txtEmail">Email:</label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="border rounded w-full p-2 text-gray-600" ReadOnly="true" />
                            </div>
                            <!-- Phone Field -->
                            <div class="mb-4">
                                <label class="font-bold text-gray-700" for="txtPhone">Phone:</label>
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="border rounded w-full p-2 text-gray-600" ReadOnly="true" />
                            </div>
                            <!-- Blood Type Field -->
                            <div class="mb-4">
                                <label class="font-bold text-gray-700" for="txtBloodType">Blood Type:</label>
                                <asp:TextBox ID="txtBloodType" runat="server" CssClass="border rounded w-full p-2 text-gray-600" ReadOnly="true" />
                            </div>
                            <!-- Edit, Save, and Cancel Buttons -->
                            <div class="text-right mt-4">
                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="bg-blue-500 text-white py-2 px-4 rounded" OnClick="btnEdit_Click" />
                                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="bg-green-500 text-white py-2 px-4 rounded hidden" OnClick="btnSave_Click" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="bg-gray-500 text-white py-2 px-4 rounded hidden" OnClick="btnCancel_Click" />
                            </div>
                        </div>
                    </asp:Panel>
                    <!-- Appointment Scheduling Panel -->
                    <asp:Panel ID="panelAppointment" runat="server" CssClass="tab-content hidden bg-white shadow-md rounded-lg p-6 mb-8">
                        <div class="flex items-center mb-6">
                            <h3 class="text-3xl font-bold text-gray-800 mb-2">Appointment Scheduling</h3>
                        </div>
                        <asp:GridView ID="gridAppointment" runat="server" AutoGenerateColumns="false" CssClass="table-auto w-full border border-gray-200 rounded-lg" OnRowCommand="gridAppointment_RowCommand" OnRowDataBound="gridAppointment_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="AppointmentID" HeaderText="Appointment ID" HeaderStyle-CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" ItemStyle-CssClass="px-4 py-2" />
                                <asp:BoundField DataField="Name" HeaderText="Doctor Name" HeaderStyle-CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" ItemStyle-CssClass="px-4 py-2" />
                                <asp:BoundField DataField="AppointmentDate" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" ItemStyle-CssClass="px-4 py-2" />
                                <asp:BoundField DataField="StartTime" HeaderText="Time" HeaderStyle-CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" ItemStyle-CssClass="px-4 py-2" />
                                <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" ItemStyle-CssClass="px-4 py-2" />
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="EditAppointment" CommandArgument='<%# Eval("AppointmentID") %>' CssClass="bg-blue-500 text-white py-2 px-4 rounded" />
                                        <asp:Button ID="btnPay" runat="server" Text="Pay" CommandName="PayAppointment" CommandArgument='<%# Eval("AppointmentID") %>' CssClass="bg-green-500 text-white py-2 px-4 rounded" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <asp:Button ID="btnAddAppointment" runat="server" Text="Add Appointment" CssClass="bg-blue-500 text-white px-4 py-2 mt-2 rounded w-1/4" OnClick="btnAddAppointment_Click" />

                        <asp:Label ID="lblNoAppointments" runat="server" Text="" CssClass="text-gray-500 italic" Visible="false"></asp:Label>
                    </asp:Panel>

                    <!-- Emergency Contact Panel -->
                    <asp:Panel ID="panelEmergency" runat="server" CssClass="tab-content hidden bg-white shadow-md rounded-lg p-6 mb-8">
                        <div class="flex items-center justify-between mb-6">
                            <h3 class="text-3xl font-bold text-gray-800">Emergency Contact</h3>
                        </div>

                        <asp:UpdatePanel ID="UpdatePanelEmergency" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:GridView ID="gridEmergency" runat="server" AutoGenerateColumns="false"
                                    CssClass="table-auto w-full border border-gray-200 rounded-lg"
                                    DataKeyNames="ContactID"
                                    OnRowEditing="gridEmergency_RowEditing"
                                    OnRowUpdating="gridEmergency_RowUpdating"
                                    OnRowCancelingEdit="gridEmergency_RowCancelingEdit"
                                    OnRowDeleting="gridEmergency_RowDeleting">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Contact Name">
                                            <HeaderStyle CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" />
                                            <ItemStyle CssClass="px-4 py-2" />
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtContactName" runat="server" Text='<%# Bind("ContactName") %>' CssClass="border rounded w-full p-2" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblContactName" runat="server" Text='<%# Eval("ContactName") %>' CssClass="text-gray-700" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Relationship">
                                            <HeaderStyle CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" />
                                            <ItemStyle CssClass="px-4 py-2" />
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtRelationship" runat="server" Text='<%# Bind("Relationship") %>' CssClass="border rounded w-full p-2" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblRelationship" runat="server" Text='<%# Eval("Relationship") %>' CssClass="text-gray-700" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Phone">
                                            <HeaderStyle CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" />
                                            <ItemStyle CssClass="px-4 py-2" />
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtPhone" runat="server" Text='<%# Bind("Phone") %>' CssClass="border rounded w-full p-2" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblPhone" runat="server" Text='<%# Eval("Phone") %>' CssClass="text-gray-700" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Email">
                                            <HeaderStyle CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" />
                                            <ItemStyle CssClass="px-4 py-2" />
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEmail" runat="server" Text='<%# Bind("Email") %>' CssClass="border rounded w-full p-2" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email") %>' CssClass="text-gray-700" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:CommandField ShowEditButton="true" ShowDeleteButton="true"
                                            EditText="Edit" DeleteText="Delete" ButtonType="Button"
                                            ItemStyle-CssClass="text-center space-x-2" />
                                    </Columns>
                                </asp:GridView>


                                <asp:Button ID="btnAddContact" runat="server" Text="Add New Contact" CssClass="bg-blue-500 text-white px-4 py-2 mt-2 rounded w-1/4" OnClick="btnAddContact_Click" />

                                <asp:Panel ID="panelAddContact" runat="server" CssClass="mt-6">
                                    <h4 class="text-xl font-bold mb-4">Add New Contact</h4>
                                    <div class="grid grid-cols-2 gap-4">
                                        <asp:TextBox ID="txtNewName" runat="server" CssClass="border rounded-lg p-2" Placeholder="Contact Name" />
                                        <asp:TextBox ID="txtNewRelationship" runat="server" CssClass="border rounded-lg p-2" Placeholder="Relationship" />
                                        <asp:TextBox ID="txtNewPhone" runat="server" CssClass="border rounded-lg p-2" Placeholder="Phone" />
                                        <asp:TextBox ID="txtNewEmail" runat="server" CssClass="border rounded-lg p-2" Placeholder="Email" />
                                    </div>
                                    <asp:Button ID="btnSaveContact" runat="server" Text="Save" CssClass="btn bg-green-500 text-white mt-4 px-4 py-2 rounded-md" OnClick="btnSaveContact_Click" />
                                </asp:Panel>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnAddContact" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnSaveContact" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>

                    </asp:Panel>


                    <!-- Payment Panel -->
                    <asp:Panel ID="panelPayment" runat="server" CssClass="tab-content hidden bg-white shadow-md rounded-lg p-6 mb-8">
                        <div class="flex items-center mb-6">
                            <h3 class="text-3xl font-bold text-gray-800 mb-2">Payment</h3>
                        </div>
                        <asp:GridView ID="gridPayment" runat="server" AutoGenerateColumns="false" CssClass="table-auto w-full border border-gray-200 rounded-lg">
                            <Columns>
                                <asp:BoundField DataField="PaymentID" HeaderText="Payment ID" HeaderStyle-CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" ItemStyle-CssClass="px-4 py-2" />
                                <asp:BoundField
                                    DataField="PaymentAmount"
                                    HeaderText="Amount (RM)"
                                    HeaderStyle-CssClass="px-4 py-2 bg-gray-100 text-left font-semibold"
                                    ItemStyle-CssClass="px-4 py-2"
                                    DataFormatString="{0:N2}" />

                                <asp:BoundField DataField="PaymentDate" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" HeaderStyle-CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" ItemStyle-CssClass="px-4 py-2" />
                                <asp:BoundField DataField="PaymentMethod" HeaderText="Payment Method" HeaderStyle-CssClass="px-4 py-2 bg-gray-100 text-left font-semibold" ItemStyle-CssClass="px-4 py-2" />
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                    <!-- Settings Panel -->
                    <asp:Panel ID="panelSettings" runat="server" CssClass="tab-content hidden">
                        <div class="settings-panel p-6 bg-gray-100 min-h-screen">
                            <h2 class="text-3xl font-bold text-gray-700 mb-6">Settings</h2>

                            <div class="grid gap-6 md:grid-cols-2 lg:grid-cols-2">
                                <!-- Change Password Card -->
                                <div class="bg-white rounded-lg shadow-lg p-5">
                                    <h3 class="text-xl font-semibold text-gray-800 mb-4">Change Password</h3>
                                    <p class="text-gray-600 mb-4">Update your account password.</p>
                                    <asp:Button ID="btnChangePassword" runat="server" Text="Edit Password"
                                        CssClass="text-blue-600 underline cursor-pointer hover:text-blue-800 transition duration-200"
                                        OnClientClick="toggleModal('changePasswordModal'); return false;" />
                                </div>

                                <!-- Profile Picture Card -->
                                <div class="bg-white rounded-lg shadow-lg p-5">
                                    <h3 class="text-xl font-semibold text-gray-800 mb-4">Change Profile</h3>
                                    <asp:FileUpload ID="fileUploadProfile" runat="server" CssClass="mt-4" />
                                    <asp:Button ID="btnUpload" runat="server" Text="Upload Profile Picture" OnClick="btnUpload_Click" CssClass="bg-blue-500 text-white px-4 py-2 mt-2 rounded w-1/2" />
                                </div>
                            </div>

                        </div>
                        <!-- Change Password Modal -->
                        <div id="changePasswordModal" class="fixed inset-0 hidden bg-gray-900 bg-opacity-50 flex items-center justify-center">
                            <div class="bg-white rounded-lg shadow-lg p-6 w-full max-w-md">
                                <h3 class="text-2xl font-semibold text-gray-800 mb-4">Change Password</h3>
                                <asp:Label ID="lblCurrentPassword" runat="server" Text="Current Password:" CssClass="block text-gray-700"></asp:Label>
                                <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="border rounded w-full p-2" TextMode="Password"></asp:TextBox>

                                <asp:Label ID="lblNewPassword" runat="server" Text="New Password:" CssClass="block text-gray-700 mt-4"></asp:Label>
                                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="border rounded w-full p-2" TextMode="Password"></asp:TextBox>

                                <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm New Password:" CssClass="block text-gray-700 mt-4"></asp:Label>
                                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="border rounded w-full p-2" TextMode="Password"></asp:TextBox>

                                <div class="flex justify-end mt-6">
                                    <button type="button" class="px-4 py-2 bg-gray-500 text-white rounded mr-2" onclick="toggleModal('changePasswordModal')">Cancel</button>
                                    <asp:Button ID="btnSubmitChangePassword" runat="server" Text="Save" CssClass="px-4 py-2 bg-blue-600 text-white rounded" OnClick="btnChangePassword_Click" />
                                </div>
                            </div>
                        </div>
                        <asp:Label ID="lblErrorMessage" runat="server"></asp:Label>
                    </asp:Panel>
                </div>
            </main>
        </div>
    </form>

    <script>
        function showTab(panelId, button) {
            // Hide all tab content panels
            document.querySelectorAll('.tab-content').forEach(panel => {
                panel.classList.add('hidden');
            });

            // Show the selected tab content
            document.getElementById(panelId).classList.remove('hidden');

            // Remove active class from all buttons
            const tabs = document.querySelectorAll('.tab-link');
            tabs.forEach(tab => {
                tab.classList.remove('bg-blue-200');
            });

            // Add active class to the clicked button
            button.classList.add('bg-blue-200');
        }

        function toggleModal(modalId) {
            const modal = document.getElementById(modalId);
            modal.classList.toggle('hidden');
        }
    </script>
</body>
</html>
