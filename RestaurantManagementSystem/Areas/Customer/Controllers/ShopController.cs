using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
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
		if (pageNumber < 1) return RedirectToAction("Index", new {pageNumber = 1, category});

		var vm = _unitOfWork.Product.GetAllPostsPage(pageNumber, category, search);

		return View(vm);
	}

	[HttpGet]
	public IActionResult Details(int id)
	{
		ShoppingCart cart = new()
		{
			Product = _unitOfWork.Product.Get(u => u.ProductId == id, includeProperties: "Category,ProductImages"),
			Count = 1,
			ProductId = id
		};
		return View(cart);
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

}
