using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MvcBook.Data;
using MvcBook.Models;
using System.Net;
using Order = MvcBook.Models.Order;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace MvcBook.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PayPalSettings _payPalSettings;
        private readonly MvcBookContext _context;
        private readonly ILogger<PaymentController> _logger;
        private readonly SmtpSettings _smtpSettings;
        private readonly IFluentEmail _emailSender;

        public PaymentController(IOptions<PayPalSettings> payPalSettings, MvcBookContext mvcBookContext, ILogger<PaymentController> logger, IOptions<SmtpSettings> smtpSettings, IFluentEmail emailSender)
        {
            _payPalSettings = payPalSettings.Value;
            _context = mvcBookContext;
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
            _emailSender = emailSender;
        }

        //set up paypal enviornment
        private PayPalHttpClient _payPalClient()
        {
            object environment;
            if (_payPalSettings.Environment == "sandbox")
            {
                environment = new SandboxEnvironment(_payPalSettings.ClientId, _payPalSettings.ClientSecret);
            }
            else
            {
                environment = new LiveEnvironment(_payPalSettings.ClientId, _payPalSettings.ClientSecret);
            }

            return new PayPalHttpClient((PayPalEnvironment)environment);
        }

        [AllowAnonymous]
        public async Task<IActionResult> CreatePayment(string orderId, string paypalId)
        {
            //get order details
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);


            if (order == null)
            {
                return NotFound();
            }

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody(order, paypalId));

            try
            {
                var response = await _payPalClient().Execute(request);
                var statusCode = response.StatusCode;
                var result = response.Result<PayPalCheckoutSdk.Orders.Order>();


                if (statusCode == HttpStatusCode.Created)
                {
                    paypalId = result.Id; // update returnurl to pass the payment id- this is updated to be the token.
                    var approvalLink = result.Links.FirstOrDefault(l => l.Rel == "approve");
                    return Redirect(approvalLink.Href);
                }
            }
            catch (PayPalHttp.HttpException ex)
            {
                return RedirectToAction("PaymentError", "Payment");
            }

            return RedirectToAction("PaymentError", "Payment");
        }

        private OrderRequest BuildRequestBody(Order order, string paypalId)
        {
            // Use the order information to create the PayPal payment request
            decimal paymentAmount = order.Total;
            return new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    //ReturnUrl = Url.Action("ExecutePayment", "Payment", new { orderId = order.OrderId, paypalId }, Request.Scheme),
                    CancelUrl = Url.Action("Index", "ShoppingCart", null, Request.Scheme),
                    ReturnUrl = Url.Action("ConfirmOrder", "Payment", new { orderId = order.OrderId, paypalId }, Request.Scheme)
                },

                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "GBP",
                            Value = paymentAmount.ToString("0.00")
                        }
                    }
                }
            };
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmOrder(string orderId, string token)
        {
            var order = _context.Orders
                .Include(o => o.ShoppingCartItems)
                .ThenInclude(s => s.Book)
                .ThenInclude(b => b.Authors)
                .FirstOrDefault(o => o.OrderId == orderId);
            order.PaymentTransactionId = token;
            await _context.SaveChangesAsync();
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.OrderId = orderId;
            ViewBag.Token = token;
            return View(order);
        }


        [AllowAnonymous]
        public async Task<IActionResult> ExecutePayment(string orderId, string token)
        {
            var order = _context.Orders
                .Include(o => o.ShoppingCartItems)
                .ThenInclude(s => s.Book)
                .ThenInclude(b => b.Authors)
                .FirstOrDefault(o => o.OrderId == orderId);
           
            if (order == null)
            {
                return NotFound();
            }

            var request = new OrdersCaptureRequest(order.PaymentTransactionId);
            request.RequestBody(new OrderActionRequest());
            try
            {
                var response = await _payPalClient().Execute(request);
                var statusCode = response.StatusCode;
                var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

                if (statusCode == HttpStatusCode.Created)
                {

                    if (result.Status == "COMPLETED")
                    {
                        //confimation of payment
                        var emailResponse = await SendEmail(order) as ContentResult;
                        if (emailResponse.Content == "created")
                        {
                            Console.WriteLine("success");
                            return View("Success", order);
   
                        }
                        else
                        {
                            return View("PaymentError"); //error handling here
                        }
                    }
                }
            }
            catch (PayPalHttp.HttpException ex)
            {
                _logger.LogError($"Request URI: {request.Path}");
                _logger.LogError($"Request Headers: {request.Headers}");
                _logger.LogError($"Request Body: {request.RequestBody}");
                _logger.LogError($"Error: {ex}");

                return RedirectToAction("PaymentError");
            }
            return RedirectToAction("PaymentError");

        }

        [AllowAnonymous]
        public async Task<IActionResult> SendEmail(Order order)
        {
            try
            {
                await _emailSender
                    .To("claudiachurch68@icloud.com")
                    .Subject("Your order confirmation")
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Payment/Email.cshtml", order)
                    .SendAsync();

                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created, //  201
                    Content = "created",
                    ContentType = "text/plain",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError, // 500
                    Content = "error",
                    ContentType = "text/plain",
                };
            }

        }

        [AllowAnonymous]
        public async Task<IActionResult> PaymentError()
        {
            return View();
        }
    }

}
