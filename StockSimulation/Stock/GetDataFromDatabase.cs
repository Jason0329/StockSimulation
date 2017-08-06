using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SQL_InsertAndAdd
{
    class GetDataFromDatabase
    {
       
        SqlCommand nonqueryCommand;
        SqlConnection Connection;


        public GetDataFromDatabase(string connectionPath = "server=USER-PC\\SQLEXPRESS;database=StockDatabase;Integrated Security=SSPI")
        {
            Connection = new SqlConnection(connectionPath);
            nonqueryCommand = Connection.CreateCommand();
                }

        public int GetData<T>(string SetCommand,ref List<T> storeData)
        {
            storeData.Clear();
            storeData = new List<T>();
            nonqueryCommand = new SqlCommand(SetCommand, Connection);
            //Console.WriteLine(nonqueryCommand.CommandText);
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
                //Console.WriteLine(SetCommand);
                Console.WriteLine("No data with this company at this time:" + SetCommand);
                //Console.ReadKey();
                myData1.Close();
                return -1;
            }

            #endregion

            string[] result ;
            #region 取得資料庫裡的data

            while (myData1.Read())
            {

                if (myData1.FieldCount < 1)
                {
                    Console.WriteLine("欄位數目有誤");
                    return -3;
                }

                result = new string[myData1.FieldCount];

                for (int i = 0; i < myData1.FieldCount; i++)
                {
                    result[i] = myData1[i].ToString();
                   // Console.WriteLine("Text={0}", result[i].ToString());
                }

                if (Type.GetTypeCode(typeof(T)) == TypeCode.String)
                {
                    storeData.Add((T)(object)result[0].Trim());
                }
                else
                {
                    storeData.Add((T)(object)result);
                }
  
            }
            #endregion

            myData1.Close();
            return 0;
        }

        public void updateData(string command)
        {
            nonqueryCommand = new SqlCommand(command, Connection);
            Console.WriteLine(nonqueryCommand.CommandText);

            try
            {
                nonqueryCommand.ExecuteNonQuery();
            }
            catch (Exception eee)
            {
                Console.WriteLine(eee.Message);
                return ;
            }
        }
    }
}
