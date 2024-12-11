<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="InsuranceInformation.aspx.cs" Inherits="fyp1.Client.InsuranceInformation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="bg-gray-50 py-16 px-6">
    <div class="max-w-7xl mx-auto bg-white rounded-xl shadow-lg p-10 mt-16">
        <!-- Header Section -->
        <div class="text-center mb-10">
            <h1 class="text-3xl font-bold text-gray-900">Premium Health Insurance Plan</h1>
            <p class="text-gray-600 mt-4">
                Secure your health and future with our comprehensive insurance plans tailored to your needs. Enjoy unmatched coverage and benefits with the flexibility you deserve.
            </p>
        </div>

        <!-- Plan Highlights and Benefits -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-10 mb-10">
            <!-- Plan Highlights -->
            <div>
                <h2 class="text-2xl font-semibold text-gray-800 mb-4">Plan Highlights</h2>
                <ul class="space-y-3">
                    <li class="flex items-start">
                        <span class="bg-blue-100 text-blue-600 p-2 rounded-full mr-3">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                        </span>
                        <p>Annual Coverage Limit: <span class="font-medium text-gray-900">$1,000,000</span></p>
                    </li>
                    <li class="flex items-start">
                        <span class="bg-blue-100 text-blue-600 p-2 rounded-full mr-3">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                        </span>
                        <p>Hospitalization: Fully Covered (Private & Public)</p>
                    </li>
                    <li class="flex items-start">
                        <span class="bg-blue-100 text-blue-600 p-2 rounded-full mr-3">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                        </span>
                        <p>Outpatient Care: Up to <span class="font-medium text-gray-900">$15,000</span> annually</p>
                    </li>
                    <li class="flex items-start">
                        <span class="bg-blue-100 text-blue-600 p-2 rounded-full mr-3">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                        </span>
                        <p>Critical Illness Coverage: Lump sum payout up to <span class="font-medium text-gray-900">$250,000</span></p>
                    </li>
                </ul>
            </div>

            <!-- Key Benefits -->
            <div>
                <h2 class="text-2xl font-semibold text-gray-800 mb-4">Key Benefits</h2>
                <ul class="space-y-3">
                    <li class="flex items-start">
                        <span class="bg-green-100 text-green-600 p-2 rounded-full mr-3">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                        </span>
                        <p>Access to <span class="font-medium text-gray-900">2,000+ hospitals</span> worldwide</p>
                    </li>
                    <li class="flex items-start">
                        <span class="bg-green-100 text-green-600 p-2 rounded-full mr-3">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                        </span>
                        <p>Annual Health Check-Ups</p>
                    </li>
                    <li class="flex items-start">
                        <span class="bg-green-100 text-green-600 p-2 rounded-full mr-3">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                        </span>
                        <p>24/7 Telemedicine & Online Consultation</p>
                    </li>
                    <li class="flex items-start">
                        <span class="bg-green-100 text-green-600 p-2 rounded-full mr-3">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                        </span>
                        <p>Worldwide Travel Coverage</p>
                    </li>
                </ul>
            </div>
        </div>

        <!-- Detailed Plan Section -->
        <div class="mb-10">
            <h2 class="text-2xl font-semibold text-gray-800 mb-4">Detailed Plan Information</h2>
            <p class="text-gray-600 leading-relaxed">
                Our plan offers comprehensive in-patient and out-patient coverage, maternity benefits, and critical illness protection. Enjoy access to a network of top-tier healthcare providers and a seamless claims process. For added convenience, you can manage your plan and claims through our user-friendly mobile app.
            </p>
        </div>

        <!-- Call to Action -->
        <div class="bg-blue-50 p-8 rounded-lg flex flex-col md:flex-row items-center justify-between">
            <div class="text-center md:text-left mb-6 md:mb-0">
                <h3 class="text-lg font-bold text-blue-800">Ready to Get Started?</h3>
                <p class="text-gray-600">Talk to one of our insurance specialists today to customize your plan.</p>
            </div>
            <a href="#"
                class="bg-blue-600 text-white px-8 py-3 rounded-lg shadow-lg font-medium hover:bg-blue-700">
                Get a Free Quote
            </a>
        </div>
    </div>
</div>
</asp:Content>
