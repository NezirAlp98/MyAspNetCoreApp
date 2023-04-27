﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using MyAspNetCoreApp.Web.Helpers;
using MyAspNetCoreApp.Web.Models;
using MyAspNetCoreApp.Web.ViewModels;

namespace MyAspNetCoreApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;
        

        private readonly ProductRepository _productRepository;

        public ProductsController(AppDbContext context, IMapper mapper)
        {
            //DI Container
            //Dependency Injection Pattern
            //IHelper hangi class'a bağlı olduğunu göstermek için program.cs'e singleton ekledik.
            _productRepository = new ProductRepository();
            _context = context;
            _mapper = mapper;

            //if (!_context.Products.Any())
            //{
            //    _context.Add(new Product { Name = "Kalem 1", Price = 100, Stock = 100,Color="Red" });
            //    _context.Add(new Product { Name = "Kalem 2", Price = 100, Stock = 200, Color = "Red" });
            //    _context.Add(new Product { Name = "Kalem 3", Price = 100, Stock = 300, Color = "Red"});

            //    _context.SaveChanges();
            //}



        }

        public IActionResult Index()
        {
            //var products = _productRepository.GetAll(); ilk hali

            var products = _context.Products.ToList();
            
            return View(_mapper.Map<List<ProductViewModel>>(products));
        }

        public IActionResult Remove(int id)
        {
            var product = _context.Products.Find(id);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            
            ViewBag.Expire= new Dictionary<string, int>()
            {
                { "1 Ay",1},
                { "3 Ay",3},
                { "6 Ay",6},
                { "12 Ay",12}
            };

            ViewBag.ColorSelect = new SelectList(new List<ColorSelectList>()
            {
                new(){Data="Mavi", Value="Mavi"},
                new(){Data="Kırmızı", Value="Kırmızı"},
                new(){Data="Sarı", Value="Sarı"}
            },"Value","Data");

            return View();
        }

        [HttpPost]
        public IActionResult Add(ProductViewModel newProduct)
        {
            //1.yöntem
            //var name = HttpContext.Request.Form["Name"].ToString();
            //var price = decimal.Parse(HttpContext.Request.Form["Price"].ToString());
            //var stock = int.Parse(HttpContext.Request.Form["Stock"].ToString());
            //var color = HttpContext.Request.Form["Color"].ToString();

            //2.yöntem
            //Product newProduct=new Product() { Name=Name,Price=Price,Stock=Stock,Color=Color};

            //if (!string.IsNullOrEmpty(newProduct.Name) && newProduct.Name.StartsWith("A"))
            //{
            //    ModelState.AddModelError(string.Empty, "Ürün ismi A harfi ile başlayamaz.");
            //}

            ViewBag.Expire = new Dictionary<string, int>()
            {
                { "1 Ay",1},
                { "3 Ay",3},
                { "6 Ay",6},
                { "12 Ay",12}
            };

            ViewBag.ColorSelect = new SelectList(new List<ColorSelectList>()
            {
                new(){Data="Mavi", Value="Mavi"},
                new(){Data="Kırmızı", Value="Kırmızı"},
                new(){Data="Sarı", Value="Sarı"}
            }, "Value", "Data");

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Products.Add(_mapper.Map<Product>(newProduct));
                    _context.SaveChanges();

                    TempData["status"] = "Ürün başarıyla eklendi.";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {

                    ModelState.AddModelError(String.Empty, "Ürün kaydedilirken hata oluştu.Tekrar deneyiniz");
                    return View();
                }


            }

            else
            {

                return View();

            }
            
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _context.Products.Find(id);

            ViewBag.ExpiredValue = product.Expire;
            ViewBag.Expire = new Dictionary<string, int>()
            {
                { "1 Ay",1},
                { "3 Ay",3},
                { "6 Ay",6},
                { "12 Ay",12}
            };

            ViewBag.ColorSelect = new SelectList(new List<ColorSelectList>()
            {
                new(){Data="Mavi", Value="Mavi"},
                new(){Data="Kırmızı", Value="Kırmızı"},
                new(){Data="Sarı", Value="Sarı"}
            }, "Value", "Data",product.Color);
            return View(_mapper.Map<ProductViewModel>(product));
        }

        [HttpPost]
        public IActionResult Update(ProductViewModel updateProduct)
        {
            
            if (!ModelState.IsValid)
            {
                ViewBag.ExpiredValue = updateProduct.Expire;
                ViewBag.Expire = new Dictionary<string, int>()
            {
                { "1 Ay",1},
                { "3 Ay",3},
                { "6 Ay",6},
                { "12 Ay",12}
            };

                ViewBag.ColorSelect = new SelectList(new List<ColorSelectList>()
            {
                new(){Data="Mavi", Value="Mavi"},
                new(){Data="Kırmızı", Value="Kırmızı"},
                new(){Data="Sarı", Value="Sarı"}
            }, "Value", "Data", updateProduct.Color);
                return View();
            }

            
            _context.Products.Update(_mapper.Map<Product>(updateProduct));
            _context.SaveChanges();

            TempData["status"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        [AcceptVerbs("GET","POST")] //Hem get hem de post metodunda çalışması sağlanır.
        public IActionResult HasProductName(string Name)
        {
            var anyProduct=_context.Products.Any(x => x.Name.ToLower() == Name.ToLower());
            if (anyProduct)
            {
                return Json("Kaydetmeye çalıştığınız ürün ismi veritabanında bulunmaktadır.");
            }
            else
            {
                return Json(true);
            }
        }
    }
}
