using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Models;
using Models.Blog;
using Models.Comment;
using Models.ViewModels;
using System.ComponentModel.Design;
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
			CommentList = _unitOfWork.Comment.GetAll(x => x.PostId == id).ToList(),
		};

        return View(vm);
    }
    [HttpPost]
	public async Task<IActionResult> Details(int id, string name, string email, string message)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				ModelState.AddModelError("Message", "Comment message is required.");
				return RedirectToAction("ViewBlogPost", new { id });
			}

			Comments comment = new()
			{
				PostId = id,
				Name = name,
				Email = email,
				Message = message,
				Created = DateTime.Now
			};

			await _unitOfWork.Comment.AddAsync(comment);

			return RedirectToAction(nameof(Details), new { id });
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("Error", "An error occurred while posting your comment.");
			return RedirectToAction("ViewBlogPost", new { id });
		}
	}
}
