using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;

namespace StockSimulation
{
    class CreateIndex
    {
        SqlConnection thisConnection = new SqlConnection("server=USER-PC\\SQLEXPRESS;database=StockDatabase;Integrated Security=SSPI");
        SqlCommand nonqueryCommand;
        List<string> storeData = new List<string>();

        public CreateIndex()
        {
            nonqueryCommand = thisConnection.CreateCommand();
            thisConnection.Open();
        }

        public void insert()
        {
            GetText();
            for(int i=0 ; i<storeData.Count ; i+=3)
            {
                AddData(i);
            }
        }

         void  GetText()
        {
            StreamReader sr = new StreamReader(@"IndexData.txt");

            string ss;
            string[] temp = new string[2];
            string[] temp1 = new string[3];
            string tempp;

            sr.ReadLine();

            ss = sr.ReadLine();

            temp = ss.Split('\t');

            while (!sr.EndOfStream)
            {
                temp1[0] = temp[0].Trim();
                temp1[1] = temp[1].Trim();

                ss=sr.ReadLine();
                temp = ss.Split('\t');

                temp1[2] = (int.Parse(temp[0].Trim()) - 1).ToString();

                tempp=temp1[2];
                temp1[2] = temp1[1];
                temp1[1] = tempp;
             
                
                storeData.Add(temp1[0]);
                storeData.Add(temp1[1]);
                storeData.Add(temp1[2]);
            }

            sr.Close();

            
        }

         void AddData(int i)
        {
            
            #region 把加入Data得SQL指令先用出來
            string command = "insert into IndexOf_tick_future_price2008_7_29_2010_9_24" + " values " + " ( '" + storeData[i]
                + "','" + storeData[i + 1] + "','" + storeData[i+2] + "')";
            #endregion

            #region 開始執行指令
            nonqueryCommand.CommandText = @"use StockDatabase";

            nonqueryCommand.ExecuteNonQuery();
            nonqueryCommand.CommandText = command;

            try
            {
                nonqueryCommand.ExecuteNonQuery();
                Console.WriteLine(nonqueryCommand.CommandText);
            }
            catch (Exception eeee)
            {
                Console.WriteLine(nonqueryCommand.CommandText);
                Console.WriteLine(eeee.Message);
            }
            Console.WriteLine("加入成功");
            #endregion


        }
    }
}
