using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Stripe;
using Stripe.Checkout;

namespace fyp1.Client
{
    public partial class CreateCheckoutSession : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var paymentID = Request.QueryString["paymentID"];

            if (string.IsNullOrEmpty(paymentID))
            {
                Response.StatusCode = 400;
                Response.Write("{\"error\":\"Missing payment ID.\"}");
                return;
            }

            StripeConfiguration.ApiKey = "your-secret-key-here"; // Your Stripe Secret Key

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Appointment Payment",
                            },
                            UnitAmount = 5000, // Amount in cents (e.g., 5000 for $50.00)
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"https://localhost:44387/Client/paymentSuccess.aspx?paymentID={paymentID}",
                CancelUrl = "https://localhost:44387/Client/paymentPage.aspx",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.ContentType = "application/json";
            Response.Write("{\"id\":\"" + session.Id + "\"}");
        }
    }
}