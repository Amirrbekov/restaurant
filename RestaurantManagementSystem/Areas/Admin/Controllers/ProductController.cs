using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels;
using Microsoft.AspNetCore.Http;

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
            //create
            return View(productViewModel);
        }
        else
        {
            //update
            productViewModel.Product = _unitOfWork.Product.Get(u => u.ProductId == id, includeProperties:"ProductImages");
            return View(productViewModel);
        }
    }

    [HttpPost]
    public IActionResult Upsert(ProductViewModel productViewModel, List<IFormFile> files)
    {
        if (ModelState.IsValid)
        {
            if (productViewModel.Product.ProductId == 0)
            {
                _unitOfWork.Product.Add(productViewModel.Product);
            }
            else
            {
                _unitOfWork.Product.Update(productViewModel.Product);
            }

            _unitOfWork.Save();

            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (files != null)
            {
                foreach (IFormFile file in files)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = @"images/products/product-" + productViewModel.Product.ProductId;
                    string finalPath = Path.Combine(wwwRootPath, productPath);

                    if (!Directory.Exists(finalPath))
                    {
                        Directory.CreateDirectory(finalPath);
                    }

                    using (var fileStream = new FileStream(Path.Combine(finalPath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProductImage productImage = new()
                    {
                        ImageUrl = @"\" + productPath + @"\" + filename,
                        ProductId = productViewModel.Product.ProductId,
                    };

                    if (productViewModel.Product.ProductImages == null)
                    {
                        productViewModel.Product.ProductImages = new List<ProductImage>();
                    }

                    productViewModel.Product.ProductImages.Add(productImage);

                    
                }

                _unitOfWork.Product.Update(productViewModel.Product);
                _unitOfWork.Save();


                //if (!string.IsNullOrEmpty(productViewModel.Product.Image))
                //{
                //    var oldImagePath = Path.Combine(wwwRootPath, productViewModel.Product.Image.TrimStart('\\'));

                //    if (System.IO.File.Exists(oldImagePath))
                //    {
                //        System.IO.File.Delete(oldImagePath);
                //    }
                //}



                //productViewModel.Product.Image = @"\images\product\" + filename;
            }


            TempData["success"] = "Product was created/updated successffuly";
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

    public IActionResult DeleteImage(int imageId)
    {
        var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
        int productId = imageToBeDeleted.ProductId;
        if (imageToBeDeleted != null)
        {
            if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
            {
                var olddImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(olddImagePath))
                {
                    System.IO.File.Delete(olddImagePath);
                }
            }

            _unitOfWork.ProductImage.Delete(imageToBeDeleted);
            _unitOfWork.Save();

            TempData["success"] = "Deleted successfully";
        }



        return RedirectToAction(nameof(Upsert), new { id = productId });
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

        string productPath = @"images/products/product-" + id;
        string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

        if (Directory.Exists(finalPath))
        {
            string[] finalPaths = Directory.GetFiles(finalPath);
            foreach (string finalPatj in finalPaths)
            {
                System.IO.File.Delete(finalPath);
            }

            Directory.Delete(finalPath);
        }


        _unitOfWork.Product.Delete(productToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion

}
