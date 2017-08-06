using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL_InsertAndAdd;

namespace StockSimulation.Stock
{
    class TechnologicalData
    {
        public int company;
        public DateTime date;
        public double OpenPrice;
        public double HighestPrice;
        public double LowestPrice;
        public double ClosePrice;
        public double ReturnOnInvestment;
        public double Yield;
        public double volume;
        public double[] Acculation;
        public double [] AverageValue;//多條均線
        public double[] AverageVolume;//量均線
        public double[] MaxPrice;
        public double[] MinPrice;
        public double[] MaxVolume;
        public double[] MinVolune;
       


        public TechnologicalData(string [] temp )
        {
            this.company = int.Parse(temp[0]);
            this.date = DateTime.Parse(temp[1]);
            try
            {
                this.OpenPrice = double.Parse(temp[2]);
            }
            catch (Exception e)
            {
                this.OpenPrice = 0;
            }
            this.HighestPrice = double.Parse(temp[3]);
            this.LowestPrice = double.Parse(temp[4]);
            this.ClosePrice = double.Parse(temp[5]);
            try
            {
                this.ReturnOnInvestment = double.Parse(temp[6]);
            }
            catch (Exception e)
            {
                this.ReturnOnInvestment = 0;
            }
            try
            {
                this.Yield = double.Parse(temp[7]);
            }
            catch (Exception e)
            {
                this.Yield = 0;
            }
            this.volume = double.Parse(temp[8]);
        }

    }
    class TaiwanStockIndex
    {
        public DateTime Date;
        public double OpenPrice;
        public double HighestPrice;
        public double LowestPrice;
        public double closePrice;
        public double difOpenClosePrice;
        public double difHighLowPrice;
        public double difLastClosePrice;

        public TaiwanStockIndex(string []temp,double lastClose)
        {
            this.Date = DateTime.Parse(temp[0]);
            this.OpenPrice = double.Parse(temp[1]);
            this.HighestPrice = double.Parse(temp[2]);
            this.LowestPrice = double.Parse(temp[3]);
            this.closePrice = double.Parse(temp[4]);
            this.difOpenClosePrice = this.closePrice - this.OpenPrice;
            this.difHighLowPrice = this.HighestPrice - this.LowestPrice;
            this.difLastClosePrice = this.closePrice - lastClose;
        }
       
    }
    class SimulationForTechnologicalAnalysis
    {
        GetDataFromDatabase gd = new GetDataFromDatabase();
        public List<TechnologicalData> TechData = new List<TechnologicalData>();
        public List<TaiwanStockIndex> TaiwanIndex = new List<TaiwanStockIndex>();
      

        public void Initial(string startDate, string EndDate, int company , bool IsOTC=false)
        {
            TechData.Clear();
            TaiwanIndex.Clear();

            #region 初始化股票
            List<string[]> temp = new List<string[]>();

            string command="";

            #region 初始化command 看是否抓取上櫃公司資料
            if (IsOTC)
            {
                command = @"SELECT [company],[date],[OpenPrice],[HighestPrice] ,[LowestPrice],[ClosePrice] ,[RewardRatio] ,[Yield] ,[Volume]
                        FROM [OverTheCounter].[dbo].[TechData] where [Index] > "
            + company + "00000000 and [Index] <" + (company + 1) +
            "00000000 and [date] between '" + startDate + @"' and '" + EndDate + "' order by [date] ";
            }
            else
            {

                command = @"SELECT [company],[date],[OpenPrice],[HighestPrice] ,[LowestPrice],[ClosePrice] ,[RewardRatio] ,[Yield] ,[Volume]
                        FROM [StockDatabaseWithTech].[dbo].[TechData] where [Index] > "
                            + company + "00000000 and [Index] <" + (company + 1) +
                            "00000000 and [date] between '" + startDate + @"' and '" + EndDate + "' order by [date] ";
            }
            #endregion


            gd.GetData(command, ref temp);
            for (int i = 0; i < temp.Count; i++)
            {
                TechData.Add(new TechnologicalData(temp[i]));
            }

            #endregion
          
            #region 初始化指數
            if (TechData.Count == 0) return;
            string sDate = TechData[0].date.ToString().Split(' ')[0].Replace("/","-");
            command = @"SELECT [date],[OpenPrice],[MaxPrice],[MinPrice] ,[ClosePrice] FROM [StockDatabase].[dbo].[FutureDayDate]
                    where [date] between '"+sDate+"' and '" + EndDate +"' order by [date]";
            gd.GetData(command, ref temp);

            double LastClose = 0;// double.Parse(temp[0][4]);

            for (int i = 0; i < temp.Count; i++)
            {
                TaiwanIndex.Add(new TaiwanStockIndex(temp[i],LastClose));
                LastClose =double.Parse(temp[i][4]);
            }
            #endregion

            #region 資料遺漏在大盤刪除
            for (int i = 0; i < TechData.Count; i++)
            {


            }
            #endregion

            Initial_AveragePrice(5,  12, 20, 60,120);
            Initial_AverageVolume(5, 10, 20, 40);
            Initial_MaxClosePrice(5, 10, 20,60,120);
            Initial_MinClosePrice(5, 10, 20, 60,240);
            Initial_MaxVolume(5, 10, 20, 40);
            Initial_MinVolume(5, 10, 20, 40);
            Initial_Acculation(3,5, 10, 20, 40);
        }
        
