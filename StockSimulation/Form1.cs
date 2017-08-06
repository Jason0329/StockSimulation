using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dynamicControlExcelTest;
using System.Data.EntityClient;
using System.Threading;
using System.Data.SqlClient;
using StockSimulation.Stock;
using StockSimulation.Future;
using StockSimulation.stockSimulation;

namespace StockSimulation
{
    public partial class Form1 
    {
        
        //SelectStock stock = new SelectStock();
        string[] aa;

        public Form1()
        {
            //SimulationForTechnologicalAnalysis SimulationT = new SimulationForTechnologicalAnalysis();
            //SimulationT.Initial("2011-01-01", "2013-09-01", 1101);
            //SimulationForSelectBasicFinancialReport Simulation = new SimulationForSelectBasicFinancialReport();
            //Simulation.Initial("2005-01-01" , "2013-09-01",2330);
            Simulation ss = new Simulation();
            ss.StartSimulation();
            //SimulateGoldenLine sg = new SimulateGoldenLine();
            //sg.StartToRun();
            //SimulationForPossibility ssp = new SimulationForPossibility();
            //ssp.Buy2330ByFuture();
            //FuturePossibility Fp = new FuturePossibility();
            //Fp.TestCountBuyAndSell();
            //Fp.TestGetRise();
            //this.Visible = false;
            //SimulateKDLine sgl = new SimulateKDLine();
            //sgl.StartToRun();
            //Simulation01 sl = new Simulation01();


           
           
        }
       
     
           
    }
}
