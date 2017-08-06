using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL_InsertAndAdd;
using StockSimulation.Stock;

namespace StockSimulation.Future
{
    class FuturePossibility:FutureBuyAndSell
    {
        GetDataFromDatabase gd = new GetDataFromDatabase();
        InsertText IT = new InsertText();
       
        public List<string[]> data = new List<string[]>();
       

        public string startDate = "2012-01-30";
        public string endDate = "2013-10-14";

        int[][] temp1;
        double rank = 30;



        public void UpdateFutruePoss()
        {
            CalPoss();

            for (int i = 3; i < 6; i++)
            {
                IT.FuturePoss(temp1, startDate, endDate, 0, 10*i);
            }
        }
        public void CalPoss()
        {
            getData();

            temp1 = new int[3][];

            for (int i = 0; i < 3; i++)
            {
                temp1[i] = new int[10];
            }
            

            int count_plus = 0;

            bool startToCount = false;

            double difference = 0;
            int dif = 0;

            for (int i = 1; i < data.Count; i++)
            {
                

                if (double.Parse(data[i][2]) < 0)
                {
                    double temp11 = (double.Parse(data[i][1]));
                    double temp22=(double.Parse(data[i-1][1]));
                    startToCount = true;
                    difference =temp11-temp22; 

                    dif = (int)(difference/rank)*(-1);

                    if (dif > 2)
                        dif = 2;
                }


                if (startToCount && double.Parse(data[i][2]) < 0 || double.Parse(data[i][2]) == 0)
                {

                    //if (double.Parse(data[i][2]) > 0)
                    //{
                    //    AccumulationRise += double.Parse(data[i][2]);
                    //}
                    //else
                    //{
                    //    AccumulationDrop += double.Parse(minus);
                    //}

                    count_plus++;
                }
                else if (startToCount && double.Parse(data[i][2]) > 0 && count_plus < 10)
                {
                    temp1[dif][count_plus - 1]++;
                    startToCount = false;
                    count_plus = 0;
                    
                }
                else if (startToCount && double.Parse(data[i][2]) > 0)
                {
                    temp1[dif][temp1.Length - 1]++;
                    startToCount = false;
                    count_plus = 0;
                    
                }
            }

            double sum_plus = 0;

            //for (int i = 0; i < temp1.Length; i++)
            //{
            //    sum_plus += temp1[i];
            //}

            double AllPossibilty = 0;

            for (int i = 0; i < temp1.Length; i++)
            {
                double possibilty = 0;
                AllPossibilty += possibilty;
                Console.WriteLine((i+1)+"天:"+possibilty);
                //Console.WriteLine();
            }
        }

