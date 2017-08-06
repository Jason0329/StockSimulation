using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StockSimulation.Stock
{
    class MyStock
    {
        public int StockNumber;
        public int share;
        public double BuyPrice;
        public string buyDate;

        public MyStock(int stockNumber,  double Price,string buyDate,int share = 1000)
        {
            this.StockNumber = stockNumber;
            this.share = share;
            this.BuyPrice = Price;
            this.buyDate = buyDate;
        }

    }

    class Settlement
    {
        public int stockNumber;
        public double revenue;
        public int share;
        public DateTime buyDate;
        public DateTime sellDate;
        public double buyPrice;
        public double sellPrice;
        public double EarningRatio;
        


        public Settlement(int stockNumber ,double revenue, int share ,string buyDate , string sellDate )
        {
            this.stockNumber = stockNumber;
            this.revenue = revenue;
            this.share = share;
            this.buyDate = DateTime.Parse(buyDate);
            this.sellDate = DateTime.Parse(sellDate);
            
        }

        public Settlement(int stockNumber, double buyPrice , double sellPrice, int share, string buyDate, string sellDate)
        {
            this.stockNumber = stockNumber;
            this.revenue = (sellPrice - buyPrice);
            this.share = share;
            this.buyDate = DateTime.Parse(buyDate);
            this.sellDate = DateTime.Parse(sellDate);
            this.buyPrice = buyPrice;
            this.sellPrice = sellPrice;
            this.EarningRatio = (sellPrice - buyPrice) / buyPrice;
        }

        public void Profit()
        {
            Console.WriteLine("股票代號:" + stockNumber);
            Console.WriteLine("損益:" + revenue * share);
            Console.WriteLine("買入時間:" + buyDate.ToLongDateString());
            Console.WriteLine("賣出時間:" + sellDate.ToLongDateString());
            Console.WriteLine();
     
        }

        public double getRevenue()
        {
           
            return revenue*share;
        }
        
    }

    class StockPossess
    {
        StreamWriter sw = new StreamWriter(@"C:\Users\user\Desktop\股票\股票venture\平均線 結果\黃金交叉死亡交叉.txt");
        List<MyStock> mystock = new List<MyStock>();
        public List<Settlement> settlement = new List<Settlement>();
        public double win;
        public double loss;

        public void BuyStock(int stockNumber, double Price, string buyDate,int share = 1000)
        {
            //Console.WriteLine("日期:" + buyDate + " 買價:" + Price);
            MyStock buy = new MyStock(stockNumber, Price, buyDate, share);
            mystock.Add(buy);
        }

        public void SellStock(int stockNumber, double Price,string sellDate, int share = 1000)
        {
            for(int i=0 ; i<mystock.Count ; i++)
            {
                if (stockNumber == mystock[i].StockNumber)
                {
                    //Console.WriteLine("日期:" + sellDate + " 賣價:" + Price);
                    settlement.Add(new Settlement(stockNumber, mystock[i].BuyPrice,Price, share, mystock[i].buyDate, sellDate));
                    mystock.Remove(mystock[i]);
                    break;
                }
            }
        }

        public double ProfitAndLoss()
        {
            double AllRevenue = 0;
            StreamWriter sw = new StreamWriter(@"C:\Users\user\Desktop\股票\股票venture\平均線 結果\"+settlement[0].stockNumber+".csv");
            foreach (Settlement ss in settlement)
            {
                if (ss.getRevenue() > 0)
                {
                    win++;
                }
                else if (ss.getRevenue() < 0)
                {
                    loss++;
                }
                    
                AllRevenue += ss.getRevenue();
                ss.Profit();
                sw.WriteLine(ss.stockNumber+","+ss.buyDate +","+ss.buyPrice+","+ss.sellDate+","+ss.sellPrice+","+ss.revenue*ss.share);
            }

           

            sw.Close();


            Console.WriteLine("總損益:" + AllRevenue);

            return AllRevenue;

        }

        public double ProfitAndLoss(ref double WinPossibility)
        {
            double AllRevenue = 0;
            win = 0;
            loss = 0;
            foreach (Settlement ss in settlement)
            {
                if (ss.getRevenue() > 0)
                {
                    win++;
                }
                else if (ss.getRevenue() < 0)
                {
                    loss++;
                }

                AllRevenue += ss.getRevenue();
                ss.Profit();
            }

            WinPossibility = win / (win + loss);


            Console.WriteLine("總損益:" + AllRevenue);

            return AllRevenue;

        }

        public double ProfitAndLoss(StreamWriter sw) //輸出到CSV 格式已固定
        {
            double AllRevenue = 0;
            win = 0;
            loss = 0;
            double WinPossibility;

            if (settlement.Count != 0)
                sw.Write(settlement[0].stockNumber);

            for (int i = 0; i < 20; i++)
            {
                if (settlement.Count == 0)
                    return 0;

                //if (i % 20 == 0 && i/20>=1)
                //    sw.WriteLine();

                if (settlement.Count > i)
                {
                    AllRevenue += settlement[i].getRevenue();

                    if (settlement[i].getRevenue() > 0)
                        win++;
                    else
                        loss++;

                    sw.Write(","+settlement[i].buyDate + "," + settlement[i].buyPrice
                        + "," + settlement[i].sellDate + "," + settlement[i].sellPrice
                        + "," + (settlement[i].sellDate.Subtract(settlement[i].buyDate).Days) + 
                        "," + settlement[i].EarningRatio
                        +","+ (365*settlement[i].EarningRatio/ (settlement[i].sellDate.Subtract(settlement[i].buyDate).Days)));

                    continue;
                }

                sw.Write(",,,,,");

            }

            WinPossibility = win / (win + loss);

            sw.Write("," + WinPossibility+","+AllRevenue);
            sw.WriteLine();
            clear();
            return 1;
        }

        public void clear(bool sss = false)
        {
            if (sss)
            {
                foreach (Settlement ss in settlement)
                {
                    ss.Profit();
                }
            }

            settlement.Clear();

        }
        
    }
}
