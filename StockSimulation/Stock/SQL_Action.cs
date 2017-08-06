using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SQL_InsertAndAdd
{
    class SQL_Action
    {
        SqlConnection thisConnection;// = new SqlConnection("server=USER-PC\\SQLEXPRESS;database=StockDatabase;Integrated Security=SSPI");
        SqlCommand nonqueryCommand;
        string FirstColumn = "Date";
        string TableName;//這個class正在使用的Table



        #region constructor
        public SQL_Action(string serverPath = "server=USER-PC\\SQLEXPRESS;database=StockDatabase;Integrated Security=SSPI")
        {
            this.thisConnection = new SqlConnection(serverPath);
            nonqueryCommand = this.thisConnection.CreateCommand();
            thisConnection.Open();
        }
        ~SQL_Action()
        {
            try
            {
                thisConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        public int AddData(string TableName, params string[] rowData)
        {
            #region 把加入Data得SQL指令先用出來
            string command = @"insert into " + TableName + " values " + "(" ;

            for (int i = 0; i < rowData.Length-1; i++)
            {
                command +=  "'"+rowData[i]+"'" +"," ;
            }

            if (rowData.Length > 1)
            {
                command += "'" + rowData[rowData.Length - 1] + "'";
            }
            else
            {
                command += rowData[rowData.Length - 1];
            }

            command += ")";
            #endregion

            #region 開始執行指令
            nonqueryCommand.CommandText = command;

            try
            {
                nonqueryCommand.ExecuteNonQuery();
               // Console.WriteLine("加入成功");
                return 0;
            }
            catch (Exception eeee)
            {
                Console.WriteLine(nonqueryCommand.CommandText);
                Console.WriteLine(eeee.Message);
                return -1;
            }
            
            #endregion


        }

        public void CreatTable(string TableName, string columnCommand, string DatabaseName = "StockDatabase")//創造一個Table TEJ股市整理專用,也可以用在TEJ股價上
        {
           

            //switch (Type.GetTypeCode(typeof(T)))
            //{
            //    case TypeCode.Boolean: data_type = "boolean";
            //        break;
            //    //case TypeCode.Byte: return (T)(object)Convert.ToByte(o);
            //    //case TypeCode.Char: return (T)(object)Convert.ToChar(o);
            //    case TypeCode.DateTime:
            //        data_type = "smalldatetime";
            //        break;
            //    //case TypeCode.Decimal: return (T)(object)Convert.ToDecimal(o);
            //    case TypeCode.Double:
            //    case TypeCode.Int16:
            //    case TypeCode.Int32:
            //    case TypeCode.Int64:
            //    case TypeCode.SByte:
            //        data_type = "float";
            //        break;
            //    case TypeCode.String:
            //        data_type = "nvarchar(15)";
            //        break;
            //    default:
            //        break;
            //}


            this.TableName = TableName.ToString();//取得這個class正在用的Table
            #region 先用一個SQL指令來創造TABLE
            string using_Database = @"use " + DatabaseName;
            string myTableName = @"CREATE TABLE " + TableName.ToString() + "(";


            myTableName += columnCommand;


            myTableName += ")";
            #endregion
            #region 開始執行指令
            try
            {
                nonqueryCommand.CommandText = using_Database;
                Console.WriteLine(nonqueryCommand.CommandText);
                nonqueryCommand.ExecuteNonQuery();
                Console.WriteLine("Database is use, now switching");
                nonqueryCommand.CommandText = myTableName;
                Console.WriteLine(nonqueryCommand.CommandText);
                Console.WriteLine("Number of Rows Affected is: {0}", nonqueryCommand.ExecuteNonQuery());
                Console.WriteLine("OK");
            }
            catch (SqlException ex)
            {

                Console.WriteLine(ex.ToString());

            }
            catch (Exception ee)
            {
                //thisConnection.Close();
                Console.WriteLine("Connection failed.");
            }
            finally
            {



            }
            #endregion
        }
        
    }
}
