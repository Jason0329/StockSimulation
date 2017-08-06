using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSimulation
{
    class SimulateFutureByAverage:FutureBuyAndSell
    {
        double[] allData;//模擬時需要的資料
        double[] shortDaysAVG;//平均資料
       
        double[] longDaysAVG;
        double[] slope;//斜率
        double[] longDaysSlope; 
        bool Normalized ;//是否規一化
        double BuySlopeStandard;
        double SellSlopeStandard;
        double angle;
        int shortAVG;
        int longAVG;
        bool IsBuy = false;
        bool CanBuy = false;
        bool PreBuy = false;

        public void TakeData(  List<MydataResult> dataResult)
        {
            allData = new double[dataResult.Count];

            for (int i = 0; i < dataResult.Count; i++)
            {
                allData[i] = double.Parse(dataResult[i].Tick);
            }

           
        }


        public SimulateFutureByAverage(bool normalized = false, double buyslopeStandard =3, double sellslopeStandard = 0, int shortAVG = 20 , int longAVG=60)
        {
            this.Normalized = normalized;
            this.BuySlopeStandard = buyslopeStandard;
            this.SellSlopeStandard = sellslopeStandard;
            this.shortAVG =shortAVG;
            this.longAVG = longAVG;
            TakeData(SQL_GetData.dataResult);
            FutureData();
            FutureSlope();
            Simulate();
        }

        void FutureData()
        {

            double temp=0;
            int dataLength = allData.Length -shortAVG + 1;
            shortDaysAVG = new double[dataLength];
            longDaysAVG = new double[allData.Length - longAVG + 1];
 
            
            for (int i = 0; i < dataLength; i++)
            {
                temp = 0;
                for (int j = 0; j <shortAVG; j++)
                {
                    temp += allData[i + j];
                }

                shortDaysAVG[i] = temp /shortAVG;
            }
            for (int i = 0; i < allData.Length - longAVG + 1; i++)
            {
                temp = 0;
                for (int j = 0; j <longAVG; j++)
                {
                    temp += allData[i + j];
                }

                longDaysAVG[i] = temp /longAVG;
            }

        }
        void FutureSlope()
        {
            slope = new double[shortDaysAVG.Length-1];
            longDaysSlope = new double[longDaysAVG.Length - 1];

            for (int i = 0; i < shortDaysAVG.Length - 1; i++)
            {
                if (Normalized == false)
                {
                    slope[i] = shortDaysAVG[i + 1] - shortDaysAVG[i];
                }
                else
                {
                    slope[i] = (shortDaysAVG[i + 1] - shortDaysAVG[i])*7000/shortDaysAVG[0];//以7000點為基準
                }
            }

            for (int i = 0; i < longDaysAVG.Length - 1; i++)
            {
                if (Normalized == false)
                {
                    longDaysSlope[i] = longDaysAVG[i + 1] - longDaysAVG[i];
                }
                else
                {
                    longDaysSlope[i] = (longDaysAVG[i + 1] - longDaysAVG[i]) * 7000 / longDaysAVG[0];//以7000點為基準
                }
            }
        }

        int tempi;
        bool BuySignal(int i)//買進訊號
        {
           // double angle = Calculate(i);
            if (allData[i] < shortDaysAVG[i - shortAVG] && allData[i] < longDaysAVG[i - longAVG] && IsBuy==false)
            {
                CanBuy = true;
            }

            //if ((longDaysAVG[i - longAVG] < shortDaysAVG[i - shortAVG] &&
            //    slope[i - shortAVG] > longDaysSlope[i - longAVG] && IsBuy == false &&
            //   CanBuy == true&&longDaysSlope[i - longAVG] > 0)&&PreBuy==false)
            //{
            //    PreBuy = true;
            //    tempi = i;
            //}

            if (longDaysSlope[i-longAVG] > 0.15 && slope[i-shortAVG]>0.1 && CanBuy == true && IsBuy==false)             
            {
               
               // PreBuy = false;
                IsBuy = true;
                CanBuy = false;
                return true;
            }
            //else if (i == tempi + 5)
            //{
            //    CanBuy = true;
            //    IsBuy = false;
            //    PreBuy = false;
            //    return false;
            //}
            else
            {
                return false;
            }
             
        }

        bool SellSignal(int i)//賣出訊號
        {
           // double angle = Calculate(i);
            if ((longDaysSlope[i-longAVG]<-0.1 ||allData[i]<shortDaysAVG[i-shortAVG])&&IsBuy==true)
            {
                Console.WriteLine(allData[i]);
                IsBuy = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        double Calculate(int i)
        {
            double result;
            result= slope[i+1]-slope[i];
            if (result == 0) return 0;
            double radians = Math.Atan(result);
            angle = radians * (180 / Math.PI);
            return angle;
        }

        void Simulate()
        {
            for (int i = longAVG; i < longDaysAVG.Length-300; i++)
            {
                if (BuySignal(i))
                {
                    
                    buyFuture(allData[i+5]);
                }
                else   if (SellSignal(i))
                {
                    sellFuture(allData[i+5]);
                }
            }
        }
    }
}
