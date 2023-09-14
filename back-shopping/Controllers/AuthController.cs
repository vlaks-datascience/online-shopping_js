using back_shopping.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using back_shopping.Interface;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace back_shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IService _service;
        public AuthController(IService userService)
        {
            _service = userService;
        }


        [HttpPost]
        [Route("/login")]
        public IActionResult Login([FromBody] UserDTO _userDTO)
        {
            UserDTO userDTO = new UserDTO();
            userDTO = _service.GetUser(_userDTO);

            if (userDTO != null)
            {
                return Ok(new { userDTO.Token, userDTO.Id, userDTO.UserType, userDTO.Status });
            }
            else
            {
                return null;
            }
        }


        [HttpPost]
        [Route("/googlelogin")]
        public IActionResult GoogleLogin([FromBody] UserDTO _userDTO)
        {
            UserDTO userDTO = new UserDTO();
            userDTO = _service.GetUserGoogle(_userDTO);
            if (userDTO != null)
            {
                return Ok(new { userDTO.Token, userDTO.Id, userDTO.UserType, userDTO.Status });
            }
            else
            {
                return null;
            }
        }


        [HttpPost]
        [Route("/register")]
        public IActionResult Register([FromForm] string _username, [FromForm] string _password, [FromForm] string _email, [FromForm] string _name, [FromForm] string _surname, [FromForm] DateTime _date, [FromForm] string _address, [FromForm] string _userType, [FromForm] IFormFile _image)
        {
            UserDTO userDTO = new UserDTO();

            userDTO.Username = _username;
            userDTO.Password = _password;
            userDTO.Email = _email;
            userDTO.Name = _name;
            userDTO.Surname = _surname;
            userDTO.Date = _date;
            userDTO.Address = _address;
            userDTO.UserType = _userType;

            if (_userType == "Seller")
            {
                userDTO.Status = "Pending";
            }
            else
            {
                userDTO.Status = "Accepted";
            }

            if (_image != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    _image.CopyTo(memoryStream);
                    byte[] imageData = memoryStream.ToArray();
                    userDTO.Image = imageData;
                }
            }

            return Ok(_service.AddUser(userDTO));
        }
    }
}
