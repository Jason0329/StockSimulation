using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL_InsertAndAdd;

namespace StockSimulation.Stock
{
    class InsertText
    {
        public string RisePossibility(string company,int [] numberOfDay , string StartTime,
            string EndTime, double StartPercent, double NextPercent, bool IsRise = true)
        {
            string[] Now = DateTime.Now.ToString("u").Trim('z').Trim('Z').Split(' '); ;
            

            string command;
            double index;

            double[] Possibility = new double[numberOfDay.Length];
            double All_poss=0;

            Possibility[numberOfDay.Length - 1] = numberOfDay[numberOfDay.Length - 1];


            for (int i = numberOfDay.Length-1; i >0; i--)
            {
                Possibility[i - 1] = Possibility[i] + numberOfDay[i - 1];
            }

            for (int i = numberOfDay.Length; i >0; i--)
            {
                Possibility[i-1] = 1-( Possibility[i-1] / Possibility[0]);
            }

            index = int.Parse(company) * double.Parse(StartTime.Split('-')[1]) * double.Parse(EndTime.Split('-')[2]) + 
                9*StartPercent -52*NextPercent +double.Parse(StartTime.Split('-')[1])  + Possibility[1];

            command = index + "," + company.Trim() + ",'" + Now[0] + "','" + StartTime + "','" + EndTime + "'," +"0"
                + "," +  ((-1*StartPercent).ToString()) + "," + (-1*NextPercent);

            for (int i = 0; i < numberOfDay.Length; i++)
            {
                command += "," + numberOfDay[i];
            }

            for (int i = 0; i < numberOfDay.Length; i++)
            {
                command += "," + Possibility[i];
            }




            

            
           
            return command;
        }

        public string FuturePoss(int[][] data, string StartTime,string EndTime , int IsRise , int dif)
        {
            SQL_Action sq = new SQL_Action("server=USER-PC\\SQLEXPRESS;database=ProssibilityDatabase;Integrated Security=SSPI");


            string[] Now = DateTime.Now.ToString("u").Trim('z').Trim('Z').Split(' ');

            int Index =int.Parse( StartTime.Split('-')[2])+int.Parse(EndTime.Split('-')[2])*(int.Parse(Now[0].Split('-')[2])+5)+dif;

           

            for (int i = 0; i < 3; i++)
            {
                string command = Index + ",'" + StartTime + "','" + EndTime + "','" + Now[0] + "',"
                       + IsRise + "," + dif;

                command += "," + (i + 1);
                double AllData=0;
                double Poss = 1;

                for (int j = 0; j < 10; j++)
                {
                    command += "," + data[i][j];
                    AllData += data[i][j];
                }

                command += ",0";

                for (int j = 1; j < 10; j++)
                {
                    Poss -= (data[i][j-1] / AllData);

                    double RisPoss = 1 - Poss;

                    command += "," + RisPoss;
                }

                sq.AddData(@"[ProssibilityDatabase].[dbo].[PossibilityOfFuture]",command);
            }

            return "";
        }
    }
}
