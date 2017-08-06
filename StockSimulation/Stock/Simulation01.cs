using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL_InsertAndAdd;

namespace StockSimulation.Stock
{
    class Simulation01
    {
        double FirstDayRatio = 0;//看第一天漲跌多少%
        double NextDayRatio = 0;//看之後幾天漲跌多少%
        double DownFirstRatio = 0;

        List<String[]> ClosePrice = new List<string[]>();
        List<String[]> RewardRatio = new List<string[]>();
        List<String[]> Volumn = new List<string[]>();

        string startDate="2009-09-30";
        string endDate = "2013-10-05";
        string company="3576";

        StockPossess sp = new StockPossess();
        GetDataFromDatabase gd = new GetDataFromDatabase();

        public void RiseLimit()
        {
            bool canbuy = true;

            string command = @"SELECT [Time],[_2330]
             FROM [StockDatabase].[dbo].[technical_analysis]
  
                where [MyIndex]=201 and [Time] between '2012-01-01' and '2013-09-30' order by [Time]" ;

            gd.GetData(command, ref ClosePrice);

            command = @"SELECT [Time],[_2330]
                        FROM [StockDatabase].[dbo].[technical_analysis]
  
                        where [MyIndex]=210  and [Time] between '2012-01-01' and '2013-09-30' order by [Time]";

            gd.GetData(command, ref RewardRatio );

            for(int i=0 ; i<ClosePrice.Count ; i++)
            {
                if (double.Parse(ClosePrice[i][1]) > 6  && canbuy)
                {
                    canbuy = false;
                    sp.BuyStock(3576,double.Parse(RewardRatio [i][1]),RewardRatio [i][0]);
                    Console.WriteLine(RewardRatio [i][1] + "    " + ClosePrice[i][1]);
                }

                if (!canbuy)
                {
                    Console.WriteLine(RewardRatio [i][1] + "    " + ClosePrice[i][1]);
                }

                if (double.Parse(ClosePrice[i][1]) < 1&&!canbuy)
                {
                    canbuy = true;
                    sp.SellStock(3576, double.Parse(RewardRatio [i][1]), RewardRatio [i][0]);
                    Console.WriteLine(RewardRatio [i][1] + "    " + ClosePrice[i][1]);
                }
            }

            sp.ProfitAndLoss();
            sp.clear();

        }

