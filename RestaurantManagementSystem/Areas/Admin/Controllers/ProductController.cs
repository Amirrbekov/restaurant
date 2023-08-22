using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels;

namespace Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }
    public IActionResult Index()
    {
        List<Product> objProduct = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

        return View(objProduct);
    }

    public IActionResult Upsert(int? id)
    {
        ProductViewModel productViewModel = new()
        {
            CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.CategoryId.ToString()
            }),
            Product = new Product()
        };

        if (id == null || id == 0)
        {
            return View(productViewModel);
        }
        else
        {
            productViewModel.Product = _unitOfWork.Product.Get(u => u.ProductId == id);
            return View(productViewModel);
        }
    }

    [HttpPost]
    public IActionResult Upsert(ProductViewModel productViewModel, IFormFile? formFile)
    {
        if (ModelState.IsValid)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (formFile != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");

                if (!string.IsNullOrEmpty(productViewModel.Product.Image))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, productViewModel.Product.Image.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }

                productViewModel.Product.Image = @"\images\product\" + filename;
            }

            if (productViewModel.Product.ProductId == 0)
            {
                _unitOfWork.Product.Add(productViewModel.Product);
            }
            else
            {
                _unitOfWork.Product.Update(productViewModel.Product);
            }

            _unitOfWork.Save();
            TempData["success"] = "Product was created successffuly";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.CategoryId.ToString()
            });
            return View(productViewModel);
        }
    }

    #region API CALLS

    public IActionResult GetAll()
    {
        List<Product> objProduct = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

        return Json(new { data = objProduct });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var productToBeDeleted = _unitOfWork.Product.Get(u => u.ProductId == id);

        if (productToBeDeleted == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var olddImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.Image.TrimStart('\\'));

        if (System.IO.File.Exists(olddImagePath))
        {
            System.IO.File.Delete(olddImagePath);
        }

        _unitOfWork.Product.Delete(productToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion

}
