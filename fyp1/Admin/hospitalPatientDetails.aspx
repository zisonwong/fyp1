<%@ Page Title="Patient Details" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalPatientDetails.aspx.cs" Inherits="fyp1.Admin.hospitalPatientDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../layout/bootstrap.bundle.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content container">
        <div class="col-xxl-12 col-lg-12">
            <h3>Patient Details</h3>
            <div class="row">
                <div class="col-xxl-4 col-lg-4">
                    <div class="card border-0 shadow h-100">
                        <div class="card-body center">
                            <asp:Image ID="imgAvatar" runat="server" CssClass="card-img-top profile-image rounded-circle" Width="150px" Height="150px" ImageUrl="~/hospitalImg/defaultAvatar.jpg" />
                            <h4 class="card-title mt-2">
                                <asp:Label ID="lblPatientName" runat="server" Text="Name"></asp:Label></h4>
                            <h6 class="card-subtitle mt-2">
                                <asp:Label ID="lblPatientEmail" runat="server" Text="Email"></asp:Label></h6>
                            <%-- <% if (Session["Role"].ToString() == "Doctor")
                            { %>
                            <asp:LinkButton CssClass="mt-2 btn btn-primary" ID="btnSendMsg" runat="server">Send Message</asp:LinkButton>
                             <% } %>--%>
                        </div>
                    </div>
                </div>
                <div class="col-xxl-8 col-lg-8">
                    <div class="card border-0 shadow h-100">
                        <div class="card-body row d-flex flex-wrap">
                            <div class="row align-items-center">
                                <div class="col-md-4">
                                    <div>
                                        <h6>Gender</h6>
                                    </div>
                                    <div>
                                        <asp:Label ID="lblGender" runat="server" Text="Gender"></asp:Label>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div>
                                        <h6>IC</h6>
                                    </div>
                                    <div>
                                        <asp:Label ID="lblIc" runat="server" Text="IC"></asp:Label>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div>
                                        <h6>Birthday</h6>
                                    </div>
                                    <div>
                                        <asp:Label ID="lblBirthday" runat="server" Text="Birthday"></asp:Label>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div>
                                        <h6>Contact</h6>
                                    </div>
                                    <div>
                                        <asp:Label ID="lblContact" runat="server" Text="Contact"></asp:Label>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div>
                                        <h6>Blood Type</h6>
                                    </div>
                                    <div>
                                        <asp:Label ID="lblBloodType" runat="server" Text="Blood Type"></asp:Label>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div>
                                        <h6>Address</h6>
                                    </div>
                                    <div>
                                        <asp:Label ID="lblAddress" runat="server" Text="Address"></asp:Label>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div>
                                        <h6>Medical Record</h6>
                                    </div>
                                    <div>
                                        <asp:Label ID="lblMedical" runat="server" Text="0"></asp:Label>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div>
                                        <h6>Appointment Record</h6>
                                    </div>
                                    <div>
                                        <asp:Label ID="lblAppointment" runat="server" Text="0"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card border-0 shadow mt-4">
            <div class="card-body">
                <ul class="nav nav-pills content fit p-2" id="myTab" role="tablist">
                    <li class="nav-item" role="presentation">
                        <button class="nav-link fw-bold active" id="upcoming-pill" data-bs-toggle="pill" data-bs-target="#upcoming" type="button" role="tab" aria-controls="upcoming" aria-selected="true">
                            Upcoming Appointments
                       
                        </button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button class="nav-link fw-bold" id="past-pill" data-bs-toggle="pill" data-bs-target="#past" type="button" role="tab" aria-controls="past" aria-selected="false">
                            Past Appointments
               
                        </button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button class="nav-link fw-bold" id="medical-pill" data-bs-toggle="pill" data-bs-target="#medical" type="button" role="tab" aria-controls="medical" aria-selected="false">
                            Medical Records
               
                        </button>
                    </li>
                </ul>

                <div class="tab-content mt-3" id="myTabContent">
                    <div class="tab-pane fade show active" id="upcoming" role="tabpanel" aria-labelledby="upcoming-pill">
                        <asp:Repeater ID="rptUpcomingAppointments"
                            OnItemCommand="rptUpcomingAppointments_ItemCommand"
                            runat="server">
                            <ItemTemplate>
                                <div class="card mb-3 shadow-sm">
                                    <div class="card-body p-0">
                                        <div class="row align-items-center">
                                            <div class="col-md-3 border-end d-flex justify-content-center">
                                                <div>
                                                    <h3><%# Eval("date", "{0:dd MMM yyyy}") %></h3>
                                                    <div class="d-flex">
                                                        <p class="mb-0"><%# Eval("startTime", "{0:hh\\:mm}") %></p>
                                                        <p class="mb-0">-</p>
                                                        <p class="mb-0"><%# Eval("endTime", "{0:hh\\:mm}") %></p>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-3 d-flex justify-content-center border-end">
                                                <div>
                                                    <p class="mb-0">Doctor Email:</p>
                                                    <strong><%# Eval("doctorEmail") %></strong>
                                                </div>
                                            </div>
                                            <div class="col-md-2 d-flex justify-content-center">
                                                <div>
                                                    <p class="mb-0">Doctor:</p>
                                                    <strong><%# Eval("doctorID") %></strong>
                                                </div>
                                            </div>
                                            <div class="col-md-2 d-flex justify-content-center">
                                                <div>
                                                    <p class="mb-0">Name:</p>
                                                    <strong><%# Eval("doctorName") %></strong>
                                                </div>
                                            </div>

                                            <div class="col-md-2 d-flex justify-content-center">
                                                <asp:LinkButton ID="btnSendEmail" runat="server" CssClass="btn btn-primary"
                                                    CommandName="SendEmail" CommandArgument='<%# Eval("appointmentID") %>'>
                                                    Notify
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="tab-pane fade" id="past" role="tabpanel" aria-labelledby="past-pill">
                        <asp:ListView
                            OnItemCommand="lvPastAppointment_ItemCommand"
                            ID="lvPastAppointment" runat="server">
                            <LayoutTemplate>
                                <table class="table table-responsive-md table-hover">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Appointment ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Patient ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Doctor ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Time</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Date</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Status</asp:LinkButton></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr style="cursor: pointer;"
                                    onclick="document.getElementById('<%# ((Control)Container).FindControl("btnRowClick").ClientID %>').click();">
                                    <asp:LinkButton
                                        ID="btnRowClick"
                                        runat="server"
                                        CommandName="SelectAppointment"
                                        CommandArgument='<%# Eval("appointmentID") %>'
                                        CssClass="d-none" />
                                    <td><%# Eval("appointmentID") %></td>
                                    <td><%# Eval("patientID") %></td>
                                    <td><%# Eval("doctorID") %></td>
                                    <td><%# Eval("time", "{0:hh\\:mm\\:ss}") %></td>
                                    <td><%# Eval("date", "{0:dd/MM/yyyy}") %></td>
                                    <td><%# Eval("status") %></td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <tr>
                                    <td colspan="6" class="text-center">No data here...</td>
                                </tr>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </div>
                    <div class="tab-pane fade" id="medical" role="tabpanel" aria-labelledby="medical-pill">
                        <asp:ListView
                            ID="lvMedicalRecord" runat="server">
                            <LayoutTemplate>
                                <table class="table table-responsive-md table-hover">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Record ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Doctor ID</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Doctor Name</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Date</asp:LinkButton></th>
                                            <th>
                                                <asp:LinkButton runat="server" CssClass="sortable-header">Medicine</asp:LinkButton></th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("recordID") %></td>
                                    <td><%# Eval("doctorID") %></td>
                                    <td><%# Eval("doctorName") %></td>
                                    <td><%# Eval("recordDate", "{0:dd/MM/yyyy}") %></td>
                                    <td><%# Eval("prescriptionNumber") %></td>
                                    <td>
                                        <asp:LinkButton ID="btnCheckDetails"
                                            CssClass="btn btn-primary fw-bold"
                                            runat="server"
                                            OnClick="btnCheckDetails_Click"
                                            CommandArgument='<%# Eval("recordID") %>'>
                                        Check Details
                                        </asp:LinkButton>

                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <tr>
                                    <td colspan="6" class="text-center">No data here...</td>
                                </tr>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </div>
                </div>
            </div>
        </div>
    </main>
</asp:Content>