        public void standardSimulation()
        {
            statistics st = new statistics();
            string command;
            command = @"SELECT [Time],[_2330]
                        FROM [StockDatabase].[dbo].[technical_analysis]
  
                        where [MyIndex]=210  and [Time] between '2012-05-01' and '2013-06-24' order by [Time]";

            gd.GetData(command, ref st.closePrice);

            command = @"SELECT [Time],[_2330]
                        FROM [StockDatabase].[dbo].[technical_analysis]
  
                        where [MyIndex]=201  and [Time] between '2012-05-01' and '2013-06-24' order by [Time]";
            gd.GetData(command, ref st.HighPrice);

            st.average(3, ref st.averageShort);
            st.average(5,ref  st.averageLong);
            st.standard(3, ref st.standardShort, st.averageShort);
           // st.standard(3,ref st.standardLong, st.averageLong);

            int count_short = 0;
            int count_long = 0;
            int count_st = 0;
            bool CanBuy = false;
            bool CanSell = false;
            bool hasBuy = false;

            int count = 0;
            int count_red = 0;

            //sp.BuyStock(2330, double.Parse(st.closePrice[1][1]), st.closePrice[1][0]);
            //sp.SellStock(2330, double.Parse(st.closePrice[st.closePrice.Count - 1][1]), st.closePrice[st.closePrice.Count - 1][0]);

            for (int i = 1; i < st.closePrice.Count; i++)
            {
               
                double ClosePrice = double.Parse(st.closePrice[i][1]);
                double RewardRatio  = st.averageShort[i];
                double temp2 = st.standardShort[i];


                if (double.Parse(st.HighPrice[i][1]) > 0 && !CanSell)
                {
                    count++;

                }
                //else if (hasBuy)
                //{
                //    count++;
                //}
                //else
                //{
                //    count = 0;
                //}

                if (CanBuy)
                {
                    sp.BuyStock(2330, double.Parse(st.closePrice[i][1]), st.closePrice[i][0]);
                    count = 0;
                    count_red++;
                    CanBuy = false;
                    CanSell = true;
                    hasBuy = true;
                    continue;
                }

                if (count == 3)
                {
                    CanBuy = true;
                }

                //if (hasBuy && double.Parse(st.HighPrice[i][1]) < 0 && double.Parse(st.HighPrice[i][1]) > -0.5)
                //{
                //    sp.BuyStock(2330, double.Parse(st.closePrice[i][1]), st.closePrice[i][0]);
                //    hasBuy = false;
                //}

                if (CanSell && (double.Parse(st.HighPrice[i][1]) < 0 || double.Parse(st.HighPrice[i][1]) == 0))
                {
                    count_red++;
                }

                if (double.Parse(st.HighPrice[i][1]) > 0.5 || count_red == 5)
                {
                    sp.SellStock(2330, double.Parse(st.closePrice[i][1]), st.closePrice[i][0]);
                    sp.SellStock(2330, double.Parse(st.closePrice[i][1]), st.closePrice[i][0]);
                    sp.SellStock(2330, double.Parse(st.closePrice[i][1]), st.closePrice[i][0]);
                    sp.SellStock(2330, double.Parse(st.closePrice[i][1]), st.closePrice[i][0]);
                    hasBuy = false;
                    CanSell = false;
                    count = 0;
                    count_red = 0;
                }
                //if (st.averageShort[i] > st.averageShort[i-1] &&
                //    st.averageShort[i] - 1 * st.standardShort[i] < double.Parse(st.closePrice[i][1]) &&
                //    st.averageShort[i]<st.averageLong[i]&&
                //    st.averageShort[i] + 0.2 * st.standardShort[i] > double.Parse(st.closePrice[i][1]) && CanBuy)
                //{
                //    sp.BuyStock(2330, double.Parse(st.closePrice[i][1]), st.closePrice[i][0]);
                //    CanBuy = false;
                //}

                //if ((st.averageShort[i] + 3 * st.standardShort[i] < double.Parse(st.closePrice[i][1]) ||
                //    st.averageShort[i] - 1.2 * st.standardShort[i] > double.Parse(st.closePrice[i][1]))&&!CanBuy)
                //{
                //    sp.SellStock(2330, double.Parse(st.closePrice[i][1]), st.closePrice[i][0]);
                //    CanBuy = true;
                //}


                //if (st.averageLong[i] + 2.5*st.standardLong[i] < double.Parse(st.closePrice[i][1]) ||
                //    st.averageLong[i] - 2.5*st.standardLong[i] > double.Parse(st.closePrice[i][1]))
                //{
                //    count_long++;
                //}

                //if (st.averageLong[i] + 2.5 * st.standardLong[i] > st.averageShort[i] + 2.5 * st.standardShort[i] ||
                //    st.averageLong[i] - 2.5 * st.standardLong[i] < st.averageShort[i] - 2.5 * st.standardShort[i])
                //{
                //    count_st++;
                //}
            }
            sp.ProfitAndLoss();
            double temmmp=0 ;
            //temmmp = 100*count_short / (st.closePrice.Count-5);

            //Console.WriteLine(temmmp);
            //temmmp = 100 - 100 * count_long / (st.closePrice.Count - 3);
            //Console.WriteLine(temmmp);
            //temmmp = 100 - 100 *  count_st / (st.closePrice.Count - 20);
            //Console.WriteLine(temmmp);
        }

