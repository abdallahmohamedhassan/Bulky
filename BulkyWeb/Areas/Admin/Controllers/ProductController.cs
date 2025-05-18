using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
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
            List<Product> products = _unitOfWork.productRepository.GetAll(includeProperties:"Category").ToList();

            return View(products);
        }
        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.categoryRepository
                 .GetAll().Select(u => new SelectListItem
                 {
                     Text = u.Name,
                     Value = u.Id.ToString(),
                 }),
                Product = new Product()
            };
            if (id == null || id==0) {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.productRepository.Get(u => u.Id == id);
                return View(productVM);

            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
           
            if (ModelState.IsValid)
            {string wwwRootPath= _webHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    string fileName=Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string productPath=Path.Combine(wwwRootPath,@"images\product");
                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.productRepository.Add(obj.Product);
                }
                else {
                    _unitOfWork.productRepository.Update(obj.Product);

                }
                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = _unitOfWork.categoryRepository
                 .GetAll().Select(u => new SelectListItem
                 {
                     Text = u.Name,
                     Value = u.Id.ToString(),
                 });
                return View(obj);

            }
        }
       
       
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.productRepository.GetAll(includeProperties: "Category").ToList();
return Json(new {data= products});

        }
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.productRepository.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }


            _unitOfWork.productRepository.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }


    }
}
