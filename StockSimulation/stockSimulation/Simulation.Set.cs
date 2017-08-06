using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSimulation.Stock
{
    partial class Simulation
    {
        #region
        public static bool IsOTC=false;//看是否為上櫃股是資料
        #endregion

        static class RiseTactics//策略
        {

            #region 看要用哪個策略
            public static bool _YieldBuy =true;
            public static bool GetRise = false;
            public static bool VolumeDrop = false;
            public static bool LongDrop = false;
            public static bool VolumeRise = false;
            public static bool _2330_DropBuy = false;
            public static bool Get_Rise = false;//看籌碼面是否有增加
            public static bool MinPrice = false;
            public static bool TurnStock = false;
            public static bool MonthRenueRise = false;
            public static bool DropBuy =false;
            #endregion

            #region 看是哪個策略買進方式
            public static bool Is_YieldBuy = false;
            public static bool Is_T70Buy = false;
            public static bool Is_VolumeDrop = false;
            public static bool Is_LongDrop = false;
            public static bool Is_VolumeRise = false;
            public static bool Is_2330_DropBuy = false;
            public static bool Is_MinPrice = false;
            public static bool Is_TurnStock = false;
            public static bool Is_MonthRenueRise = false;
            public static bool Is_DropBuy = false;
            #endregion

            public static void Initial_Var()
            {
                Is_YieldBuy = false;
                Is_T70Buy = false;
                Is_VolumeDrop= false;
                Is_LongDrop = false;
                Is_MinPrice = false;
                Is_TurnStock = false;
                Is_MonthRenueRise = false;
                Is_DropBuy = false;
            }
        }

        void BuySell_ConditionForRise()
        {

            if (RiseTactics.MonthRenueRise)
            {
                MonthRevenueRise();
            }


            if (RiseTactics._YieldBuy)
            {
                YieldBuy();
            }

            if (RiseTactics.GetRise)
            {
                GetRise1();
            }

            if (RiseTactics.VolumeDrop)
            {
                VolumeDropBuy();
            }

            if (RiseTactics.LongDrop)
            {
                LongDrop();
            }

            if (RiseTactics.VolumeRise)
            {
                VolumeRise();
            }

            if (RiseTactics._2330_DropBuy)
            {
                _2330_DropBuy();
            }

            if (RiseTactics.Get_Rise)
            {
                _GetRise();
            }

            if (RiseTactics.MinPrice)
            {
                MinPriceBuy();
            }

            if (RiseTactics.TurnStock)
            {
                TurnStock();
            }

            if (RiseTactics.DropBuy)
            {
                DropBuy();
            }

        }
    

    }
}
