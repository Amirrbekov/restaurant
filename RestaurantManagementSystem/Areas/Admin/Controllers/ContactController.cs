using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

[Area("Admin"), Authorize]
public class ContactController : Controller
{
	private readonly ApplicationDbContext _db;
    public ContactController(ApplicationDbContext db)
    {
        _db = db;
    }
    public IActionResult Index()
    {
        return View(_db.Contacts.ToList());
    }

	public async Task<IActionResult> Details(int? id)
	{
		if (id == null || _db.Contacts == null)
		{
			return NotFound();
		}

		var contact = await _db.Contacts
			.FirstOrDefaultAsync(m => m.Id == id);
		if (contact == null)
		{
			return NotFound();
		}

		return View(contact);
	}

	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null || _db.Contacts == null)
		{
			return NotFound();
		}

		var contact = await _db.Contacts
			.FirstOrDefaultAsync(m => m.Id == id);
		if (contact == null)
		{
			return NotFound();
		}

		return View(contact);
	}
}
