using back_shopping.DTOs;
using back_shopping.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace back_shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IService _service;
        public OrderController(IService userService)
        {
            _service = userService;
        }


        [HttpPost]
        [Route("/ordering")]
        public IActionResult Ordering([FromBody] OrderDTO _orderDTO)
        {
            _orderDTO.OrderDate = DateTime.Now;
            Random random = new Random();
            _orderDTO.IsCanceled = false;
            _orderDTO.IsSent = false;

            if (_orderDTO.PaymentMethod == "cod")
            {
                _orderDTO.IsPaid = true;
            }
            else
            {
                _orderDTO.IsPaid = false;
            }

            return Ok(_service.Ordering(_orderDTO));
        }


        [HttpGet("/previousorders/{_userId}")]
        public IActionResult PreviousOrders(int _userId)
        {
            return Ok(_service.GetAllOrders(_userId));
        }


        [HttpGet("/activeorders/{_userId}")]
        public IActionResult ActiveOrders(int _userId)
        {

            return Ok(_service.GetActiveOrders(_userId));
        }


        [HttpGet("/ordersonwait/{_userId}")]
        public IActionResult WaitingOrders(int _userId)
        {

            return Ok(_service.GetWaitingOrders(_userId));
        }


        [HttpGet("/ordersonwaitseller/{_userId}")]
        public IActionResult WaitingOrdersSeller(int _userId)
        {

            return Ok(_service.GetWaitingOrdersSeller(_userId));
        }


        [HttpGet("/paypalorders/{_userId}")]
        public IActionResult PayPalOrders(int _userId)
        {
            return Ok(_service.GetPayPalOrders(_userId));
        }


        [HttpPut("/cancelorder/{_orderId}")]
        public string CancelOrder(int _orderId)
        {
            return (_service.CancelOrder(_orderId));
        }


        [HttpPut("/sendorder/{_orderId}")]
        public string SendOrder(int _orderId)
        {
            return (_service.SendOrder(_orderId));
        }


        [HttpPut("/paybypaypal/{_orderId}")]
        public string PayOrder(int _orderId)
        {
            return (_service.PayOrder(_orderId));
        }


        [HttpGet("/sellerorders/{_userId}")]
        public IActionResult GetSellerOrders(int _userId)
        {
            return Ok(_service.SellerOrders(_userId));
        }


        [HttpGet("/selleractiveorders/{_userId}")]
        public IActionResult GetSellerActiveOrders(int _userId)
        {
            return Ok(_service.SellerActiveOrders(_userId));
        }


        [HttpGet("/allactiveorders")]
        public IActionResult GetAllActiveOrders()
        {
            return Ok(_service.GetAllActiveOrders());
        }


        [HttpGet("/allcanceledorders")]
        public IActionResult GetAllCanceledOrders()
        {
            return Ok(_service.GetAllCanceledOrders());
        }


        [HttpGet("/allpreviousorders")]
        public IActionResult GetAllPreviousOrders()
        {
            return Ok(_service.GetAllPreviousOrders());
        }


        [HttpGet("/allnottakenorders")]
        public IActionResult GetAllNotTakenOrders()
        {
            return Ok(_service.GetAllNotTakenOrders());
        }
    }
}
