<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="fyp1.Client.Checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <div class="min-h-screen bg-gray-50 py-8">
            <div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 mt-20">
                <!-- Payment Header -->
                <div class="mb-8">
                    <h1 class="text-3xl font-bold text-gray-900">Payment Details</h1>
                    <p class="mt-2 text-sm text-gray-600">Please complete your payment to confirm your appointment</p>
                </div>

                <!-- Main Content -->
                <div class="grid grid-cols-1 gap-8 lg:grid-cols-3">
                    <!-- Payment Form Section -->
                    <div class="col-span-2">
                        <div class="rounded-lg bg-white p-6 shadow-md">
                            <!-- Appointment Details -->
                            <div class="mb-6 border-b border-gray-200 pb-6">
                                <h2 class="mb-4 text-xl font-semibold text-gray-800">Appointment Information</h2>
                                <div class="grid grid-cols-2 gap-4">
                                    <div>
                                        <p class="text-sm text-gray-600">Doctor</p>
                                        <asp:Label ID="lblDoctorName" runat="server" CssClass="text-base font-medium text-gray-900"></asp:Label>
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-600">Date</p>
                                        <asp:Label ID="lblAppointmentDate" runat="server" CssClass="text-base font-medium text-gray-900"></asp:Label>
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-600">Time</p>
                                        <asp:Label ID="lblAppointmentTime" runat="server" CssClass="text-base font-medium text-gray-900"></asp:Label>
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-600">Consultation Fee</p>
                                        <asp:Label ID="lblConsultationFee" runat="server" CssClass="text-base font-medium text-gray-900"></asp:Label>
                                    </div>
                                </div>
                            </div>

                            <!-- Payment Method Selection -->
                            <div class="mb-6">
                                <h2 class="mb-4 text-xl font-semibold text-gray-800">Payment Method</h2>
                                <div class="grid grid-cols-2 gap-4">
                                    <!-- Credit Card Option -->
                                    <asp:Panel ID="creditCardContainer" runat="server" CssClass="relative flex cursor-pointer rounded-lg border p-4 shadow-sm focus:outline-none">
                                        <div class="flex w-full items-center justify-between">
                                            <div class="flex items-center">
                                                <div class="text-sm">
                                                    <asp:RadioButton ID="creditCard" GroupName="paymentMethod" runat="server"
                                                        Checked="true" AutoPostBack="True"
                                                        OnCheckedChanged="PaymentMethod_CheckedChanged" EnableViewState="True" />
                                                    <span class="font-medium text-gray-900">Credit Card</span>
                                                </div>
                                            </div>
                                            <div class="ml-4 flex-shrink-0">
                                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-6">
                                                    <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 8.25h19.5M2.25 9h19.5m-16.5 5.25h6m-6 2.25h3m-3.75 3h15a2.25 2.25 0 0 0 2.25-2.25V6.75A2.25 2.25 0 0 0 19.5 4.5h-15a2.25 2.25 0 0 0-2.25 2.25v10.5A2.25 2.25 0 0 0 4.5 19.5Z" />
                                                </svg>
                                            </div>
                                        </div>
                                    </asp:Panel>

                                    <!-- PayPal Option -->
                                    <asp:Panel ID="paypalContainer" runat="server" CssClass="relative flex cursor-pointer rounded-lg border p-4 shadow-sm focus:outline-none">
                                        <div class="flex w-full items-center justify-between">
                                            <div class="flex items-center">
                                                <div class="text-sm">
                                                    <asp:RadioButton ID="paypal" GroupName="paymentMethod" runat="server"
                                                        AutoPostBack="True"
                                                        OnCheckedChanged="PaymentMethod_CheckedChanged" EnableViewState="True" />
                                                    <span class="font-medium text-gray-900">PayPal</span>
                                                </div>
                                            </div>
                                            <div class="ml-4 flex-shrink-0">
                                                <i class="fa-brands fa-paypal"></i>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>

                            <!-- Credit Card Form -->
                            <asp:Panel ID="creditCardForm" runat="server" CssClass="space-y-4">
                                <div>
                                    <label class="block text-sm font-medium text-gray-700">Card Number</label>
                                    <asp:TextBox ID="txtCardNumber" runat="server"
                                        CssClass="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                                        placeholder="1234 5678 9012 3456" MaxLength="16" />
                                    <asp:Label ID="lblCreditError" runat="server" CssClass="mt-1 text-sm text-red-600" />
                                    <asp:RequiredFieldValidator ID="rfvCardNumber" runat="server"
                                        ControlToValidate="txtCardNumber"
                                        ErrorMessage="Card number is required"
                                        Display="Dynamic"
                                        CssClass="text-red-600 text-sm" />
                                </div>

                                <div class="grid grid-cols-2 gap-4">
                                    <div>
                                        <label class="block text-sm font-medium text-gray-700">Expiration Date</label>
                                        <asp:TextBox ID="txtExpirationDate" runat="server"
                                            CssClass="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                                            placeholder="MM/YY" MaxLength="5" />
                                        <asp:Label ID="lblDateError" runat="server" CssClass="mt-1 text-sm text-red-600" />
                                        <asp:RequiredFieldValidator ID="rfvExpirationDate" runat="server"
                                            ControlToValidate="txtExpirationDate"
                                            ErrorMessage="Expiration date is required"
                                            Display="Dynamic"
                                            CssClass="text-red-600 text-sm" />
                                    </div>

                                    <div>
                                        <label class="block text-sm font-medium text-gray-700">CVV</label>
                                        <asp:TextBox ID="txtCvv" runat="server"
                                            CssClass="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                                            placeholder="123" MaxLength="3" TextMode="Password" />
                                        <asp:Label ID="lblCvvError" runat="server" CssClass="mt-1 text-sm text-red-600" />
                                        <asp:RequiredFieldValidator ID="rfvCVV" runat="server"
                                            ControlToValidate="txtCvv"
                                            ErrorMessage="CVV is required"
                                            Display="Dynamic"
                                            CssClass="text-red-600 text-sm" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>

                    <!-- Order Summary Section -->
                    <div class="col-span-1">
                        <div class="rounded-lg bg-white p-6 shadow-md">
                            <h2 class="mb-4 text-xl font-semibold text-gray-800">Payment Summary</h2>

                            <div class="space-y-4">
                                <div class="flex justify-between">
                                    <span class="text-gray-600">Consultation Fee</span>
                                    <asp:Label ID="lblSummaryFee" runat="server" CssClass="font-medium text-gray-900" />
                                </div>

                                <div class="border-t border-gray-200 pt-4">
                                    <div class="flex justify-between">
                                        <span class="text-lg font-medium text-gray-900">Total</span>
                                        <asp:Label ID="lblTotal" runat="server" CssClass="text-lg font-medium text-gray-900" />
                                    </div>
                                </div>

                                <asp:Button ID="btnSubmitPayment" runat="server"
                                    Text="Complete Payment"
                                    OnClick="btnSubmitPayment_Click"
                                    CssClass="mt-6 w-full rounded-md bg-blue-600 px-4 py-3 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2" />
                            </div>
                        </div>

                        <!-- Error Messages -->
                        <asp:Label ID="lblError" runat="server" CssClass="mt-4 block text-sm text-red-600" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Payment Success Modal -->
        <asp:Panel ID="paymentSuccessModal" runat="server" CssClass="fixed inset-0 z-10 hidden mt-20">
            <div class="flex min-h-screen items-end justify-center px-4 pt-4 pb-20 text-center sm:block sm:p-0">
                <div class="fixed inset-0 transition-opacity" aria-hidden="true">
                    <div class="absolute inset-0 bg-gray-500 opacity-75"></div>
                </div>

                <div class="inline-block transform overflow-hidden rounded-lg bg-white text-left align-bottom shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg sm:align-middle">
                    <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
                        <div class="sm:flex sm:items-start">
                            <div class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-green-100 sm:mx-0 sm:h-10 sm:w-10">
                                <!-- Success Icon -->
                                <svg class="h-6 w-6 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                                </svg>
                            </div>
                            <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left">
                                <h3 class="text-lg font-medium leading-6 text-gray-900">Payment Successful!</h3>
                                <div class="mt-2">
                                    <p class="text-sm text-gray-500">
                                        Your appointment has been confirmed. You can view your appointment details in your profile.
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="bg-gray-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6">
                        <asp:Button ID="btnGoToProfile" runat="server"
                            Text="Go to Profile"
                            OnClick="GoToProfile_click"
                            CausesValidation="false"
                            CssClass="inline-flex w-full justify-center rounded-md border border-transparent bg-blue-600 px-4 py-2 text-base font-medium text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 sm:ml-3 sm:w-auto sm:text-sm" />
                        <asp:Button ID="btnCloseModal" runat="server"
                            Text="Close"
                            OnClick="CloseModal_click"
                            CausesValidation="false"
                            CssClass="mt-3 inline-flex w-full justify-center rounded-md border border-gray-300 bg-white px-4 py-2 text-base font-medium text-gray-700 shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm" />
                    </div>
                </div>
            </div>
        </asp:Panel>

    </form>
    <!-- JavaScript for Input Formatting -->
    <script type="text/javascript">
        function formatCardNumber(input) {
            let value = input.value.replace(/\D/g, '');
            input.value = value;
        }

        function formatExpirationDate(input) {
            let value = input.value.replace(/\D/g, '');
            if (value.length > 2) {
                value = value.substring(0, 2) + '/' + value.substring(2);
            }
            input.value = value;
        }

        // Add event listeners
        document.getElementById('<%= txtCardNumber.ClientID %>').addEventListener('input', function () {
            formatCardNumber(this);
        });

        document.getElementById('<%= txtExpirationDate.ClientID %>').addEventListener('input', function () {
            formatExpirationDate(this);
        });

        document.getElementById('<%= txtCvv.ClientID %>').addEventListener('input', function () {
            this.value = this.value.replace(/\D/g, '');
        });
    </script>

</asp:Content>
