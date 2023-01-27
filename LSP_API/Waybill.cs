using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP_API
{
    public class Waybill
    {
        public string ShipmentNumber { get; set; }
            public string Created { get; set; }// Документ создан, но еще не обработан
            public string LSP_Msk_WH_arrive {get ; set; } //Груз поступил на место хранения
            public string Msk_WH_departure { get; set; }//Груз выбыл с место хранения
            public string Dest_LSP_WH_arrive { get; set; } //Груз поступил на склад 
            public string Dest_LSP_WH_departure { get; set; } //Груз передан на доставку 
            public string ASC_Arrival { get; set; } // Груз доставлен
            public string Error{ get; set; }
    }
}
