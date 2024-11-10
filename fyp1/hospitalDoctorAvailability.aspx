﻿<%@ Page Title="" Language="C#" MasterPageFile="~/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalDoctorAvailability.aspx.cs" Inherits="fyp1.hospitalDoctorAvailability" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="layout/PageStyle.css" rel="stylesheet" />
    <script src="layout/availability.js"></script>
    <style>
        .availableBadges {
            display: inline-block;
            padding: 5px 10px;
            font-size: 12px;
            border-radius: 12px;
            margin: 5px;
            color: #fff;
            background-color: #007bff;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-5">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>Working Hours</h2>
            <div class="d-flex align-items-center">
               <asp:Button ID="btnPrevious" runat="server" Text="←" CssClass="btn btn-outline-secondary me-2" 
                   OnClick="btnPrevious_Click"/>
                <h4 class="mb-0 d-none" id="lblMonth">November 2024</h4>
                <asp:Label ID="lblMonth2" runat="server" CssClass="fs-4 fw-bold" Text="Label"></asp:Label>
                <asp:Button ID="btnNext" runat="server" Text="→" CssClass="btn btn-outline-secondary ms-2" 
                    OnClick="btnNext_Click"/>
            </div>
            <button type="button" class="btn btn-primary"><i class="fas fa-plus"></i></button>
        </div>
        <div class="card p-4 shadow-sm bg-white">
            <div class="row text-center fw-bold text-uppercase">
                <div class="col">Sun</div>
                <div class="col">Mon</div>
                <div class="col">Tue</div>
                <div class="col">Wed</div>
                <div class="col">Thu</div>
                <div class="col">Fri</div>
                <div class="col">Sat</div>
            </div>
            <div class="row gp-2">
                <asp:Repeater ID="rptCalendar" runat="server">
                    <ItemTemplate>
                        <div class="col border p-2 calendar-day text-center bg-white <%# !(bool)Eval("IsPlaceholder") ? "clickable-date" : "" %>"
                            data-date='<%# !(bool)Eval("IsPlaceholder") ? Eval("Day") : "" %>'>
                            <%# !(bool)Eval("IsPlaceholder") ? Eval("Day") : "" %>
                            <br />
                            <asp:Repeater ID="rptAvailabilityTimes" runat="server" DataSource='<%# Eval("AvailabilityTimes") %>'>
                                <ItemTemplate>
                                    <span class="availableBadges"><%# Container.DataItem %></span><br />
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    <!-- Availability Modal -->
    <div class="modal fade" id="availabilityModal" tabindex="-1" aria-labelledby="availabilityModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="availabilityModalLabel">Set Availability</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3 d-flex">
                        <div class="col-5 me-auto">
                            <label for="txtDate" class="form-label">Date</label>
                            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-5">
                            <label for="ddlIntervalTime" class="form-label">Duration</label>
                            <asp:DropDownList ID="ddlIntervalTime" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Choose Duration" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="mb-3 d-flex">
                        <div class="col-5 me-auto">
                            <label for="ddlAvailableFrom" class="form-label">From:</label>
                            <asp:DropDownList ID="ddlAvailableFrom" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Choose Time" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-5">
                            <label for="ddlAvailableTo" class="form-label">To:</label>
                            <asp:DropDownList ID="ddlAvailableTo" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Choose Time" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mb-3 d-flex">
                                <div class="col-5 me-auto">
                                    <asp:RadioButton ID="rbRepeat" runat="server" AutoPostBack="true" GroupName="RepeatOption" OnCheckedChanged="RadioButton_CheckedChanged" />
                                    <label for="rbRepeat" class="form-label">Repeat Weekly</label>
                                </div>
                                <div class="col-5">
                                    <asp:RadioButton ID="rbNoRepeat" runat="server" AutoPostBack="true" GroupName="RepeatOption" OnCheckedChanged="RadioButton_CheckedChanged" />
                                    <label for="rbNoRepeat" class="form-label">Does Not Repeat</label>
                                </div>
                            </div>

                            <asp:Panel ID="pnlRepeatDays" runat="server" Visible="false">
                                <div class="mb-3 d-flex">
                                    <div class="d-flex me-auto">
                                        <label for="selectDay" class="form-label">What days?</label>
                                    </div>
                                    <div>
                                        <asp:Repeater ID="rptDaysOfWeek" runat="server">
                                            <ItemTemplate>
                                                <div class="day-circle text-center">
                                                    <%# Eval("Day") %>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:LinkButton ID="btnSave" CssClass="btn btn-primary" OnClick="btnSave_Click" runat="server">Save</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</asp:Content>