        public void UpdatePossibilityAboutRoseaDay()
        {
            List<string> company=new List<string>();
            InsertText IT = new InsertText();
            SQL_Action sq = new SQL_Action("server=USER-PC\\SQLEXPRESS;database=ProssibilityDatabase;Integrated Security=SSPI");
            string command = @"SELECT company  FROM [StockDatabase].[dbo].[standard_analysis] group by company order by company";
            gd.GetData(command , ref company);
            int[] temp = null;


            //for (int j = 0; j < 7; j++)
            //{
            int j = 6;
                for (int i = 0; i < company.Count; i++)
                {
                    this.company = company[i].Trim();
                    this.FirstDayRatio = j;
                    this.DownFirstRatio = j + 1;
                    //this.NextDayRatio = k;
                    StockDownDay(ref temp);

                    command = IT.RisePossibility(company[i], temp, this.startDate, this.endDate, j, this.DownFirstRatio, false);

                    sq.AddData("[ProssibilityDatabase].[dbo].[PossibilityAboutRoseAndDrop]", command);
                }
                Console.WriteLine("加入成功 " );
            //}
            
        }
        public void UpdateAvgMaxMin()
        {
            SQL_Action sq = new SQL_Action("server=USER-PC\\SQLEXPRESS;database=ProssibilityDatabase;Integrated Security=SSPI");
            List<string> company=new List<string>();
            string command = @"SELECT company  FROM [StockDatabase].[dbo].[standard_analysis] group by company order by company";
            gd.GetData(command , ref company);
            int[] days = { 5, 10, 20, 60 };

            for (int j = 2; j < 7; j++)
            {
                for (int i = 0; i < company.Count; i++)
                {
                    this.company = company[i];
                    GetData();


                    int index = int.Parse(company[i].Trim()) * days[1]*(-j)*3-j*51;
                    command = index + "," + company[i].Trim() + ",0," + j;

                    for (int k = 0; k < days.Length; k++)
                    {

                        command += "," + AccumulateDrop((-1)*j, days[k]);

                    }
                    sq.AddData("[ProssibilityDatabase].[dbo].[AvgExpectation]", command);
                }
            }
            
           
        }

        public void CalculatePossibility()//看上漲天數
        {
            GetData();

            int[] plus_count = new int[10];
            int[] minus_count = new int[10];

            int count_plus = 0;
            int count_minus = 0;

            for (int i = 0; i < RewardRatio.Count; i++)
            {

                if (double.Parse(RewardRatio[i][1]) > 0 )
                {
                    if (count_minus > 0 && count_minus < 10)
                    {
                        minus_count[count_minus-1]++;
                    }
                    else if (count_minus > 9)
                    {
                        minus_count[9]++;
                    }
                    count_plus++;
                    count_minus = 0;
                }

                if (double.Parse(RewardRatio[i][1]) < 0)
                {
                    if (count_plus > 0 && count_plus < 10)
                    {
                        plus_count[count_plus-1]++;
                    }
                    else if(count_plus>9)
                    {
                        plus_count[9]++;
                    }

                    count_plus=0;
                    count_minus++;
                }

                if (double.Parse(RewardRatio[i][1]) == 0)
                {
                    if (count_plus > 0)
                        count_plus ++;
                    else if (count_minus > 0)
                        count_minus ++;
                }

            }

            int count = 0;

            double plus = 0;
            double minus = 0;

            for (int i = 0; i < plus_count.Length; i++)
            {
                plus += plus_count[i];
                minus += plus_count[i];
            }

            for (int i = 0; i < plus_count.Length; i++)
            {
                double temp_plus = plus_count[i] / plus;
                double temp_minus = minus_count[i] / minus;
                count += (i+1)*plus_count[i] +(i+1)* minus_count[i];
                Console.WriteLine("上漲"+(i + 1) + "天機率:" + temp_plus);
                Console.WriteLine("下跌"+(i + 1) + "天機率:" + temp_minus);
                Console.WriteLine();
            }

            Console.WriteLine(count);
        }

