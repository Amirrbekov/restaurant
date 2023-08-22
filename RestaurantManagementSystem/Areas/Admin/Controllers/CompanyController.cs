using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfwork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public CompanyController(IUnitOfWork unitOfwork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfwork = unitOfwork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        List<Company> objCompany = _unitOfwork.Company.GetAll().ToList();

        return View(objCompany);
    }

    public IActionResult Upsert(int? id)
    {
        if (id == null || id == 0)
        {
            return View(new Company());
        }
        else
        {
            Company companyObj = _unitOfwork.Company.Get(u => u.CompanyId == id);
            return View(companyObj);
        }
    }

    [HttpPost]
    public IActionResult Upsert(Company companyObj)
    {
        if (ModelState.IsValid)
        {
            if (companyObj.CompanyId == 0)
            {
                _unitOfwork.Company.Add(companyObj);
            }
            else
            {
                _unitOfwork.Company.Update(companyObj);
            }

            _unitOfwork.Save();
            TempData["success"] = "Company was created successffuly";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return View(companyObj);
        }
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        List<Company> objCompany = _unitOfwork.Company.GetAll().ToList();

        return Json(new { data = objCompany });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var CompanyToBeDeleted = _unitOfwork.Company.Get(u => u.CompanyId == id);

        if (CompanyToBeDeleted == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        _unitOfwork.Company.Delete(CompanyToBeDeleted);
        _unitOfwork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion
}
