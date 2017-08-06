using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL_InsertAndAdd;

namespace StockSimulation.Stock
{
    class Bargin
    {
        public int company;
        public DateTime date;
        public double ForeignInvestorNetBuySell;
        public double InvestmentTruthNetBuySell;
        public double DealerNetBuySell;
        public double ForeignInvestorAccumulateBuySell;
        public double InvestmentTrustAccumulateBuySell;
        public double DealerAccumulateBuySell;
        public double AllAccumulateBuySell;
        public double marginloan;
        public double stockloan;
        public double marginloanUseRatio;
        public double stockloanUseRatio;
        public double MarginStockRatio;

        public double[] AcculateForeignInvestorNetBuySell;
        public double[] AcculateInvestmentTrustNetBuySell;
        public double[] AcculateDealerNetBuySell;
        public double[] MaxForeignInvestorNetBuySell;
        public double[] MaxInvestmentTrustNetBuySell;
        public double[] MaxDealerNetBuySell;
        public double[] MinForeignInvestorNetBuySell;
        public double[] MinInvestmentTrustNetBuySell;
        public double[] MinDealerNetBuySell;

        public Bargin(string[] temp)
        {
            this.company = int.Parse(temp[0]);
            this.date = DateTime.Parse(temp[1]);
            this.ForeignInvestorNetBuySell = double.Parse(temp[2]);
            this.InvestmentTruthNetBuySell = double.Parse(temp[3]);
            this.DealerNetBuySell = double.Parse(temp[4]);
            this.ForeignInvestorAccumulateBuySell = double.Parse(temp[5]);
            this.InvestmentTrustAccumulateBuySell = double.Parse(temp[6]);
            this.DealerAccumulateBuySell = double.Parse(temp[7]);
            this.AllAccumulateBuySell = double.Parse(temp[8]);
            this.marginloan = double.Parse(temp[9]);
            this.stockloan = double.Parse(temp[10]);
            this.marginloanUseRatio = double.Parse(temp[11]);
            this.stockloanUseRatio = double.Parse(temp[12]);
            this.MarginStockRatio = double.Parse(temp[13]);
        }
    
    }

    class BarginReward
    {
        public int company;
        public DateTime date;
        public double ForeignInvestorNetBuySell;
        public double InvestmentTrustNetBuySell;
        public double DealerNetBuySell;
        public double ForeignInvestorBuySellDay;
        public double InvestmentTrustBuySellDay;
        public double DealerBuySellDay;
        public double AllBuySellDay;
        public double ForeignInvestorReward;
        public double InvestmentTrustReward;
        public double DealerReward;
        public double AllReward;

        public BarginReward(string[] temp)
        {
            this.company = int.Parse(temp[0]);
            this.date = DateTime.Parse(temp[1]);
            this.ForeignInvestorNetBuySell = double.Parse(temp[2]);
            this.InvestmentTrustNetBuySell = double.Parse(temp[3]);
            this.DealerNetBuySell = double.Parse(temp[4]);
            this.ForeignInvestorBuySellDay = double.Parse(temp[5]);
            this.InvestmentTrustBuySellDay = double.Parse(temp[6]);
            this.DealerBuySellDay = double.Parse(temp[7]);
            this.AllBuySellDay = double.Parse(temp[8]);
            this.ForeignInvestorReward = double.Parse(temp[9]);
            this.InvestmentTrustReward = double.Parse(temp[10]);
            this.DealerReward =double.Parse(temp[11]);
            this.AllReward = double.Parse(temp[12]);
        }
    }

    class SimulationForBargain
    {
        GetDataFromDatabase gd = new GetDataFromDatabase();
        public List<Bargin> bargin = new List<Bargin>();
        public List<BarginReward> barginReward = new List<BarginReward>();

