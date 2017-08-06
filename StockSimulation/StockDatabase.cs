using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;

namespace StockSimulation
{

    class StockDatabase
    {
        SqlConnection thisConnection = new SqlConnection("server=USER-PC\\SQLEXPRESS;database=StockDatabase;Integrated Security=SSPI");
        SqlCommand nonqueryCommand;
        string FirstColumn = "Date";//第一個column名稱

        string TableName;//這個class正在使用的Table

        static dynamicControlExcelTest.Excel_Use myExcel;//使用的excel資料庫

        string FuturePath = @"C:\Users\user\Desktop\股票\程式選股\台指TICK資料\1998~2008.7.24 TICK.txt";

        public string _FirstColumn
        {
            set { FirstColumn = value; }
        }

        public StockDatabase()
        {
            nonqueryCommand = thisConnection.CreateCommand();
            thisConnection.Open();
        }

        string ExcelTextOperate(string operate)
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
        }//把日期格是重新改寫 才能存
        string ExcelTextOperateForPrice(string operate)//看格式是否是"-"
        {
            if (operate.CompareTo("-") == 0)
                return "0";
            else
                return operate;
        }

        string GetColumn(string [] s)//定義欄位名稱，創造資料表時使用
        {
            string tempp = "";

            if (Parameter.IsEnum == true)
            {
                foreach (Row ss in Enum.GetValues(typeof(Row)))
                {
                    tempp += ", " + ss + " float";
                }
                return tempp;
            }
            else
            {

                for (int i = 0; i < s.Length; i++)
                {
                    tempp += ", " + FirstColumn + ExcelTextOperate(s[i]) + " float";
                }
                return tempp;
            }
        }

        void CreatTable<T>(T TableName,string [] column, string DatabaseName = "StockDatabase")//創造一個Table TEJ股市整理專用,也可以用在TEJ股價上
        {
            string data_type = " ";

            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean: data_type = "boolean";
                    break;
                //case TypeCode.Byte: return (T)(object)Convert.ToByte(o);
                //case TypeCode.Char: return (T)(object)Convert.ToChar(o);
                case TypeCode.DateTime:
                    data_type = "smalldatetime";
                    break;
                //case TypeCode.Decimal: return (T)(object)Convert.ToDecimal(o);
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                    data_type = "float";
                    break;
                case TypeCode.String:
                    data_type = "nvarchar(15)";
                    break;
                default:
                    break;
            }


            this.TableName = TableName.ToString();//取得這個class正在用的Table
            #region 先用一個SQL指令來創造TABLE
            string using_Database = @"use " + DatabaseName;
            string myTableName = @"CREATE TABLE " + TableName.ToString() + " (";

            myTableName += FirstColumn + " "+data_type+"  PRIMARY KEY ";


