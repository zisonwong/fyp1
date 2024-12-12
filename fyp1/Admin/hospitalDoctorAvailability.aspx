<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalDoctorAvailability.aspx.cs" Inherits="fyp1.Admin.hospitalDoctorAvailability" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <link href="~/layout/PageStyle.css" rel="stylesheet" />
    <script src="../layout/availability.js"></script>
    <style>
        .badge-primary {
            color: #fff;
            background-color: #007bff;
        }

        .badge-danger {
            color: #fff;
            background-color: #bb2d3b;
        }

        .badge-secondary {
            color: #fff;
            background-color: #6c757d;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-5 container">
        <% if (Session["Role"] != null && Session["Role"].ToString() == "Nurse")
            { %>
        <div class="position-relative d-flex w-25 mb-3">
            <asp:DropDownList ID="ddlSelectDoctor" runat="server" CssClass="form-control"
                AutoPostBack="true" OnSelectedIndexChanged="ddlSelectDoctor_SelectedIndexChanged">
                <asp:ListItem Text="Select Doctor" Value=""></asp:ListItem>
            </asp:DropDownList>
            <i class="bi bi-chevron-down dropdown-icon"></i>
        </div>
        <% } %>
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>Working Hours</h2>
            <div class="d-flex align-items-center">
                <asp:Button ID="btnPrevious" runat="server" Text="←" CssClass="btn btn-outline-primary me-2"
                    OnClick="btnPrevious_Click" />
                <asp:Label ID="lblMonth2" runat="server" CssClass="fs-4 fw-bold" Text="Label"></asp:Label>
                <asp:Button ID="btnNext" runat="server" Text="→" CssClass="btn btn-outline-primary ms-2"
                    OnClick="btnNext_Click" />
            </div>
            <div>
                <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#deleteAvailabilityModal">
                    <svg xmlns="http://www.w3.org/2000/svg" width="25" height="25" fill="currentColor" class="bi bi-dash" viewBox="0 0 16 16">
                        <path d="M4 8a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 0 1h-7A.5.5 0 0 1 4 8" />
                    </svg>
                </button>
                <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#availabilityModal">
                    <svg xmlns="http://www.w3.org/2000/svg" width="25" height="25" fill="currentColor" class="bi bi-plus" viewBox="0 0 16 16">
                        <path d="M8 4a.5.5 0 0 1 .5.5v3h3a.5.5 0 0 1 0 1h-3v3a.5.5 0 0 1-1 0v-3h-3a.5.5 0 0 1 0-1h3v-3A.5.5 0 0 1 8 4" />
                    </svg></button>
            </div>
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
                <asp:Repeater ID="rptCalendar"
                    runat="server">
                    <ItemTemplate>
                        <div class="col border p-2 calendar-day text-center bg-white <%# !(bool)Eval("IsPlaceholder") ? "clickable-date" : "" %>"
                            data-date='<%# !(bool)Eval("IsPlaceholder") ? Eval("Day") : "" %>'>
                            <%# !(bool)Eval("IsPlaceholder") ? Eval("Day") : "" %>
                            <br />
                            <asp:Repeater ID="rptAvailabilityTimes" runat="server" DataSource='<%# Eval("AvailabilityTimes") %>'>
                                <ItemTemplate>
                                    <span id="availabilityBadge"
                                        class="availableBadges <%# GetBranchColorClass(Eval("BranchID")) %>"
                                        title='<%# GetBranchName(Eval("BranchID")) %>'
                                        onclick="openEditModal(event, '<%# Eval("TimeSlot") %>', '<%# Eval("AvailableIDs") %>', this)"
                                        data-timeslot='<%# Eval("TimeSlot") %>'
                                        data-availableids='<%# Eval("AvailableIDs") %>'
                                        data-branchid='<%# Eval("BranchID") %>'>
                                        <%# Eval("TimeSlot") %>
                                    </span>
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
                            <div class="d-flex position-relative">
                                <asp:DropDownList ID="ddlIntervalTime" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="Choose Duration" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <i class="bi bi-chevron-down dropdown-icon"></i>
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mb-3 col-12">
                                <label for="ddlDoctorBranch" class="form-label">Branch:</label>
                                <div class="position-relative d-flex">
                                    <asp:DropDownList ID="ddlDoctorBranch" runat="server" CssClass="form-control"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlDoctorBranch_SelectedIndexChanged">
                                        <asp:ListItem Text="Choose Branch" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <i class="bi bi-chevron-down dropdown-icon"></i>
                                </div>
                            </div>
                            <div class="mb-3 d-flex">
                                <div class="col-5 me-auto">
                                    <label for="ddlAvailableFrom" class="form-label">From:</label>
                                    <div class="position-relative d-flex">
                                        <asp:DropDownList ID="ddlAvailableFrom" runat="server" CssClass="form-control"
                                            AutoPostBack="true" OnSelectedIndexChanged="ddlAvailableFrom_SelectedIndexChanged">
                                            <asp:ListItem Text="Choose Time" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <i class="bi bi-chevron-down dropdown-icon"></i>
                                    </div>
                                </div>
                                <div class="col-5">
                                    <label for="ddlAvailableTo" class="form-label">To:</label>
                                    <div class="position-relative d-flex">
                                        <asp:DropDownList ID="ddlAvailableTo" runat="server" CssClass="form-control" AutoPostBack="true">
                                            <asp:ListItem Text="Choose Time" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <i class="bi bi-chevron-down dropdown-icon"></i>
                                    </div>
                                </div>
                            </div>
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
                            <div class="mb-3 d-flex">
                                <div class="col-5 me-auto">
                                    <asp:RadioButton ID="rbRepeatMonth" runat="server" AutoPostBack="true" GroupName="RepeatOption" OnCheckedChanged="RadioButton_CheckedChanged" />
                                    <label for="rbRepeatMonth" class="form-label">Repeat This Month</label>
                                </div>
                                <div class="col-5">
                                    <asp:RadioButton ID="rbRepeat3Month" runat="server" AutoPostBack="true" GroupName="RepeatOption" OnCheckedChanged="RadioButton_CheckedChanged" />
                                    <label for="rbRepeat3Month" class="form-label">Repeat 3 Month</label>
                                </div>
                            </div>
                            <div class="mb-3 d-flex">
                                <div class="col-5 me-auto">
                                    <asp:RadioButton ID="rbRepeat6Month" runat="server" AutoPostBack="true" GroupName="RepeatOption" OnCheckedChanged="RadioButton_CheckedChanged" />
                                    <label for="rbRepeat6Month" class="form-label">Repeat 6 Month</label>
                                </div>
                                <div class="col-5">
                                    <asp:RadioButton ID="rbRepeatYear" runat="server" AutoPostBack="true" GroupName="RepeatOption" OnCheckedChanged="RadioButton_CheckedChanged" />
                                    <label for="rbRepeatYear" class="form-label">Repeat This Year</label>
                                </div>
                            </div>
                            <div class="col-12 d-flex align-items-center">
                                <asp:RadioButton ID="rbXMonth" runat="server" AutoPostBack="true" GroupName="RepeatOption" OnCheckedChanged="RadioButton_CheckedChanged" />
                                <label for="rbXMonth" class="form-label mb-0 ms-1">Repeat</label>
                                <asp:Label ID="txtDisplayMonthText" CssClass="form-label mb-0 ms-1" runat="server" Text="X"></asp:Label>
                                <asp:TextBox ID="txtXMonth" runat="server" CssClass="form-control w-25 ms-1" />
                                <label for="txtXMonth" class="form-label mb-0 ms-1">Month</label>
                            </div>
                            <asp:Panel ID="pnlRepeatDays" runat="server" Visible="false">
                                <div class="mb-3 d-flex">
                                    <div class="d-flex me-auto">
                                        <label for="selectDay" class="form-label">What days?</label>
                                    </div>
                                    <div>
                                        <asp:Repeater ID="rptDaysOfWeek" runat="server" OnItemCommand="rptDaysOfWeek_ItemCommand">
                                            <ItemTemplate>
                                                <asp:Button
                                                    ID="btnDay"
                                                    runat="server"
                                                    CommandName="ToggleDay"
                                                    CommandArgument='<%# Container.ItemIndex %>'
                                                    Text='<%# Eval("Day") %>'
                                                    CssClass='<%# (SelectedDayIndices.Contains(Container.ItemIndex)) ? "day-circle text-center selected" : "day-circle text-center" %>' />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
                    <asp:LinkButton ID="btnSave" CssClass="btn btn-primary" OnClick="btnSave_Click" runat="server">Save</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnSelectedAvailableIDs" runat="server" />
    <!-- Edit Availability Modal -->
    <div class="modal fade" id="editAvailabilityModal" tabindex="-1" aria-labelledby="editAvailabilityModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="editAvailabilityModalLabel">Edit Availability</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3 d-flex">
                        <div class="col-12">
                            <label for="txtEditDate" class="form-label">Date</label>
                            <asp:TextBox ID="txtEditDate" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-3 d-flex">
                        <div class="col-5 me-auto">
                            <label for="ddlEditAvailableFrom" class="form-label">From:</label>
                            <asp:TextBox ID="txtEditAvailableFrom" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-5">
                            <label for="ddlEditAvailableTo" class="form-label">To:</label>
                            <asp:TextBox ID="txtEditAvailableTo" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnDelete" CssClass="btn btn-danger" runat="server" OnClick="btnDelete_Click">Delete</asp:LinkButton>
                    <asp:LinkButton ID="btnSaveChanges" CssClass="btn btn-primary" runat="server" OnClick="btnSaveEdit_Click">Save Changes</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <!-- Delete Availability Modal -->
    <div class="modal fade" id="deleteAvailabilityModal" tabindex="-1" aria-labelledby="deleteAvailabilityModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="deleteAvailabilityModalLabel">Delete Availability</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mb-3 d-flex">
                                <div class="col-5 me-auto">
                                    <label for="txtDeletStartDate" class="form-label">Start Date</label>
                                    <asp:TextBox ID="txtDeleteStartDate" TextMode="Date" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-5">
                                    <label for="txtDeleteEndDate" class="form-label">End Date</label>
                                    <asp:TextBox ID="txtDeleteEndDate" TextMode="Date" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="mb-3 col-12">
                                <label for="ddlDeleteDoctorBranch" class="form-label">Branch:</label>
                                <div class="position-relative d-flex">
                                    <asp:DropDownList ID="ddlDeleteDoctorBranch" runat="server" CssClass="form-control"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlDeleteDoctorBranch_SelectedIndexChanged">
                                        <asp:ListItem Text="Choose Branch" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <i class="bi bi-chevron-down dropdown-icon"></i>
                                </div>
                            </div>
                            <div class="mb-3 d-flex">
                                <div class="col-5 me-auto">
                                    <label for="ddlDeleteAvailableFrom" class="form-label">From:</label>
                                    <div class="position-relative d-flex">
                                        <asp:DropDownList ID="ddlDeleteAvailableFrom" runat="server" CssClass="form-control"
                                            AutoPostBack="true">
                                            <asp:ListItem Text="Choose Time" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <i class="bi bi-chevron-down dropdown-icon"></i>
                                    </div>
                                </div>
                                <div class="col-5">
                                    <label for="ddlDeleteAvailableTo" class="form-label">To:</label>
                                    <div class="position-relative d-flex">
                                        <asp:DropDownList ID="ddlDeleteAvailableTo" runat="server" CssClass="form-control" AutoPostBack="true">
                                            <asp:ListItem Text="Choose Time" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <i class="bi bi-chevron-down dropdown-icon"></i>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" data-bs-dismiss="modal">Close</button>
                    <asp:LinkButton ID="LinkButton1" CssClass="btn btn-danger" runat="server" OnClick="btnDeleteAvailability_Click">Delete</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnAvailableIDs" runat="server" />
    <script type="text/javascript">
        var lblMonth2ClientID = '<%= lblMonth2.ClientID %>';
        function openEditModal(event, timeSlot, availableIDs, element) {
            event.stopPropagation();

            console.log('TimeSlot:', timeSlot);
            console.log('AvailableIDs:', availableIDs);

            if (!timeSlot) {
                console.error('No time slot provided');
                return;
            }

            try {
                // Split the time slot into from and to times
                const [fromTime, toTime] = timeSlot.split(' - ');
                document.getElementById('<%= hdnAvailableIDs.ClientID %>').value = availableIDs;
                // Get the selected day from the clicked element
                const day = element.closest('.calendar-day').dataset.date;

                // Call the new function to show the edit modal and prefill the data
                showEditAvailabilityModal(day, timeSlot, availableIDs);

            } catch (error) {
                console.error('Error in openEditModal:', error);
            }
        }
    </script>

</asp:Content>