        public void Initial(string startDate, string EndDate, int company)
        {
            bargin.Clear();
            barginReward.Clear();

            #region 初始化籌碼
            List<string[]> temp = new List<string[]>();
            string command = @"SELECT [company],[date],[ForeignInvestorNetBuySell],[InvestmentTrustNetBuySell],[DealerNetBuySell]
                ,[ForeignInvestorAccumulateBuySell],[InvestmentTrustAccumulateBuySell],[DealerAccumulateBuySell],[AllAccumulateBuySell],[marginloan]
                ,[stockloan],[marginloanUseRatio],[stockloanUseRatio],[MarginStockRatio] FROM [StockDatabaseWithTech].[dbo].[BargainData] where [Index] > "
                        + company + "00000000 and [Index] <" + (company + 1) +
                        "00000000 and [date] between '" + startDate + @"' and '" + EndDate + "' order by [date] ";

            gd.GetData(command, ref temp);

            for (int i = 0; i < temp.Count; i++)
            {
                bargin.Add(new Bargin(temp[i]));
            }
            #endregion

            #region 籌碼報酬初始化
            temp = new List<string[]>();

            command = @"SELECT [company],[date],[ForeignInvestorNetBuySell],[InvestmentTrustNetBuySell],[DealerNetBuySell]
            ,[ForeignInvestorBuySellDay],[InvestmentTrustBuySellDay],[DealerBuySellDay],[AllBuySellDay],[ForeignInvestorReward],[InvestmentTrustReward]
            ,[DealerReward],[AllReward] FROM [StockDatabaseWithTech].[dbo].[BargainRewardData] where [Index] > "
                        + company + "00000000 and [Index] <" + (company + 1) +
                        "00000000 and [date] between '" + startDate + @"' and '" + EndDate + "' order by [date] ";

            gd.GetData(command, ref temp);

            for (int i = 0; i < temp.Count; i++)
            {
                barginReward.Add(new BarginReward(temp[i]));
            }
            #endregion

            Initial_AcculateForeignInvestorNetBuySell(5, 10, 20, 120);
            Initial_AcculateInvestmentTrustNetBuySell(5, 10, 20, 120);
            Initial_AcculateAverageDealerNetBuySell(5, 10, 20, 120);
            Initial_MaxForeignInvestorNetBuySell(5, 10, 20, 40);
            Initial_MaxInvestmentTrustNetBuySell(5, 10, 20, 40);
            Initial_MaxDealerNetBuySell(5, 10, 20, 40);
            Initial_MinForeignInvestorNetBuySell(5, 10, 20, 40);
            Initial_MinInvestmentTrustNetBuySell(5, 10, 20, 40);
            Initial_MinDealerNetBuySell(5, 10, 20, 40);
        }

        void Initial_AcculateForeignInvestorNetBuySell(params int[] AverageDays)//外資籌碼平均
        {
            double sum = 0;


            for (int k = 0; k < AverageDays.Length; k++)
            {
                for (int i = AverageDays[k]; i < bargin.Count; i++)
                {
                    if (bargin[i].AcculateForeignInvestorNetBuySell == null)
                    {
                        bargin[i].AcculateForeignInvestorNetBuySell = new double[AverageDays.Length];
                    }

                    for (int j = 0; j < AverageDays[k]; j++)
                    {
                        sum += bargin[i - j].ForeignInvestorNetBuySell;
                    }

                    bargin[i].AcculateForeignInvestorNetBuySell[k] = sum;
                    sum = 0;
                }
            }
        }
        void Initial_AcculateInvestmentTrustNetBuySell(params int[] AverageDays)//投信籌碼平均
        {
            double sum = 0;


            for (int k = 0; k < AverageDays.Length; k++)
            {
                for (int i = AverageDays[k]; i < bargin.Count; i++)
                {
                    if (bargin[i].AcculateInvestmentTrustNetBuySell == null)
                    {
                        bargin[i].AcculateInvestmentTrustNetBuySell = new double[AverageDays.Length];
                    }

                    for (int j = 0; j < AverageDays[k]; j++)
                    {
                        sum += bargin[i - j].InvestmentTruthNetBuySell;
                    }

                    bargin[i].AcculateInvestmentTrustNetBuySell[k] = sum;
                    sum = 0;
                }
            }
        }
        void Initial_AcculateAverageDealerNetBuySell(params int[] AverageDays)//自營商籌碼平均
        {
            double sum = 0;


            for (int k = 0; k < AverageDays.Length; k++)
            {
                for (int i = AverageDays[k]; i < bargin.Count; i++)
                {
                    if (bargin[i].AcculateDealerNetBuySell == null)
                    {
                        bargin[i].AcculateDealerNetBuySell = new double[AverageDays.Length];
                    }

                    for (int j = 0; j < AverageDays[k]; j++)
                    {
                        sum += bargin[i - j].DealerNetBuySell;
                    }

                    bargin[i].AcculateDealerNetBuySell[k] = sum;
                    sum = 0;
                }
            }
        }

