using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL_InsertAndAdd;
using System.IO;

namespace StockSimulation.stockSimulation
{
    class SimulationForPossibility:Variable
    {

        List<string> _company = new List<string>();

        public void Initial(string company , string startdata= "2008-10-01" , string enddate = "2013-10-05")
        {
            this.company = company;
            this.startDate = startdata;
            this.endDate = enddate;
        }
        void GetCompany(string command = @"SELECT company FROM [StockDatabase].[dbo].[standard_analysis] group by company order by company")
        {
            SQL_Action sq = new SQL_Action("server=USER-PC\\SQLEXPRESS;database=ProssibilityDatabase;Integrated Security=SSPI");
            List<string> company = new List<string>();
            GetDatabase.GetData(command, ref _company);
        }

        public void BuyByPoss1()
        {
            
           
            GetCompany();
            int countRise = 0, countDrop = 0;
            bool IsBuy = false;
            bool counted = false;
            int count = 0;

            Initial("2330");
            GetData();
           
            for(int i=0 ;i<ClosePrice.Count ; i++)
            {
                if (double.Parse(RewardRatio[i][1]) < 0 && !IsBuy)
                {
                    countRise = 0;
                    countDrop++;
                }

                if (double.Parse(RewardRatio[i][1]) > 0 && IsBuy)
                {
                    countDrop = 0;
                    countRise ++;
                }

                if (double.Parse(RewardRatio[i][1]) == 0)
                {
                    if (countDrop > 0)
                        countDrop++;
                    else if (countRise > 0)
                        countRise++;
                }


                if ((countDrop ==3||countDrop==4) && !IsBuy)
                {
                    sp.BuyStock(2330,double.Parse(ClosePrice[i][1]),(ClosePrice[i][0]));
                    IsBuy = true;                    
                }

               

                if ((countRise > 1 || countDrop > 4) && IsBuy)
                {
                    sp.SellStock(2330, double.Parse(ClosePrice[i][1]), (ClosePrice[i][0]));
                    sp.SellStock(2330, double.Parse(ClosePrice[i][1]), (ClosePrice[i][0]));
                    IsBuy = false;
                }

                if (double.Parse(RewardRatio[i][1]) > 5)
                {
                    counted = true;
                }
                if (counted)
                {
                    count++;
                    countDrop = 0;
                    countRise = 0;
                }

                if (count == 20)
                {
                    sp.SellStock(2330, double.Parse(ClosePrice[i][1]), (ClosePrice[i][0]));
                    sp.SellStock(2330, double.Parse(ClosePrice[i][1]), (ClosePrice[i][0]));
                    count = 0;
                    counted = false;
                }

                

            }

            sp.ProfitAndLoss();
        }

        public void BuyByPoss2()
        {
            string command1 = @"SELECT [company] FROM [ProssibilityDatabase].[dbo].[AvgExpectation]
  
                     where RiseOrDropRatio>4 and IsRise=1  and Max5>20 and Max10<30 group by company ";

            string commanddd = @"SELECT [company] FROM [StockDatabase].[dbo].[standard_analysis]
  
                     where company =1455 group by company ";

            string FileName = "companyAboutHighRisk_5_LongTerm.csv";

            const int countDay = 10;
            GetCompany(commanddd);

            int countDrop = 0;
            int count = 0;
            bool CanBuy = true;
            double rev = 0;
            List<string> SelectedCompany = new List<string>();
            double AllRev = 0;
            double Poss = 0;
            List<double> WinPoss = new List<double>();
            List<double> AllR = new List<double>();

            for (int i = 0; i < _company.Count; i++)
            {
                Initial(_company[i]);
                ClosePrice = new List<string[]>();
                RewardRatio = new List<string[]>();
                Volumn = new List<string[]>();
                GetData();
                #region test
                //i = 0;
                //_company.Clear();
                //_company.Add("6223");
                #endregion
                for (int j = 0; j < RewardRatio.Count; j++)
                {
                    if (double.Parse(RewardRatio[j][1]) > 6 && CanBuy)
                    {
                        sp.BuyStock(int.Parse(_company[i].Trim()), double.Parse(ClosePrice[j][1]), ClosePrice[j][0]);
                        CanBuy = false;
                    }

                    if (!CanBuy)
                    {
                        if((double.Parse(RewardRatio[j][1])<0))
                        {
                            countDrop++;
                        }
                        count++;
                        AllRev += double.Parse(RewardRatio[j][1]);
                    }

                    if ((AllRev < -2 && count > 4 || countDrop==2) &&!CanBuy)
                    {
                        sp.SellStock(int.Parse(_company[i].Trim()), double.Parse(ClosePrice[j][1]), ClosePrice[j][0]);
                        count = 0;
                        CanBuy = true;
                        AllRev = 0;
                        countDrop = 0;
                    }

                    if ((count == countDay|| count>countDay) &&!CanBuy)
                    {
                        sp.SellStock(int.Parse(_company[i].Trim()), double.Parse(ClosePrice[j][1]), ClosePrice[j][0]);
                        count = 0;
                        CanBuy = true;
                        AllRev = 0;
                        countDrop = 0;
                    }

                    //if (double.Parse(RewardRatio[j][1]) > 5 && !CanBuy)
                    //{
                    //    count -= 2;
                    //}

                }

                rev=sp.ProfitAndLoss(ref Poss);
                sp.clear();
                if (rev /(1000*double.Parse(ClosePrice[1][1])) > 0.1)
                {
                    SelectedCompany.Add(_company[i]);
                    WinPoss.Add(Poss);
                    AllR.Add(rev);
                }
            }

            StreamWriter sw = new StreamWriter(FileName);

            

            double All = 0;
            for (int i = 0; i < AllR.Count; i++)
            {
                All += AllR[i];
            }

            Console.WriteLine("\n\n " + All);

            if (SelectedCompany.Count > 0)
            {
                sw.Write(SelectedCompany[0]);
            }
            else
                return;

            for(int i=1 ; i<SelectedCompany.Count ; i++)
            {
                sw.Write(","+SelectedCompany[i]);
            }

            sw.WriteLine();
            sw.Write(WinPoss[0]);

            for (int i = 1; i < SelectedCompany.Count; i++)
            {
                sw.Write("," + WinPoss[i]);
            }

            sw.WriteLine();
            sw.WriteLine(command1);
            sw.WriteLine("5% 買 放10天");

            sw.Close();
        }

