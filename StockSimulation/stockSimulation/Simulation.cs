using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL_InsertAndAdd;
using System.IO;

namespace StockSimulation.Stock
{
    class UsefulVariable
    {
        public bool CanBuy = false;
        public bool HasBuy = false;
        public bool CanSell = false;
        public bool HasSell = false;
        public int HaveStockDay = 0;
        public double Accumulation = 0;
        public double AccumulationRise = 0;
        public double AccumulationDrop = 0;
        public bool PreSell = false;
        public double startToCountDropAfterPreSell = 0;
        public int CountDropDay = 0;
        public int Circumstance = 0;

        public void Initial()
        {
            CountDropDay = 0;
            CanBuy = false;
            CanSell = false;
            HasBuy = false;
            HasSell = false;
            PreSell = false;
            HaveStockDay = 0;
            Accumulation = 0;
            AccumulationDrop = 0;
            AccumulationRise = 0;
            startToCountDropAfterPreSell = 0;
        }
    }
    partial class Simulation:StockPossess
    {
        #region 參數
        UsefulVariable uv = new UsefulVariable();
        SimulationForTechnologicalAnalysis TechData;
        SimulationForSelectBasicFinancialReport BasicFinancial;
        SimulationForBargain Bargin;
        int i = 0;
        string TechStartDate = "2013-10-14";
        string TechEndDate="2014-10-14";
        string BasicStartDate = "1996-01-01";
        string BasicEndDate = "2014-11-01";
        string FilePath = @"C:\Users\user\Desktop\股票\模擬結果\2011_2014.csv";
        double AllWin = 0;
        double AllLoss = 0;
        #endregion

        public void StartSimulation()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();//引用stopwatch物件
            sw.Reset();//碼表歸零
            sw.Start();//碼表開始計時

            SimulationToStart();

            sw.Stop();
            TimeSpan el = sw.Elapsed;
            Console.WriteLine("花費 {0} ", el);
            long ms = sw.ElapsedMilliseconds;
            Console.WriteLine("花費 {0} 毫秒", ms);



          

        }

        public void SimulationToStart()
        {
            StreamWriter sw = new StreamWriter(FilePath);
            
            
            List<string> company = new List<string>();
            GetCompany(ref company);
            sw.Write("\uFEFF");

            sw.Write("公司代號");

            for (int i = 0; i < 20; i++)
            {
                sw.Write(",買進時間,買進價格,賣出時間,賣出價格"+",持有時間"
                    +",單次投報率"
                    +",年化報酬率");
            }

            sw.WriteLine(",勝率,總損益");

            
            for (int j = 0; j < company.Count; j++)
            {
                Run(int.Parse(company[j]));//初始化公司
                #region 開始跑公司資料
                for (i=40; i < TechData.TechData.Count; i++)
                {
                    #region 初始參數
                    if (uv.HasBuy)
                    {
                        uv.HaveStockDay++;
                        uv.Accumulation += TechData.TechData[i].ReturnOnInvestment;

                        if (TechData.TechData[i].ReturnOnInvestment > 0)
                            uv.AccumulationRise += TechData.TechData[i].ReturnOnInvestment;
                        else if (TechData.TechData[i].ReturnOnInvestment < 0)
                            uv.AccumulationDrop += TechData.TechData[i].ReturnOnInvestment;
                    }
                    #endregion
                    #region 看買賣條件
                    BuySell_ConditionForRise();
                    #endregion
                    #region 買賣
                    if (!uv.HasBuy && uv.CanBuy)
                    {
                        BuyStock(TechData.TechData[i].company, TechData.TechData[i].ClosePrice,
                            TechData.TechData[i].date.ToString());

                        uv.HasBuy = true;
                    }

                    if (uv.HasBuy && !uv.CanBuy)
                    {
                        SellStock(TechData.TechData[i].company, TechData.TechData[i].ClosePrice,
                            TechData.TechData[i].date.ToString());
                        uv.HasBuy = false;
                        uv.Initial();
                    }
                    #endregion
                }

                if (uv.HasBuy)
                {
                    SellStock(TechData.TechData[i - 1].company, TechData.TechData[i - 1].ClosePrice,
                         TechData.TechData[i - 1].date.ToString());
                    Console.WriteLine(TechData.TechData[i - 1].ClosePrice);
                    Console.WriteLine(TechData.TechData[i - 1].date);
                    uv.HasBuy = false;
                    uv.Initial();
                }
                #endregion

               
                ProfitAndLoss(sw);
                AllWin += win;
                AllLoss += loss;
                
            }

            sw.Write("勝," + AllWin + ",負," + AllLoss);
            sw.WriteLine(",勝率," + (AllWin / (AllWin + AllLoss)));
            sw.Close();
        }

        public void Run(int company)
        {
            TechData.Initial(TechStartDate, TechEndDate, company ,IsOTC);
            BasicFinancial.Initial(BasicStartDate, BasicEndDate, company , IsOTC);
            Bargin.Initial(TechStartDate, TechEndDate, company);
        }

        void GetCompany(ref  List<string> company)
        {
            string command ="";

            if (IsOTC)
            {
                command = @"SELECT [company] FROM [OverTheCounter].[dbo].[standard_analysis] 
                      group by [company]  order by [company] ";
            }
            else
            {
                command = @"SELECT [company] FROM [StockDatabase].[dbo].[standard_analysis] 
                      group by [company]  order by [company] ";
            }

            GetDataFromDatabase gd = new GetDataFromDatabase();

            gd.GetData(command, ref company);
        }

     
    }
}
