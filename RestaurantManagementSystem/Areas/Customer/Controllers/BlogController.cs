using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Models;
using Models.Blog;
using Models.ViewModels;
using System.Security.Claims;

namespace RestaurantManagementSystem.Areas.Customer.Controllers;

[Area("Customer")]
public class BlogController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public BlogController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index(int pageNumber, string search)
    {
        if (pageNumber < 1) return RedirectToAction("Index", new { pageNumber = 1 });

        var vm = _unitOfWork.Post.GetAllPostsPage(pageNumber, search);

        return View(vm);
    }

    public IActionResult Details(int id)
    {
		PostDetailViewModel vm = new()
		{
			Post = _unitOfWork.Post.Get(u => u.PostId == id),
		};

        return View(vm);
    }

}
