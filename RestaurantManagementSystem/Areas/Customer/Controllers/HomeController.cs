using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

	[HttpPost]
	public IActionResult Index(TableReservation reservation)
	{
		if (!ModelState.IsValid)
		{
			return View(reservation);
		}


		Table table = _unitOfWork.Table.Get(x => x.Seats == reservation.NumberOfPeople);

		if (table == null)
		{
			ModelState.AddModelError("", "No available table with enough seats.");
			return View(reservation);
		}

		TableReservation tableReservation = new()
		{
			TableId = table.Id,
			Name = reservation.Name,
			NumberOfPeople = reservation.NumberOfPeople,
			Id = reservation.Id,
			IsActive = true,
			ArrivalDateTime = reservation.ArrivalDateTime,
		};

		_unitOfWork.TableReservation.Add(tableReservation);
		_unitOfWork.Save();


		return RedirectToAction("Index");
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