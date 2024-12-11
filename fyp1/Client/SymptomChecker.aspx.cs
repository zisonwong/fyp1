using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class SymptomChecker : System.Web.UI.Page
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string HUGGING_FACE_API_URL = "https://api-inference.huggingface.co/models/Zabihin/Symptom_to_Diagnosis";
        private const string HUGGING_FACE_API_KEY = "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";

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.BufferOutput = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnCheckSymptoms.Click += new EventHandler(btnCheckSymptoms_Click);
        }

        protected async void btnCheckSymptoms_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSymptoms.Text))
            {
                DisplayErrorMessage("Please enter some symptoms.");
                return;
            }

            try
            {
                await PerformSymptomAnalysis(txtSymptoms.Text);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage($"An error occurred: {ex.Message}");
            }
        }

        private async Task<List<SymptomClassificationResult>> ClassifySymptomWithHuggingFace(string symptoms)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HUGGING_FACE_API_KEY);

            try
            {
                var payload = new { inputs = symptoms };
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var apiResponse = await httpClient.PostAsync(HUGGING_FACE_API_URL, content);
                apiResponse.EnsureSuccessStatusCode();

                var responseContent = await apiResponse.Content.ReadAsStringAsync();

                // Parse the nested JSON array
                var parsedArray = JArray.Parse(responseContent);
                var innerArray = parsedArray[0] as JArray;

                if (innerArray == null)
                {
                    return null;
                }

                // Convert to our classification results
                var results = innerArray.Select(item => new SymptomClassificationResult
                {
                    Label = item["label"]?.ToString(),
                    Score = item["score"]?.Value<float>() ?? 0f
                }).ToList();

                return results;
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                System.Diagnostics.Debug.WriteLine($"API Call Error: {ex}");
                return null;
            }
        }

        private async Task PerformSymptomAnalysis(string symptoms)
        {
            try
            {
                var aiClassifications = await ClassifySymptomWithHuggingFace(symptoms);

                if (aiClassifications == null || !aiClassifications.Any())
                {
                    DisplayErrorMessage("No classification result returned from the AI. Please check the input or try again later.");
                    return;
                }

                DisplayCombinedResults(aiClassifications, symptoms);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Symptom Analysis Error: {ex}");
                DisplayErrorMessage($"An error occurred during symptom analysis: {ex.Message}");
            }
        }

        private void DisplayCombinedResults(List<SymptomClassificationResult> aiClassifications, string originalSymptoms)
        {
            var resultBuilder = new StringBuilder();

            // Original Symptoms
            resultBuilder.Append($"<div class='original-symptoms'>");
            resultBuilder.Append($"<h3>Your Reported Symptoms</h3>");
            resultBuilder.Append($"<p>{HttpUtility.HtmlEncode(originalSymptoms)}</p>");
            resultBuilder.Append("</div>");

            // AI Classification Result
            resultBuilder.Append("<div class='ai-classification'>");
            resultBuilder.Append("<h3>Potential Conditions</h3>");
            resultBuilder.Append("<table class='table table-striped'>");
            resultBuilder.Append("<thead><tr><th>Condition</th><th>Confidence</th></tr></thead>");
            resultBuilder.Append("<tbody>");

            // Sort classifications by score in descending order
            var sortedClassifications = aiClassifications
                .OrderByDescending(c => c.Score)
                .Take(5); // Limit to top 5 conditions

            foreach (var classification in sortedClassifications)
            {
                resultBuilder.Append("<tr>");
                resultBuilder.Append($"<td>{HttpUtility.HtmlEncode(classification.Label)}</td>");
                resultBuilder.Append($"<td>{classification.Score:P2}</td>");
                resultBuilder.Append("</tr>");
            }

            resultBuilder.Append("</tbody>");
            resultBuilder.Append("</table>");
            resultBuilder.Append("</div>");

            // Disclaimer
            resultBuilder.Append("<div class='disclaimer'>");
            resultBuilder.Append("<p><strong>DISCLAIMER:</strong> This is NOT a professional medical diagnosis. Always consult a healthcare professional.</p>");
            resultBuilder.Append("</div>");

            // Update results
            litResults.Text = resultBuilder.ToString();
            pnlResults.Visible = true;
        }

        private void DisplayErrorMessage(string message)
        {
            litResults.Text = $"<p style='color:red;'>{message}</p>";
            pnlResults.Visible = true;
        }

        public class SymptomClassificationResult
        {
            public string Label { get; set; }
            public float Score { get; set; }
        }

        protected void BackToHomeBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("clientHome.aspx");
        }

        protected void btnClearText_Click(object sender, EventArgs e)
        {
            txtSymptoms.Text = string.Empty;
            pnlResults.Visible = false;
        }
    }
}