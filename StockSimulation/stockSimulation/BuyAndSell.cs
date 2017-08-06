using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSimulation
{
    class BuyAndSell
    {
        int stock = 0;
        double revenue = 0, preRevenue = 0;
        double Buy_stockPrice=0;//平均成本

        public void BuyStock(double Price , int stockNumber=1)
        {
            double CurrentlyStockPrice = stock * Buy_stockPrice;//目前持有股票市值
            stock += stockNumber;
            Buy_stockPrice = (CurrentlyStockPrice + stockNumber * Price) / stock;
        }

        public void SellStock(double Price , int stockNumber=1)
        {

            if (stock < stockNumber)
            {
                Console.WriteLine("no stock can be sell");
                return;
            }

            if (Price - Buy_stockPrice > SimulationResult.winStandard)
            {
                SimulationResult.win++;
            }
            else
            {
                SimulationResult.loss++;
            }


            stock -= stockNumber;

            revenue += stockNumber * (Price - Buy_stockPrice) * 1000;           
        }

        void pre_sellStock(double price)
        {
            preRevenue = (price - Buy_stockPrice) * 1000 * stock + revenue;
        }

        
    }
}
