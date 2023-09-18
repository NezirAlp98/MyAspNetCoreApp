using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using MyAspNetCoreApp.Web.Filters;
using MyAspNetCoreApp.Web.Helpers;
using MyAspNetCoreApp.Web.Models;
using MyAspNetCoreApp.Web.ViewModels;

namespace MyAspNetCoreApp.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class ProductsController : Controller
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;
        

        private readonly ProductRepository _productRepository;
        private readonly IFileProvider _fileProvider;

        public ProductsController(AppDbContext context, IMapper mapper, IFileProvider fileProvider)
        {
            //DI Container
            //Dependency Injection Pattern
            //IHelper hangi class'a bağlı olduğunu göstermek için program.cs'e singleton ekledik.
            _productRepository = new ProductRepository();
            _context = context;
            _mapper = mapper;
            _fileProvider = fileProvider;

            //if (!_context.Products.Any())
            //{
            //    _context.Add(new Product { Name = "Kalem 1", Price = 100, Stock = 100,Color="Red" });
            //    _context.Add(new Product { Name = "Kalem 2", Price = 100, Stock = 200, Color = "Red" });
            //    _context.Add(new Product { Name = "Kalem 3", Price = 100, Stock = 300, Color = "Red"});

            //    _context.SaveChanges();
            //}



        }

        //[CacheResourceFilter]
        public IActionResult Index()
        {
            //var products = _productRepository.GetAll(); ilk hali

            var products = _context.Products.ToList();
            
            return View(_mapper.Map<List<ProductViewModel>>(products));
        }

        //[HttpGet("{page}/{pageSize}")]
        [Route("[controller]/[action]/{page}/{pageSize}",Name ="productpage")]
        public IActionResult Pages(int page,int pageSize)
        {
            //page=1 pagesize=3=>ilk 3 kayıt
            //page=2 pagesize=3=>ikinci 3 kayıt
            //page=3 pagesize=3=>üçüncü 3 kayıt

            var products = _context.Products.Skip((page-1)*pageSize).Take(pageSize).ToList();

            ViewBag.page = page;
            ViewBag.pageSize=pageSize;

            return View(_mapper.Map<List<ProductViewModel>>(products));
        }

        [ServiceFilter(typeof(NotFoundFilter))] //notfoundfilter içerisinde constructor tanımladığımızdan böyle oluşturduk. 
        [Route("urunler/urun/{productid}",Name ="product")]
        public IActionResult GetById(int productid)
        {
            var product = _context.Products.Find(productid);
            return View(_mapper.Map<ProductViewModel>(product));
        }

        [ServiceFilter(typeof(NotFoundFilter))]
        [HttpGet("{id}")]
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

            IActionResult result = null;
            
            if (ModelState.IsValid)
            {
                try
                {

                    var root = _fileProvider.GetDirectoryContents("wwwroot");
                    var images = root.First(x => x.Name == "images");

                    var randomImageName = Guid.NewGuid() + Path.GetExtension(newProduct.Image.FileName);

                    var path = Path.Combine(images.PhysicalPath, randomImageName);
                    using var stream = new FileStream(path, FileMode.Create);
                    newProduct.Image.CopyTo(stream);

                    var product = _mapper.Map<Product>(newProduct);
                    product.ImagePath = randomImageName;



                    _context.Products.Add(product);
                    _context.SaveChanges();

                    TempData["status"] = "Ürün başarıyla eklendi.";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {

                    
                    result= View();
                }


            }

            else
            {

                result= View();

            }

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

            return result;


        }

        [ServiceFilter(typeof(NotFoundFilter))]
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

