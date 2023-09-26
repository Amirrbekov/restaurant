using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Utility;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

[Authorize(Roles = SD.Role_Admin)]
[Area("Admin")]
public class DashBoardController : Controller
{
	private readonly IUnitOfWork _unitOfWork;

    public DashBoardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
	{
        var result = _unitOfWork.OrderHeader.GetAll();
        var totalTable = _unitOfWork.Table.GetAll();
        var totalUser = _unitOfWork.ApplicationUser.GetAll();
        DashboardVm vm = new DashboardVm()
        {
            TotalOrderCount = result.Count(),
            TotalOrderPrice = result.Sum(x=>x.OrderTotal),
            TotalTableCount = totalTable.Count(),
            TotalUserCount = totalUser.Count(),
        };
		return View(vm);
	}
}