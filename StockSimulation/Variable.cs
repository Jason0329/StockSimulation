using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL_InsertAndAdd;
using StockSimulation.Stock;

namespace StockSimulation
{
    class Variable
    {
        protected List<String[]> ClosePrice = new List<string[]>();
        protected List<String[]> RewardRatio = new List<string[]>();
        protected List<String[]> Volumn = new List<string[]>();
       

        protected string startDate = "2013-01-01";
        protected string endDate = "2014-01-01";
        protected string company = "3576";

        protected GetDataFromDatabase GetDatabase = new GetDataFromDatabase();
        protected StockPossess sp = new StockPossess();

    }
}