        public void Buy2330ByFuture()//下跌買進2330
        {
            Future.FuturePossibility Fp = new Future.FuturePossibility();
            BuyAndSell BuySell = new BuyAndSell();
            company = "2330";
            GetData();
            Fp.startDate = this.startDate;
            Fp.endDate = this.endDate;
            Fp.getData();
            bool HasBuy = false;

            double AccDrop = 0;
            double AccRise = 0;
            double countDrop = 0;
            double countRise = 0;
            double stCountRise = 3;

            for (int i = 0; i < ClosePrice.Count; i++)
            {
                if (double.Parse(RewardRatio[i][1]) < 0)
                {
                    AccDrop += double.Parse(RewardRatio[i][1]);
                    AccRise = 0;
                    countDrop++;
                    countRise = 0;
                }
                else if (double.Parse(RewardRatio[i][1]) > 0)
                {
                    AccDrop = 0;
                    AccRise = double.Parse(RewardRatio[i][1]);
                    countRise++;
                    countDrop = 0;
                }

                if (AccRise > 5)
                {
                    stCountRise++;
                }


                if (HasBuy && ( AccDrop<0 || countRise==stCountRise))
                {
                    stCountRise = 3;
                    sp.SellStock(2330, double.Parse(ClosePrice[i][1]), ClosePrice[i][0]);
                    HasBuy = false;
                }

                if ((AccDrop < -3.5 || countDrop==3) &&!HasBuy)
                {
                    sp.BuyStock(2330, double.Parse(ClosePrice[i][1]), ClosePrice[i][0]);
                    AccDrop = 0;
                    HasBuy = true;
                }

                
            }

            double poss=0;
            sp.ProfitAndLoss(ref poss);

            

            
        }

        void GetData()
        {
            string command;
            command = @"SELECT [Time],[_" + company + @"]
                        FROM [StockDatabase].[dbo].[technical_analysis]
  
                        where [MyIndex]=209  and [Time] between '" + startDate + "' and '" + endDate + "' order by [Time]";

            GetDatabase.GetData(command, ref ClosePrice);

            command = @"SELECT [Time],[_" + company + @"]
                        FROM [StockDatabase].[dbo].[technical_analysis]
  
                        where [MyIndex]=201  and [Time] between '" + startDate + "' and '" + endDate + "' order by [Time]";
            GetDatabase.GetData(command, ref  RewardRatio);

            command = @"SELECT [Time],[_" + company + @"]
                        FROM [StockDatabase].[dbo].[technical_analysis]
  
                        where [MyIndex]=208  and [Time] between '" + startDate + "' and '" + endDate + "' order by [Time]";

            GetDatabase.GetData(command, ref  Volumn);
        }

       
    }
}
