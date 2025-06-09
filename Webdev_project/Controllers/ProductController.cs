using Microsoft.AspNetCore.Mvc;
using Webdev_project.Data;
using Webdev_project.Models;
using Webdev_project.Interfaces;
using Webdev_project.Helpers;
using System.Runtime.InteropServices;
namespace Webdev_project.Controllers
{
    [Route("Product")]
    public class ProductController : BaseController
    {
       
        public readonly IProductRepository productRepository;
        public readonly IUserDetailRepository userDetailRepository;
        public readonly IAuthenticationRepository authenticationRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IReviewRepository reviewRepository;
        public ProductController(IProductRepository productRepository, IAuthenticationRepository authenticationRepository, IUserDetailRepository userDetailRepository,IReviewRepository reviewRepository , IWebHostEnvironment environment) : base(authenticationRepository)
        {
            this.productRepository = productRepository;
            this.authenticationRepository = authenticationRepository;
            this .userDetailRepository = userDetailRepository;
            this.reviewRepository = reviewRepository;
            _environment = environment;
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }
            string sessionId = HttpContext.Request.Cookies["SessionId"];
            if (sessionId != null) 
            {
                var user= authenticationRepository.RetrieveFromSession(sessionId);
                if (user != null) 
                {
                    await userDetailRepository.UpdateCategoryScoreAsync(user.Id, product.Category, UserAction.Click);
                }
            }
            var reviews = await reviewRepository.GetReviewsByProductIdAsync(product.ProductId);
            ViewBag.Reviews = reviews;
            ViewBag.Product = product;
            //return NotFound(ViewBag.Product);
            return View();
        }



        [HttpPost]
        [Route("AddReview")]
        public async Task<IActionResult> AddReview(IFormCollection form)
        {
            string productId = form["productId"];
            string comment = form["comment"];
            int.TryParse(form["rating"], out int stars);
            // Replace with your logic to get the current user ID
            var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]);

            int userId = user.Id; // Example hardcoded user ID

            var uploadedFiles = form.Files;
            var mediaUrls = new List<string>();

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            // Ensure the folder exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var file in uploadedFiles)
            {
                if (file.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    mediaUrls.Add("/uploads/" + uniqueFileName);
                }
            }

            var review = new Review
            {
                //ReviewId = Guid.NewGuid().ToString(),
                ProductId = productId,
                UserID = userId,
                Stars = stars,
                Comment = comment,
                CreatedTime = DateTime.Now,
                MediaURLs = mediaUrls
            };
            
            // TODO: Save `review` to your database
            await reviewRepository.CreateReviewAsync(review);
            await productRepository.IncrementProductRatingAsync(productId, stars);

            return Ok(review);
        }












        [HttpGet("Search")]
        public async Task<IActionResult> Search(string keyword, int page = 1)
        {
            int pageSize = 42;
            ConverterHelper converterHelper = new ConverterHelper();

            var searchResults = await productRepository.SearchAsync(keyword);
            List<Product_zip> product_Zips = converterHelper.ConvertProductListToProductZipList(searchResults);

            int totalProducts = product_Zips.Count;
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            var pagedProducts = product_Zips.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.product = pagedProducts;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.Keyword = keyword;

            return View("~/Views/SearchResult/SearchResults.cshtml", pagedProducts);

        }



    }
}
