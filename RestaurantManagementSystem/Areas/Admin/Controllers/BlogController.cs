using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Blog;
using System.Security.Claims;

namespace RestaurantManagementSystem.Areas.Admin.Controllers;

[Area("Admin")]
public class BlogController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public BlogController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Upsert(int? id)
    {

		Post post = new();

        if (id == null || id == 0)
        {
            return View(post);
        }
        else
        {
            post = _unitOfWork.Post.Get(u => u.PostId == id);
            return View(post);
        }
    }
    [HttpPost]
    public IActionResult Upsert(Post post, IFormFile? formFile)
    {
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

		post.ApplicationUserId = userId;

		if (ModelState.IsValid)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (formFile != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\post");

                if (!string.IsNullOrEmpty(post.Image))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, post.Image.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }

                post.Image = @"\images\post\" + filename;
            }

            if (post.PostId == 0)
            {
                _unitOfWork.Post.Add(post);
            }
            else
            {
                _unitOfWork.Post.Update(post);
            }

            _unitOfWork.Save();
            TempData["success"] = "Post was created successffuly";
            return RedirectToAction(nameof(Index));
        }

        return View(post);
    }

    #region API CALLS

    public IActionResult GetAll()
    {
        List<Post> objPost = _unitOfWork.Post.GetAll().ToList();

        return Json(new { data = objPost });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var postToBeDeleted = _unitOfWork.Post.Get(u => u.PostId == id);

        if (postToBeDeleted == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var olddImagePath = Path.Combine(_webHostEnvironment.WebRootPath, postToBeDeleted.Image.TrimStart('\\'));

        if (System.IO.File.Exists(olddImagePath))
        {
            System.IO.File.Delete(olddImagePath);
        }

        _unitOfWork.Post.Delete(postToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }



    #endregion
}