            myTableName += GetColumn(column);


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
                thisConnection.Close();
                Console.WriteLine("Connection Closed.");
            }
            finally
            {



            }
            #endregion
        }

        void CreatColunmByExcel()//新增欄位
        {

            nonqueryCommand.CommandText = @"use StockDatabase";
            Console.WriteLine(nonqueryCommand.CommandText);
            nonqueryCommand.ExecuteNonQuery();

            string command = "ALTER TABLE " + myExcel.mySheet[0].Name + " ADD ";
            string temp = "";

            temp += FirstColumn + ExcelTextOperate(myExcel.mySheet[0].Cells[1, 2].Text) + " float";
            for (int i = 2; i < myExcel.CountExcelWidth; i++)
            {
                temp += ", " + FirstColumn + ExcelTextOperate(myExcel.mySheet[0].Cells[1, i + 1].Text) + " float";
            }

            command += temp;
            nonqueryCommand.CommandText = command;
            try
            {
                Console.WriteLine(nonqueryCommand.CommandText);
                nonqueryCommand.ExecuteNonQuery();
                Console.WriteLine("加入成功");
            }
            catch (Exception eee)
            {
                Console.WriteLine(eee.Message);
                return;
            }

        }

        public void CreatColunm<T>(string TableName , params T [] data)//新增欄位
        {
            string data_type=" ";
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean: data_type = "boolean";
                    break;
                //case TypeCode.Byte: return (T)(object)Convert.ToByte(o);
                //case TypeCode.Char: return (T)(object)Convert.ToChar(o);
                case TypeCode.DateTime:
                    data_type = "smalldatetime";
                        break;
                //case TypeCode.Decimal: return (T)(object)Convert.ToDecimal(o);
                case TypeCode.Double: 
                case TypeCode.Int16: 
                case TypeCode.Int32: 
                case TypeCode.Int64: 
                case TypeCode.SByte:
                    data_type = "float";
                    break;
                case TypeCode.String:
                    data_type = "nvarchar(15)";
                    break;
                default:
                    break;
            }

            nonqueryCommand.CommandText = @"use StockDatabase";
            Console.WriteLine(nonqueryCommand.CommandText);
            nonqueryCommand.ExecuteNonQuery();

            string command = "ALTER TABLE " + TableName + " ADD ";
            string temp = "";

            for (int i = 0; i < data.Length; i++)
            {
                temp += ", " + FirstColumn + ExcelTextOperate(data[i].ToString()) +" " + data_type;
            }

            command += temp;
            nonqueryCommand.CommandText = command;
            try
            {
                Console.WriteLine(nonqueryCommand.CommandText);
                nonqueryCommand.ExecuteNonQuery();
                Console.WriteLine("加入成功");
            }
            catch (Exception eee)
            {
                Console.WriteLine(eee.Message);
                return;
            }

        }

        void AddData(string companyName,string TableName, params float[] rowData)
        {
            #region 把加入Data得SQL指令先用出來
            string command = @"insert into " + TableName + " values " + " ( '" + companyName + "'";

            for (int i = 0; i < rowData.Length; i++)
            {
                command += "," + rowData[i];
            }

            command += ")";
            #endregion

            #region 開始執行指令
            nonqueryCommand.CommandText = @"use StockDatabase";
            
            nonqueryCommand.ExecuteNonQuery();
            nonqueryCommand.CommandText = command;
           
            try
            {
                nonqueryCommand.ExecuteNonQuery();
            }
            catch (Exception eeee)
            {
                Console.WriteLine(nonqueryCommand.CommandText);
                Console.WriteLine(eeee.Message);
            }
            Console.WriteLine("加入成功");
            #endregion


        }

        public void GetFutureData()
        {




            // AddFutureData(5, "2011/1/1" , "13:00", 2001, 1002);
            int i = 1;
            StreamReader sr = new StreamReader(FuturePath);
            string temp;
            string[] tempp = new string[4];


            while (!sr.EndOfStream)
            {
                temp = sr.ReadLine();
                tempp = temp.Split(',');
                try
                {
                    AddFutureData(i, tempp[0], tempp[1], float.Parse(tempp[2]), float.Parse(tempp[3]));
                    i++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Console.WriteLine(temp);
                }
            }
            sr.Close();
        }
        
        void AddFutureData(int index ,string date , string time, float price, float volume)
        {
            #region 把加入Data得SQL指令先用出來
            string command = @"insert into " + @"tick_thirty_1998_2008_7_24" + " values " + " ( " + index + ",'" + date + " " + time + "'"
                + "," + price + "," + volume +"";
            command += ")";
            Console.WriteLine(command);
            #endregion

            #region 開始執行指令
            nonqueryCommand.CommandText = @"use StockDatabase";

            nonqueryCommand.ExecuteNonQuery();
            nonqueryCommand.CommandText = command;

            try
            {
                nonqueryCommand.ExecuteNonQuery();
            }
            catch (Exception eeee)
            {
                Console.WriteLine(nonqueryCommand.CommandText);
                Console.WriteLine(eeee.Message);
            }
            Console.WriteLine("加入成功");
            #endregion
        }

        ~StockDatabase()
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

        public static void CreateDatabase(dynamicControlExcelTest.Excel_Use myExcel)//創造新的Table
        {
            StockDatabase.myExcel = myExcel;

            float[] tempData;//每筆(整列)資料佔存

            StockDatabase data = new StockDatabase();



            for (int k = 0; k < myExcel.mySheet.Length; k++)
            {
                myExcel.UsingSheet = k;

                #region try creatTable
                try
                {
                   // data.CreatTable(myExcel.mySheet[k].Name);
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.Message);
                }
                #endregion

                tempData = new float[myExcel.CountExcelWidth - 1];



                for (int i = 1; i < myExcel.CountExcelHeight; i++)
                {
                    for (int j = 1; j < myExcel.CountExcelWidth; j++)
                    {
                        string tempp = myExcel.mySheet[k].Cells[i + 1, j + 1].Text;

                        try
                        {
                            if (myExcel.mySheet[k].Cells[i + 1, j + 1].Text.CompareTo("-") == 0)
                                tempData[j - 1] = 0;
                            else if (myExcel.mySheet[k].Cells[i + 1, j + 1].Text.CompareTo("") == 0)
                                tempData[j - 1] = 0;
                            else if (myExcel.mySheet[k].Cells[i + 1, j + 1].Text.CompareTo(" ") == 0)
                                tempData[j - 1] = 0;
                            else
                                tempData[j - 1] = float.Parse(myExcel.mySheet[k].Cells[i + 1, j + 1].Text);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                   

                    //data.AddData(myExcel.mySheet[k].Cells[i + 1, 1].Text,, tempData);//加入資料



                    #region TempUseCode//暫時休息 讓CPU不會占用太多資源
                    System.Threading.Thread.Sleep(500);
                    #endregion
                }
            }
        }

        public void UpdateDatabase(dynamicControlExcelTest.Excel_Use myExcel)//加入新的column所用
        {
            StockDatabase.myExcel = myExcel;
            try
            {
                //  CreatColunm();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            string command = "UPDATE " + myExcel.mySheet[0].Name + " SET ";

            for (int j = 1; j < myExcel.CountExcelHeight; j++)
            {

                command += FirstColumn + ExcelTextOperate(myExcel.mySheet[0].Cells[1, 2].Text)
                        + "=" + ExcelTextOperateForPrice(myExcel.mySheet[0].Cells[j + 1, 2].Text);

                for (int i = 2; i < myExcel.CountExcelWidth; i++)
                {
                    command += ", " + FirstColumn + ExcelTextOperate(myExcel.mySheet[0].Cells[1, i + 1].Text)
                        + "=" + ExcelTextOperateForPrice(myExcel.mySheet[0].Cells[j + 1, 1 + i].Text);
                }

                command += @" where " + FirstColumn + "= '" + myExcel.mySheet[0].Cells[j + 1, 1].Text + @"'";

                nonqueryCommand.CommandText = @"use StockDatabase";
                //Console.WriteLine(nonqueryCommand.CommandText);
                nonqueryCommand.ExecuteNonQuery();
                nonqueryCommand.CommandText = command;
                Console.WriteLine(nonqueryCommand.CommandText);
                nonqueryCommand.ExecuteNonQuery();
                System.Threading.Thread.Sleep(500);
                Console.WriteLine("加入成功");

                command = "UPDATE " + myExcel.mySheet[0].Name + " SET ";
            }


        }

        public void GetThreeMonthRevenue(string FilePath , string TableName)
        {
            StreamReader sr = new StreamReader(FilePath, System.Text.Encoding.Default);
            StockDatabase data = new StockDatabase();
            string line;
            string [] temp;
            float[] tempData;//每筆(整列)資料佔存



            if (!sr.EndOfStream)
            {
                line = sr.ReadLine();

                temp = line.Split(',');
               


                string[] tempp = new string[temp.Length -1];

                for (int i = 1; i < temp.Length; i++)
                {
                    tempp[i-1] = temp[i];
                }

                #region try creatTable
                try
                {
                    CreatTable(TableName, tempp);
                }
                 catch (Exception ee)
                {
                    Console.WriteLine(ee.Message);
                }
                #endregion
            }
            else
            {
                return;
            }

            int ss;
            while (!sr.EndOfStream)
            {
                nonqueryCommand.CommandText = @"use StockDatabase";
                Console.WriteLine(nonqueryCommand.CommandText);
                nonqueryCommand.ExecuteNonQuery();

                line = sr.ReadLine();
                temp = line.Split(',');

                if (temp[0].CompareTo(" ") == 0 || temp[0].CompareTo("") == 0 )
                    break;

                tempData = new float[temp.Length-1];
                for (int i = 1; i < temp.Length; i++)
                {
                    try
                    {
                        if (temp[i].CompareTo("-") == 0)
                            tempData[i - 1] = 0;
                        else if (temp[i].CompareTo("") == 0)
                            tempData[i - 1] = 0;
                        else if (temp[i].CompareTo(" ") == 0)
                            tempData[i - 1] = 0;
                        else
                            tempData[i - 1] = float.Parse(temp[i]);
                        
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee.Message);
                    }
                }



                data.AddData(temp[0], TableName,tempData);//加入資料



                #region TempUseCode//暫時休息 讓CPU不會占用太多資源
               // System.Threading.Thread.Sleep(500);
                #endregion

             

            }







            sr.Close();
            sr.Dispose();

                  
                
            }
        }
    }


