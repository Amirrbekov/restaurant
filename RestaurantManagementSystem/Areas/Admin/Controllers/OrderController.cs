using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using Utility;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public OrderViewModel OrderViewModel { get; set; }

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }
    //Details id = 0 and orderViewModel is null. Me need fix that
    [HttpGet]
    public IActionResult Details(int orderId)
    {
        OrderViewModel = new()
        {
            OrderHeader = _unitOfWork.OrderHeader.Get(u => u.OrderHeaderId == orderId, includeProperties: "ApplicationUser"),
            OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
        };

        return View(OrderViewModel);
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult UpdateOrderDetail()
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.OrderHeaderId == OrderViewModel.OrderHeader.OrderHeaderId);
        orderHeaderFromDb.Name = OrderViewModel.OrderHeader.Name;
        orderHeaderFromDb.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
        orderHeaderFromDb.City = OrderViewModel.OrderHeader.City;

        if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.Carrier))
        {
            orderHeaderFromDb.Carrier = OrderViewModel.OrderHeader.Carrier;
        }
        if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.TrackingNumber))
        {
            orderHeaderFromDb.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
        }
        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();

        TempData["success"] = "Order Details Updated Seccessfully.";

        return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.OrderHeaderId });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult StartProcessing()
    {
        _unitOfWork.OrderHeader.UpdateStatus(OrderViewModel.OrderHeader.OrderHeaderId, SD.StatusInProcess);
        _unitOfWork.Save();

        TempData["success"] = "Order Details Updated Seccessfully.";

        return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.OrderHeaderId });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult ShipOrder()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(u => u.OrderHeaderId == OrderViewModel.OrderHeader
        .OrderHeaderId);

        orderHeader.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
        orderHeader.Carrier = OrderViewModel.OrderHeader.Carrier;
        orderHeader.OrderStatus = SD.StatusShipped;
        orderHeader.ShippingDate = DateTime.Now;

        if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        }

        _unitOfWork.OrderHeader.Update(orderHeader);
        _unitOfWork.Save();

        TempData["success"] = "Order Shipped Seccessfully.";

        return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.OrderHeaderId });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult CancelOrder()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(u => u.OrderHeaderId == OrderViewModel.OrderHeader.OrderHeaderId);

        if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.OrderHeaderId, SD.StatusCancelled, SD.StatusRefunded);
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.OrderHeaderId, SD.StatusCancelled, SD.StatusCancelled);
        }

        _unitOfWork.Save();

        TempData["success"] = "Order Cancelled Seccessfully.";

        return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.OrderHeaderId });
    }

    [ActionName(nameof(Details))]
    [HttpPost]
    public IActionResult Details_PAY_NOW()
    {
        OrderViewModel.OrderHeader = _unitOfWork.OrderHeader.Get(u => u.OrderHeaderId == OrderViewModel.OrderHeader.OrderHeaderId, includeProperties: "ApplicationUser");
        OrderViewModel.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderViewModel.OrderHeader.OrderHeaderId, includeProperties: "Product");

        //stripe logic
        var domain = "https://localhost:7240/";
        var options = new SessionCreateOptions
        {
            SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?orderHeaderId={OrderViewModel.OrderHeader.OrderHeaderId}",
            CancelUrl = domain + $"Admin/Order/details?orderId={OrderViewModel.OrderHeader.OrderHeaderId}",
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
        };

        foreach (var item in OrderViewModel.OrderDetail)
        {
            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name
                    }
                },
                Quantity = item.Count
            };
            options.LineItems.Add(sessionLineItem);
        }

        var service = new SessionService();
        Session session = service.Create(options);

        _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderViewModel.OrderHeader.OrderHeaderId, session.Id, session.PaymentIntentId);
        _unitOfWork.Save();

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public IActionResult PaymentConfirmation(int orderHeaderId)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.OrderHeaderId == orderHeaderId);
        if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            //this is an order by company
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
        }

        return View(orderHeaderId);
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> objOrderHeaders;

        if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
        {
            objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
        }

        switch (status)
        {
            case "pending":
                objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                break;
            case "inprocess":
                objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.StatusInProcess);
                break;
            case "completed":
                objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.StatusShipped);
                break;
            case "approved":
                objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusApproved);
                break;
            default:
                break;
        }

        return Json(new { data = objOrderHeaders });
    }

    #endregion
}