        void Initial_MaxForeignInvestorNetBuySell(params int[] MaxDays)//外資籌碼最高
        {
            double Max = 0;


            for (int k = 0; k < MaxDays.Length; k++)
            {
                for (int i = MaxDays[k]; i < bargin.Count; i++)
                {
                    if (bargin[i].MaxForeignInvestorNetBuySell == null)
                    {
                        bargin[i].MaxForeignInvestorNetBuySell = new double[MaxDays.Length];
                    }

                    for (int j = 0; j < MaxDays[k]; j++)
                    {
                        if (bargin[i - j].ForeignInvestorNetBuySell > Max)
                            Max = bargin[i - j].ForeignInvestorNetBuySell;
                    }

                    bargin[i].MaxForeignInvestorNetBuySell[k] = Max;
                    Max = 0;
                }
            }
        }
        void Initial_MaxInvestmentTrustNetBuySell(params int[] MaxDays)//投信籌碼最高
        {
            double Max = 0;


            for (int k = 0; k < MaxDays.Length; k++)
            {
                for (int i = MaxDays[k]; i < bargin.Count; i++)
                {
                    if (bargin[i].MaxInvestmentTrustNetBuySell == null)
                    {
                        bargin[i].MaxInvestmentTrustNetBuySell = new double[MaxDays.Length];
                    }

                    for (int j = 0; j < MaxDays[k]; j++)
                    {
                        if (bargin[i - j].InvestmentTruthNetBuySell > Max)
                            Max = bargin[i - j].InvestmentTruthNetBuySell;
                    }

                    bargin[i].MaxInvestmentTrustNetBuySell[k] = Max;
                    Max = 0;
                }
            }
        }
        void Initial_MaxDealerNetBuySell(params int[] MaxDays)//自營商最高
        {
            double Max = 0;


            for (int k = 0; k < MaxDays.Length; k++)
            {
                for (int i = MaxDays[k]; i < bargin.Count; i++)
                {
                    if (bargin[i].MaxDealerNetBuySell == null)
                    {
                        bargin[i].MaxDealerNetBuySell = new double[MaxDays.Length];
                    }

                    for (int j = 0; j < MaxDays[k]; j++)
                    {
                        if (bargin[i - j].DealerNetBuySell > Max)
                            Max = bargin[i - j].DealerNetBuySell;
                    }

                    bargin[i].MaxDealerNetBuySell[k] = Max;
                    Max = 0;
                }
            }
        }

        void Initial_MinForeignInvestorNetBuySell(params int[] MinDays)//外資籌碼最低
        {
            double Min = 10000;


            for (int k = 0; k < MinDays.Length; k++)
            {
                for (int i = MinDays[k]; i < bargin.Count; i++)
                {
                    if (bargin[i].MinForeignInvestorNetBuySell == null)
                    {
                        bargin[i].MinForeignInvestorNetBuySell = new double[MinDays.Length];
                    }

                    for (int j = 0; j < MinDays[k]; j++)
                    {
                        if (bargin[i - j].ForeignInvestorNetBuySell < Min)
                            Min = bargin[i - j].ForeignInvestorNetBuySell;
                    }

                    bargin[i].MinForeignInvestorNetBuySell[k] = Min;
                    Min = 10000;
                }
            }
        }
        void Initial_MinInvestmentTrustNetBuySell(params int[] MinDays)///投信籌碼最低
        {
            double Min = 10000;


            for (int k = 0; k < MinDays.Length; k++)
            {
                for (int i = MinDays[k]; i < bargin.Count; i++)
                {
                    if (bargin[i].MinInvestmentTrustNetBuySell == null)
                    {
                        bargin[i].MinInvestmentTrustNetBuySell = new double[MinDays.Length];
                    }

                    for (int j = 0; j < MinDays[k]; j++)
                    {
                        if (bargin[i - j].InvestmentTruthNetBuySell < Min)
                            Min = bargin[i - j].InvestmentTruthNetBuySell;
                    }

                    bargin[i].MinInvestmentTrustNetBuySell[k] = Min;
                    Min = 10000;
                }
            }
        }
        void Initial_MinDealerNetBuySell(params int[] MinDays)///自營商最低
        {
            double Min = 10000;


            for (int k = 0; k < MinDays.Length; k++)
            {
                for (int i = MinDays[k]; i < bargin.Count; i++)
                {
                    if (bargin[i].MinDealerNetBuySell == null)
                    {
                        bargin[i].MinDealerNetBuySell = new double[MinDays.Length];
                    }

                    for (int j = 0; j < MinDays[k]; j++)
                    {
                        if (bargin[i - j].DealerNetBuySell < Min)
                            Min = bargin[i - j].DealerNetBuySell;
                    }

                    bargin[i].MinDealerNetBuySell[k] = Min;
                    Min = 10000;
                }
            }
        }
    }


}
