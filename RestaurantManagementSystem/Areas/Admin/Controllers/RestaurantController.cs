using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

[Authorize(Roles = SD.Role_Admin)]
[Area("Admin")]
public class RestaurantController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public RestaurantController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        List<RestaurantsGroup> objRestaurants = _unitOfWork.RestaurantGroup.GetAll().ToList();
        return View(objRestaurants);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(RestaurantsGroup restaurantsGroup)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.RestaurantGroup.Add(restaurantsGroup);
            _unitOfWork.Save();
            TempData["success"] = "Restaurants Group was added successffuly";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        RestaurantsGroup? restaurantsGroupFromDb = _unitOfWork.RestaurantGroup.Get(u => u.Id == id);

        if (restaurantsGroupFromDb == null)
        {
            return NotFound(nameof(RestaurantsGroup));
        }

        return View(restaurantsGroupFromDb);
    }

    [HttpPost]
    public IActionResult Edit(RestaurantsGroup restaurantsGroup)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.RestaurantGroup.Update(restaurantsGroup);
            _unitOfWork.Save();
            TempData["success"] = "Restaurants Group was updated successffuly";
            return RedirectToAction(nameof(Index));
        }

        return View();

    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        RestaurantsGroup? restaurantsGroupFromDb = _unitOfWork.RestaurantGroup.Get(u => u.Id == id);

        if (restaurantsGroupFromDb == null)
        {
            return NotFound(nameof(RestaurantsGroup));
        }

        return View(restaurantsGroupFromDb);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {
        RestaurantsGroup? restaurantsGroup = _unitOfWork.RestaurantGroup.Get(u => u.Id == id);

        if (restaurantsGroup == null)
        {
            return NotFound();
        }
        _unitOfWork.RestaurantGroup.Delete(restaurantsGroup);
        _unitOfWork.Save();
        TempData["success"] = "Restaurants Group was deleted successffuly";
        return RedirectToAction(nameof(Index));
    }
}
