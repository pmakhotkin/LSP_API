using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP_API
{
    public interface iLSP
    {
        string Name {get;}
        DataTable GetStatus(List<Order> orders);
        DataTable GetTableStatus(List<string> waybills);

        List<Waybill> GetListStatus(List<string> waybills);
    }
}