        void Initial_AveragePrice(params int [] AverageDays )//均價
        {
            double sum = 0;
            

            for (int k = 0; k < AverageDays.Length; k++)
            {
                for (int i = AverageDays[k]; i < TechData.Count; i++)
                {
                    if (TechData[i].AverageValue == null)
                    {
                        TechData[i].AverageValue = new double[AverageDays.Length];
                    }

                    for (int j = 0; j < AverageDays[k]; j++)
                    {
                        sum += TechData[i - j].ClosePrice;
                    }

                    TechData[i].AverageValue[k] = sum / AverageDays[k];
                    sum = 0;
                }
            }
        }
        void Initial_AverageVolume(params int[] AverageDays)//均量
        {
            double sum = 0;


            for (int k = 0; k < AverageDays.Length; k++)
            {
                for (int i = AverageDays[k]; i < TechData.Count; i++)
                {
                    if (TechData[i].AverageVolume == null)
                    {
                        TechData[i].AverageVolume = new double[AverageDays.Length];
                    }

                    for (int j = 0; j < AverageDays[k]; j++)
                    {
                        sum += TechData[i - j].volume;
                    }

                    TechData[i].AverageVolume[k] = sum / AverageDays[k];
                    sum = 0;
                }
            }
        }
        void Initial_MaxClosePrice(params int[] MaxDays)//最高價
        {
            double Max = 0;


            for (int k = 0; k < MaxDays.Length; k++)
            {
                for (int i = MaxDays[k]; i < TechData.Count; i++)
                {
                    if (TechData[i].MaxPrice== null)
                    {
                        TechData[i].MaxPrice = new double[MaxDays.Length];
                    }

                    for (int j = 0; j < MaxDays[k]; j++)
                    {
                        if (TechData[i - j].ClosePrice > Max)
                            Max = TechData[i - j].ClosePrice;
                    }

                    TechData[i].MaxPrice[k] = Max;
                    Max = 0;
                }
            }
        }
        void Initial_MinClosePrice(params int[] MinDays)//最低價
        {
            double Min = 10000;


            for (int k = 0; k < MinDays.Length; k++)
            {
                for (int i = MinDays[k]; i < TechData.Count; i++)
                {
                    if (TechData[i].MinPrice == null)
                    {
                        TechData[i].MinPrice = new double[MinDays.Length];
                    }

                    for (int j = 0; j < MinDays[k]; j++)
                    {
                        if (TechData[i - j].ClosePrice < Min)
                            Min = TechData[i - j].ClosePrice;
                    }

                    TechData[i].MinPrice[k] = Min;
                    Min = 10000;
                }
            }
        }
        void Initial_MaxVolume(params int[] MaxDays)//最大量
        {
            double Max = 0;


            for (int k = 0; k < MaxDays.Length; k++)
            {
                for (int i = MaxDays[k]; i < TechData.Count; i++)
                {
                    if (TechData[i].MaxVolume == null)
                    {
                        TechData[i].MaxVolume = new double[MaxDays.Length];
                    }

                    for (int j = 0; j < MaxDays[k]; j++)
                    {
                        if (TechData[i - j].volume > Max)
                            Max = TechData[i - j].volume;
                    }

                    TechData[i].MaxVolume[k] = Max;
                    Max = 0;
                }
            }
        }
        void Initial_MinVolume(params int[] MinDays)//最低價
        {
            double Min = 10000;


            for (int k = 0; k < MinDays.Length; k++)
            {
                for (int i = MinDays[k]; i < TechData.Count; i++)
                {
                    if (TechData[i].MinVolune == null)
                    {
                        TechData[i].MinVolune = new double[MinDays.Length];
                    }

                    for (int j = 0; j < MinDays[k]; j++)
                    {
                        if (TechData[i - j].volume < Min)
                            Min = TechData[i - j].volume;
                    }

                    TechData[i].MinVolune[k] = Min;
                    Min = 10000;
                }
            }
        }

        void Initial_Acculation(params int[] AcculationDays)
        {
            double Acc = 0;
            for (int k = 0; k < AcculationDays.Length; k++)
            {
                for (int i = AcculationDays[k]; i < TechData.Count; i++)
                {
                    if (TechData[i].Acculation == null)
                    {
                        TechData[i].Acculation = new double[AcculationDays.Length];
                    }

                    for (int j = 0; j < AcculationDays[k]; j++)
                    {
                        Acc += TechData[i - j].ReturnOnInvestment;
                    }

                    TechData[i].Acculation[k] = Acc;
                    Acc = 0;
                }
            }
        }
    }
}
