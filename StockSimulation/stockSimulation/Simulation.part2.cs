using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSimulation.Stock
{
    partial class Simulation
    {
        public Simulation()
        {
            TechData = new SimulationForTechnologicalAnalysis();
            BasicFinancial = new SimulationForSelectBasicFinancialReport();
            Bargin = new SimulationForBargain();
            TechData.Initial("2013-01-01", "2013-05-01", 6206);
            BasicFinancial.Initial("1996-01-01", "2013-09-01", 6206);
            Bargin.Initial("2013-01-01", "2013-05-01", 6206);
        }

        #region 惟庭的策略
        void YieldBuy()
        {

            if (!uv.HasBuy
                && TechData.TechData[i].Yield >=5
                && IsBuy_Condition()
                && TechData.TechData[i].date.Year ==2014 && TechData.TechData[i].date.Month==4 &&TechData.TechData[i].date.Day>=11
                //&& TechData.TechData[i].ClosePrice== TechData.TechData[i].MinPrice[0]
                //&& TechData.TaiwanIndex[i].closePrice < 9000
                //&& TechData.TechData[i].ReturnOnInvestment<=-3
                )
                
            {

                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }

            if (uv.HasBuy&&
                (
                TechData.TechData[i].Yield <= 2.5
                
                ||
                (IsSell_Condition() && uv.Accumulation>10
                //&& (TechData.TechData[i].Yield<=5 || TechData.TechData[i-1].ReturnOnInvestment>=5) 

                )
                ||(IsSell_Condition() && uv.Accumulation <=-5)
                //|| uv.HaveStockDay>=200  //看看持有半年多的報酬
                ))
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
            }
        }

        bool IsSell_Condition()
        {
            BasicFinancial.InitialDate(TechData.TechData[i].date);

            int temp = BasicFinancial.RevenueInt;
            if ((BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth < 0
                && BasicFinancial.RevenueList[temp - 1].MonthRiseRatioInSameMonth < 0)
               ||(BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth <-15 || BasicFinancial.RevenueList[temp - 1].MonthRiseRatioInSameMonth<-15)
               )
            {
                return true;
            }

            return false;
        }
        bool IsBuy_Condition()
        {
            SumVariable Var5 = new SumVariable();
            SumVariable Var1 = new SumVariable();

            BasicFinancial.InitialDate(TechData.TechData[i].date);
            int temp = BasicFinancial.RevenueInt;
            int temp1 = BasicFinancial.BasicFinancialInt;
           // YearsSum(1, BasicFinancial.DataList[temp1].Date, ref Var1);//啟昇

            if (temp1 - 2 < 0)
                return false;

            if (
              
                BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth > -5
                && BasicFinancial.RevenueList[temp - 1].MonthRiseRatioInSameMonth > -5
                && BasicFinancial.RevenueList[temp - 2].MonthRiseRatioInSameMonth > -5

                &&BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth+
                     BasicFinancial.RevenueList[temp - 1].MonthRiseRatioInSameMonth
                     >20
                ///////////////啟昇

                 &&BasicFinancial.DataList[temp1].LongTermLiabilities / BasicFinancial.DataList[temp1].AllLiabilities < 0.5
                )
            {
                //return true;
                YearsSum(5, BasicFinancial.DataList[temp1].Date, ref Var5);
                if (
                    Var5.FCF > 0 &&
                    Var5.ROE / 5 >= 10//ROE 15改成10
                    && Var5.NetIncomeAfterTaxRatio / 5 >= 10 && Var5.CashFromOperation / Var5.NetIncomeAfterTax >= 0.5
                    )
                {
                    YearsSum(1, BasicFinancial.DataList[temp1].Date, ref Var1);
                    if (
                        Var1.FCF > 0 &&
                        Var1.ROE >= 10
                        && Var1.NetIncomeAfterTaxRatio >= 10
                        )
                        return true;
                }
            }

            return false;
        }    
        #endregion

        #region
        void MonthRevenueRise()//主要以月營收增加 預期之後股價成長
        {

            if (!uv.HasBuy
                && TechData.TechData[i].volume >= 500
                && TechData.TechData[i-1].volume >= 500
                && TechData.TechData[i].Acculation[0]>8
                //&& IsBuy_ConditionForRevenue()

                 && TechData.TechData[i].ReturnOnInvestment < 5
                 && TechData.TechData[i - 1].ReturnOnInvestment < 5
                 && TechData.TechData[i - 2].ReturnOnInvestment < 5
                //  && TechData.TechData[i].ReturnOnInvestment > 0
                //&& TechData.TechData[i - 1].ReturnOnInvestment > 0
                // && TechData.TechData[i - 2].ReturnOnInvestment > 0
                && TechData.TechData[i].volume > TechData.TechData[i-1].volume*0.6
                && TechData.TechData[i-1].volume > TechData.TechData[i - 2].volume * 0.6
                && TechData.TechData[i-2].volume > TechData.TechData[i - 3].volume * 0.6
                && TechData.TechData[i-3].volume > TechData.TechData[i - 4].MaxVolume[2]
                 //&& TechData.TechData[i - 3].ReturnOnInvestment < 5
                 //&& TechData.TechData[i - 4].ReturnOnInvestment < 5
                 //&& TechData.TechData[i].ClosePrice >10
               // && TechData.TechData[i].ClosePrice <
                
                //&& TechData.TechData[i].date.Year ==2014 && TechData.TechData[i].date.Month>=2 &&TechData.TechData[i].date.Day>=20
                )
            {

                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }

            if (uv.HasBuy &&
                (

               //TechData.TechData[i].AverageValue[0] > TechData.TechData[i].ClosePrice
                 uv.Accumulation < -10
                 || uv.Accumulation > 10
                //|| (TechData.TechData[i].volume * 2 < TechData.TechData[i].AverageVolume[1] && TechData.TechData[i].ReturnOnInvestment < 2 && TechData.TechData[i].AverageVolume[0] > TechData.TechData[i].AverageVolume[1])
                //||TechData.TechData[i].ReturnOnInvestment<-5
                //||
                //(IsSell_Condition() && uv.Accumulation > 10
                ////&& (TechData.TechData[i].Yield<=5 || TechData.TechData[i-1].ReturnOnInvestment>=5) 

                //)
                //|| (IsSell_Condition() && uv.Accumulation <= -5)
                //uv.HaveStockDay>=30  //看看持有半年多的報酬
                ))
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
            }
        }
        bool IsBuy_ConditionForRevenue()
        {
          
            SumVariable Var1 = new SumVariable();

            BasicFinancial.InitialDate(TechData.TechData[i].date);
            int temp = BasicFinancial.RevenueInt;
            int temp1 = BasicFinancial.BasicFinancialInt;

            if (temp1 - 2 < 0)
                return false;

            if (

               BasicFinancial.DataList[temp1].EPS > 0
               && BasicFinancial.DataList[temp1-1].EPS > 0
               && BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth >0
               && BasicFinancial.RevenueList[temp-1].MonthRiseRatioInSameMonth > 0

               &&   BasicFinancial.DataList[temp1].LongTermLiabilities / BasicFinancial.DataList[temp1].AllLiabilities < 0.5
                )
            {
                    
                   
                        return true;
            }

            return false;
        }    
        #endregion

        #region
        void DropBuy()
        {
            if (!uv.HasBuy
    && TechData.TechData[i].volume >= 500
    && TechData.TechData[i - 1].volume >= 500
                   && TechData.TechData[i-3].Acculation[0] > -20
                && TechData.TechData[i].ReturnOnInvestment >2
                 && TechData.TechData[i - 1].ReturnOnInvestment < 0
                  
                 && TechData.TechData[i - 2].ReturnOnInvestment < 1
                 && TechData.TechData[i - 3].ReturnOnInvestment < 1
                 && TechData.TechData[i - 4].ReturnOnInvestment < 1
    && TechData.TechData[i].ClosePrice<=50

    )
            {

                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }

            if (uv.HasBuy &&
                (
              uv.Accumulation>=10
              || uv.Accumulation<=-10
               //TechData.TechData[i].AverageValue[2] > TechData.TechData[i].ClosePrice
               // || TechData.TechData[i].ReturnOnInvestment < -5
               // || (TechData.TechData[i].volume * 2 < TechData.TechData[i].AverageVolume[2] && TechData.TechData[i].ReturnOnInvestment < 2 && TechData.TechData[i].AverageVolume[0] > TechData.TechData[i].AverageVolume[2])
                ))
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
            }
        }
        #endregion


        #region 昱嘉的策略
        void GetRise1()
        {
            if (!uv.HasBuy
               
                && TechData.TechData[i].Yield <= 5
                /////////////////////////昱嘉
                && (TechData.TechData[i - 1].AverageValue[0] > TechData.TechData[i - 1].AverageValue[3])
                && TechData.TechData[i - 2].MaxVolume[3] < TechData.TechData[i - 1].volume//這個條件在 TechData.TechData[i-1].MaxVolume[3] < TechData.TechData[i].volume 表現比較不好 但可使用在30 元以下選股
                && TechData.TechData[i - 1].AverageVolume[0] > 500
               
                && TechData.TechData[i].ReturnOnInvestment > 6
                //&&TechData.TechData[i].ReturnOnInvestment>-1
                //&& TechData.TechData[i].ReturnOnInvestment<6.7
                //&& TechData.TechData[i].ClosePrice <= 30
                ////&& TechData.TechData[i].volume > TechData.TechData[i - 1].MaxVolume[3]

                //////&& (TechData.TechData[i - 1].MaxPrice[0] < TechData.TechData[i ].ClosePrice)

                //&& TechData.TaiwanIndex[i].difLastClosePrice > -30
                && IsBuy_ConditionForGetRise()
                //&& Bargin.bargin[i - 1].stockloanUseRatio <= 10
                //&& TechData.TechData[i].date.Year == 2014 && TechData.TechData[i].date.Month >= 3 && TechData.TechData[i].date.Day >= 06
                )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }

            if (uv.HasBuy &&uv.Circumstance==0&&
                ///////////昱嘉
               (
               
               TechData.TechData[i].AverageValue[1] > TechData.TechData[i].ClosePrice
                || (TechData.TechData[i].volume * 2 < TechData.TechData[i].AverageVolume[2] && TechData.TechData[i].ReturnOnInvestment < 2 && TechData.TechData[i].AverageVolume[0] > TechData.TechData[i].AverageVolume[2])
                || (TechData.TechData[i].ReturnOnInvestment < -6)
                || uv.Accumulation < -7         
                ||(uv.HaveStockDay<5 && TechData.TechData[i].ReturnOnInvestment<2 &&TechData.TechData[i-1].ReturnOnInvestment<2)
                ))
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
            }

        
        }

        bool IsSell_ConditionForGetRise()
        {
            BasicFinancial.InitialDate(TechData.TechData[i].date);

            int temp = BasicFinancial.RevenueInt;
            if (BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth < 0
                && BasicFinancial.RevenueList[temp - 1].MonthRiseRatioInSameMonth < 0
                && BasicFinancial.RevenueList[temp - 2].MonthRevenueRiseRatio < 0
               )
            {
                return true;
            }

            return false;
        }
        bool IsBuy_ConditionForGetRise()
        {
            SumVariable Var5 = new SumVariable();
            SumVariable Var1 = new SumVariable();

            BasicFinancial.InitialDate(TechData.TechData[i].date);
            int temp = BasicFinancial.RevenueInt;
            int temp1 = BasicFinancial.BasicFinancialInt;

            if (temp1 >= BasicFinancial.DataList.Count)
                return false;

            YearsSum(1, BasicFinancial.DataList[temp1].Date, ref Var1);//啟昇

            if (temp1 - 2 < 0)
                return false;

            if (
                ((BasicFinancial.DataList[temp1].EPS - BasicFinancial.DataList[temp1 - 1].EPS > 0 && (BasicFinancial.DataList[temp1-1].EPS - BasicFinancial.DataList[temp1 - 2].EPS>0))
            ||
                (BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth > -10
               && BasicFinancial.RevenueList[temp - 1].MonthRiseRatioInSameMonth > -10
               )
               //||
               //(BasicFinancial.RevenueList[temp].MonthRevenueRiseRatio>30
               //&&BasicFinancial.RevenueList[temp-1].MonthRevenueRiseRatio>30)
                )
                &&BasicFinancial.DataList[temp1].LongTermLiabilities / BasicFinancial.DataList[temp1].AllLiabilities < 0.8
                )
            {
                return true;
            }

            return false;
        }

        #endregion
        #region 成交量下降 
        void VolumeDropBuy()
        {
            BasicFinancial.InitialDate(TechData.TechData[i].date);
            

            double temp;
            try
            {
                temp = BasicFinancial.DataList[BasicFinancial.BasicFinancialInt].Asset;
            }
            catch (Exception e)
            {
                return;
            }

            if (!uv.HasBuy
                //&&TechData.TechData[i].ClosePrice<=30
               //////////////////////////啟昇
                && TechData.TechData[i].AverageVolume[2]>500
                && TechData.TechData[i-1].AverageVolume[0] < TechData.TechData[i-1].AverageVolume[2]
                &&TechData.TechData[i-1].volume <TechData.TechData[i-1].AverageVolume[2]
                &&TechData.TechData[i].volume > TechData.TechData[i].AverageVolume[2]
                && TechData.TechData[i].ReturnOnInvestment >= 0 && TechData.TechData[i].ReturnOnInvestment <= 3
                && TechData.TechData[i - 1].ReturnOnInvestment >= 0 && TechData.TechData[i-1].ReturnOnInvestment <= 3
                 && TechData.TechData[i - 2].ReturnOnInvestment >= -2 
                && TechData.TechData[i].AverageVolume[0] < TechData.TechData[i].AverageVolume[2]
                &&(temp< 100000000 || TechData.TechData[i].ClosePrice<=20)
                //&& TechData.TechData[i].ClosePrice / TechData.TechData[i].MinPrice[3] <1.1
                //&& TechData.TechData[i].ClosePrice / TechData.TechData[i].MinPrice[3] >1
                && BuyCondition()
                //////////////////////////
               )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }

            if (uv.Accumulation > 10 && uv.HasBuy)
            {
                uv.PreSell = true;
            }

            if (uv.PreSell )
            {
                if(TechData.TechData[i].ReturnOnInvestment<0)
                    uv.startToCountDropAfterPreSell += TechData.TechData[i].ReturnOnInvestment;
            }

            if (uv.PreSell && uv.HasBuy      
                && ((TechData.TechData[i-1].ReturnOnInvestment<-1 && TechData.TechData[i].ReturnOnInvestment<-3)||
                TechData.TechData[i].ReturnOnInvestment < -5) 
                )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
                uv.PreSell = false;
                uv.startToCountDropAfterPreSell = 0;
                uv.HaveStockDay = 0;
                uv.Accumulation = 0;
            }

            if (uv.HasBuy &&
                /////////////啟昇
                (TechData.TechData[i].AverageVolume[0] > TechData.TechData[i].AverageVolume[2] &&
                 TechData.TechData[i].AverageVolume[2] > 2 * TechData.TechData[i].volume && TechData.TechData[i].ReturnOnInvestment <= -2)
                 || (uv.HaveStockDay >= 10 && uv.Accumulation <= 2)
                || uv.Accumulation<=-5
                ///////////////////////

                )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
                uv.PreSell = false;
                uv.startToCountDropAfterPreSell = 0;
                uv.HaveStockDay = 0;
                uv.Accumulation = 0;
            }
        }

        bool BuyCondition()
        {
            BasicFinancial.InitialDate(TechData.TechData[i].date);
            int tempp = BasicFinancial.RevenueInt;

            if (BasicFinancial.RevenueList[tempp].MonthRevenueRiseRatio > 20
                && BasicFinancial.RevenueList[tempp-1].MonthRevenueRiseRatio > 20
                &&(BasicFinancial.RevenueList[tempp].MonthRiseRatioInSameMonth>20 && BasicFinancial.RevenueList[tempp-1].MonthRiseRatioInSameMonth>20)
                || (BasicFinancial.RevenueList[tempp].MonthRevenueRiseRatio>50 && BasicFinancial.RevenueList[tempp].MonthRiseRatioInSameMonth>40))
            {
                return true;
            }
            return false;
        }
        #endregion
        #region 破底
        void LongDrop()
        {
            BasicFinancial.InitialDate(TechData.TechData[i].date);


            double temp;
            try
            {
                temp = BasicFinancial.DataList[BasicFinancial.BasicFinancialInt].Asset;
            }
            catch (Exception e)
            {
                return;
            }

            if (!uv.HasBuy
                //&&TechData.TechData[i].ClosePrice<=30
                //////////////////////////啟昇
                && TechData.TechData[i].AverageVolume[2] > 500
                &&TechData.TechData[i-2].ClosePrice < TechData.TechData[i-2].MinPrice[3] *1.05
                && TechData.TechData[i-1].ReturnOnInvestment>0 && TechData.TechData[i-1].ReturnOnInvestment<3
                && TechData.TechData[i].ReturnOnInvestment>0 &&TechData.TechData[i].ReturnOnInvestment<3
                &&TechData.TechData[i].volume > TechData.TechData[i].AverageVolume[0]
                && (temp < 100000000 || TechData.TechData[i].ClosePrice <= 20)
                && BuyConditionForLongDrop()
                //////////////////////////
               )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }

            if (!uv.PreSell &&uv.Accumulation > 10 && uv.HasBuy)
            {
                uv.PreSell = true;
            }

            if (uv.PreSell)
            {
                if (TechData.TechData[i].ReturnOnInvestment < 0)
                    uv.startToCountDropAfterPreSell += TechData.TechData[i].ReturnOnInvestment;
            }

            if (uv.PreSell && uv.HasBuy 
                && ((TechData.TechData[i - 1].ReturnOnInvestment < -1 && TechData.TechData[i].ReturnOnInvestment < -3) ||
                TechData.TechData[i].ReturnOnInvestment < -5
                || uv.startToCountDropAfterPreSell>10)
                )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
            }

            if (uv.HasBuy &&
                /////////////啟昇
               TechData.TechData[i].ReturnOnInvestment <= -5
                 || (uv.HaveStockDay >= 10 && uv.Accumulation <= 2)
                || uv.Accumulation <= -5
                ///////////////////////

                )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
            }
        }
        bool BuyConditionForLongDrop()
        {
            BasicFinancial.InitialDate(TechData.TechData[i].date);
            int tempp = BasicFinancial.RevenueInt;

            if (BasicFinancial.RevenueList[tempp].MonthRevenueRiseRatio > 20
                && BasicFinancial.RevenueList[tempp - 1].MonthRevenueRiseRatio > 20
                && (BasicFinancial.RevenueList[tempp].MonthRiseRatioInSameMonth > 20 && BasicFinancial.RevenueList[tempp - 1].MonthRiseRatioInSameMonth > 20)
                || (BasicFinancial.RevenueList[tempp].MonthRevenueRiseRatio > 50 && BasicFinancial.RevenueList[tempp].MonthRiseRatioInSameMonth > 40))
            {
                return true;
            }
            return false;
        }
        #endregion
        #region 爆量
        void VolumeRise()
        {
            BasicFinancial.InitialDate(TechData.TechData[i].date);


            double temp;
            try
            {
                temp = BasicFinancial.DataList[BasicFinancial.BasicFinancialInt].Asset;
            }
            catch (Exception e)
            {
                return;
            }

            if (!uv.HasBuy
                //&&TechData.TechData[i].ClosePrice<=30
                //////////////////////////啟昇
                && TechData.TechData[i].AverageVolume[2] > 500
                &&TechData.TechData[i].AverageVolume[0]*2 < TechData.TechData[i].volume
                && TechData.TechData[i].ReturnOnInvestment<3 && TechData.TechData[i].ReturnOnInvestment>-1
                && (temp < 100000000 || TechData.TechData[i].ClosePrice <= 20)
                && BuyConditionForVolumeRise()
                //////////////////////////
               )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }

            if (!uv.PreSell && uv.Accumulation > 10 && uv.HasBuy)
            {
                uv.PreSell = true;
            }

            if (uv.PreSell)
            {
                if (TechData.TechData[i].ReturnOnInvestment < 0)
                    uv.startToCountDropAfterPreSell += TechData.TechData[i].ReturnOnInvestment;
            }

            if (uv.PreSell && uv.HasBuy 
                && ((TechData.TechData[i - 1].ReturnOnInvestment < -1 && TechData.TechData[i].ReturnOnInvestment < -3) ||
                TechData.TechData[i].ReturnOnInvestment < -5 || uv.startToCountDropAfterPreSell < -10)
                )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
            }

            if (uv.HasBuy &&
                /////////////啟昇
               TechData.TechData[i].ReturnOnInvestment <= -5
                 || (uv.HaveStockDay >= 10 && uv.Accumulation <= 2)
                || uv.Accumulation <= -5
                ///////////////////////

                )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
            }
        }
        bool BuyConditionForVolumeRise()
        {
            BasicFinancial.InitialDate(TechData.TechData[i].date);
            int tempp = BasicFinancial.RevenueInt;

            if (BasicFinancial.RevenueList[tempp].MonthRevenueRiseRatio > 20
                && BasicFinancial.RevenueList[tempp - 1].MonthRevenueRiseRatio > 20
                && (BasicFinancial.RevenueList[tempp].MonthRiseRatioInSameMonth > 20 && BasicFinancial.RevenueList[tempp - 1].MonthRiseRatioInSameMonth > 20)
                || (BasicFinancial.RevenueList[tempp].MonthRevenueRiseRatio > 50 && BasicFinancial.RevenueList[tempp].MonthRiseRatioInSameMonth > 40))
            {
                return true;
            }
            return false;
        }
        #endregion
        #region 籌碼面1
        void _GetRise()//看籌碼面是否有增加
        {
            if (!uv.HasBuy
                && TechData.TechData[i - 1].AverageVolume[1] > 500
                //&& TechData.TechData[i - 2].ReturnOnInvestment >= 0
                 //&& TechData.TechData[i - 1].ReturnOnInvestment >=2
                //
                && Bargin.bargin[i-1].AcculateInvestmentTrustNetBuySell[0]>500
                &&_GetRise_IsBuyCondition()
                //((Bargin.bargin[i - 1].InvestmentTruthNetBuySell > 50 && Bargin.bargin[i - 2].InvestmentTruthNetBuySell > 50
                //&& Bargin.bargin[i - 3].InvestmentTruthNetBuySell > 50)
                //|| (Bargin.bargin[i - 1].InvestmentTruthNetBuySell > 5 * Bargin.bargin[i - 2].InvestmentTruthNetBuySell
                //&& Bargin.bargin[i - 2].InvestmentTruthNetBuySell > 50))
                && Bargin.bargin[i-1].MarginStockRatio<=5
                //&& (Bargin.bargin[i - 1].ForeignInvestorNetBuySell > 50 &&Bargin.bargin[i-1].DealerNetBuySell>50)
                //&& Bargin.barginReward[i-1].AllBuySellDay >=3
                //&& Bargin.barginReward[i-1].AllReward>=0
                //&& Bargin.bargin[i-1].InvestmentTruthNetBuySell > Bargin.bargin[i-1].AverageInvestmentTrustNetBuySell[2] *3
                //&& Bargin.bargin[i-1].InvestmentTruthNetBuySell > Bargin.bargin[i-1].AverageInvestmentTrustNetBuySell[0] *2)
                && TechData.TechData[i].Yield<=5.5
            )
            {

                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }


            if (uv.HasBuy &&
                (Bargin.barginReward[i-1].AllReward>100
                //|| Bargin.bargin[i - 1].MinInvestmentTrustNetBuySell[2] == Bargin.bargin[i - 1].ForeignInvestorNetBuySell
                //||Bargin.bargin[i - 1].MinInvestmentTrustNetBuySell[2] == Bargin.bargin[i - 1].InvestmentTruthNetBuySell
                //||Bargin.bargin[i - 1].MinDealerNetBuySell[2] == Bargin.bargin[i - 1].DealerNetBuySell
                //||(Bargin.bargin[i-1].AverageForeignInvestorNetBuySell[0] <0 && Bargin.bargin[i-1].ForeignInvestorNetBuySell<0)
                //|| TechData.TechData[i - 10].AverageVolume[1] > TechData.TechData[i].volume*2
                //|| uv.Accumulation < -7
                //|| TechData.TechData[i].ReturnOnInvestment<-2
                //|| (TechData.TechData[i].volume * 2 < TechData.TechData[i-10].AverageVolume[2] && TechData.TechData[i].ReturnOnInvestment < 2 && TechData.TechData[i].AverageVolume[0] > TechData.TechData[i].AverageVolume[2])
                //|| TechData.TechData[i].AverageValue[1] > TechData.TechData[i].ClosePrice
                || Bargin.bargin[i - 1].MarginStockRatio >= 15
                //|| (Bargin.bargin[i - 1].ForeignInvestorNetBuySell + Bargin.bargin[i - 1].InvestmentTruthNetBuySell
                //+ Bargin.bargin[i - 1].DealerNetBuySell < -100 && TechData.TechData[i].volume > TechData.TechData[i].AverageVolume[1] * 2 && TechData.TechData[i].ReturnOnInvestment < 2)
                ////|| Bargin.barginReward[i-1].AllReward>=30
                || (uv.Accumulation < -10)
                || TechData.TechData[i].ReturnOnInvestment < -4
                //|| (uv.HaveStockDay >25 && uv.Accumulation <10)
                //|| TechData.TechData[i].ReturnOnInvestment <= -4
                ))
            {
                
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
               
            }
        }

        bool _GetRise_IsBuyCondition()
        {
            SumVariable Var5 = new SumVariable();
            SumVariable Var1 = new SumVariable();

            BasicFinancial.InitialDate(TechData.TechData[i].date);
            int temp = BasicFinancial.RevenueInt;
            int temp1 = BasicFinancial.BasicFinancialInt;
            YearsSum(1, BasicFinancial.DataList[temp1].Date, ref Var1);
                    
                     


            if (  
                Bargin.bargin[i-1].ForeignInvestorNetBuySell + Bargin.bargin[i-1].InvestmentTruthNetBuySell
                + Bargin.bargin[i-1].DealerNetBuySell > 100
                &&
                Bargin.bargin[i - 2].ForeignInvestorNetBuySell + Bargin.bargin[i - 2].InvestmentTruthNetBuySell
                + Bargin.bargin[i - 2].DealerNetBuySell > 100)
            {
                //if (((Bargin.bargin[i - 1].ForeignInvestorNetBuySell > 50 && Bargin.bargin[i - 1].InvestmentTruthNetBuySell > 50)
                //    || (Bargin.bargin[i - 1].ForeignInvestorNetBuySell > 50 && Bargin.bargin[i - 1].DealerNetBuySell > 50)
                //     || (Bargin.bargin[i - 1].DealerNetBuySell > 50 && Bargin.bargin[i - 1].InvestmentTruthNetBuySell > 50))
                //    && Bargin.bargin[i - 1].MarginStockRatio <= 3
                //    )
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
        #region 抄底
        void MinPriceBuy()
        {
            if (!uv.HasBuy
                && TechData.TechData[i-1].MinPrice[4] == TechData.TechData[i-1].ClosePrice
               // //&& TechData.TechData[i].date.Month == 1 && TechData.TechData[i].date.Year == 2014
                && TechData.TechData[i].ReturnOnInvestment > 0
               // && TechData.TechData[i - 1].ReturnOnInvestment > 0
               // && MinPriceBuy_IsBuyCondition()
               // && TechData.TechData[i].ClosePrice < TechData.TechData[i].AverageValue[3]
               // //&&Bargin.bargin[i-1].ForeignInvestorNetBuySell + Bargin.bargin[i-1].InvestmentTruthNetBuySell
               // //+ Bargin.bargin[i-1].DealerNetBuySell > 100

               //&& TechData.TechData[i].Yield >= 2
               //&& TechData.TechData[i].Yield <= 8 
           )
            {

                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }
            

            if (uv.HasBuy 
               && 
               (
              //  ((TechData.TechData[i].AverageValue[2] < TechData.TechData[i].ClosePrice) && (TechData.TechData[i].ReturnOnInvestment > 3 || (TechData.TechData[i].ReturnOnInvestment > 2 && TechData.TechData[i - 1].ReturnOnInvestment > 2)))
              //  //||uv.HaveStockDay >1
              //  //|| uv.Accumulation > 1
              //  //||(uv.HaveStockDay>=15 && uv.Accumulation <0)
              uv.Accumulation>10  || uv.Accumulation < -10
              //  || (TechData.TechData[i].ReturnOnInvestment + TechData.TechData[i - 1].ReturnOnInvestment) <= -7
              // //|| (TechData.TechData[i].MinPrice[3] == TechData.TechData[i].ClosePrice)
              // //|| (TechData.TechData[i].volume > TechData.TechData[i].AverageVolume[1] * 2 && TechData.TechData[i].ReturnOnInvestment <= 0)
              // //|| TechData.TechData[i].ReturnOnInvestment < -4
              //||(uv.HaveStockDay==40 && uv.Accumulation<=10)
              // //|| (TechData.TechData[i].ReturnOnInvestment < 0 && TechData.TechData[i - 1].ReturnOnInvestment < 0 && TechData.TechData[i - 2].ReturnOnInvestment < 0)
               ))
            {

                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;

            }
        }

        bool MinPriceBuy_IsBuyCondition()
        {
            SumVariable Var5 = new SumVariable();
            SumVariable Var1 = new SumVariable();

            BasicFinancial.InitialDate(TechData.TechData[i].date);
            int temp = BasicFinancial.RevenueInt;
            int temp1 = BasicFinancial.BasicFinancialInt;
            YearsSum(1, BasicFinancial.DataList[temp1].Date, ref Var1);


            if (temp1 == 0 || temp == 0)
                return false;

            if ((BasicFinancial.RevenueList[temp-1].MonthRiseRatioInSameMonth > 0
                 &&(BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth > 0
                ))
                ||(BasicFinancial.DataList[temp1].EPS - BasicFinancial.DataList[temp1 - 1].EPS > 0))
            {
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
        #region 轉機 25元以下佳 10元以下最好
       
        void TurnStock()
        {

            if (!uv.HasBuy
                
                && IsBuy_ConditionForTurn()
                //&& TechData.TechData[i].date.Year ==2014 && TechData.TechData[i].date.Month==1 &&TechData.TechData[i].date.Day>=11
                && TechData.TechData[i].ClosePrice == TechData.TechData[i].MinPrice[0]
                )
            {

                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }

            if (uv.HasBuy &&
                (
                (IsSell_Condition() && uv.Accumulation > 10)
                || (IsSell_Condition() && uv.Accumulation < -5)
                ||(uv.HaveStockDay==60 && uv.Accumulation<=10)
               
                  
              ))
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
            }
        }

        bool IsSell_ConditionForTrun()
        {
            BasicFinancial.InitialDate(TechData.TechData[i].date);

            int temp = BasicFinancial.RevenueInt;
            int temp1 = BasicFinancial.BasicFinancialInt;
            if ((BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth < 0
                && BasicFinancial.RevenueList[temp - 1].MonthRiseRatioInSameMonth < 0)
                || (BasicFinancial.DataList[temp1].EPS - BasicFinancial.DataList[temp1 - 1].EPS < 0)
                || BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth < -20

               )
            {
                return true;
            }

            return false;
        }
        bool IsBuy_ConditionForTurn()
        {
            
            SumVariable Var1 = new SumVariable();

            BasicFinancial.InitialDate(TechData.TechData[i].date);
            int temp = BasicFinancial.RevenueInt;
            int temp1 = BasicFinancial.BasicFinancialInt;

            if (temp1 -3 < 0)
                return false;
            if (
                BasicFinancial.RevenueList[temp].MonthRiseRatioInSameMonth > 0
                && BasicFinancial.RevenueList[temp - 1].MonthRiseRatioInSameMonth > 0
                && BasicFinancial.DataList[temp1].EPS>0
                && BasicFinancial.DataList[temp1-1].EPS<0
                &&BasicFinancial.DataList[temp1].EPS - BasicFinancial.DataList[temp1 - 1].EPS > 0
                && BasicFinancial.DataList[temp1-1].EPS - BasicFinancial.DataList[temp1 - 2].EPS > 0

                && BasicFinancial.DataList[temp1].LongTermLiabilities / BasicFinancial.DataList[temp1].AllLiabilities < 0.6
                )
            {
   
                    YearsSum(1, BasicFinancial.DataList[temp1].Date, ref Var1);
                    if (Var1.FCF > 0)
                        return true;
                
            }

            return false;
        }
        #endregion
       

        #region 2330
        void _2330_DropBuy()
        {
            if (TechData.TechData[i].company != 2330)
                return;

            if (!uv.HasBuy && TechData.TechData[i].ReturnOnInvestment < 0)
            {
                uv.CountDropDay++;
                uv.AccumulationDrop += TechData.TechData[i].ReturnOnInvestment;
            }
            else if (TechData.TechData[i].ReturnOnInvestment != 0)
                uv.CountDropDay = 0;

            if (!uv.HasBuy
                  && (uv.CountDropDay==3 || TechData.TechData[i].ReturnOnInvestment<=-2)
               
               )
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = true;
            }


            if (uv.HasBuy
                && (uv.HaveStockDay==1&& TechData.TechData[i].ReturnOnInvestment<=1
            ||( uv.AccumulationRise>5 && TechData.TechData[i].ReturnOnInvestment<=1)
            ||(TechData.TechData[i].AverageVolume[0]>TechData.TechData[i].volume *1.8 && TechData.TechData[i].ReturnOnInvestment<=1)
            ||(uv.AccumulationRise<5 && TechData.TechData[i].AverageVolume[1]>TechData.TechData[i].volume )
            
                ))
            {
                Console.WriteLine(TechData.TechData[i].ClosePrice);
                Console.WriteLine(TechData.TechData[i].date);
                uv.CanBuy = false;
            }
     
        }
        #endregion
        public class SumVariable//需要累加的參數
        {
            public double FCF;
            public double ROE;
            public double NetIncomeAfterTax;
            public double debt;
            public double NetIncomeAfterTaxRatio;
            public double CashFromOperation;
        }

        void YearsSum(int countYear, DateTime date, ref SumVariable sum)//每年累家數值 例如CashFlow 等
        {
            int temp = BasicFinancial.BasicFinancialInt;
            int countY = 0;
            bool startToCount = false;
            int countT = 0;//數累加了多少次

           

            if (countYear == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (temp - i < 0)
                    {
                        return;
                    }

                    sum.FCF += BasicFinancial.DataList[temp - i].CashFromOperationActivity -
                        BasicFinancial.DataList[temp - i].CashFromInvestingActivity;
                    sum.ROE += BasicFinancial.DataList[temp - i].ROE;
                    sum.NetIncomeAfterTax += BasicFinancial.DataList[temp - i].ProfitAfterTax;
                    sum.NetIncomeAfterTaxRatio += BasicFinancial.DataList[temp - i].ProfitAfterTaxRatio;
                    sum.CashFromOperation += BasicFinancial.DataList[temp - i].CashFromOperationActivity;
                }

                sum.NetIncomeAfterTaxRatio = sum.NetIncomeAfterTaxRatio / 4;
                return;
            }

            if (countYear != 1)
            {
                for (int i = temp; i >=0; i--)
                {
                    if (BasicFinancial.DataList[i].Date.Month == 12)
                    {
                        countY++;
                        startToCount = true;
                    }

                    if (countY == countYear+1)
                        return;

                    if (startToCount)
                    {
                        sum.FCF += BasicFinancial.DataList[i].CashFromOperationActivity -
                            BasicFinancial.DataList[i].CashFromInvestingActivity;
                        sum.ROE += BasicFinancial.DataList[i].ROE;
                        sum.NetIncomeAfterTax += BasicFinancial.DataList[i].ProfitAfterTax;
                        sum.NetIncomeAfterTaxRatio += BasicFinancial.DataList[i].ProfitAfterTaxRatio;
                        sum.CashFromOperation += BasicFinancial.DataList[i].CashFromOperationActivity;
                        countT++;
                    }

                    
                }

                sum.NetIncomeAfterTaxRatio = sum.NetIncomeAfterTaxRatio / countT;
            }
        }
    }
}
