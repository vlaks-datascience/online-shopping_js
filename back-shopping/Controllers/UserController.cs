using back_shopping.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using back_shopping.Interface;
using System.Xml.Linq;

namespace back_shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IService _service;
        public UserController(IService userService)
        {
            _service = userService;
        }


        [HttpGet("{_id}")]
        public IActionResult Get(int _id)
        {
            UserDTO u = _service.GetCurrentUser(_id);
            var imageBase64 = Convert.ToBase64String(u.Image);
            return Ok(new { u, imageBase64 });
        }


        [HttpPut("{_id}")]
        public string Updateprofile(int _id, [FromForm] string _username, [FromForm] string _name, [FromForm] string _surname, [FromForm] DateTime _date, [FromForm] string _email, [FromForm] string _address, [FromForm] IFormFile _image)
        {
            UserDTO us = new UserDTO();
            us.Username = _username;
            us.Name = _name;
            us.Surname = _surname;
            us.Date = _date;
            us.Email = _email;
            us.Address = _address;
            
            if (_image != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    _image.CopyTo(memoryStream);
                    byte[] imageData = memoryStream.ToArray();
                    us.Image = imageData;
                }
            }
            return _service.Updateuser(_id, us);
        }


        [HttpPut("updatepassword/{_id}")]
        public string UpdatePassword(int _id, [FromBody] UserDTO _userDTO)
        {
            return _service.UpdatePassword(_id, _userDTO.Password);
        }


        [HttpGet]
        [Route("/getunacceptedusers")]
        public IActionResult GetUnacceptedusers()
        {
            return Ok(_service.GetUnacceptedUsers());
        }


        [HttpGet]
        [Route("/getacceptedusers")]
        public IActionResult GetAcceptedusers()
        {
            return Ok(_service.GetAcceptedUsers());
        }


        [HttpGet]
        [Route("/getdeclinedusers")]
        public IActionResult GetDeclinedusers()
        {
            return Ok(_service.GetDeclinedUsers());
        }


        [HttpPut("acceptuser/{_id}")]
        public string AcceptUser(int _id)
        {
            return _service.AcceptUser(_id);
        }


        [HttpPut("declineuser/{_id}")]
        public string DeclineUser(int _id)
        {
            return _service.DeclineUser(_id);
        }
    }
}
