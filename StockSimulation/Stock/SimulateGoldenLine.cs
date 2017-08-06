using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL_InsertAndAdd;
using System.IO;

namespace StockSimulation.Stock
{
    class AverageLine
    {
        public string stockNumber;
        public List<string[]> PriceAndDate = new List<string[]>();
        public int shortTermDays;//短周期天數
        public int longTermDays;//長週期天數
        public double[] ShortTermLine;
        public double[] LongTermLine;
        public double[] _60daysLine;
        public double[] _120daysLine;

        public double[] ShortTermSlope;


        public void Initial(int shortTerm, int longTerm, List<string[]> priceanddate,string stockNumber="2330")
        {
            this.stockNumber = stockNumber;
            this.PriceAndDate = priceanddate;
            ShortTermLine = new double[PriceAndDate.Count];
            LongTermLine = new double[PriceAndDate.Count];
            _60daysLine = new double[PriceAndDate.Count];
            _120daysLine = new double[PriceAndDate.Count];

            Average(_60daysLine, 60);
            Average(_120daysLine, 120);
            Average(ShortTermLine, shortTerm);
            Average(LongTermLine, longTerm);

            Slope(ref ShortTermSlope, ShortTermLine);

        }

        void Average(double [] average , int AverageDays)
        {
            for (int i = AverageDays; i < PriceAndDate.Count; i++)
            {
                double temp = 0;
                for (int j = i; j >i-AverageDays; j--)
                {
                    temp += double.Parse(PriceAndDate[j][1]);
                }

                average[i] = temp / AverageDays;
            }
        }

        void Slope(ref double [] slope , double [] average)
        {
            slope = new double[average.Length];

            for (int i = 1; i < slope.Length; i++)
            {
                slope[i] = average[i] - average[i - 1];
            }

        }

    }
    class KD_MACD_LINE
    {
        public List<string[]> closePrice;
        public List<string[]> HighPrice;
        public List<string[]> LowPrcie;
        public double []RSV;
        public double[] K_Line;
        public double[] D_Line;
        public double[] lowest_Price;
        public double[] highest_Price;
        public double[] DI;
        public double[] LongTermEMA;
        public double[] ShortTermEMA;
        public double[] DIF;//離差值
        public double[] MACD;

        int n_days = 3;

        public double standard_KD_parameter = 0.6667;

        public KD_MACD_LINE()
        {
            closePrice = new List<string[]>();
            HighPrice = new List<string[]>();
            LowPrcie = new List<string[]>();

        }

        public void Initial_HighLowPrice(int n_days=9)
        {
            double lowest=0;
            double highest=0;

           

            highest_Price = new double[closePrice.Count];
            lowest_Price = new double[closePrice.Count];

            for (int i = 0; i < n_days; i++)
            {
                highest_Price[i] = 6;
                lowest_Price[i] = 5;
            }

            for (int j = n_days-1; j < HighPrice.Count; j++)
            {
                lowest = 1000000;
                highest = 0;
                for (int i = j-n_days+1; i < j+1; i++)
                {
                    if(double.Parse(HighPrice[i][1])>highest)
                    {
                        highest = double.Parse(HighPrice[i][1]);
                    }

                    if(double.Parse(LowPrcie[i][1])<lowest)
                    {
                        lowest = double.Parse(LowPrcie[i][1]);
                    }
                }

                highest_Price[j] = highest;
                lowest_Price[j] = lowest;
            }
        }
        #region KD Line
        void Initial_RSV()
        {
            RSV = new double[closePrice.Count];

            for (int i = n_days; i < RSV.Length; i++)
            {
                RSV[i] = (double.Parse(closePrice[i][1]) - lowest_Price[i]) / (highest_Price[i] - lowest_Price[i]) * 100;
            }
        }
        public void KD_Line()
        {
            Initial_RSV();
            K_Line = new double[closePrice.Count];
            D_Line = new double[closePrice.Count];

            K_Line[n_days] = 50;
            D_Line[n_days] = 50;

            for (int i = n_days+1; i < K_Line.Length; i++)
            {
                K_Line[i] = K_Line[i - 1] * standard_KD_parameter + RSV[i] * (1 - standard_KD_parameter);
                D_Line[i] = D_Line[i - 1] * standard_KD_parameter + K_Line[i] * (1 - standard_KD_parameter);
            }
        }
        #endregion
        #region MACD
        void Initial_DI()
        {
            DI = new double[closePrice.Count];

            for (int i = 0; i < DI.Length; i++)
            {
                DI[i] = (double.Parse(HighPrice[i][1]) + double.Parse(LowPrcie[i][1]) +
                    2 * double.Parse(closePrice[i][1])) / 4;
            }
        }
        public void Initial_MACD(int shortDays=12 , int longDays=26 , int MACDDays = 9)
        {
            Initial_DI();
            ShortTermEMA = new double[closePrice.Count];
            LongTermEMA = new double[closePrice.Count];

            double averageShortEMA=0;
            double averageLongEMA=0;

            for(int i =0 ; i<shortDays ; i++)
            {
                averageShortEMA+=DI[i];
            }

            averageShortEMA=averageShortEMA/shortDays;//first day EMA for short
            ShortTermEMA[0]=averageShortEMA;

            for (int i = 0; i < longDays; i++)
            {
                averageLongEMA += DI[i];
            }

            averageLongEMA = averageLongEMA / longDays;//first day EMA for long
            LongTermEMA[0] = averageLongEMA;//initial EMA

            for(int i=1 ; i<ShortTermEMA.Length ; i++)
            {
                ShortTermEMA[i] = (ShortTermEMA[i - 1] * (shortDays - 2) + DI[i] * 2) / shortDays;
            }

            for (int i = 1; i < LongTermEMA.Length; i++)
            {
                LongTermEMA[i] = (LongTermEMA[i - 1] * (longDays - 2) + DI[i] * 2) / longDays;
            }

            DIF = new double[closePrice.Count];

            for (int i = 0; i < closePrice.Count; i++)
            {
                DIF[i] = ShortTermEMA[i] - LongTermEMA[i];
            }

            MACD = new double [ closePrice.Count];

            for (int i = 0; i < MACDDays; i++)
            {
                MACD[0] += DIF[i];
            }

            MACD[0] = MACD[0] / MACDDays;

            for (int i = 1; i < closePrice.Count; i++)
            {
                MACD[i] = (MACD[i - 1] * (MACDDays - 1) + DIF[i] * 2) / (MACDDays + 1);
            }
        }
        #endregion

    }
    class SimulateGoldenLine
    {
        List<String[]> temp = new List<string[]>();
        
