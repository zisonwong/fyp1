<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Admin/adminSideBar.Master" AutoEventWireup="true" CodeBehind="adminHome.aspx.cs" Inherits="fyp1.Admin.adminHome" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../layout/bootstrap.bundle.min.js"></script>
    <script src="../layout/highcharts.js"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container content">
    <div class="row">
        <!-- Cards -->
        <div class="col-12 col-sm-6 col-xxl-3 d-flex">
            <div class="card2 border-0 shadow mb-4">
                <div class="card-body p-0">
                    <div class="row g-0 w-100">
                        <div class="col-6 p-3">
                            <h6 class="card-title">Welcome Back,
                                <asp:Label ID="lblDashboardStaffName" runat="server" Text="Label"></asp:Label></h6>
                            <p class="mb-0">IHMS Dashboard</p>
                        </div>
                        <div class="col-6 align-self-sm-end">
                            <img src="../hospitalImg/admin.png" class="img-fluid" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Total Patient Card -->
        <div class="col-12 col-sm-6 col-xxl-3 d-flex flex-fill">
            <div class="card2 border-0 shadow mb-4" style="width: 100%;">
                <div class="card-body p-3">
                    <div class="d-flex">
                        <div class="circle-background flex-grow-1">
                            <img src="../hospitalImg/patient.png" class="icon-size"/>
                        </div>
                        <div class="ms-3" style="width: 100%;">
                            <div class="mb-2">
                                <h6 class="card-title">Total Patient</h6>
                            </div>
                            <h3>
                                <asp:Label ID="totalPatient" runat="server" Text="Label" />
                            </h3>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Total Staff Card -->
        <div class="col-12 col-sm-6 col-xxl-3 d-flex flex-fill">
            <div class="card2 border-0 shadow mb-4" style="width: 100%;">
                <div class="card-body p-3">
                    <div class="d-flex">
                        <div class="circle-background flex-grow-1">
                            <img src="../hospitalImg/staff.png" class="icon-size"/>
                        </div>
                        <div class="ms-3" style="width: 100%;">
                            <div class="mb-2">
                                <h6 class="card-title">Total Staff</h6>
                            </div>
                            <h3>
                                <asp:Label ID="totalStaff" runat="server" Text="Label" />
                            </h3>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Appointments Card -->
        <div class="col-12 col-sm-6 col-xxl-3 d-flex flex-fill">
            <div class="card2 border-0 shadow mb-4" style="width: 100%;">
                <div class="card-body p-3">
                    <div class="d-flex">
                        <div class="circle-background flex-grow-1">
                            <img src="../hospitalImg/appointment.png" class="icon-size"/>
                        </div>
                        <div class="ms-3" style="width: 100%;">
                            <div class="mb-2">
                                <h6 class="card-title">This Month Appointment</h6>
                            </div>
                            <h3>
                                <asp:Label ID="thisMonthAppointment" runat="server" Text="Label" />
                            </h3>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Charts (Aligns with the Cards Above) -->
        <div class="col-12 col-md-4">
            <div class="card3 border-0 shadow">
                <div class="card-body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="chart-navigation" class="d-flex justify-content-between align-items-center">
                                <asp:LinkButton ID="prevChartButton" runat="server" CssClass="btn btn-outline-primary"
                                    OnClick="prevChartButton_Click">
                                    <i class="bi bi-chevron-left"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="nextChartButton" runat="server" CssClass="btn btn-outline-primary"
                                    OnClick="nextChartButton_Click">
                                    <i class="bi bi-chevron-right"></i>
                                </asp:LinkButton>
                            </div>
                            <div id="appointments-pie-chart" style="width: 100%; height: 400px;"></div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div class="col-12 col-md-8">
            <div class="card3 border-0 shadow">
                <div class="card-body">
                    <div id="appointments-line-chart" style="width: 100%; height: 435px;"></div>
                </div>
            </div>
        </div>

        <div class="col-12 mt-4">
            <div class="card border-0 shadow">
                <div class="card-body">
                    <div id="branch-bar-chart" style="width: 100%; height: 400px;"></div>
                </div>
            </div>
        </div>
    </div>
</main>

    <script>
        (() => {
            // Data for line chart
            let lineData = <%= ViewState["lineData"] %>;

            Highcharts.chart('appointments-line-chart', {
                chart: {
                    type: 'line' // Set chart type as line
                },
                title: {
                    text: 'Monthly Appointments' // Chart title
                },
                tooltip: {
                    valueSuffix: ' appointments' // Tooltip unit
                },
                xAxis: {
                    type: 'datetime', // Treat X-axis as time-based
                    title: {
                        text: 'Months'
                    },
                    labels: {
                        formatter: function () {
                            return Highcharts.dateFormat('%b %Y', this.value);
                        }
                    }
                },
                yAxis: {
                    title: {
                        text: 'Total Appointments'
                    },
                    allowDecimals: false, // Prevent decimals
                    min: 0
                },
                series: [{
                    name: 'Appointments', // Series name
                    data: lineData // Use generated data from server
                }]
            });
        })();

        (() => {
            let barChartCategories = <%= barChartCategories %>;
            let barChartData = <%= barChartData %>;

            Highcharts.chart('branch-bar-chart', {
                chart: {
                    type: 'column' // Use column for side-by-side bars
                },
                title: {
                    text: 'Total Appointments by Branch'
                },
                xAxis: {
                    categories: barChartCategories, // Months in ascending order
                    title: {
                        text: 'Months'
                    },
                    labels: {
                        formatter: function () {
                            return this.value; // Already formatted as 'MMM yyyy'
                        }
                    }
                },
                yAxis: {
                    title: {
                        text: 'Total Appointments'
                    },
                    allowDecimals: false // Ensure whole numbers
                },
                tooltip: {
                    shared: true, // Show all series in a single tooltip
                    pointFormat: '<b>{series.name}: {point.y}</b><br/>'
                },
                plotOptions: {
                    column: {
                        grouping: true, // Side-by-side bars
                        dataLabels: {
                            enabled: true,
                            format: '{point.y}'
                        }
                    }
                },
                series: barChartData // Pass prepared data to the chart
            });
        })();
        // Function to render the pie chart based on index
        function renderPieChart(index) {
            const chartDataList = [
                { title: 'Appointments Comparison', data: <%= pieChartDataAppointments %> },
                { title: 'Doctors Join Comparison', data: <%= pieChartDataDoctors %> },
                { title: 'Nurses Join Comparison', data: <%= pieChartDataNurses %> }
            ];

            const chartData = chartDataList[index];

            Highcharts.chart('appointments-pie-chart', {
                chart: {
                    type: 'pie'
                },
                title: {
                    text: chartData.title
                },
                tooltip: {
                    formatter: function () {
                        return `<b>${this.point.name}</b><br>
                Total: <b>${this.point.options.totalAppointments || this.point.options.totalDoctors || this.point.options.totalNurses}</b><br>`;
                    }
                },
                plotOptions: {
                    series: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: [{
                            enabled: true,
                            distance: 20
                        },
                        {
                            enabled: true,
                            distance: -50,
                            format: '{point.percentage:.1f}%',
                            style: {
                                color: 'white',
                                fontSize: '1em',
                                fontWeight: 'bold',
                                textOutline: 'none'
                            }
                        }],
                    }
                },
                series: [{
                    name: chartData.title,
                    colorByPoint: true,
                    data: chartData.data
                }]
            });
        }

        // Initial render on page load (optional)
        document.addEventListener('DOMContentLoaded', function () {
            renderPieChart(0); // Start with the first chart
        });
    </script>



</asp:Content>
