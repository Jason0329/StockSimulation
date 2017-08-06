using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace StockSimulation
{
    public class MydataResult
    {
        public string Tick { set; get; }
        public string volume { set; get; }
        public string Time { set; get; }

        public MydataResult(string tick , string volume , string time)
        {
            this.Tick = tick;
            this.volume = volume;
            this.Time = time;
        }
    }

    class SQL_GetData : ISQL_TakeData
    {
        public string command
        {
            set;
            get;
        }
        public SqlDataReader myData;
        SqlCommand nonqueryCommand;
        SqlConnection Connection = new SqlConnection("server=USER-PC\\SQLEXPRESS;database=StockDatabase;Integrated Security=SSPI");
        string using_Database = @"use " + "StockDatabase";
        int countField = 3; //取得欄位的數目
        public static  List<MydataResult> dataResult=new List<MydataResult>();
      //  MydataResult [] dataResult;
        string [] index;

        public SQL_GetData()
        {
            nonqueryCommand = Connection.CreateCommand();
            Connection.Open();
        }

        public void RunGetData(string command, string[] store )
        {
        }

        public int SetCommand(string date)
        {
            this.command = @"SELECT [index] , [IndexEnd]
   
             FROM [StockDatabase].[dbo].[IndexOf_tick_future_price2008_7_29_2010_9_24]
  
             where  convert(char, [date], 112)=convert(char," + date + @", 112)";

            nonqueryCommand = new SqlCommand(this.command, Connection);
            Console.WriteLine(nonqueryCommand.CommandText);
            SqlDataReader myData1;
            #region
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
                Console.WriteLine("No data2.");
                myData1.Close();
                return -1;
            }

            #endregion

            index = new string[2];
            string[] result = new string[3];
            #region 取得資料庫裡的data

            while (myData1.Read())
            {

                if (myData1.FieldCount < 2)
                {
                    Console.WriteLine("欄位數目有誤");
                    return -3;
                }


                for (int i = 0; i < myData1.FieldCount; i++)
                {
                 result[i]=myData1[i].ToString();
                 Console.WriteLine("Text={0}", result[i].ToString());
                }

                index[0] = result[0];
                index[1] = result[1];
                //for (int i = 0; i < countField; i++)
                //{

                //    result[i] = myData[i].ToString();
                //    Console.WriteLine("Text={0}", result[i].ToString());

                //}
                //dataResult.Add(result);
                //Console.WriteLine(dataResult);

                // buyStock.Add(result[0]);//存在buyStock

            }
            #endregion
            Console.WriteLine(index[0] + "  "+index[1]);
            myData1.Close();
            return 0;

        }

        public int DatabaseQueryCommand()
        {
            //dataResult = new List<string []>();
           
            string[] result = new string[countField];
            #region 使用資料庫
            this.command =@"SELECT AVG([price]) as Tick
      ,SUM([volume]) as Volume
      , convert(char, [date], 108) as time
  FROM [StockDatabase].[dbo].[tick_future_price2008_7_29_2010_9_24]  
 where [index] between "+ index[0]+" AND "+ index[1] + @"
  GROUP BY [date]
  order by time";
           // nonqueryCommand.CommandText = using_Database;
            //Console.WriteLine(nonqueryCommand.CommandText);
            //try
            //{
            //    nonqueryCommand.ExecuteNonQuery();
            //}
            //catch (Exception ee)
            //{
            //    Console.WriteLine(ee.Message);
            //}
            #endregion

            #region 取得資料庫資料的指標

            nonqueryCommand = new SqlCommand(command, Connection);
            Console.WriteLine(nonqueryCommand.CommandText);

            try
            {
                myData = nonqueryCommand.ExecuteReader();
            }
            catch (Exception eee)
            {
                Console.WriteLine(eee.Message);
                return -2;
            }

            if (!myData.HasRows)
            {
                // 如果沒有資料,顯示沒有資料的訊息
                Console.WriteLine("No data3.");
                myData.Close();
                return -1;
            }
            #endregion

            #region 取得資料庫裡的data
           
            while (myData.Read())
            {
              

                if (myData.FieldCount < countField)
                {
                    Console.WriteLine("欄位數目有誤");
                    return -3;
                }

                

               

                for (int i = 0; i < countField; i++)
                {

                    result[i] = myData[i].ToString();
                    Console.WriteLine("Text={0}", result[i].ToString());

                }
                
                dataResult.Add(new MydataResult(result[0],result[1],result[2]));
                Console.WriteLine(dataResult[0].Tick);

               // buyStock.Add(result[0]);//存在buyStock

            }
            #endregion

            myData.Close();
            return 0;
           
        }
    }
}
