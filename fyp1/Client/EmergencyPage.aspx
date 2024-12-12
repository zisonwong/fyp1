<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="EmergencyPage.aspx.cs" Inherits="fyp1.Client.EmergencyPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAbFkinyyHf8XwboZ1KHr7yupFq2yo_ufo&libraries=places"></script>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
     <script>
         function initMap(branchLat, branchLng, patientLat, patientLng) {
             const branchLocation = { lat: parseFloat(branchLat), lng: parseFloat(branchLng) };
             const patientLocation = { lat: parseFloat(patientLat), lng: parseFloat(patientLng) };

             const map = new google.maps.Map(document.getElementById("map"), {
                 zoom: 12,
                 center: branchLocation,
             });

             const directionsService = new google.maps.DirectionsService();
             const directionsRenderer = new google.maps.DirectionsRenderer();
             directionsRenderer.setMap(map);

             const request = {
                 origin: branchLocation,
                 destination: patientLocation,
                 travelMode: 'DRIVING',
             };

             directionsService.route(request, function (result, status) {
                 if (status === 'OK') {
                     directionsRenderer.setDirections(result);
                 } else {
                     alert('Could not retrieve directions: ' + status);
                 }
             });
         }

         function getUserAddressFromCookie() {
             const cookieName = "UserAddress";
             const cookies = document.cookie.split(";");

             for (let i = 0; i < cookies.length; i++) {
                 const cookie = cookies[i].trim();
                 if (cookie.indexOf(cookieName + "=") == 0) {
                     return decodeURIComponent(cookie.substring(cookieName.length + 1));
                 }
             }
             return null; // Return null if the cookie is not found
         }

         function getCurrentLocation() {
             if (navigator.geolocation) {
                 navigator.geolocation.getCurrentPosition(function (position) {
                     const lat = position.coords.latitude;
                     const lng = position.coords.longitude;
                     console.log("Current Location:", lat, lng);
                     // Now you can pass these coordinates to your back-end or use in the map
                 }, function (error) {
                     alert("Error getting location: " + error.message);
                 });
             } else {
                 alert("Geolocation is not supported by this browser.");
             }
         }

         // Call the functions
         window.onload = function () {
             const userAddress = getUserAddressFromCookie();
             if (userAddress) {
                 console.log("User Address from Cookie:", userAddress);
                 // You can use userAddress for further actions like retrieving branches or calculating distances
             } else {
                 console.log("No user address found in cookie.");
             }

             // Call the function to get the user's current location
             getCurrentLocation();
         };
     </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <!-- Header -->
        <header class="w-full bg-red-500 py-4 shadow-md">
            <div class="container mx-auto text-center mt-24">
                <h1 class="text-2xl font-bold text-white">Emergency Assistance</h1>
            </div>
        </header>

        <!-- Content -->
        <main class="flex flex-col items-center w-full px-4 py-6">
            <!-- Branch Info -->
            <div class="w-full max-w-4xl bg-white shadow-md rounded-lg p-6 mb-6">
                <h2 class="text-xl font-semibold text-gray-800 border-b pb-3 mb-4">Nearest Branch Details</h2>
                <div class="flex flex-col gap-4">
                    <div>
                        <h3 class="text-lg font-bold text-gray-700">Branch Name:</h3>
                        <asp:Label ID="lblBranchName" runat="server" CssClass="text-gray-600"/>
                    </div>
                    <div>
                        <h3 class="text-lg font-bold text-gray-700">Branch Address:</h3>
                        <asp:Label ID="lblBranchAddress" runat="server" CssClass="text-gray-600"/>
                    </div>
                    <div>
                        <h3 class="text-lg font-bold text-gray-700">Distance:</h3>
                        <asp:Label ID="lblDistance" runat="server" CssClass="text-gray-600"/>
                    </div>
                    <div>
                        <h3 class="text-lg font-bold text-gray-700">Estimated Time:</h3>
                        <asp:Label ID="lblTime" runat="server" CssClass="text-gray-600"/>
                    </div>
                </div>
            </div>

            <!-- Map Section -->
            <div class="w-full max-w-4xl bg-white shadow-md rounded-lg p-6">
                <h2 class="text-xl font-semibold text-gray-800 border-b pb-3 mb-4">Route Map</h2>
                <div id="map" class="w-full h-96 bg-gray-200 rounded"></div>
            </div>

            <!-- Actions -->
            <div class="w-full max-w-4xl flex justify-center gap-4 mt-6">
                <button onclick="getLocation()"
                    class="bg-blue-500 text-white px-6 py-2 rounded-lg shadow hover:bg-blue-600">
                    Refresh Location
           
                </button>
                <button onclick="alert('Emergency team contacted!')"
                    class="bg-red-500 text-white px-6 py-2 rounded-lg shadow hover:bg-red-600">
                    Contact Emergency Team
           
                </button>
            </div>
        </main>

        <script>
            // JavaScript for Google Maps and Data Population
            let patientLatitude = 0;
            let patientLongitude = 0;

            function getLocation() {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(position => {
                        patientLatitude = position.coords.latitude;
                        patientLongitude = position.coords.longitude;

                        // Fetch nearest branch and distance from server
                        fetch('/EmergencyPage.aspx', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({ lat: patientLatitude, lng: patientLongitude })
                        })
                            .then(response => response.json())
                            .then(data => {
                                document.getElementById('branchName').innerText = data.branchName;
                                document.getElementById('branchAddress').innerText = data.branchAddress;
                                document.getElementById('distance').innerText = data.distance + " km";
                                document.getElementById('time').innerText = data.time + " mins";

                                // Initialize map with directions
                                initMap(data.branchLat, data.branchLng, patientLatitude, patientLongitude);
                            });
                    });
                } else {
                    alert("Geolocation is not supported by this browser.");
                }
            }

            
    </script>

        <script>
            // Call initMap with server-side passed values
            initMap(<%= BranchLatitude %>, <%= BranchLongitude %>, <%= PatientLatitude %>, <%= PatientLongitude %>);
    </script>

        <script async
            src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAbFkinyyHf8XwboZ1KHr7yupFq2yo_ufo&callback=getLocation">
    </script>
    </form>
</asp:Content>
