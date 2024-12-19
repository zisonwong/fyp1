using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class Checkout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string appointmentID = Request.QueryString["appointmentID"];
            if (!string.IsNullOrEmpty(appointmentID))
            {
                // Store it for use in CreateOrder
                ViewState["AppointmentID"] = appointmentID;
            }
            else
            {
                // Handle missing or invalid appointmentID
                Response.Redirect("ErrorPage.aspx?error=MissingAppointmentID");
            }
        }

        private static string EnvironmentMode => ConfigurationManager.AppSettings["EnvironmentMode"];
        public string PayPalClientID => ConfigurationManager.AppSettings["PayPalClientID"];
        private static string PayPalSecret => ConfigurationManager.AppSettings["PayPalSecret"];

        private static PayPalEnvironment GetEnvironment()
        {
            return EnvironmentMode == "sandbox"
                ? (PayPalEnvironment)new SandboxEnvironment(ConfigurationManager.AppSettings["PayPalClientID"], ConfigurationManager.AppSettings["PayPalSecret"])
                : new LiveEnvironment(ConfigurationManager.AppSettings["PayPalClientID"], ConfigurationManager.AppSettings["PayPalSecret"]);
        }


        private static PayPalHttpClient Client = new PayPalHttpClient(GetEnvironment());

        [WebMethod]
        public static string CreateOrder()
        {
            var appointmentID = HttpContext.Current.Session["AppointmentID"] as string;

            var orderRequest = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest
            {
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = "MYR",
                    Value = "30.00"
                },
                CustomId = appointmentID // Attach appointment ID
            }
        }
            };

            try
            {
                var environment = new SandboxEnvironment(ConfigurationManager.AppSettings["PayPalClientID"], ConfigurationManager.AppSettings["PayPalSecret"]);
                var client = new PayPalHttpClient(environment);
                var request = new OrdersCreateRequest();
                request.Prefer("return=representation");
                request.RequestBody(orderRequest);

                var response = client.Execute(request).Result;
                var order = response.Result<Order>();

                // Debug output for server-side logging
                Console.WriteLine("Response: " + new JavaScriptSerializer().Serialize(order));

                var approvalUrl = order.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;

                if (!string.IsNullOrEmpty(approvalUrl))
                {
                    return new JavaScriptSerializer().Serialize(new { approvalUrl = approvalUrl });
                }
                else
                {
                    throw new Exception("Approval URL is missing in the PayPal response.");
                }
            }
            catch (Exception ex)
            {
                return new JavaScriptSerializer().Serialize(new { error = ex.Message });
            }
        }

    }
}