        void StockUpDay(ref int[] plus_count)//上漲天數 分別計算上漲?%之後上漲情形 和上漲?%之後下跌不到?%的情形
        {
            GetData();

            plus_count = new int[10];

            int count_plus = 0;

            bool startToCount = false;

            double AccumulationRise = 0;
            double AccumulationDrop = 0;

            for (int i = 0; i < RewardRatio.Count; i++)
            {
                if (double.Parse(RewardRatio[i][1]) > FirstDayRatio)
                    startToCount = true;

                if (startToCount && (double.Parse(RewardRatio[i][1]) > NextDayRatio || double.Parse(RewardRatio[i][1]) == NextDayRatio)
                    && (AccumulationRise * 0.66 > AccumulationDrop || (AccumulationRise * 0.66 == AccumulationDrop)))
                {
                    if (double.Parse(RewardRatio[i][1]) > 0)
                    {
                        AccumulationRise += double.Parse(RewardRatio[i][1]);
                    }
                    else
                    {
                        AccumulationDrop += double.Parse(RewardRatio[i][1]);
                    }

                    count_plus++;
                }
                else if (startToCount && double.Parse(RewardRatio[i][1]) < NextDayRatio && count_plus<10)
                {
                    plus_count[count_plus-1]++;
                    startToCount = false;
                    count_plus = 0;
                    AccumulationRise = 0;
                    AccumulationDrop = 0;
                }
                else if (startToCount && double.Parse(RewardRatio[i][1]) < NextDayRatio)
                {
                    plus_count[plus_count.Length - 1]++;
                    startToCount = false;
                    count_plus = 0;
                    AccumulationRise = 0;
                    AccumulationDrop = 0;
                }
            }

            double sum_plus=0;

            for (int i = 0; i < plus_count.Length; i++)
            {
                sum_plus += plus_count[i];
            }

            double AllPossibilty = 0;

            for (int i = 0; i < plus_count.Length; i++)
            {
                double possibilty = plus_count[i]/sum_plus;
                AllPossibilty += possibilty;
                //Console.WriteLine((i+1)+"天:"+possibilty);
                //Console.WriteLine();
            }

            //Console.WriteLine(AllPossibilty);
        }
        void StockDownDay(ref int[] minus_count)//下跌天數 分別計算下跌?%之後下跌情形 和下跌?%之後上漲不到?%的情形
        {
            GetData();

            minus_count = new int[10];

            int count_minus = 0;

            bool startToCount = false;

            

            for (int i = 0; i < RewardRatio.Count; i++)
            {
                if (double.Parse(RewardRatio[i][1]) < (-1) * FirstDayRatio && double.Parse(RewardRatio[i][1]) > (-1) * DownFirstRatio)
                    startToCount = true;

                if (startToCount && (double.Parse(RewardRatio[i][1]) < (-1)*NextDayRatio || double.Parse(RewardRatio[i][1]) ==(-1)* NextDayRatio))
                   
                {
                  

                    count_minus++;
                }
                else if (startToCount && double.Parse(RewardRatio[i][1]) > (-1) * NextDayRatio && count_minus < 10)
                {
                    minus_count[count_minus - 1]++;
                    startToCount = false;
                    count_minus = 0;
                    
                }
                else if (startToCount && double.Parse(RewardRatio[i][1]) > (-1) * NextDayRatio)
                {
                    minus_count[minus_count.Length - 1]++;
                    startToCount = false;
                    count_minus = 0;
                 
                }
            }

            double sum_minus = 0;

            for (int i = 0; i < minus_count.Length; i++)
            {
                sum_minus += minus_count[i];
            }

            double AllPossibilty = 0;

            for (int i = 0; i < minus_count.Length; i++)
            {
                double possibilty = minus_count[i] / sum_minus;
                AllPossibilty += possibilty;
                //Console.WriteLine((i+1)+"天:"+possibilty);
                //Console.WriteLine();
            }
        }

