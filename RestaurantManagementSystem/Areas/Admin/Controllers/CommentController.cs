using Microsoft.AspNetCore.Mvc;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

public class CommentController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}
