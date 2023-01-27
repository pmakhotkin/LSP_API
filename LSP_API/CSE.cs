using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LSP_API
{
    public class CSE: iLSP
    {
        public string Url_server { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        string iLSP.Name { get => "CSE"; }

        public CSE(string url_server, string login, string password)
        {
            Url_server = url_server;
            Login = login;
            Password = password;
        }

        public CSE()
        {
            /// <summary>
            /// По умолчанию создается объект класса с параметрами подключения для тестового сервера КСЕ
            /// </summary>
            /// 
            Url_server = "http://lk-test.cse.ru/1c/ws/web1c.1cws"; //"http://web.cse.ru/1c/ws/Web1C.1cws";
            Login = "test"; // "Самсунг СДС Глобал ЭсСиЭл"
            Password = "2016"; // "DNILfwk4P7O5rLBL";
        }

        public DataTable GetStatus(List<Order> orders)
        {
            DataTable statuses = new DataTable();
            String request = CreateRequestStatus(orders);
            String response = GetResponse(request).Result;
            if (response == "Сервер КСЕ не отвечает")
            {
                statuses.Columns.Add("Error");
                DataRow row = statuses.NewRow();
                row["Error"] = response;
                statuses.Rows.Add(row);
                statuses.AcceptChanges();
            }
            else
            {
                statuses = GetTableStatuses(response);
            }
            statuses = GetTableStatuses(response);
            return statuses;
        }
        public DataTable GetTableStatus(List<string> waybills)
        {
            String request = CreateRequestStatus(waybills);
            var response = GetResponse(request);
            DataTable result = GetTableStatuses(response.Result);
            return result;
        }

        public List<Waybill> GetListStatus(List<string> waybills)
        {
            int maxCountWaybill = 250;
            List<Waybill> listTotal = new List<Waybill>();
            List<string> listTemp = new List<string>();
            
            foreach (var waybill in waybills)
            {
                listTemp.Add(waybill);
                
                if (listTemp.Count == maxCountWaybill)
                {
                    String request = CreateRequestStatus(listTemp);
                    var response = GetResponse(request);
                    listTotal = (listTotal.Concat(GetListStatuses(response.Result))).ToList();
                    listTemp.Clear();
                }
            }

            if (listTemp.Count > 0)
            {
                String request1 = CreateRequestStatus(listTemp);
                var response1 = GetResponse(request1);
                listTotal = (listTotal.Concat(GetListStatuses(response1.Result))).ToList();
                listTemp.Clear();
            }
            
            return listTotal;
        }

        private async Task<string> GetResponse(string request)
        {
            String result;
            /// <summary>
            /// отправка запроса на сервер ксе и получение ответа.
            /// Если ответ не получен возвращает строку "Сервер КСЕ не отвечает."
            /// </summary>
            /// <param name="request">тело запроса для отправки</param>
            /// <returns>string возвращает ответ от сервера ксе</returns>
            /// 

            HttpResponseMessage response = new HttpResponseMessage();
            HttpContent content = new StringContent(request);
            using (var httpClient = new HttpClient())
            {
                try
                {
                    response = httpClient.PostAsync(Url_server, content).Result; 
                    result = await response.Content.ReadAsStringAsync();  //ReadAsStreamAsync(); //.ReadAsStringAsync();   
                }
                catch (Exception e)
                {
                    result = "Сервер КСЕ не отвечает.";
                }

                return result;
            }
        }
        private string CreateRequestNewOrders(List<Order> orders) 
        {
            /// <summary>
            /// возвращает готовый string request для создания заказа у перевозчика.
            /// </summary>
            /// <param name="orders">список List заказов для создания у ксе</param>
            /// 

            string request = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:m=""http://www.cargo3.ru"">
          <soap:Header/>
          <soap:Body>
          <m:SaveDocuments>
          <m:login>{Login}</m:login>
          <m:password>{Password}</m:password>
          <m:data>
          <m:Key>Orders</m:Key>
          <m:List>
          <m:Key>Order</m:Key>
          <m:Properties>
          <m:Key>ClientNumber</m:Key>
          <m:Value>D4174416091</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Properties>
          <m:Fields>
          <m:Key>Department</m:Key>
          <m:Value>D</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>TakeDate</m:Key>
          <m:Value>2021-08-24T00:00:00</m:Value>
          <m:ValueType>dateTime</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>TakeTime</m:Key>
          <m:Value>10:00 18:00</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>Comment</m:Key>
          <m:Value>Упаковка при курьере| !Подчеркнуть:Отпр - ль не предоставил груз для проверки(запечатан)| Сверить серийные номера\Серийные не совпадают</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>Sender</m:Key>
          <m:Value>Татьяна Юрьевна Округина</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>SenderOfficial</m:Key>
          <m:Value>Частное лицо</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>SenderGeography</m:Key>
          <m:Value>postcode-666904</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>SenderAddress</m:Key>
          <m:Value>Иркутская обл., Бодайбо, Стояновича 93</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>SenderPhone</m:Key>
          <m:Value>9526259470   9526259470   9526259470</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>SenderInfo</m:Key>
          <m:Value>Позвонить за час/Сверить серийные номера</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>Recipient</m:Key>
          <m:Value>АСЦ Лэмпорт</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>RecipientOfficial</m:Key>
          <m:Value>Авторизованный сервис SAMSUNG АСЦ Лэмпорт</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>RecipientGeography</m:Key>
          <m:Value>postcode-117418</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>RecipientAddress</m:Key>
          <m:Value>Москва, Москва, ул.Новочеремушкинская, д. 57</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>RecipientPhone</m:Key>
          <m:Value>+7(495)7185555</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>RecipientInfo</m:Key>
          <m:Value>Позвонить за час/Доставка C 9:00 по 18:00</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>Urgency</m:Key>
          <m:Value>18c4f207-458b-11dc-9497-0015170f8c09</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>Payer</m:Key>
          <m:Value>0</m:Value>
          <m:ValueType>float</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>PaymentMethod</m:Key>
          <m:Value>1</m:Value>
          <m:ValueType>float</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>ShippingMethod</m:Key>
          <m:Value>e45b6d73-fd62-44da-82a6-44eb4d1d9490</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>TypeOfCargo</m:Key>
          <m:Value>4aab1fc6-fc2b-473a-8728-58bcd4ff79ba</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>Weight</m:Key>
          <m:Value>0.5</m:Value>
          <m:ValueType>float</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>CargoDescription</m:Key>
          <m:Value>BY271010674773 (V55243CF700196F); возможна горизонтальная транспортировка</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>CargoPackageQty</m:Key>
          <m:Value>1</m:Value>
          <m:ValueType>float</m:ValueType>
          </m:Fields>
          <m:Fields>
          <m:Key>ReplySMSPhone</m:Key>
          <m:Value>+79526259470</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:Fields>
          </m:List>
          </m:data>
          <m:parameters>
          <m:Key>Parameters</m:Key>
          <m:List>
          <m:Key>DocumentType</m:Key>
          <m:Value>order</m:Value>
          <m:ValueType>string</m:ValueType>
          </m:List>
          </m:parameters>
          </m:SaveDocuments></soap:Body>
          </soap:Envelope>";

            return "";
        }
        private string CreateRequestStatus(List<Order> orders)
        {
            /// <summary>
            /// формирует и возвращает готовый string request для проверки статуса у ксе.
            /// </summary>
            /// <param name="orders">список List<Order> заказов</param>
            /// <returns>string request</returns>
            /// 
            string listWaybills = "";
            foreach (var order in orders)
            {
                listWaybills +=$@"<ns1:List>
                <ns1:Key>{order.Waybill}</ns1:Key>
                    </ns1:List>";
            }

            string request = $@"<SOAP-ENV:Envelope 
                xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" 
                xmlns:ns1=""http://www.cargo3.ru"">
               <SOAP-ENV:Body>
               <ns1:Tracking>
               <ns1:login>{Login}</ns1:login>
               <ns1:password>{Password}</ns1:password>
               <ns1:documents>
               <ns1:Key>Documents</ns1:Key>
               <ns1:Properties>
               <ns1:Key>DocumentType</ns1:Key>
               <ns1:Value>Waybill</ns1:Value>
               <ns1:ValueType>string</ns1:ValueType>
               </ns1:Properties>
               <ns1:Properties>
               <ns1:Key>OnlySelectedType</ns1:Key>
               <ns1:Value>true</ns1:Value>
               <ns1:ValueType>boolean</ns1:ValueType>
               </ns1:Properties>
                <ns1:Properties>
                <ns1:Key>NumberType</ns1:Key>
                <ns1:Value>ClientNumber</ns1:Value>
                <ns1:ValueType>string</ns1:ValueType>
                </ns1:Properties>
               {listWaybills}
               </ns1:documents>
               <ns1:parameters>
               <ns1:Key>Parameters</ns1:Key>
               </ns1:parameters>
               </ns1:Tracking>
               </SOAP-ENV:Body>
               </SOAP-ENV:Envelope>";
            
            return request;
        }
        private string CreateRequestStatus(List<string> waybills)
        {
            /// <summary>
            /// формирует и возвращает готовый string request для проверки статуса у ксе.
            /// </summary>
            /// <param name="waybills">список List<string>номеров накладных ксе</param>
            /// <returns>string request</returns>
            /// 

            string listWaybills = "";
            foreach (var waybill in waybills)
            {
                listWaybills += $@"<ns1:List>
                <ns1:Key>{waybill}</ns1:Key>
                    </ns1:List>";
            }
            string request = $@"<SOAP-ENV:Envelope 
                xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" 
                xmlns:ns1=""http://www.cargo3.ru"">
               <SOAP-ENV:Body>
               <ns1:Tracking>
               <ns1:login>{Login}</ns1:login>
               <ns1:password>{Password}</ns1:password>
               <ns1:documents>
               <ns1:Key>Documents</ns1:Key>
               <ns1:Properties>
               <ns1:Key>DocumentType</ns1:Key>
               <ns1:Value>Waybill</ns1:Value>
               <ns1:ValueType>string</ns1:ValueType>
               </ns1:Properties>
               <ns1:Properties>
               <ns1:Key>OnlySelectedType</ns1:Key>
               <ns1:Value>true</ns1:Value>
               <ns1:ValueType>boolean</ns1:ValueType>
               </ns1:Properties>
                <ns1:Properties>
                <ns1:Key>NumberType</ns1:Key>
                <ns1:Value>ClientNumber</ns1:Value>
                <ns1:ValueType>string</ns1:ValueType>
                </ns1:Properties>
               {listWaybills}
               </ns1:documents>
               <ns1:parameters>
               <ns1:Key>Parameters</ns1:Key>
               </ns1:parameters>
               </ns1:Tracking>
               </SOAP-ENV:Body>
               </SOAP-ENV:Envelope>";

            return request;
        }
        private DataTable GetTableStatuses(String sResponse)
        {
            /// <summary>
            /// парсинг ответа от ксе в объект DataTable
            /// </summary>
            /// <param name="sResponse">ответ для парсинга</param>
            /// <returns>DataTable</returns>
            ///

            Regex pattern_waybill_number = new Regex(@"^S\d*|\d{7}");

            #region Add_Columns_Status Table
            DataTable dt = new DataTable();
            dt.Columns.Add("Shipment Number");
            dt.Columns.Add("Created"); // Документ создан, но еще не обработан
            dt.Columns.Add("LSP Msk W/H arrive"); //Груз поступил на место хранения
            dt.Columns.Add("Msk W/H departure");//Груз выбыл с место хранения
            dt.Columns.Add("Dest LSP W/H arrive"); //Груз поступил на склад 
            dt.Columns.Add("Dest LSP W/H departure"); //Груз передан на доставку 
            dt.Columns.Add("ASC Arrival"); // Груз доставлен
            dt.Columns.Add("Error"); // внутренняя ошибка ксе
            #endregion
            if (sResponse == "Сервер КСЕ не отвечает.")
            {
                DataRow row = dt.NewRow();
                row["Error"] = "Сервер КСЕ не отвечает";
                dt.Rows.Add(row);
                dt.AcceptChanges();
                return dt;
            }
            XDocument TotalList = XDocument.Parse(sResponse);
            XNamespace xNamespace = "http://www.cargo3.ru";
            var Waybills = from result in TotalList.Descendants(xNamespace + "List")
                           where pattern_waybill_number.IsMatch(result.Element(xNamespace + "Key").ToString())
                           select result;

            foreach (var waybill in Waybills)
            {
                string waybillNumber = waybill.Element(xNamespace + "Key").Value;

                var StatusList = from result in waybill.Descendants(xNamespace + "List")
                                 where !pattern_waybill_number.IsMatch(result.Element(xNamespace + "Key").ToString())
                                 select result;

                DataRow row = dt.NewRow();
                foreach (var status in StatusList)
                {
                    string Geography = "";
                    string key = status.Element(xNamespace + "Key").Value;
                    string value = status.Element(xNamespace + "Value").Value;

                    // проверяем ответ на ошибки, если есть добавляем строку с номером накладной и описанием ошибки
                    // если нет ошибки парсим статусы
                    if (key != "Description")
                    {
                        var _geography = from prop in status.Descendants(xNamespace + "Properties")
                                         where prop.Element(xNamespace + "Key").Value == "Geography"
                                         select prop.Element(xNamespace + "Value").Value.ToString();
                        string[] geography_temp = _geography.ToArray();

                        Geography = geography_temp[0].ToString();

                        var StatusDate = from prop in status.Descendants(xNamespace + "Properties")
                                         where prop.Element(xNamespace + "Key").Value == "DateTime"
                                         select prop.Element(xNamespace + "Value").Value.ToString();

                        string[] statusdate_temp = StatusDate.ToArray();
                        DateTime DateTime_Status = Convert.ToDateTime((statusdate_temp[0].ToString()).Replace("T", " "));

                        row["Shipment Number"] = waybillNumber;

                        switch (key)
                        {
                            case "Создан":
                                row["Created"] = DateTime_Status;
                                break;
                            case "Груз поступил на место хранения":
                                if (value.Contains("Томилино"))
                                {
                                    row["LSP Msk W/H arrive"] = DateTime_Status;
                                }
                                else
                                {
                                    row["Dest LSP W/H arrive"] = DateTime_Status;
                                }
                                break;
                            case "Груз выбыл с место хранения":
                                if (Geography.Contains("Москва"))
                                {
                                    row["Msk W/H departure"] = DateTime_Status;
                                }
                                else
                                {
                                    row["Dest LSP W/H departure"] = DateTime_Status;
                                }
                                break;
                            case "Груз передан на доставку":
                                row["Dest LSP W/H departure"] = DateTime_Status;
                                break;
                            case "Груз доставлен":
                                row["ASC Arrival"] = DateTime_Status;
                                break;

                        }
                    }
                    else
                    {
                        row["Shipment Number"] = waybillNumber;
                        row["Error"] = GetErrorDescription(value);
                    }
                }
                dt.Rows.Add(row);
                dt.AcceptChanges();

            }
            return dt;
        }

        private List<Waybill> GetListStatuses(String sResponse)
        {
            List<Waybill> waybillList = new List<Waybill>();
            /// <summary>
            /// парсинг ответа от ксе в List
            /// </summary>
            /// <param name="sResponse">ответ для парсинга</param>
            /// <returns>List<WayBlill></returns>
            ///

            Regex pattern_waybill_number = new Regex(@"^S\d*|\d{7}");
            
            if (sResponse == "Сервер КСЕ не отвечает.")
            {
                Waybill waybill1 = new Waybill();
                waybill1.Error = "Сервер КСЕ не отвечает";
                waybillList.Add (waybill1);
                return waybillList;
            }
            XDocument TotalList = XDocument.Parse(sResponse);
            XNamespace xNamespace = "http://www.cargo3.ru";
            var Waybills = from result in TotalList.Descendants(xNamespace + "List")
                           where pattern_waybill_number.IsMatch(result.Element(xNamespace + "Key").ToString())
                           select result;

            foreach (var waybill in Waybills)
            {
                string waybillNumber = waybill.Element(xNamespace + "Key").Value;

                var StatusList = from result in waybill.Descendants(xNamespace + "List")
                                 where !pattern_waybill_number.IsMatch(result.Element(xNamespace + "Key").ToString())
                                 select result;

                Waybill waybill1 = new Waybill();
                foreach (var status in StatusList)
                {
                    string Geography = "";
                    string key = status.Element(xNamespace + "Key").Value;
                    string value = status.Element(xNamespace + "Value").Value;

                    // проверяем ответ на ошибки, если есть добаляем элемент списка с номером накладной и описанием ошибки
                    // если нет ошибки парсим статусы
                    if (key != "Description")
                    {
                        var _geography = from prop in status.Descendants(xNamespace + "Properties")
                                         where prop.Element(xNamespace + "Key").Value == "Geography"
                                         select prop.Element(xNamespace + "Value").Value.ToString();
                        string[] geography_temp = _geography.ToArray();

                        Geography = geography_temp[0].ToString();

                        var StatusDate = from prop in status.Descendants(xNamespace + "Properties")
                                         where prop.Element(xNamespace + "Key").Value == "DateTime"
                                         select prop.Element(xNamespace + "Value").Value.ToString();

                        string[] statusdate_temp = StatusDate.ToArray();
                        DateTime DateTime_Status = Convert.ToDateTime((statusdate_temp[0].ToString()).Replace("T", " "));

                        waybill1.ShipmentNumber = waybillNumber;

                        switch (key)
                        {
                            case "Создан":
                                waybill1.Created = DateTime_Status.ToString();
                                break;
                            case "Груз поступил на место хранения":
                                if (value.Contains("Томилино"))
                                {
                                    waybill1.LSP_Msk_WH_arrive = DateTime_Status.ToString();
                                }
                                else
                                {
                                    waybill1.Dest_LSP_WH_arrive = DateTime_Status.ToString();
                                }
                                break;
                            case "Груз выбыл с место хранения":
                                if (Geography.Contains("Москва"))
                                {
                                    waybill1.Msk_WH_departure = DateTime_Status.ToString();
                                }
                                else
                                {
                                    waybill1.Dest_LSP_WH_departure = DateTime_Status.ToString();
                                }
                                break;
                            case "Груз передан на доставку":
                                waybill1.Dest_LSP_WH_departure = DateTime_Status.ToString();
                                break;
                            case "Груз доставлен":
                                waybill1.ASC_Arrival = DateTime_Status.ToString();
                                break;
                        }
                    }
                    else
                    {
                        waybill1.ShipmentNumber = waybillNumber;
                        waybill1.Error = GetErrorDescription(value);
                    }
                }
                waybillList.Add(waybill1);

            }
            return waybillList;
        }

        private string GetErrorDescription(string error_number)
        {
            string error_description = $"код ошибки {error_number} не найден в CSE_Error.txt";

            var file = File.ReadAllLines("CSE_Error.txt");
            Regex regex = new Regex($"{error_number}.*");

            foreach (var line in file)
            {
                if (regex.IsMatch(line))
                {
                    error_description = line;
                }
            }
            return error_description;
        }

    }
}
