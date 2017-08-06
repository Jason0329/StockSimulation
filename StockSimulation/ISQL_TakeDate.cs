using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace StockSimulation
{
    public interface ISQL_TakeData
    {

        string command{get;set;}
        int SetCommand(string command);
        int DatabaseQueryCommand();
    }
}
