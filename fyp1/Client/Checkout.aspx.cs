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
            var orderRequest = new OrdersCreateRequest();
            orderRequest.RequestBody(new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new System.Collections.Generic.List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "MYR",
                        Value = "30.00" // Example amount
                    }
                }
            }
            });

            try
            {
                var response = Client.Execute(orderRequest).Result;
                var order = response.Result<Order>();
                return new JavaScriptSerializer().Serialize(new { id = order.Id });
            }
            catch (Exception ex)
            {
                return new JavaScriptSerializer().Serialize(new { error = ex.Message });
            }
        }

        [WebMethod]
        public static string CaptureOrder(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            try
            {
                var response = Client.Execute(request).Result;
                var capture = response.Result<Order>();
                return new JavaScriptSerializer().Serialize(capture);
            }
            catch (Exception ex)
            {
                return new JavaScriptSerializer().Serialize(new { error = ex.Message });
            }
        }
    }
}