        string AccumulateDrop(double standardNumber , int countDay)//回傳AVG MAX MIN
        {
            GetData();

            double Max = 0, Min = 100000, average = 0;

            int MaxI = 0, minI = 0;

            bool IsCount = false;
            int count =0;
            double temp = 0;
            string Data;

            List<double>  Accumulate = new List<double>();
            List<string[]> dateData = new List<string[]>();

            for (int i = 0; i < RewardRatio.Count; i++)
            {
                if (double.Parse(RewardRatio[i][1]) < standardNumber && standardNumber <0)
                {
                    IsCount = true;
                    dateData.Add(RewardRatio[i]);
                }

                if (double.Parse(RewardRatio[i][1]) > standardNumber && standardNumber > 0)
                {
                    IsCount = true;
                    dateData.Add(RewardRatio[i]);
                }

                for (int j = 0; j < countDay && IsCount; j++, i++)
                {
                    if (i == RewardRatio.Count - 1)
                        break;
                    temp+=double.Parse(RewardRatio[i][1]);
                }

                if(temp!=0)
                 Accumulate.Add(temp);

                IsCount = false;
                temp = 0;
                
            }

            for (int i = 0; i < Accumulate.Count; i++)
            {
                if (Accumulate[i] > Max)
                {
                    Max = Accumulate[i];
                    MaxI = i;
                }

                if (Accumulate[i] < Min)
                {
                    Min = Accumulate[i];
                    minI = i;
                }

                temp += Accumulate[i];

            }

            if (Accumulate.Count == 0)
            {
                return 0 + "," + 0 + ",'"+"1000-01-01"+"',"
                + 0 + ",'" + "1000-01-01" + "'";

     

            }
            average = temp / Accumulate.Count;

            Data = average + "," + Max + ",'"+dateData[MaxI][0].Split(' ')[0].Replace("/","-")+"',"
                + Min + ",'" + dateData[minI][0].Split(' ')[0].Replace("/", "-") + "'";

            return Data;
        }

       


        public void CalculateValue()//看上漲%數
        {
            GetData();

            double[] plus_count = new double[10];
            double[] minus_count = new double[10];
            int stand = 1;

            double count_plus = 0;
            double count_minus = 0;

            for (int i = 0; i < RewardRatio.Count; i++)
            {
                if (double.Parse(RewardRatio[i][1]) > 0)
                {
                    if (count_minus / stand < 0 && count_minus / stand > -10)
                    {
                        minus_count[(-1)*(int)(count_minus/stand)]++;
                    }
                    else if (count_minus / stand < -10)
                    {
                        minus_count[9]++;
                    }
                    count_plus+=double.Parse(RewardRatio[i][1]);
                    count_minus = 0;
                }

                if (double.Parse(RewardRatio[i][1]) < 0)
                {
                    if (count_plus / stand > 0 && 10 > count_plus / stand)
                    {
                        plus_count[(int)(count_plus / stand)]++;
                    }
                    else if (count_plus / stand > 10)
                    {
                        plus_count[9]++;
                    }

                    count_plus = 0;
                    count_minus+=double.Parse(RewardRatio[i][1]);
                }

                if (double.Parse(RewardRatio[i][1]) == 0)
                {
                    if (count_plus > 0)
                        count_plus+=double.Parse(RewardRatio[i][1]);
                    else if (count_minus > 0)
                        count_minus+=double.Parse(RewardRatio[i][1]);
                }
            }

            int count = 0;

            double plus = 0;
            double minus = 0;

            for (int i = 0; i < plus_count.Length; i++)
            {
                plus += plus_count[i];
                minus += plus_count[i];
            }

            for (int i = 0; i < plus_count.Length; i++)
            {
                double plus_t = plus_count[i] / plus ;
                double minus_t = minus_count[i]/minus;
                //count += (i + 1) * plus_count[i] + (i + 1) * minus_count[i];
                Console.WriteLine("上漲" + stand * (i + 1) + "%:" + plus_t);
                Console.WriteLine("下跌" + stand * (i + 1) + "%:" + minus_t);
                Console.WriteLine();
            }

            Console.WriteLine(count);
        }

