using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace dynamicControlExcelTest
{
    class Excel_Use
    {
        //引用Excel Application類別
        _Application myExcel = null;
        //引用活頁簿類別
        _Workbook myBook = null;
        //引用工作表類別
        public _Worksheet [] mySheet;
       // public _Worksheet WorkingSheet1;
       // public _Worksheet WorkingSheet2;
        //引用Range類別
        //Range myRange = null;
        public int CountExcelWidth = 0, CountExcelHeight = 0;

        int _UsingSheet = 0;
        public int UsingSheet { set { _UsingSheet = value; getWidthAndHeight(); } get { return _UsingSheet; } }

        const string path = @"C:\Users\user\Desktop\" + @"活頁簿1.xlsx";//AppDomain.CurrentDomain.BaseDirectory +

        
        public Excel_Use(string spath = path,bool _NeedNewSheet = false)//開啟已經存在的檔案
        {
            //開啟一個新的應用程式
            #region
            myExcel = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                myBook = myExcel.Workbooks.Open(spath, 0, 0, 5,
                        "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false,
                        0, true);
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                Console.WriteLine("Excel 已經開啟  請先關閉");
                return;
            }
            catch (Exception e1)
            {
                Console.WriteLine("開啟錯誤");
                return;
            }

            #endregion
            #region//多一個sheet
            if (_NeedNewSheet == false)
            {
                mySheet = new _Worksheet[myBook.Sheets.Count];

                for (int i = 0; i < myBook.Sheets.Count; i++)
                {
                    mySheet[i] = myBook.Sheets[i + 1];
                    //Console.WriteLine(mySheet[i].Name);
                }
                    
            }
            else
            {
                mySheet = new _Worksheet[myBook.Sheets.Count+1];

                for (int i = 0; i < myBook.Sheets.Count; i++)
                {
                    mySheet[i] = myBook.Sheets[i + 1];
                }

                mySheet[mySheet.Length - 1] = (Worksheet)myBook.Worksheets.Add(System.Type.Missing, 
                    System.Type.Missing, System.Type.Missing, System.Type.Missing);
            }

#endregion//
            UsingSheet = 0;

            #region//暫時放著的程式碼
            myExcel.AskToUpdateLinks = true;
            myExcel.ScreenUpdating = true;
           // myBook.a
            myExcel.Visible = false;
            #endregion
        }

        ~Excel_Use()
        {
            try
            {
                myBook.Close();
                myBook = null;
            }
            catch (Exception e)
            {
            }
        }

        public void SheetUse(string sheetName = " ")
        {
            #region//如果都沒有指定修改的sheet
            if (sheetName.CompareTo(" ") == 0)
            {
               // return null;
            }
            else
            {
                for (int i = 0; i < mySheet.Length; i++)
                {
                    if (mySheet[i].Name.CompareTo(sheetName) == 0)
                    {
                        Console.WriteLine(mySheet[i].Name);
                 //       return (Sheets)mySheet[i];
                    }
                }

                
            }

            //return null;
            #endregion
        }

        public virtual string []  RetrunSheetsRow()
        {
            string[] ss = new string[2]{"a0","1"};
            return ss;
        }

        void getWidthAndHeight()//計算sheet 高和寬
        {
            this.CountExcelHeight = 0;
            this.CountExcelWidth = 0;
            string s = this.mySheet[_UsingSheet].Cells[1, CountExcelWidth + 1].Text;
            while (true)
            {
                if (this.mySheet[_UsingSheet].Cells[1, CountExcelWidth + 1].Text.CompareTo("") == 0)
                {
                    break;
                }
                CountExcelWidth++;
            }
            while (true)
            {
                if (this.mySheet[_UsingSheet].Cells[CountExcelHeight + 1, 1].Text.CompareTo("") == 0)
                {
                    break;
                }
                CountExcelHeight++;
            }
        }


    }
}