        StockPossess sp = new StockPossess();
        GetDataFromDatabase gd = new GetDataFromDatabase();
        string command = @"SELECT [Time],[_2062] FROM [StockDatabase].[dbo].[technical_analysis] where [MyIndex]=210 and [Time] between '2012-11-01' and '2013-06-30' order by [Time]";
       

        public void StartToRun()
        {
            gd.GetData(command, ref temp);
            double []TheBest = new double [10];
            double tempp;
            int[] longday = new int[10]; 
            int[] shortday = new int[10];
            bool CanBuy = true;
            int number = 2062;

            //for (int k = 1;  k< 100; k++)
            //{
                //for (int j = k+1; j < 100; j++)
                //{
                    AverageLine _2330 = new AverageLine();
                    _2330.Initial(5, 20, temp);

                    for (int i = _2330.longTermDays + 1; i < temp.Count; i++)
                    {
                        if (_2330.LongTermLine[i - 1] > _2330.ShortTermLine[i - 1] && _2330.ShortTermLine[i] > _2330.LongTermLine[i]&&CanBuy)
                        {
                            CanBuy = false;
                            sp.BuyStock(number, double.Parse(temp[i][1]), temp[i][0]);
                           
                        }
                        //if ((_2330.ShortTermSlope[i - 1] < 0 && _2330.ShortTermSlope[i] > 0) && (_2330.ShortTermLine[i] < _2330.LongTermLine[i]) && CanBuy)
                        //{
                        //    CanBuy = false;
                        //    sp.BuyStock(2330, double.Parse(temp[i][1]), temp[i][0]);
                        //}

                        if ((_2330.LongTermLine[i - 1] < _2330.ShortTermLine[i - 1] && _2330.ShortTermLine[i] < _2330.LongTermLine[i])
                            ||( _2330.LongTermLine[i - 1] < _2330.ShortTermLine[i - 1] && _2330.ShortTermLine[i] == _2330.LongTermLine[i]
                            )
                            && !CanBuy)
                        {
                            CanBuy = true;
                            sp.SellStock(number, double.Parse(temp[i][1]), temp[i][0]);
                        }

                        //if ((_2330.ShortTermSlope[i - 1] > 0 && _2330.ShortTermSlope[i] < 0) && (_2330.ShortTermLine[i] > _2330.LongTermLine[i]) && !CanBuy)
                        //{
                        //    CanBuy = true;
                        //    sp.SellStock(2330, double.Parse(temp[i][1]), temp[i][0]);
                        //}


                    }

                    //for (int jj = 0; jj < _2330.ShortTermLine.Length; jj++)
                    //{
                    //    Console.WriteLine(_2330.PriceAndDate[jj][0]+"   "+  _2330.ShortTermLine[jj] + "       " + _2330.LongTermLine[jj]);
                    //}
                  
                    tempp = sp.ProfitAndLoss();
                   // sp.clear(true);
                   
                    //#region
                    //if (TheBest[0] < tempp)
                    //{
                    //    sp.clear(true);
                    //    TheBest[0] = tempp;
                    //    shortday[0] = k;
                    //    longday[0] = j;
                    //}
                    //else if (TheBest[1] < tempp)
                    //{
                    //    TheBest[1] = tempp;
                    //    shortday[1] = k;
                    //    longday[1] = j;
                    //}
                    //else if (TheBest[2] < tempp)
                    //{
                    //    TheBest[2] = tempp;
                    //    shortday[2] = k;
                    //    longday[2] = j;
                    //}
                    //else if (TheBest[3] < tempp)
                    //{
                    //    TheBest[3] = tempp;
                    //    shortday[3] = k;
                    //    longday[3] = j;
                    //}
                    //else if (TheBest[4] < tempp)
                    //{
                    //    TheBest[4] = tempp;
                    //    shortday[4] = k;
                    //    longday[4] = j;
                    //}
                    //else if (TheBest[5] < tempp)
                    //{
                    //    TheBest[5] = tempp;
                    //    shortday[5] = k;
                    //    longday[5] = j;
                    //}
                    //else if (TheBest[6] < tempp)
                    //{
                    //    TheBest[6] = tempp;
                    //    shortday[6] = k;
                    //    longday[6] = j;
                    //}
                    //else if (TheBest[7] < tempp)
                    //{
                    //    TheBest[7] = tempp;
                    //    shortday[7] = k;
                    //    longday[7] = j;
                    //}
                    //else if (TheBest[8] < tempp)
                    //{
                    //    TheBest[8] = tempp;
                    //    shortday[8] = k;
                    //    longday[8] = j;
                    //}
                    //else if (TheBest[9] < tempp)
                    //{
                    //    TheBest[9] = tempp;
                    //    shortday[9] = k;
                    //    longday[9] = j;
                    //}
                    //#endregion

                    sp.clear();
                //}
            //}

            Console.WriteLine(TheBest + "  " + shortday + " " + longday);

            //StreamWriter sw = new StreamWriter(@"C:\Users\user\Desktop\股票\股票venture\平均線 結果\黃金交叉死亡交叉.txt");

            //for (int i = 0; i < TheBest.Length; i++)
            //{
            //    sw.WriteLine(TheBest[i] + "  " + shortday[i] + "   " + longday[i]);
            //}
            //sw.Close();
        }
    }