        public void CountRiseAndDrop()//計算漲跌天數
        {
            getData();
            int[] countRise = new int[10];
            int[] countDrop = new int[10];
            int AllDrop = 0;
            int AllRise = 0;

            int countDayRise = 0 , countDayDrop=0;

            for (int i = 0; i < data.Count; i++)
            {
                if (double.Parse(data[i][2]) > 0)
                {
                    if (countDayDrop != 0)
                    {
                        if (countDayDrop < 10)
                            countDrop[countDayDrop-1]++;
                        else
                            countDrop[9]++;
                        countDayDrop = 0;
                    }
                    countDayRise++;                   
                }

                if (double.Parse(data[i][2]) < 0)
                {
                    if (countDayRise != 0)
                    {
                        if (countDayRise < 10)
                            countRise[countDayRise-1]++;
                        else
                            countRise[9]++;
                        countDayRise = 0;
                    }
                    countDayDrop++;
                }

                if (double.Parse(data[i][2]) == 0)
                {
                    if (countDayDrop > 0)
                        countDayDrop++;
                    else
                        countDayRise++;
                }
            }


            for (int i = 0; i < countRise.Length; i++)
            {
                Console.WriteLine((i + 1) + " up:" + countRise[i]);
                AllRise += (i + 1) * countRise[i];
                Console.WriteLine((i + 1) + " down:" + countDrop[i]);
                AllDrop += (i+1) *countDrop[i];
            }
            Console.WriteLine(AllRise + " " + AllDrop);
        }
        public void TestCountBuyAndSell()//三天跌買進 隔兩天賣出
        {
            getData();

            int countStandard = 3;
            int countDrop = 0;
            int countRise=0;
            bool HasBuy = false;

            double DPrice;//當下看到的價格

            for (int i = 1; i < data.Count; i++)
            {
                DPrice = RandomPrice(double.Parse(data[i][4]),double.Parse(data[i][5]));

                if (double.Parse(data[i][2]) < 0 || double.Parse(data[i][2]) == 0)
                {
                    countDrop++;
                }
                else
                {
                    countDrop = 0;
                }

            

                if (HasBuy && double.Parse(data[i][2]) < 0)
                {
                    countDrop = 0;
                    sellFuture(DPrice);
                    sellFutureForDetail(DPrice, data[i][0]);
                    countRise = 0;
                    HasBuy = false;
                }

                if (HasBuy && ((double.Parse(data[i][2]) > 0 || double.Parse(data[i][2]) == 0)))
                {
                    countDrop = 0;
                    countRise++;
                }

                if (double.Parse(data[i - 1][1]) - double.Parse(data[i][1]) > 100)
                {
                    countDrop--;
                }

                

                if (countDrop == countStandard && (double.Parse(data[i][1])!=double.Parse(data[i][4])))
                {
                    buyFuture(DPrice);
                    buyFutureForDetail(DPrice, data[i][0]);
                    HasBuy = true;
                   
                }


               
              

                if (HasBuy && countRise == 2)
                {
                    sellFuture(DPrice);
                    sellFutureForDetail(DPrice, data[i][0]);
                    HasBuy = false;
                    countRise = 0;
                }




            }

            ReturnDetail();

            Returns();
        }
        public void TestGetRise()//追漲
        {
            getData();
            bool HasBuy = false;

            double DPrice;//當下看到的價格
            

            for (int i = 1; i < data.Count; i++)
            {

                DPrice = RandomPrice(double.Parse(data[i][1]), double.Parse(data[i][5]),true);

                if (!HasBuy && (double.Parse(data[i][1]) - double.Parse(data[i - 1][1])) > 90 && (double.Parse(data[i][4]) - double.Parse(data[i][1]) > 10))
                {
                    buyFuture(DPrice);
                    buyFutureForDetail(DPrice, data[i][0]);
                    HasBuy = true;
                }

                if (HasBuy && ((double.Parse(data[i][1]) - double.Parse(data[i - 1][1])) < -30))
                {
                    DPrice = RandomPrice(double.Parse(data[i][1]), double.Parse(data[i][5]), true);
                    sellFuture(DPrice);
                    sellFutureForDetail(DPrice, data[i][0]);
                    HasBuy = false;
                }

                //if (hasBuy && ((double.Parse(data[i][3]) - double.Parse(data[i - 1][3])) < -30))
                //{
                //    sellFuture(double.Parse(data[i][3]));
                //    sellFutureForDetail(double.Parse(data[i][3]), data[i][0]);
                //    hasBuy = false;
                //}
            }

            ReturnDetail();

            Returns();
        }


        double RandomPrice(double highPriceOrClosePrice, double lowPrice , bool IsClosePrice = false)
        {
            Random rnd = new Random();
            double rndNumber = rnd.NextDouble();
            if (!IsClosePrice)
                return lowPrice + rndNumber * (highPriceOrClosePrice - lowPrice);
            else
                return highPriceOrClosePrice;
        }
        
        public void getData()
        {
            string command;
            command = @"SELECT [date],[ClosePrice],[Reward],[OpenPrice],[MaxPrice],[MinPrice]
                        FROM [StockDatabase].[dbo].[FutureDayDate] where [date] between '" +startDate +"' and '"+endDate+"' order by [date]";
            gd.GetData(command,ref data);

           

        }
    }
}
