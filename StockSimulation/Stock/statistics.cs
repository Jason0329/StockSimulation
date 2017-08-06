using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSimulation.Stock
{
    class statistics
    {
        public List<string[]> closePrice=new List<string[]>();
        public List<string[]> HighPrice=new List<string[]>();
        public List<string[]> LowPrcie=new List<string[]>();

        public double[] lowest_Price;
        public double[] highest_Price;

        public double[] averageShort;
        public double[] averageLong;
        public double[] standardShort;
        public double[] standardLong;

        public void initial()
        {

        }

        public void average(int count,ref double [] average )
        {
            average = new double[closePrice.Count];
            for (int j = count; j < closePrice.Count; j++)
            {
                for (int i = j-count; i < j; i++)
                {
                    average[j] += double.Parse(closePrice[i][1]);
                }
                average[j] /= count;
            }
            
        }

        public void standard(int count,ref double[] standard , double [] averagePrice)
        {
            standard = new double[closePrice.Count];
            double[] square = new double[closePrice.Count];

            for (int i = 0; i < closePrice.Count; i++)
            {
                square[i] = Math.Pow(double.Parse(closePrice[i][1]),2);
            }

            double[] tempAverage = new double[closePrice.Count];
            double temp;

            for (int i = count; i < closePrice.Count; i++)
            {
                temp = 0;

                for (int j = i - count; j < i; j++)
                {
                    temp += square[j];
                }

                tempAverage[i] = temp/count;
            }

            for (int i = 0; i < closePrice.Count; i++)
            {
                standard[i] = Math.Sqrt(tempAverage[i] -Math.Pow(averagePrice[i],2));
            }
        }

        

    }
}