    class SimulateKDLine
    {
        List<String[]> temp = new List<string[]>();

        

        StockPossess sp = new StockPossess();
        GetDataFromDatabase gd = new GetDataFromDatabase();
        KD_MACD_LINE _company = new KD_MACD_LINE();
        bool hasbuy = false;
        
        public void StartToRun(string company = "3008", string startdate="2012-01-01" , string enddate="2013-09-30")
        {
            string command = @"SELECT [Time],[_"+company+"] FROM [StockDatabase].[dbo].[technical_analysis] where [MyIndex]=209 and [Time] between '"+startdate+"' and '"+enddate +"' order by [Time]";

            gd.GetData(command, ref _company.closePrice);

            command = @"SELECT [Time],[_"+company+"] FROM [StockDatabase].[dbo].[technical_analysis] where [MyIndex]=213 and [Time] between '"+startdate+"' and '"+enddate +"' order by [Time]";

            gd.GetData(command, ref _company.HighPrice);

            command = @"SELECT [Time],[_" + company + "] FROM [StockDatabase].[dbo].[technical_analysis] where [MyIndex]=210 and [Time] between '" + startdate + "' and '" + enddate + "' order by [Time]";

            gd.GetData(command, ref _company.LowPrcie);

            _company.Initial_HighLowPrice();
            _company.KD_Line();
            _company.Initial_MACD();


            for (int i =  40; i < _company.closePrice.Count-1; i++)
            {
                double J = 3 * _company.D_Line[i] - 2 * _company.D_Line[i];

                if (_company.D_Line[i - 1] > _company.K_Line[i - 1] && _company.K_Line[i] > _company.D_Line[i]&& _company.K_Line[i-1]<20
                    &&!hasbuy )
                {
                    sp.BuyStock(int.Parse(company), double.Parse(_company.closePrice[i][1])
                        , _company.closePrice[i][0]);
                    hasbuy = true;
                }

                if ((_company.D_Line[i] > 80 || _company.K_Line[i] > 80 ||
                    (_company.D_Line[i - 1] < _company.K_Line[i - 1] && _company.K_Line[i] < _company.D_Line[i]&&
                    _company.K_Line[i] > 70) || (_company.D_Line[i - 1] < _company.K_Line[i - 1] && _company.K_Line[i] < _company.D_Line[i] &&
                    _company.K_Line[i] < 30)
                    )&&hasbuy )
                {
                    sp.SellStock(int.Parse(company), double.Parse(_company.closePrice[i][1])
                      , _company.closePrice[i][0]);
                    hasbuy = false;
                }
            }

            double tempp;
            tempp = sp.ProfitAndLoss();
                 
        }
    }

}
