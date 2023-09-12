using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using Utility;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class TableReservationController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public TableReservationController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index() => View(_unitOfWork.TableReservation.GetAll());

    public IActionResult CreateReservation(int? id)
    {
        TableReservationViewModel tableReservationViewModel = new()
        {
            TableList = _unitOfWork.Table.GetAll().Select(u => new SelectListItem
            {
                Text = u.Seats.ToString(),
                Value = u.Id.ToString()
            }),
            TableReservation = new TableReservation()
        };

        if (id == null || id == 0)
        {
            //create
            return View(tableReservationViewModel);
        }
        else
        {
            //update
            tableReservationViewModel.TableReservation = _unitOfWork.TableReservation.Get(u => u.Id == id);
            return View(tableReservationViewModel);
        }
    }

    [HttpPost]
    public IActionResult CreateReservation(TableReservationViewModel tableReservationViewModel)
    {
        if (ModelState.IsValid)
        {
            if (tableReservationViewModel.TableReservation.Id == 0)
            {
                _unitOfWork.TableReservation.Add(tableReservationViewModel.TableReservation);
            }
            else
            {
                _unitOfWork.TableReservation.Update(tableReservationViewModel.TableReservation);
            }

            _unitOfWork.Save();

            TempData["success"] = "Product was created/updated successffuly";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            tableReservationViewModel.TableList = _unitOfWork.Table.GetAll().Select(u => new SelectListItem
            {
                Text = u.Seats.ToString(),
                Value = u.Id.ToString()
            });
            return View(tableReservationViewModel);
        }       
    }

    #region API CALLS

    public IActionResult GetAll()
    {
        var objTableReservation = _unitOfWork.TableReservation.GetAll(includeProperties: "Table").ToList();

        return Json(new { data = objTableReservation });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var tableReservationToBeDeleted = _unitOfWork.TableReservation.Get(u => u.Id == id);

        if (tableReservationToBeDeleted == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        _unitOfWork.TableReservation.Delete(tableReservationToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion
}
