using AutoMapper;
using back_shopping.Data;
using back_shopping.DTOs;
using back_shopping.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Linq;
using System.Security.Cryptography;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using back_shopping.Interface;

namespace back_shopping
{
    public class Service : IService
    {
        private readonly IMapper _mapper;
        private readonly UserDBContext _dbContext;
        private readonly IConfiguration _configuration;

        public Service(IMapper mapper, UserDBContext dBContext, IConfiguration configuration)
        {
            _mapper = mapper;
            _dbContext = dBContext;
            _dbContext.Database.EnsureCreated();
            _configuration = configuration;
        }


        public UserDTO AddUser(UserDTO _userDTO)
        {
            if (!_dbContext.Users.Any(u => u.Username == _userDTO.Username && u.Email == _userDTO.Email))
            {
                User user = _mapper.Map<User>(_userDTO);
                user.Password = HashPassword(user.Password);
                user.Token = "";

                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                return _mapper.Map<UserDTO>(_userDTO);
            }
            else
            {
                return null;
            }
        }


        private static string HashPassword(string _password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_password));
                return Convert.ToBase64String(hashedBytes);
            }
        }


        public UserDTO GetUser(UserDTO _userDTO)
        {
            string hashedPassword = HashPassword(_userDTO.Password);
            if (_dbContext.Users.FirstOrDefault(u => u.Username == _userDTO.Username && u.Password == hashedPassword) != null)
            {
                User u = _dbContext.Users.FirstOrDefault(u => u.Username == _userDTO.Username);
                string token = GenerateJwtToken(_userDTO.Username);
                u.Token = token;

                _dbContext.Entry(u).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _mapper.Map<UserDTO>(u);
            }
            else
            {
                return null;
            }
        }


        public UserDTO GetUserGoogle(UserDTO _userDTO)
        {
            if (_dbContext.Users.FirstOrDefault(u => u.Email == _userDTO.Email) != null && _userDTO.Token != null)
            {
                User u = _dbContext.Users.FirstOrDefault(u => u.Email == _userDTO.Email);
                string token = GenerateJwtToken(u.Username);
                u.Token = token;

                _dbContext.Entry(u).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _mapper.Map<UserDTO>(u);
            }
            else
            {
                return null;
            }
        }


        public UserDTO GetCurrentUser(int _id)
        {
            var user = _dbContext.Users.Find(_id);
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserDTO>(user);
        }


        private string GenerateJwtToken(string _username)
        {
            var key = Encoding.ASCII.GetBytes("your_secret_key_here");
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("username", _username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public string Updateuser(int _id, UserDTO _userDTO)
        {
            var user = _dbContext.Users.Find(_id);
            user.Username = _userDTO.Username;
            user.Name = _userDTO.Name;
            user.Surname = _userDTO.Surname;
            user.Date = _userDTO.Date;
            user.Email = _userDTO.Email;
            user.Address = _userDTO.Address;
            user.Image = _userDTO.Image;

            try
            {
                _dbContext.SaveChanges();
                return "User updated successfully!";
            }
            catch (Exception ex)
            {
                return $"An exception occurred during UpdateUser: {ex.Message}";
            }
        }


        public string UpdatePassword(int _id, string _newPassword)
        {
            var user = _dbContext.Users.Find(_id);

            string newPassword = HashPassword(_newPassword);
            user.Password = newPassword;
            try
            {
                _dbContext.SaveChanges();
                return "Password changed successfully!";
            }
            catch (Exception ex)
            {
                return $"An exception occurred during UpdatePassword: {ex.Message}";
            }
        }


        public List<UserDTO> GetUnacceptedUsers()
        {
            List<UserDTO> users = new List<UserDTO>();
            var pendingSellers = _dbContext.Users.Where(u => u.Status == "Pending" && u.UserType == "Seller");
            foreach (var pendingSeller in pendingSellers)
            {
                users.Add(_mapper.Map<UserDTO>(pendingSeller));
            }

            return users;
        }

        public string AcceptUser(int _id)
        {
            var user = _dbContext.Users.Find(_id);
            user.Status = "Accepted";

            try
            {
                _dbContext.SaveChanges();
                string message = "Your request has been accepted";
                SendEmail(user.Email, message, message);
                return "User accepted successfully!";
            }
            catch (Exception ex)
            {
                return $"An exception occurred during AcceptUser: {ex.Message}";
            }
        }


        public string DeclineUser(int _id)
        {
            var user = _dbContext.Users.Find(_id);
            user.Status = "Declined";

            try
            {
                _dbContext.SaveChanges();
                string message = "Your request has been denied";
                SendEmail(user.Email, message, message);
                return "User denied successfully!";
            }
            catch (Exception ex)
            {
                return $"An exception occurred during DeclineUser: {ex.Message}";
            }
        }


        public void SendEmail(string _recipientEmail, string _subject, string _message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Your Name", _configuration["EmailSettings:Username"]));
            emailMessage.To.Add(new MailboxAddress("", _recipientEmail));
            emailMessage.Subject = _subject;
            emailMessage.Body = new TextPart("plain") { Text = _message };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), false);
                client.Authenticate(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }


        public string AddProduct(ProductDTO _productDTO)
        {
            if (!_dbContext.Products.Any(p => p.Name == _productDTO.Name))
            {
                Product product = _mapper.Map<Product>(_productDTO);

                _dbContext.Products.Add(product);
                _dbContext.SaveChanges();
                return "Product added successfully!";
            }
            else
            {
                return "Product failed to be added!";
            }
        }

        public List<ProductDTO> GetProducts(int _userId)
        {
            List<ProductDTO> products = new List<ProductDTO>();
            var p = _dbContext.Products.Where(p => p.UserId == _userId);
            foreach (var pro in p)
            {
                products.Add(_mapper.Map<ProductDTO>(pro));
            }
            return products;
        }

        public string DeleteProduct(int _id)
        {
            var product = _dbContext.Products.Find(_id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
            }

            try
            {
                _dbContext.SaveChanges();
                return "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                return $"An exception occurred during DeleteProduct: {ex.Message}";
            }
        }

        public ProductDTO GetProduct(int _id)
        {
            if (_dbContext.Products.FirstOrDefault(p => p.Id == _id) != null)
            {
                Product product = _dbContext.Products.FirstOrDefault(p => p.Id == _id);
                return _mapper.Map<ProductDTO>(product);
            }
            else
            {
                return null;
            }
        }

        public string ChangeProduct(int _id, ProductDTO _productDTO)
        {
            var pro = _dbContext.Products.Find(_id);
            pro.Name = _productDTO.Name;
            pro.Amount = _productDTO.Amount;
            pro.Description = _productDTO.Description;
            pro.Price = _productDTO.Price;

            try
            {
                _dbContext.SaveChanges();
                return "Product changed successfully!";
            }
            catch (Exception ex)
            {
                return $"An exception occurred during ChangeProduct: {ex.Message}";
            }
        }

        public List<UserDTO> GetAcceptedUsers()
        {
            List<UserDTO> users = new List<UserDTO>();
            var acceptedUsers = _dbContext.Users.Where(u => u.Status == "Accepted" && u.UserType == "Seller");

            foreach (var user in acceptedUsers)
            {
                users.Add(_mapper.Map<UserDTO>(user));
            }

            return users;
        }

        public List<UserDTO> GetDeclinedUsers()
        {
            List<UserDTO> users = new List<UserDTO>();
            var declinedUsers = _dbContext.Users.Where(u => u.Status == "Declined" && u.UserType == "Seller");
            foreach (var user in declinedUsers)
            {
                users.Add(_mapper.Map<UserDTO>(user));
            }

            return users;
        }

        public List<ProductDTO> GetAllProducts()
        {
            List<ProductDTO> products = new List<ProductDTO>();
            var allProducts = _dbContext.Products;
            foreach (var product in allProducts)
            {
                products.Add(_mapper.Map<ProductDTO>(product));
            }

            return products;
        }


        public string Ordering(OrderDTO _orderDTO)
        {
            Order order = _mapper.Map<Order>(_orderDTO);
            foreach (var orderProduct in order.OrderProducts)
            {
                var product = _dbContext.Products.FirstOrDefault(p => p.Id == orderProduct.ProductId);

                if (product != null)
                {
                    orderProduct.ProductName = product.Name;
                    orderProduct.ProductPrice = product.Price;
                }
            }

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            foreach (var orderProduct in _orderDTO.OrderProducts)
            {
                var product = _dbContext.Products.FirstOrDefault(p => p.Id == orderProduct.ProductId);
                if (product != null)
                {
                    product.Amount -= orderProduct.Quantity;
                }
            }

            _dbContext.SaveChanges();

            return "Done";
        }

        public List<OrderDTO> GetAllOrders(int _userId)
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;

            var o = _dbContext.Orders.Include(o => o.OrderProducts)
                                    .Where(o => o.OrderExpire <= currentTime && !o.IsCanceled && o.UserId == _userId && o.IsPaid && o.IsSent);

            foreach (var order in o)
            {
                var orderDTO = _mapper.Map<OrderDTO>(order);
                orders.Add(orderDTO);
            }

            return orders;
        }

        public List<OrderDTO> GetActiveOrders(int _userId)
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;
            var allActiveOrders = _dbContext.Orders.Include(o => o.OrderProducts)
                                     .Where(o => o.OrderExpire >= currentTime && !o.IsCanceled && o.UserId == _userId && o.IsPaid && o.IsSent);

            foreach (var activeOrder in allActiveOrders)
            {
                var orderDTO = _mapper.Map<OrderDTO>(activeOrder);
                orders.Add(orderDTO);
            }
            return orders;
        }

        public List<OrderDTO> GetWaitingOrders(int _userId)
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;
            var allWaitingOrders = _dbContext.Orders.Include(o => o.OrderProducts)
                                     .Where(o => !o.IsCanceled && o.UserId == _userId && o.IsPaid && !o.IsSent);

            foreach (var waitingOrder in allWaitingOrders)
            {
                var orderDTO = _mapper.Map<OrderDTO>(waitingOrder);
                orders.Add(orderDTO);
            }
            return orders;
        }


        public List<OrderDTO> GetPayPalOrders(int _userId)
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;
            var allPayPalOrders = _dbContext.Orders.Include(o => o.OrderProducts)
                                     .Where(o => o.PaymentMethod == "paypal" && !o.IsCanceled && o.UserId == _userId && !o.IsPaid && !o.IsSent);

            foreach (var paypalOrder in allPayPalOrders)
            {
                var orderDTO = _mapper.Map<OrderDTO>(paypalOrder);
                orders.Add(orderDTO);
            }
            return orders;
        }


        public string CancelOrder(int _orderId)
        {
            var order = _dbContext.Orders.Find(_orderId);
            order.IsCanceled = true;
            foreach (var orderProduct in order.OrderProducts)
            {
                var product = _dbContext.Products.FirstOrDefault(p => p.Id == orderProduct.ProductId);
                if (product != null)
                {
                    product.Amount += orderProduct.Quantity;
                }
            }

            _dbContext.SaveChanges();
            try
            {
                _dbContext.SaveChanges();
                return "Order canceled successfully!";
            }
            catch (Exception ex)
            {
                return $"An exception occurred during CancelOrder: {ex.Message}";
            }
        }


        public List<OrderDTO> SellerOrders(int _userId)
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;

            var ordersWithSellerProduct = _dbContext.Orders
                .Where(order => order.OrderProducts.Any(op => _dbContext.Products.Any(p => p.Id == op.ProductId && p.UserId == _userId)))
                .Where(o => o.OrderExpire <= currentTime && !o.IsCanceled && o.IsSent && o.IsPaid)
                .Include(o => o.OrderProducts)
                .ToList();

            foreach (var order in ordersWithSellerProduct)
            {
                var orderDTO = _mapper.Map<OrderDTO>(order);
                orders.Add(orderDTO);
            }

            return orders;
        }


        public List<OrderDTO> SellerActiveOrders(int _userId)
        {

            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;

            var ordersWithSellerProduct = _dbContext.Orders
                .Where(order => order.OrderProducts.Any(op => _dbContext.Products.Any(p => p.Id == op.ProductId && p.UserId == _userId)))
                 .Where(o => o.OrderExpire >= currentTime && !o.IsCanceled && o.IsSent && o.IsPaid)
                .Include(o => o.OrderProducts)
                .ToList();

            foreach (var order in ordersWithSellerProduct)
            {
                var orderDTO = _mapper.Map<OrderDTO>(order);
                orders.Add(orderDTO);
            }

            return orders;
        }


        public List<OrderDTO> GetAllActiveOrders()
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;
            var allActiveOrders = _dbContext.Orders.Include(o => o.OrderProducts)
                                     .Where(o => o.OrderExpire >= currentTime && !o.IsCanceled && o.IsSent && o.IsPaid);

            foreach (var order in allActiveOrders)
            {
                var orderDTO = _mapper.Map<OrderDTO>(order);
                orders.Add(orderDTO);
            }
            return orders;
        }


        public List<OrderDTO> GetAllCanceledOrders()
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;
            var allCanceledOrders = _dbContext.Orders.Include(o => o.OrderProducts)
                                     .Where(o => o.IsCanceled);

            foreach (var order in allCanceledOrders)
            {
                var orderDTO = _mapper.Map<OrderDTO>(order);
                orders.Add(orderDTO);
            }
            return orders;
        }


        public List<OrderDTO> GetAllPreviousOrders()
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;
            var allPreviousOrders = _dbContext.Orders.Include(o => o.OrderProducts)
                                     .Where(o => o.OrderExpire <= currentTime && !o.IsCanceled && o.IsSent && o.IsPaid);

            foreach (var order in allPreviousOrders)
            {
                var orderDTO = _mapper.Map<OrderDTO>(order);
                orders.Add(orderDTO);
            }
            return orders;
        }


        public List<OrderDTO> GetAllNotTakenOrders()
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;
            var allNotTakenOrders = _dbContext.Orders.Include(o => o.OrderProducts)
                                     .Where(o => !o.IsCanceled && !o.IsSent && o.IsPaid);

            foreach (var order in allNotTakenOrders)
            {
                var orderDTO = _mapper.Map<OrderDTO>(order);
                orders.Add(orderDTO);
            }
            return orders;
        }


        public string PayOrder(int _orderId)
        {
            var order = _dbContext.Orders.Find(_orderId);
            order.IsPaid = true;
            try
            {
                _dbContext.SaveChanges();
                return "Order paid successfully!";
            }
            catch (Exception ex)
            {
                return $"An exception occurred during PayOrder: {ex.Message}";
            }
        }


        public List<OrderDTO> GetWaitingOrdersSeller(int _userId)
        {

            List<OrderDTO> orders = new List<OrderDTO>();
            var currentTime = DateTime.Now;

            var ordersWithSellerProduct = _dbContext.Orders
                .Where(order => order.OrderProducts.Any(op => _dbContext.Products.Any(p => p.Id == op.ProductId && p.UserId == _userId)))
                .Where(o => o.OrderExpire <= currentTime && !o.IsCanceled && o.IsPaid && !o.IsSent)
                .Include(o => o.OrderProducts)
                .ToList();

            foreach (var order in ordersWithSellerProduct)
            {
                var orderDTO = _mapper.Map<OrderDTO>(order);
                orders.Add(orderDTO);
            }

            return orders;
        }


        public string SendOrder(int _orderId)
        {
            var order = _dbContext.Orders.Find(_orderId);
            order.IsSent = true;
            Random random = new Random();
            int randomHours = random.Next(2, 24);
            order.OrderExpire = DateTime.Now.AddHours(randomHours);
            try
            {
                _dbContext.SaveChanges();
                return "Order sent successfully!";
            }
            catch (Exception ex)
            {
                return $"An exception occurred during SendOrder: {ex.Message}";
            }
        }
    }
}
