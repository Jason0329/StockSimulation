using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSimulation
{
    static class SimulationResult
    {
        static public double win=0;
        static public double loss = 0;
        static public double Revenue=0;
        static public double winStandard=0;
       
      


        public static  void reStart()
        {
            win = 0;
            loss = 0;
            Revenue = 0;
        }


        public static double WinPossibility()
        {
            if (win + loss != 0)
            {
                double temp = win/(win+loss) ;
                
                return temp;
            }
            return 0;
        }

        public static  double GetAllRevenue()
        {
            return Revenue;
        }
    }
}
