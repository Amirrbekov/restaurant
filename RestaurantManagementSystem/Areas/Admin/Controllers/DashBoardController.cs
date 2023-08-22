using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

[Authorize(Roles = SD.Role_Admin)]
[Area("Admin")]
public class DashBoardController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}