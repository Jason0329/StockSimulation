using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSimulation
{
    class FutureBuyAndSell
    {
        int HasBuyFutures;//手中買進幾口期貨
        int HasSellFutures;//賣出幾口期貨
        int Transactions ;//交易次數(手續費用)
        double FutureAveragePrice ;
        double TotalRevenue ;//所賺取點數
        List<double>TimesRevenue;//每次賺取點數

        List<string[]> buyDetail = new List<string[]>();
        List<string[]> sellDetail = new List<string[]>();

        public FutureBuyAndSell()
        {
            HasBuyFutures=0;
            HasSellFutures=0;
            Transactions = 0;
            FutureAveragePrice = 0;
            TotalRevenue = 0;
            TimesRevenue = new List<double>();
        }

        public void Returns()
        {
            

            //foreach (double dd in TimesRevenue)
            //{
            //    Console.WriteLine("每次營收:" + dd);
            //}

            Console.WriteLine("總營收:" + TotalRevenue);
            Console.WriteLine("交易次數:" + Transactions);
        }

        protected void buyFuture(double Price , int buySheet=1)
        {
            Transactions++;


            if (HasSellFutures ==0 )
            {
                
                FutureAveragePrice = (Price * buySheet + FutureAveragePrice * HasBuyFutures) / (buySheet + HasBuyFutures);
                HasBuyFutures += buySheet;
            }
            else if (HasSellFutures>0 && buySheet > HasSellFutures)
            {
                GetRevenue(Price, HasSellFutures);
             
                HasBuyFutures += buySheet - HasSellFutures;

                HasSellFutures = 0;

                FutureAveragePrice = Price;

            }
            else if (HasSellFutures > 0 && buySheet < HasSellFutures)
            {
                GetRevenue(Price, buySheet);

                HasSellFutures -= buySheet;
            }
            else if (HasSellFutures > 0 && buySheet == HasSellFutures)
            {
                GetRevenue(Price, buySheet);

                HasSellFutures -= buySheet;

                FutureAveragePrice = 0;
            }
        }

        protected void sellFuture(double Price, int sellSheet = 1)
        {
            Transactions++;


            if (HasBuyFutures == 0)
            {
                FutureAveragePrice = (Price * sellSheet + FutureAveragePrice * HasBuyFutures) / (sellSheet + HasBuyFutures);
                HasSellFutures += sellSheet;
            }
            else if (HasBuyFutures > 0 && sellSheet > HasBuyFutures)
            {

                GetRevenue(Price, HasBuyFutures);

                HasSellFutures += sellSheet - HasBuyFutures;

                HasBuyFutures = 0;

                FutureAveragePrice = Price;

            }
            else if (HasBuyFutures > 0 && sellSheet < HasBuyFutures)
            {
                GetRevenue(Price, sellSheet);

                HasBuyFutures -= sellSheet;
            }
            else if (HasBuyFutures > 0 && sellSheet == HasBuyFutures)
            {
               

                GetRevenue(Price, sellSheet);

                HasBuyFutures -= sellSheet;

                FutureAveragePrice = 0;
            }

        }

        protected void Cover(double Price)//平倉
        {
            Transactions++;
            FutureAveragePrice = 0;

            if (HasSellFutures != 0 && HasBuyFutures!=0)
            {
                Console.WriteLine("error");
            }
            else if (HasBuyFutures != 0)
            {
                GetRevenue(Price, HasBuyFutures);
                HasBuyFutures = 0;
            }
            else if (HasSellFutures != 0)
            {
                GetRevenue(Price, HasSellFutures);
                HasSellFutures = 0;
            }


        }

        protected void buyFutureForDetail(double Price, string BuyDate, int buySheet = 1)//紀錄詳細購買細節 目前只買
        {
            string[] temp = new string[2];
            temp[0] = BuyDate;
            temp[1] = Price.ToString();

            buyDetail.Add(temp);
        }

        protected void sellFutureForDetail(double Price, string sellDate, int sellSheet = 1)//紀錄詳細賣出細節 目前只賣
        {
            string[] temp = new string[2];
            temp[0] = sellDate;
            temp[1] = Price.ToString();

            sellDetail.Add(temp);
        }

        protected void ReturnDetail()
        {
            for (int i = 0; i < sellDetail.Count; i++)
            {
                Console.WriteLine("買入:"+buyDetail[i][0] +" "+buyDetail[i][1]);
                Console.WriteLine("賣出:" + sellDetail[i][0] + " " + sellDetail[i][1]);

                Console.WriteLine("\n獲利:" + (double.Parse(sellDetail[i][1]) - double.Parse(buyDetail[i][1]))+"\n");
            }
            Console.WriteLine("\n");
        }

        void GetRevenue(double Price , int FuturePiece)
        {
          
            double renven = (Price - FutureAveragePrice) * FuturePiece;

            TimesRevenue.Add(renven);

            TotalRevenue += renven;

        }
    }
}
