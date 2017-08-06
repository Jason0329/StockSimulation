using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace StockSimulation
{
    class simulation:LongTermSelectStock
    {
        List<float> buyStock = new List<float>();

        string simulationDate1 = @"'2010-05-10 00:00:00'";
        string simulationDate2 = @"'2011-05-11 00:00:00'";

        SqlConnection thisConnection = new SqlConnection("server=USER-PC\\SQLEXPRESS;database=StockDatabase;Integrated Security=SSPI");
        SqlCommand nonqueryCommand;
        //SqlDataReader myData1;

        public simulation()
        {
            nonqueryCommand = thisConnection.CreateCommand();
            thisConnection.Open();
        }

        string TextOperate(string operate)//把日期格是重新改寫 才能存
        {
            string[] temp = operate.Split(' ');
            operate = "";
            for (int i = 0; i < temp.Length - 1; i++)
            {
                operate += temp[i] + "_";
            }

            operate += temp[temp.Length - 1];


            temp = operate.Split('/');
            operate = "";
            for (int i = 0; i < temp.Length - 1; i++)
            {
                operate += temp[i] + "_";
            }

            operate += temp[temp.Length - 1];

            temp = operate.Split('-');
            operate = "";
            for (int i = 0; i < temp.Length - 1; i++)
            {
                operate += temp[i] + "_";
            }

            operate += temp[temp.Length - 1];

            return operate;
        }

        public void simulation01(string DatabaseName = "StockDatabase")
        {
           
            int succeed = 0;  
            string command = "";

            float buyPrice, sellPrice;
            

            ROE_Now();
            FreeCashFlowNow();
            averageFreeCashFlow();
            averageROE();
            GetDataFromDatabase();

            foreach (string s in searchData)
            {
                BuyAndSell buy = new BuyAndSell();
                command = @"select a.Date" + TextOperate(s) + @" from StockPrice as a , company_yield as b
                        where b.Date" + TextOperate(s) + " > " + SimulationParameter.buyROE + @" and a.Date between "
                            + simulationDate1 + " and " + simulationDate2
                            + @" and a.Date = b.Date";
               

                succeed = DatabaseQueryCommand(command);

                if (succeed != 0)
                    continue;

                buyPrice = getLowPrice(buyStock);
                buy.BuyStock(buyPrice);

                command = @"select a.Date" + TextOperate(s) + @" from StockPrice as a , company_yield as b
                        where b.Date" + TextOperate(s) + " < " + SimulationParameter.SellROE + @" and a.Date between "
                           + simulationDate1 + " and " + simulationDate2
                           + @" and a.Date = b.Date";

                succeed=DatabaseQueryCommand(command);

                if (succeed != 0)
                {
                    SimulationResult.loss++;
                    continue;
                }

                sellPrice = getHighPrice(buyStock);
                buy.SellStock(sellPrice);

                //myData1.Close();

            }

            double winP = SimulationResult.WinPossibility();
            Console.WriteLine(winP);
            
        }

        void testMonthRevenue(string s)
        {
            string command = "select " + s + " from month_revenue";
        }

        int DatabaseQueryCommand(string command , string using_Database = @"use " +"StockDatabase" )
        {
            buyStock.Clear();
            SqlDataReader myData1;
            int countField = 1; //取得欄位的數目
            float[] result = new float[countField];
            #region 使用資料庫
            nonqueryCommand.CommandText = using_Database;
            Console.WriteLine(nonqueryCommand.CommandText);
            try
            {
                nonqueryCommand.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            #endregion

            #region 取得資料庫資料的指標

            nonqueryCommand = new SqlCommand(command, thisConnection);
            Console.WriteLine(nonqueryCommand.CommandText);
            try
            {
                myData1 = nonqueryCommand.ExecuteReader();
            }
            catch (Exception eee)
            {
                Console.WriteLine(eee.Message);
                return -2;
            }

            if (!myData1.HasRows)
            {
                // 如果沒有資料,顯示沒有資料的訊息
                Console.WriteLine("No data4.");
                myData1.Close();
                return -1;
            }
            #endregion

            #region 取得資料庫裡的data
            while (myData1.Read())
            {

                if (myData1.FieldCount < countField)
                {
                    Console.WriteLine("欄位數目有誤");
                    return -3;
                }

                for (int i = 0; i < countField; i++)
                {
                    result[i] = float.Parse(myData1[i].ToString());
                    Console.WriteLine("Text={0}", result[i].ToString());

                }

                buyStock.Add(result[0]);//存在buyStock
                
            }
            #endregion

            myData1.Close();
            return 0;
        }

        float getHighPrice(List<float> highprice)
        {
            float temp=0;

            foreach (float tempp in highprice)
            {
                if (tempp > temp)
                    temp = tempp;
            }

            return temp;
        }

        float getLowPrice(List<float> highprice)
        {
            float temp = 2000;

            foreach (float tempp in highprice)
            {
                if (tempp < temp)
                    temp = tempp;
            }

            return temp;
        }
    }
}
