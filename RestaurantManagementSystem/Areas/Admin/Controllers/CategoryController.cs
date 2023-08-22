using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Utility;

namespace Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        List<Category> objCategory = _unitOfWork.Category.GetAll().ToList();
        return View(objCategory);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Add(category);
            _unitOfWork.Save();
            TempData["success"] = "Category was created successffuly";
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
        Category? categoryFromDb = _unitOfWork.Category.Get(u => u.CategoryId == id);

        if (categoryFromDb == null)
        {
            return NotFound(nameof(Category));
        }

        return View(categoryFromDb);
    }

    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            TempData["success"] = "Category was updated successffuly";
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
        Category? categoryFromDb = _unitOfWork.Category.Get(u => u.CategoryId == id);

        if (categoryFromDb == null)
        {
            return NotFound(nameof(Category));
        }

        return View(categoryFromDb);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {
        Category? category = _unitOfWork.Category.Get(u => u.CategoryId == id);

        if (category == null)
        {
            return NotFound();
        }
        _unitOfWork.Category.Delete(category);
        _unitOfWork.Save();
        TempData["success"] = "Category was deleted successffuly";
        return RedirectToAction(nameof(Index));
    }
}