        public void PriceVolumnRelation()//價縮量漲 價漲量縮
        {
            GetData();
            double standartVolumn = 0.9;
            double couutVolumnDownPriceUp=0;
            double countVolumnUpPriceDown=0;

            int[] countVolumnDownDown = new int[5];//價漲量縮 後勢看跌
            int VolumnDownDown = 0;
            int[] countVolumnUpUp = new int[5];//價縮量漲 後勢看漲
            int VolumnUpUp = 0;
            bool CountVolumnUpUp = false;
            bool CountVolumnDownDown = false;
            

            for (int i = 1; i < RewardRatio.Count; i++)
            {
                double pastVolumnUp = (double.Parse(Volumn[i - 1][1]) * standartVolumn);
                double pastVolumnDown =  (double.Parse(Volumn[i - 1][1]) / standartVolumn);

                if (double.Parse(RewardRatio[i][1]) > 0 &&  pastVolumnUp  > (double.Parse(Volumn[i][1]))&&
                    double.Parse(RewardRatio[i-1][1]) >0&& !CountVolumnDownDown)
                {
                    couutVolumnDownPriceUp++;
                    CountVolumnDownDown = true;
                    CountVolumnUpUp = false;
                }
                else if (double.Parse(RewardRatio[i][1]) < 0 && pastVolumnDown < (double.Parse(Volumn[i][1]))&& 
                   double.Parse(RewardRatio[i-1][1])<0 && !CountVolumnUpUp)
                {
                    countVolumnUpPriceDown++;
                    CountVolumnUpUp = true;
                    CountVolumnDownDown = false;
                }

                #region 計算價跌量漲 後勢幾天價漲
                if (CountVolumnUpUp && (double.Parse(RewardRatio[i][1])<0 ||double.Parse(RewardRatio[i][1])==0))
                {
                    VolumnUpUp++;
                }
                else if ( VolumnUpUp != 0 && VolumnUpUp<5)
                {
                    countVolumnUpUp[VolumnUpUp-1]++;
                    CountVolumnUpUp = false;
                    VolumnUpUp = 0;
                }
                else if (VolumnUpUp > 4)
                {
                    countVolumnUpUp[4]++;
                    CountVolumnUpUp = false;
                    VolumnUpUp = 0;
                }
                #endregion
                #region 計算價漲量跌 後勢幾天價跌
                if (CountVolumnDownDown && (double.Parse(RewardRatio[i][1]) > 0 || double.Parse(RewardRatio[i][1]) == 0))
                {
                    VolumnDownDown++;
                }
                else if (VolumnDownDown != 0 && VolumnDownDown < 5)
                {
                    countVolumnDownDown[VolumnDownDown - 1]++;
                    CountVolumnDownDown = false;
                    VolumnDownDown = 0;
                }
                else if (VolumnDownDown > 4)
                {
                    countVolumnDownDown[4]++;
                    CountVolumnDownDown = false;
                    VolumnDownDown = 0;
                }
                #endregion

            }

            for (int i = 0; i < countVolumnDownDown.Length; i++)
            {
                double countDownDown = countVolumnDownDown[i]/couutVolumnDownPriceUp;
                double countUpUp = countVolumnUpUp[i] / countVolumnUpPriceDown;

                Console.WriteLine("價漲量縮"+(i+1)+"天後勢看跌的機率:" + countDownDown);
                Console.WriteLine("價縮量漲"+(i+1)+"天後勢看漲的機率:" + countUpUp);
                Console.WriteLine();
            }

        }

     


        void GetData()
        {
            string command;
            command = @"SELECT [Time],[_"+company+@"]
                        FROM [StockDatabase].[dbo].[technical_analysis]
  
                        where [MyIndex]=209  and [Time] between '"+startDate+"' and '"+endDate+ "' order by [Time]";

            gd.GetData(command, ref ClosePrice);

            command = @"SELECT [Time],[_" + company + @"]
                        FROM [StockDatabase].[dbo].[technical_analysis]
  
                        where [MyIndex]=201  and [Time] between '" + startDate + "' and '" + endDate + "' order by [Time]";
            gd.GetData(command, ref  RewardRatio);

            command = @"SELECT [Time],[_" + company + @"]
                        FROM [StockDatabase].[dbo].[technical_analysis]
  
                        where [MyIndex]=208  and [Time] between '" + startDate + "' and '" + endDate + "' order by [Time]";

            gd.GetData(command, ref  Volumn);
        }

       
    }
}
