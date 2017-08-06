using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace StockSimulation
{
    class LongTermSelectStock
    {
        public List<string> searchData = new List<string>();

        SqlConnection thisConnection = new SqlConnection("server=USER-PC\\SQLEXPRESS;database=StockDatabase;Integrated Security=SSPI");
        SqlCommand nonqueryCommand;
        string command;


        public LongTermSelectStock()
        {
            nonqueryCommand = thisConnection.CreateCommand();
            thisConnection.Open();
        }

        public void ROE_Now()
        {
            if (command != null)
            {
                command += " INTERSECT ";
            }
            command += "SELECT " + Parameter.SelectColumn + " from " + Parameter.TableName + Parameter.year + " where ROE>" + Parameter.ROE;

        }
         
        public void FreeCashFlowNow()
        {
            if (command != null)
            {
                command += " INTERSECT ";
            }
            command += "SELECT " + Parameter.SelectColumn + " from " + Parameter.TableName
                + Parameter.year + " where CashFlowForOperating+CashFlowForInvestment >" + Parameter.FreeCashFlow;
        }

        public void averageROE(int countYear = 5)
        {
            if (command != null)
            {
                command += " INTERSECT ";
            }

            command += @"SELECT ";
            command += "a0" + "." + Parameter.SelectColumn + " FROM "+
                Parameter.TableName + Parameter.year + " a0";

            for (int i = 1; i < countYear; i++)
            {
                command += " ," + Parameter.TableName + (Parameter.year - i) + " a" + i;

            }

            command += " where ";

            for (int i = 1; i < countYear; i++)
            {
                command += "a0.company= a" + i + ".company AND ";
            }

            command +="(a0.ROE";

            for (int i = 1; i < countYear; i++)
            {
                command+= "+a"+i+".ROE";
            }

            command += @")/" + countYear + ">" + Parameter.ROE;
        }

        public void averageFreeCashFlow(int countYear=5)//平均cashFlow大於某數字
        {

            if (command != null)
            {
                command += " INTERSECT ";
            }

            command += @"SELECT ";
            command += "a0" + "." + Parameter.SelectColumn + " FROM " +
                Parameter.TableName + Parameter.year + " a0";

            for (int i = 1; i < countYear; i++)
            {
                command += " ," + Parameter.TableName + (Parameter.year - i) + " a" + i;

            }

            command += " where ";

            for (int i = 1; i < countYear; i++)
            {
                command += "a0.company= a" + i + ".company AND ";
            }

            command += "(a0.CashFlowForOperating+a0.CashFlowForInvestment";

            for (int i = 1; i < countYear; i++)
            {
                command += "+a" + i + ".CashFlowForOperating" + "+a" + i + ".CashFlowForInvestment" ;
            }

            command += @")" + ">" + Parameter.FreeCashFlow;
        }

        public void monthRevenue()
        {
            if (command != null)
            {
                command += " INTERSECT ";
            }

            command = @"SELECT " ;
        }

        public void GetDataFromDatabase( int countField = 1, string DatabaseName = "StockDatabase")
        {
            SqlDataReader myData1;
            string[] result = new string[countField];//取得欄位的數目
            string using_Database = @"use " + DatabaseName;

            #region 使用資料庫
            nonqueryCommand.CommandText = using_Database;
            Console.WriteLine(nonqueryCommand.CommandText);
            nonqueryCommand.ExecuteNonQuery();
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
                return;
            }

            if (!myData1.HasRows)
            {
                // 如果沒有資料,顯示沒有資料的訊息
                Console.WriteLine("No data5.");
                return;
            }
            #endregion
            #region 取得資料庫裡的data
            while (myData1.Read())
            {

                if (myData1.FieldCount < countField)
                {
                    Console.WriteLine("欄位數目有誤");
                    return;
                }

                for (int i = 0; i < countField; i++)
                {
                    result[i] = myData1[i].ToString();
                    Console.WriteLine("Text={0}", result[i].ToString());

                }
                searchData.Add(result[0]);//存在searchData
            }
            #endregion
            myData1.Close();

            foreach (string s in searchData)
            {
                Console.WriteLine(s);
            }
        }




     


       

      



    }
}

