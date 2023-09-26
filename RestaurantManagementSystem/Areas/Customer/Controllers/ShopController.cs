using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Comment;
using Models.ViewModels;
using System.Security.Claims;
using Utility;

namespace RestaurantManagementSystem.Areas.Customer.Controllers;
[Area("Customer")]
public class ShopController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ShopController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index(int pageNumber, string category, string search)
    {
        if (pageNumber < 1) return RedirectToAction("Index", new { pageNumber = 1, category });

        var vm = _unitOfWork.Product.GetAllPostsPage(pageNumber, category, search);

        return View(vm);
    }

    [HttpGet]
    public IActionResult Details(int id)
        {
        ProductDetailViewModel vm = new()
        {
            ShoppingCart = new ShoppingCart()
            {
                Product = _unitOfWork.Product.Get(u => u.ProductId == id, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId = id
            },
            ProductCommentList = _unitOfWork.ProductComment.GetAll(x => x.ProductId == id).ToList(),
        };
        return View(vm);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        shoppingCart.ApplicationUserId = userId;

        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

        if (cartFromDb != null)
        {
            //shopping card exists
            cartFromDb.Count += shoppingCart.Count;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
        }
        else
        {
            //add card record
            _unitOfWork.ShoppingCart.Add(shoppingCart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
        }

        TempData["success"] = "Cart updated successfully";

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> MakeCommentRating(int productId, int rating, string name, string email, string message)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ModelState.AddModelError("Message", "Comment message is required.");
                return RedirectToAction("Details", new { productId });
            }

            ProductComment productComment = new()
            {
                ProductId = productId,
                Name = name,
                Email = email,
                Message = message,
                Created = DateTime.Now,
                Rating = rating
            };

            await _unitOfWork.ProductComment.AddAsync(productComment);

            return RedirectToAction(nameof(Details), new { id = productId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Error", "An error occurred while posting your comment.");
            return RedirectToAction("Details", new { productId });
        }
    }
}
