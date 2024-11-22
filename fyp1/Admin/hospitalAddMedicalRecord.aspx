<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalAddMedicalRecord.aspx.cs" Inherits="fyp1.Admin.hospitalAddMedicaRecord" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content">
        <div class="col-xxl-12 col-lg-12">
            <div class="d-flex mb-2">
                <div class="me-auto">
                    <h3>Add Record</h3>
                </div>
                <asp:LinkButton CssClass="btn btn-primary fw-bold"
                    ID="btnConfirmAddRecord" runat="server" OnClick="btnConfirmAddRecord_Click">
                    Add Record
                </asp:LinkButton>
            </div>
            <div class="card -border-0 shadow">
                <div class="card-body">
                    <div class="row">
                        <div class="col-xxl-4 col-lg-4">
                            <div class="mb-2">
                                <p class="mb-0">Patient Name</p>
                                <asp:DropDownList ID="ddlPatientName" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPatientName_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Gender</p>
                                <asp:TextBox ID="txtGender" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Blood Type</p>
                                <asp:TextBox ID="txtBloodType" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Age</p>
                                <asp:TextBox ID="txtAge" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Date</p>
                                <asp:TextBox ID="txtCreateDate" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xxl-8 col-lg-8">
                            <div class="mb-2">
                                <p class="mb-0">Problems</p>
                                <asp:TextBox ID="txtProblems" CssClass="w-100 form-control resize height" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Diagnosis</p>
                                <asp:TextBox ID="txtDiagnosis" CssClass="w-100 form-control resize height" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-2">
                                <p class="mb-0">Treatment</p>
                                <asp:TextBox ID="txtTreatment" CssClass="w-100 form-control resize height" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </main>

    <div class="modal fade" id="confirmationModal" tabindex="-1" aria-labelledby="confirmationModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="confirmationModalLabel">Add Prescription</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    Do you want to add a prescription for this medical record?
           
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Cancel</button>
                    <asp:LinkButton CssClass="btn btn-primary" ID="btnAddPrescription" runat="server" OnClick="btnAddPrescription_Click">
                    Add Prescription
                </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <script>
        function showConfirmationModal() {
            var myModal = new bootstrap.Modal(document.getElementById('confirmationModal'));
            myModal.show();
        }
</script>
</asp:Content>
