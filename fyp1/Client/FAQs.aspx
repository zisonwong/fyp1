<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="FAQs.aspx.cs" Inherits="fyp1.Client.FAQs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="bg-gray-50 py-16 px-6">
    <div class="max-w-7xl mx-auto bg-white rounded-3xl shadow-xl p-12 mt-12">
        <!-- Header Section -->
        <div class="text-center mb-12">
            <h1 class="text-4xl font-extrabold text-gray-900 tracking-tight">Frequently Asked Questions (FAQ)</h1>
            <p class="text-xl text-gray-600 mt-6 max-w-3xl mx-auto">
                Find answers to common questions about our insurance plans and services. If you have a question that’s not listed, feel free to ask below!
            </p>
        </div>

        <!-- FAQ Section -->
        <div class="mb-12">
            <h2 class="text-3xl font-semibold text-gray-800 mb-6">Popular Questions</h2>
            <div class="space-y-6">
                <!-- FAQ Item 1 -->
                <div class="bg-gray-100 p-6 rounded-lg shadow-md">
                    <button class="w-full text-left text-xl font-medium text-gray-900 focus:outline-none" onclick="toggleFAQ('faq1')">
                        What is the coverage of the health insurance plan?
                    </button>
                    <div id="faq1" class="mt-4 text-gray-700 hidden">
                        <p>
                            Our health insurance plan covers a wide range of healthcare services, including inpatient and outpatient care, diagnostic tests, surgeries, and more. Additionally, we offer coverage for critical illnesses, dental, and vision care. The total coverage limit depends on the plan you select.
                        </p>
                    </div>
                </div>
                
                <!-- FAQ Item 2 -->
                <div class="bg-gray-100 p-6 rounded-lg shadow-md">
                    <button class="w-full text-left text-xl font-medium text-gray-900 focus:outline-none" onclick="toggleFAQ('faq2')">
                        How do I file a claim?
                    </button>
                    <div id="faq2" class="mt-4 text-gray-700 hidden">
                        <p>
                            Filing a claim is simple. You can submit your claim through our online portal or by contacting our customer service team. Ensure that you have all the required documents, including medical receipts and reports, to process your claim.
                        </p>
                    </div>
                </div>

                <!-- FAQ Item 3 -->
                <div class="bg-gray-100 p-6 rounded-lg shadow-md">
                    <button class="w-full text-left text-xl font-medium text-gray-900 focus:outline-none" onclick="toggleFAQ('faq3')">
                        Can I add my family members to my insurance plan?
                    </button>
                    <div id="faq3" class="mt-4 text-gray-700 hidden">
                        <p>
                            Yes, our insurance plans offer family coverage. You can add your spouse, children, and even parents to your plan. The additional cost will depend on the number of people you wish to cover and the level of coverage chosen.
                        </p>
                    </div>
                </div>

                <!-- FAQ Item 4 -->
                <div class="bg-gray-100 p-6 rounded-lg shadow-md">
                    <button class="w-full text-left text-xl font-medium text-gray-900 focus:outline-none" onclick="toggleFAQ('faq4')">
                        What does the health check-up include?
                    </button>
                    <div id="faq4" class="mt-4 text-gray-700 hidden">
                        <p>
                            Our annual health check-up includes a full-body screening, blood tests, cholesterol checks, and consultations with our doctors. Depending on your age and medical history, additional tests may be included to ensure comprehensive monitoring of your health.
                        </p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Submit New Question Form -->
        <div class="mt-12 bg-blue-100 p-8 rounded-lg shadow-xl">
            <h2 class="text-3xl font-semibold text-gray-800 mb-6">Have a Question? Ask Us!</h2>
            <form action="#" method="POST" class="space-y-6">
                <div>
                    <label for="name" class="block text-lg font-medium text-gray-700">Your Name</label>
                    <input type="text" id="name" name="name" required class="w-full p-4 mt-2 bg-gray-50 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500">
                </div>

                <div>
                    <label for="email" class="block text-lg font-medium text-gray-700">Your Email</label>
                    <input type="email" id="email" name="email" required class="w-full p-4 mt-2 bg-gray-50 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500">
                </div>

                <div>
                    <label for="question" class="block text-lg font-medium text-gray-700">Your Question</label>
                    <textarea id="question" name="question" rows="4" required class="w-full p-4 mt-2 bg-gray-50 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"></textarea>
                </div>

                <div class="flex items-center justify-between">
                    <button type="submit" class="px-6 py-3 bg-blue-600 text-white font-semibold rounded-md shadow-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500">
                        Submit Question
                    </button>
                    <p class="text-sm text-gray-600">We will get back to you shortly.</p>
                </div>
            </form>
        </div>
    </div>
</div>

<script>
    function toggleFAQ(faqId) {
        const faqContent = document.getElementById(faqId);
        faqContent.classList.toggle('hidden');
    }
</script>

</asp:Content>
