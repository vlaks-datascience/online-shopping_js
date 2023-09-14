using back_shopping.DTOs;
using System.Collections.Generic;

namespace back_shopping.Interface
{
    public interface IService
    {
        UserDTO AddUser(UserDTO newUser);
        UserDTO GetUser(UserDTO newUser);
        string AcceptUser(int id);
        string DeclineUser(int id);
        UserDTO GetCurrentUser(int id);
        UserDTO GetUserGoogle(UserDTO newUser);
        string Updateuser(int id, UserDTO us);
        string UpdatePassword(int id, string newPassword);
        List<UserDTO> GetUnacceptedUsers();
        List<UserDTO> GetAcceptedUsers();
        List<UserDTO> GetDeclinedUsers();
        string AddProduct(ProductDTO newProduct);
        List<ProductDTO> GetProducts(int UserId);
        string DeleteProduct(int id);
        ProductDTO GetProduct(int id);
        string ChangeProduct(int id, ProductDTO product);
        List<ProductDTO> GetAllProducts();
        string Ordering(OrderDTO orderDTO);
        List<OrderDTO> GetAllOrders(int userId);
        List<OrderDTO> GetActiveOrders(int userId);
        List<OrderDTO> GetWaitingOrders(int userId);
        List<OrderDTO> GetPayPalOrders(int userId);
        string CancelOrder(int orderId);
        List<OrderDTO> SellerOrders(int userId);
        List<OrderDTO> SellerActiveOrders(int userId);
        List<OrderDTO> GetAllActiveOrders();
        List<OrderDTO> GetAllCanceledOrders();
        List<OrderDTO> GetAllPreviousOrders();
        List<OrderDTO> GetAllNotTakenOrders();
        string PayOrder(int orderId);
        List<OrderDTO> GetWaitingOrdersSeller(int userId);
        string SendOrder(int orderId);
    }
}
