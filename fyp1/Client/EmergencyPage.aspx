<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="EmergencyPage.aspx.cs" Inherits="fyp1.Client.EmergencyPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <div style="text-align: center; margin-top: 50px;">
            <h2>Emergency Alert</h2>
            <p>If you are in an emergency, press the button below to send an alert with your location.</p>
            <!-- Emergency Button -->
            <button id="btnEmergency" type="button" class="btn btn-danger" style="padding: 10px 20px; font-size: 16px; background-color: red; color: white; border: none; border-radius: 5px;">
                Send Emergency Alert
           
            </button>
            <br />
            <!-- Error or success message -->
            <span id="errorMessage" style="color: red; margin-top: 10px; display: block;"></span>
        </div>
    </form>

    <!-- JavaScript for Geolocation and AJAX -->
    <script>
        document.getElementById('btnEmergency').addEventListener('click', function () {
            // Check if geolocation is supported
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    function (position) {
                        const latitude = position.coords.latitude;
                        const longitude = position.coords.longitude;

                        // Prepare data to send to the server
                        const patientID = '12345'; // Replace with actual patient ID
                        const data = {
                            patientID: patientID,
                            latitude: latitude,
                            longitude: longitude
                        };

                        // Send data to the server using AJAX
                        fetch('EmergencyPage.aspx/EmergencyAlert', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify(data)
                        })
                            .then(response => response.json())
                            .then(result => {
                                if (result.d.success) {
                                    alert('Emergency alert sent successfully!');
                                    document.getElementById('errorMessage').textContent = '';
                                } else {
                                    document.getElementById('errorMessage').textContent = result.d.error;
                                }
                            })
                            .catch(error => {
                                document.getElementById('errorMessage').textContent = 'Error sending alert. Please try again.';
                                console.error(error);
                            });
                    },
                    function (error) {
                        document.getElementById('errorMessage').textContent = 'Unable to retrieve location. Please allow location access.';
                    }
                );
            } else {
                document.getElementById('errorMessage').textContent = 'Geolocation is not supported by this browser.';
            }
        });
    </script>
</asp:Content>
