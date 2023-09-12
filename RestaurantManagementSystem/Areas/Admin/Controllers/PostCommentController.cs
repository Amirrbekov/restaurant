using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.Comment;
using Models.ViewModels;
using Utility;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class PostCommentController : Controller
{
	private readonly IUnitOfWork _unitOfWork;

	public PostCommentController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}
	public IActionResult Index()
	{
		List<Comments> objPostComment = _unitOfWork.Comment.GetAll().ToList();

		return View(objPostComment);
	}

	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	public IActionResult Create(PostCommentViewModel postCommentViewModel)
	{
		if (ModelState.IsValid)
		{
			_unitOfWork.Comment.Add(postCommentViewModel.Comments);
			_unitOfWork.Save();
			TempData["success"] = "Comment was added successffuly";
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
		PostCommentViewModel viewModel = new()
		{
			Comments = _unitOfWork.Comment.Get(u => u.Id == id),
            PostList = _unitOfWork.Post.GetAll().Select(u => new SelectListItem
            {
                Text = u.Title,
                Value = u.PostId.ToString()
            }),
        };

		if (viewModel == null)
		{
			return NotFound(nameof(Comments));
		}

		return View(viewModel);
	}

	[HttpPost, ActionName("Delete")]
	public IActionResult DeletePOST(int? id)
	{
        Comments? comments = _unitOfWork.Comment.Get(u => u.Id == id);

		if (comments == null)
		{
			return NotFound();
		}
		_unitOfWork.Comment.Delete(comments);
		_unitOfWork.Save();
		TempData["success"] = "Comment was deleted successffuly";
		return RedirectToAction(nameof(Index));
	}
}
