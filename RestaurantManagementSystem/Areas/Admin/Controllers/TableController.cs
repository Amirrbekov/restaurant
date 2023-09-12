using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using Models.ViewModels;
using Utility;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class TableController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public TableController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        var objTable = _unitOfWork.Table.GetAll(includeProperties: "RestaurantGroup").ToList();

        return View(objTable);
    }

    public IActionResult Upsert(int? id)
    {
        TableViewModel tableViewModel = new()
        {
            RestaurantGroupList = _unitOfWork.RestaurantGroup.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Table = new Models.Table()
        };

        if (id == null || id == 0)
        {
            //create
            return View(tableViewModel);
        }
        else
        {
            //update
            tableViewModel.Table = _unitOfWork.Table.Get(u => u.Id == id);
            return View(tableViewModel);
        }
    }

    [HttpPost]
    public IActionResult Upsert(TableViewModel tableViewModel)
    {
        if (ModelState.IsValid)
        {
            if (tableViewModel.Table.Id == 0)
            {
                _unitOfWork.Table.Add(tableViewModel.Table);
            }
            else
            {
                _unitOfWork.Table.Update(tableViewModel.Table);
            }

            _unitOfWork.Save();

            TempData["success"] = "Table was added/updated successffuly";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            tableViewModel.RestaurantGroupList = _unitOfWork.RestaurantGroup.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(tableViewModel);
        }
    }

    #region API CALLS

    public IActionResult GetAll()
    {
        var objTable = _unitOfWork.Table.GetAll(includeProperties: "RestaurantGroup").ToList();

        return Json(new { data = objTable });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var tableToBeDeleted = _unitOfWork.Table.Get(u => u.Id == id);

        if (tableToBeDeleted == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        _unitOfWork.Table.Delete(tableToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion
}
