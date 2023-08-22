using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Stripe.Checkout;
using System.Security.Claims;
using Utility;

namespace RestaurantManagementSystem.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartViewModel = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
            OrderHeader = new()
        };

        IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

        foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
        {
            cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.ProductId).ToList();
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        return View(ShoppingCartViewModel);
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartViewModel = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
            OrderHeader = new()
        };

        ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

        //Name
        ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
        //Phone Number
        ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
        //StreetAddress
        ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
        //City
        ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;

        foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        return View(ShoppingCartViewModel);
    }

    [HttpPost, ActionName("Summary")]
    public IActionResult SummaryPOST()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

        ShoppingCartViewModel.OrderHeader.OrderDate = System.DateTime.Now;
        ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;

        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

        foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            // it is a regular customer
            ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusPending;
        }
        else
        {
            //it is a company user
            ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusApproved;
        }

        _unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
        _unitOfWork.Save();

        foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
        {
            OrderDetail orderDetail = new()
            {
                ProductId = cart.ProductId,
                OrderHeaderId = ShoppingCartViewModel.OrderHeader.OrderHeaderId,
                Price = cart.Price,
                Count = cart.Count,
            };
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Save();
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            // it is a regular customer account and we need to capture payment 
            //stripe logic
            var domain = "https://localhost:7039/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.OrderHeaderId}",
                CancelUrl = domain + "Customer/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in ShoppingCartViewModel.ShoppingCartList)
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

            _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartViewModel.OrderHeader.OrderHeaderId, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.OrderHeaderId });
    }

    public IActionResult OrderConfirmation(int id)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.OrderHeaderId == id, includeProperties: "ApplicationUser");
        if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
        {
            //this is an order by customer
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
            HttpContext.Session.Clear();
        }

        List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

        _unitOfWork.ShoppingCart.DeleteAll(shoppingCarts);
        _unitOfWork.Save();

        return View(id);
    }

    public IActionResult Plus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId, tracked: true);
        cartFromDb.Count += 1;
        _unitOfWork.ShoppingCart.Update(cartFromDb);
        _unitOfWork.Save();
        return RedirectToAction("Index");
    }

    public IActionResult Minus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId, tracked: true);
        if (cartFromDb.Count <= 1)
        {
            //remove
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.ShoppingCart.Delete(cartFromDb);
        }
        else
        {
            cartFromDb.Count -= 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }
        _unitOfWork.Save();
        return RedirectToAction("Index");
    }
    
    public IActionResult Remove(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId, tracked: true);
        HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
        _unitOfWork.ShoppingCart.Delete(cartFromDb);
        _unitOfWork.Save();
        return RedirectToAction("Index");
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        return shoppingCart.Product.Price;
    }
}
