using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSimulation.Stock
{
    class Standard_Analysis
    {
        #region debt
        public string DebtRatio(string startDate="2013-03-01" , string endDate="2013-09-01",int ratio=50)
        {
            string command;
            command = "SELECT company FROM [StockDatabase].[dbo].[standard_analysis] where  Column144<" + ratio + " and Column1 between '"
                +startDate +"' and '" + endDate+"'";
            return command;
        }

        public string DebtWithLongTerm(string startDate = "2013-03-01", string endDate = "2013-09-01", int ratio = 50)
        {
            string command;
            command = @"SELECT company FROM [StockDatabase].[dbo].[standard_analysis] 

                        where (Column19-Column16)/ISNULL(NULLIF(Column15,0),100) < "+ratio +@" and 
                        
                        Column1 between '"+startDate+"' and '"+endDate+"'";
            
            return command;
        }
        #endregion

    }
}
