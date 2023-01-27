using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP_API
{
    public class Vestovoy
    {
        public string Url_server { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public Vestovoy(string url_server, string login, string password)
        {
            // test параметры , логин: login пароль: pass.
            //[] < auth extra = "10" login = "Samsung SDS" pass = "1369023" ></ auth >

            Url_server = url_server;
            Login = login;
            Password = password;
        }
        public Vestovoy()
        {
            /// <summary>
            /// По умолчанию создается объект класса с параметрами подключения для тестового сервера
            /// </summary>
            /// 
            Url_server = "https://home.courierexe.ru/8";
            Login = "login";
            Password = "password";
        }
    }
}
