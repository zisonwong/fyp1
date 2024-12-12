<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/adminSideBar.Master" AutoEventWireup="true" CodeBehind="adminHome.aspx.cs" Inherits="fyp1.Admin.adminHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container content">
        <div class="row">
            <div class="col-12 col-sm-6 col-xxl-3 d-flex">
                <div class="card2 border-0 shadow mb-4">
                    <div class="card-body p-0">
                        <di class="row g-0 w-100">
                            <div class="col-6 p-3">
                                <h6 class="card-title">Welcome Back,
                                <asp:Label ID="lblDashboardStaffName" runat="server" Text="Label"></asp:Label></h6>
                                <p class="mb-0">Bakataro Dashboard</p>
                            </div>
                            <div class="col-6 align-self-sm-end">
                                <img src="../hospitalImg/admin.png" class="img-fluid" />
                            </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-6 col-xxl-3 d-flex flex-fill">
                <div class="card2 border-0 shadow mb-4" style="width: 100%;">
                    <div class="card-body p-3">
                        <div class="d-flex">
                            <div class="circle-background flex-grow-1">
                                <img src="Icon/clipboard.png" class="icon-size" />
                            </div>
                            <div class="ms-3" style="width: 100%;">
                                <div class="mb-2">
                                    <h6 class="card-title">Total Patient</h6>
                                </div>
                                <h3>
                                    <asp:Label ID="totalPatient" runat="server" Text="Label" /></h3>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-6 col-xxl-3 d-flex flex-fill">
                <div class="card2 border-0 shadow mb-4" style="width: 100%;">
                    <div class="card-body p-3">
                        <div class="d-flex">
                            <div class="circle-background flex-grow-1">
                                <img src="Icon/money.png" class="icon-size" />
                            </div>
                            <div class="ms-3" style="width: 100%;">
                                <div class="mb-2">
                                    <h6 class="card-title">Total Staff</h6>
                                </div>
                                <h3>
                                    <asp:Label ID="totalStaff" runat="server" Text="Label" /></h3>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-6 col-xxl-3 d-flex flex-fill">
                <div class="card2 border-0 shadow mb-4" style="width: 100%;">
                    <div class="card-body p-3">
                        <div class="d-flex">
                            <div class="circle-background flex-grow-1">
                                <img src="Icon/customer.png" class="icon-size" />
                            </div>
                            <div class="ms-3" style="width: 100%;">
                                <div class="mb-2">
                                    <h6 class="card-title">This Month Appointment</h6>
                                </div>
                                <h3>
                                    <asp:Label ID="thisMonthAppointment" runat="server" Text="Label" /></h3>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </main>
</asp:Content>
