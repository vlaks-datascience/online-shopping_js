using back_shopping.DTOs;
using back_shopping.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace back_shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IService _service;
        public ProductController(IService userService)
        {
            _service = userService;
        }

        [HttpPost]
        [Route("/newproduct")]
        public IActionResult NewProduct([FromForm] double _amount, [FromForm] double _price, [FromForm] string _description, [FromForm] string _productName, [FromForm] IFormFile _image, [FromForm] int _userId)
        {
            ProductDTO productDTO = new ProductDTO();
            productDTO.Amount = _amount;
            productDTO.Price = _price;
            productDTO.Description = _description;
            productDTO.Name = _productName;
            productDTO.UserId = _userId;
            if (_image != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    _image.CopyTo(memoryStream);
                    byte[] imageData = memoryStream.ToArray();
                    productDTO.Image = imageData;

                }
            }

            return Ok(_service.AddProduct(productDTO));
        }


        [HttpGet("getproducts/{_userId}")]
        public IActionResult GetProducts(int _userId)
        {
            return Ok(_service.GetProducts(_userId));
        }


        [HttpPut("deleteproduct/{_id}")]
        public string DeleteProduct(int _id)
        {
            return _service.DeleteProduct(_id);
        }


        [HttpGet("getproduct/{_id}")]
        public IActionResult GetProduct(int _id)
        {
            return Ok(_service.GetProduct(_id));
        }


        [HttpPut("editproduct/{_id}")]
        public IActionResult EditProduct(int _id, [FromBody] ProductDTO _pro)
        {
            return Ok(_service.ChangeProduct(_id, _pro));
        }


        [HttpGet("/getallproducts")]
        public IActionResult GetAllProducts()
        {
            return Ok(_service.GetAllProducts());
        }
    }
}
