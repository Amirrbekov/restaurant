using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Comment;
using Models.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using Utility;

namespace Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public HomeController( IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

	public IActionResult Contact()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Contact(Contact contact)
    {
        if (ModelState.IsValid)
        {
            try
            {
                contact.CreateDate = DateTime.Now;
                _unitOfWork.Contact.Add(contact);
                _unitOfWork.Save();
                TempData["Message"] = "Message was submited successfully";
                return RedirectToAction(nameof(Contact));
            }
            catch (Exception)
            {
                TempData["Message"] = "Something went wrong! Message was't submitted";
                throw;
            }
        }
        return View(contact);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}