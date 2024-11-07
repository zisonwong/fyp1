<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clientProfile.aspx.cs" Inherits="fyp1.Client.clientProfile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="preconnect" href="https://fonts.gstatic.com/" crossorigin="" />

    <title></title>
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
            color: #4a5568; /* text-gray-700 */
            font-weight: 500;
        }

        .checkbox {
            margin-left: 8px;
        }
    </style>
</head>

<body class="bg-gray">
    <form id="form1" runat="server">
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
                        <asp:Button ID="btnProfile" Text="Profile Management" CssClass="tab-link text-left w-full px-4 py-2 hover:bg-blue-200 transition ease-in-out duration-200" OnClientClick="showTab('panelProfile', this); return false;" CausesValidation="false" runat="server" />
                        <asp:Button ID="btnAppointment" Text="Appointment Scheduling" CssClass="tab-link text-left w-full px-4 py-2 hover:bg-blue-200 transition ease-in-out duration-200" OnClientClick="showTab('panelAppointment', this); return false;" CausesValidation="false" runat="server" />
                        <asp:Button ID="btnInsurance" Text="Insurance" CssClass="tab-link text-left w-full px-4 py-2 hover:bg-blue-200 transition ease-in-out duration-200" OnClientClick="showTab('panelInsurance', this); return false;" CausesValidation="false" runat="server" />
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
                            <div class="mb-4">
                                <label class="font-bold text-gray-700" for="txtName">Name:</label>
                                <asp:TextBox ID="txtName" runat="server" ReadOnly="true" CssClass="border rounded w-full p-2 text-gray-600" />
                            </div>
                            <div class="mb-4">
                                <label class="font-bold text-gray-700" for="txtDOB">Date of Birth:</label>
                                <asp:TextBox ID="txtDOB" runat="server" ReadOnly="true" CssClass="border rounded w-full p-2 text-gray-600" />
                            </div>
                            <div class="mb-4">
                                <label class="font-bold text-gray-700" for="txtEmail">Email:</label>
                                <asp:TextBox ID="txtEmail" runat="server" ReadOnly="true" CssClass="border rounded w-full p-2 text-gray-600" />
                            </div>
                            <div class="mb-4">
                                <label class="font-bold text-gray-700" for="txtPhone">Phone:</label>
                                <asp:TextBox ID="txtPhone" runat="server" ReadOnly="true" CssClass="border rounded w-full p-2 text-gray-600" />
                            </div>
                            <div class="mb-4">
                                <label class="font-bold text-gray-700" for="txtBloodType">Blood Type:</label>
                                <asp:TextBox ID="txtBloodType" runat="server" ReadOnly="true" CssClass="border rounded w-full p-2 text-gray-600" />
                            </div>
                        </div>
                    </asp:Panel>

                    <!-- Appointment Scheduling Panel -->
                    <asp:Panel ID="panelAppointment" runat="server" CssClass="tab-content hidden">
                        <div class="flex items-center mb-6">
                            <h3 class="text-2xl font-semibold text-gray-800">Appointment Scheduling</h3>
                        </div>
                        <asp:GridView ID="gridAppointment" runat="server" AutoGenerateColumns="false" CssClass="table-auto w-full">
                            <Columns>
                                <asp:BoundField DataField="AppointmentID" HeaderText="Appointment ID" />
                                <asp:BoundField DataField="DoctorName" HeaderText="Doctor Name" />
                                <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                                <asp:BoundField DataField="Time" HeaderText="Time" />
                                <asp:BoundField DataField="Status" HeaderText="Status" />
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>

                    <!-- Insurance Panel -->
                    <asp:Panel ID="panelInsurance" runat="server" CssClass="tab-content hidden">
                        <div class="flex items-center mb-6">
                            <h3 class="text-2xl font-semibold text-gray-800">Insurance</h3>
                        </div>
                        <asp:GridView ID="gridInsurance" runat="server" AutoGenerateColumns="false" CssClass="table-auto w-full">
                            <Columns>
                                <asp:BoundField DataField="PolicyID" HeaderText="Policy ID" />
                                <asp:BoundField DataField="ProviderName" HeaderText="Provider Name" />
                                <asp:BoundField DataField="PolicyType" HeaderText="Policy Type" />
                                <asp:BoundField DataField="ExpirationDate" HeaderText="Expiration Date" DataFormatString="{0:MM/dd/yyyy}" />
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>

                    <!-- Emergency Contact Panel -->
                    <asp:Panel ID="panelEmergency" runat="server" CssClass="tab-content hidden">
                        <div class="flex items-center mb-6">
                            <h3 class="text-2xl font-semibold text-gray-800">Emergency Contact</h3>
                        </div>
                        <asp:GridView ID="gridEmergency" runat="server" AutoGenerateColumns="false" CssClass="table-auto w-full">
                            <Columns>
                                <asp:BoundField DataField="ContactName" HeaderText="Contact Name" />
                                <asp:BoundField DataField="Relationship" HeaderText="Relationship" />
                                <asp:BoundField DataField="Phone" HeaderText="Phone" />
                                <asp:BoundField DataField="Email" HeaderText="Email" />
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>

                    <!-- Payment Panel -->
                    <asp:Panel ID="panelPayment" runat="server" CssClass="tab-content hidden">
                        <div class="flex items-center mb-6">
                            <h3 class="text-2xl font-semibold text-gray-800">Payment</h3>
                        </div>
                        <asp:GridView ID="gridPayment" runat="server" AutoGenerateColumns="false" CssClass="table-auto w-full">
                            <Columns>
                                <asp:BoundField DataField="PaymentID" HeaderText="Payment ID" />
                                <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:C}" />
                                <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                                <asp:BoundField DataField="PaymentMethod" HeaderText="Payment Method" />
                                <asp:BoundField DataField="Status" HeaderText="Status" />
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>

                    <!-- Settings Panel -->
                    <asp:Panel ID="panelSettings" runat="server" CssClass="tab-content hidden">
                        <div class="settings-panel p-6 bg-gray-100 min-h-screen">
                            <h2 class="text-3xl font-bold text-gray-700 mb-6">Settings</h2>

                            <div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                                <!-- Change Password Card -->
                                <div class="bg-white rounded-lg shadow-lg p-5">
                                    <h3 class="text-xl font-semibold text-gray-800 mb-4">Change Password</h3>
                                    <p class="text-gray-600 mb-4">Update your account password.</p>
                                    <asp:Button ID="btnChangePassword" runat="server" Text="Edit Password"
                                        CssClass="text-blue-600 underline cursor-pointer hover:text-blue-800 transition duration-200"
                                        OnClientClick="toggleModal('changePasswordModal'); return false;" />
                                </div>

                                <!-- Notifications Card -->
                                <div class="bg-white rounded-lg shadow-lg p-5">
                                    <h3 class="text-xl font-semibold text-gray-800 mb-4">Notifications</h3>
                                    <p class="text-gray-600 mb-4">Manage notification preferences.</p>
                                    <asp:Button ID="btnNotificationSettings" runat="server" Text="Edit Notifications"
                                        CssClass="text-blue-600 underline cursor-pointer hover:text-blue-800 transition duration-200"
                                        OnClientClick="toggleModal('notificationModal'); return false;" />
                                </div>

                                <!-- Privacy Settings Card -->
                                <div class="bg-white rounded-lg shadow-lg p-5">
                                    <h3 class="text-xl font-semibold text-gray-800 mb-4">Privacy Settings</h3>
                                    <p class="text-gray-600 mb-4">Adjust your privacy options.</p>
                                    <asp:Button ID="btnPrivacySettings" runat="server" Text="Edit Privacy"
                                        CssClass="text-blue-600 underline cursor-pointer hover:text-blue-800 transition duration-200"
                                        OnClientClick="toggleModal('privacyModal'); return false;" />
                                </div>
                            </div>

                            <asp:FileUpload ID="fileUploadProfile" runat="server" CssClass="mt-4" />
                            <asp:Button ID="btnUpload" runat="server" Text="Upload Profile Picture" OnClick="btnUpload_Click" CssClass="bg-blue-500 text-white px-4 py-2 mt-2 rounded w-1/4" />
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

                        <!-- Notification Settings Modal -->
                        <div id="notificationModal" class="fixed inset-0 hidden bg-gray-900 bg-opacity-50 flex items-center justify-center">
                            <div class="bg-white rounded-lg shadow-lg p-6 w-full max-w-md">
                                <h3 class="text-2xl font-semibold text-gray-800 mb-4">Notification Settings</h3>

                                <div class="form-row">
                                    <asp:Label ID="lblEmailNotifications" runat="server" Text="Email Notifications:" CssClass="label"></asp:Label>
                                    <asp:CheckBox ID="chkEmailNotifications" runat="server" CssClass="checkbox" />
                                </div>

                                <div class="form-row">
                                    <asp:Label ID="lblSMSNotifications" runat="server" Text="SMS Notifications:" CssClass="label"></asp:Label>
                                    <asp:CheckBox ID="chkSMSNotifications" runat="server" CssClass="checkbox" />
                                </div>

                                <div class="form-row">
                                    <asp:Label ID="lblPushNotifications" runat="server" Text="Push Notifications:" CssClass="label"></asp:Label>
                                    <asp:CheckBox ID="chkPushNotifications" runat="server" CssClass="checkbox" />
                                </div>

                                <div class="flex justify-end mt-6">
                                    <button type="button" class="px-4 py-2 bg-gray-500 text-white rounded mr-2" onclick="toggleModal('notificationModal')">Cancel</button>
                                    <asp:Button ID="btnSubmitNotificationSettings" runat="server" Text="Save" CssClass="px-4 py-2 bg-blue-600 text-white rounded" OnClick="btnNotificationSettings_Click" />
                                </div>
                            </div>
                        </div>

                        <!-- Privacy Settings Modal -->
                        <div id="privacyModal" class="fixed inset-0 hidden bg-gray-900 bg-opacity-50 flex items-center justify-center">
                            <div class="bg-white rounded-lg shadow-lg p-8 w-full max-w-lg">
                                <h3 class="text-2xl font-semibold text-gray-800 mb-6">Privacy Settings</h3>

                                <!-- Profile Visibility Dropdown -->
                                <div class="mb-4">
                                    <asp:Label ID="lblProfileVisibility" runat="server" Text="Profile Visibility:" CssClass="block text-gray-700 font-medium mb-2"></asp:Label>
                                    <asp:DropDownList ID="ddlProfileVisibility" runat="server" CssClass="border border-gray-300 rounded-lg w-full px-3 py-2">
                                        <asp:ListItem Text="Public" Value="Public"></asp:ListItem>
                                        <asp:ListItem Text="Private" Value="Private"></asp:ListItem>
                                        <asp:ListItem Text="Friends Only" Value="FriendsOnly"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <!-- Data Sharing Checkbox -->
                                <div class="flex items-center mb-4">
                                    <asp:Label ID="lblDataSharing" runat="server" Text="Data Sharing:" CssClass="text-gray-700 font-medium w-40"></asp:Label>
                                    <asp:CheckBox ID="chkDataSharing" runat="server" Text="Allow data sharing with third-party services" CssClass="ml-2 text-gray-600" />
                                </div>

                                <!-- Ad Preferences Checkbox -->
                                <div class="flex items-center mb-4">
                                    <asp:Label ID="lblAdPreferences" runat="server" Text="Ad Preferences:" CssClass="text-gray-700 font-medium w-40"></asp:Label>
                                    <asp:CheckBox ID="chkAdPreferences" runat="server" Text="Personalized Ads" CssClass="ml-2 text-gray-600" />
                                </div>

                                <!-- Action Buttons -->
                                <div class="flex justify-end mt-6">
                                    <button type="button" class="px-4 py-2 bg-gray-400 text-white rounded hover:bg-gray-500 mr-2" onclick="toggleModal('privacyModal')">Cancel</button>
                                    <asp:Button ID="btnSubmitPrivacySettings" runat="server" Text="Save" CssClass="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700" OnClick="btnPrivacySettings_Click" />
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
