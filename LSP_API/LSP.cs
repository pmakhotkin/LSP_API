using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP_API
{
   public abstract class LSP
    {
        public abstract string url_server { get; set; }
        public abstract string login { get; set; }
        public abstract string password { get; set; }
        public abstract string SendRequest(string reguest);
        // отправка запроса
        // на вход запрос
        // возвращает string responce
        
        public abstract string CreateRequestNewOrders(List<Order> orders);
        // возвращает готовый запрос для создания заказа у перевозчика.
        
    }
}
