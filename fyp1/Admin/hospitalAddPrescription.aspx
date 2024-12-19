<%@ Page Title="Add Prescription" Language="C#" MasterPageFile="~/Admin/adminSidebar.Master" AutoEventWireup="true" CodeBehind="hospitalAddPrescription.aspx.cs" Inherits="fyp1.Admin.hospitalAddPrescription" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <link href="~/layout/PageStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content container">
        <div class="col-xxl-12 col-lg-12">
            <h3>Add Prescription</h3>
            <div class="card border-0 shadow">
                <div class="card-body">
                    <asp:Repeater ID="rptPrescription" runat="server"
                        OnItemCommand="rptPrescription_ItemCommand">
                        <ItemTemplate>
                             <asp:HiddenField ID="hfMedicineID" Value='<%# Eval("medicineID") %>' runat="server" />
                            <div class="card mb-3 shadow-sm">
                                <div class="card-body p-0">
                                    <div class="row align-items-center">
                                        <div class="col-md-3 border-end d-flex justify-content-center">
                                            <div>
                                                <h3><%# Eval("name") %></h3>
                                            </div>
                                        </div>
                                        <div class="col-md-3 d-flex justify-content-center border-end">
                                            <div>
                                                <p class="mb-0">Type:</p>
                                                <strong><%# Eval("type") %></strong>
                                            </div>
                                        </div>
                                        <div class="col-md-2 d-flex justify-content-center border-end">
                                            <div>
                                                <p class="mb-0">Quantity:</p>
                                                <strong><%# Eval("quantity") %></strong>
                                            </div>
                                        </div>
                                        <div class="col-md-2 d-flex justify-content-center">
                                            <div>
                                                <p class="mb-0">Dosage:</p>
                                                <strong><%# Eval("dosage") %></strong>
                                            </div>
                                        </div>

                                        <div class="col-md-2 d-flex justify-content-center">
                                            <asp:LinkButton
                                                ID="btnDetails"
                                                CssClass="btn btn-primary"
                                                runat="server"
                                                CommandArgument='<%# Eval("medicineID") %>'
                                                CommandName="Prescribe">
                                                Prescribe
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </main>
    <div class="modal fade" id="prescriptionModal" tabindex="-1" aria-labelledby="prescriptionModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="prescriptionModalLabel">Add Prescription</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mb-3 d-flex">
                                <div class="col-5 me-auto">
                                    <label for="txtRecordID" class="form-label">Record ID</label>
                                    <div class="d-flex position-relative">
                                        <asp:DropDownList ID="ddlRecordID" runat="server" CssClass="form-control"
                                            AutoPostBack="True"
                                            OnSelectedIndexChanged="ddlRecordID_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <i class="bi bi-chevron-down dropdown-icon"></i>
                                    </div>
                                </div>
                                <div class="col-5">
                                    <label for="txtPatientName" class="form-label">Patient Name</label>
                                    <asp:TextBox ID="txtPatientName" CssClass="form-control" ReadOnly="true" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="mb-3 d-flex">
                        <div class="col-5 me-auto">
                            <label for="txtMedicineName" class="form-label">Medicine Name</label>
                            <asp:TextBox ID="txtMedicineName" CssClass="form-control" ReadOnly="true" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-5">
                            <label for="txtQuantity" class="form-label">Quantity</label>
                            <asp:TextBox ID="txtQuantity" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="mb-3">
                        <div class="col-12">
                            <label for="txtDetails" class="form-label">Details</label>
                            <asp:TextBox ID="txtDetails" CssClass="w-100 form-control resize height" TextMode="MultiLine" runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
                    <asp:LinkButton ID="btnSave" CssClass="btn btn-primary" OnClick="btnSave_Click" runat="server">Save</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <script>
        function showModal() {
            var modal = new bootstrap.Modal(document.getElementById('prescriptionModal'));
            modal.show();
        }
</script>

</asp:Content>
