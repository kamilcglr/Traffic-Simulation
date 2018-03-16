using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LiveCharts.Defaults;
using Microsoft.Office.Interop.Excel;
using Window = System.Windows.Window;
using Excel = Microsoft.Office.Interop.Excel;

namespace Simulateur_0._0._2
{

    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _simuTimer = new DispatcherTimer();
        public static int nbVoituresSimu = 1;
        public static int VitesseMaxSimu = 10;

        public static int ligneVitesse = 2;
        public static int colonneNbVehicules =2 ;

        public static List<List<double>> Tableau = new List<List<double>>();
        public void incrementerVitesse()
        {
            VitesseMaxSimu += 5;
            ChoixVitessemax.Value = VitesseMaxSimu;
            ligneVitesse++;
        }
        public void incrementerNbVehiculesArret()
        {
            nbVoituresSimu += 1;
            ChoixNombrevoitures.Value = nbVoituresSimu;
            colonneNbVehicules++;
        }
        private void _simuTimer_Tick(object sender, EventArgs e)
        {
            Tableau[ligneVitesse-2][colonneNbVehicules-2] = ValVmoyLabel;
            Suivant();
        }
        public void Editer_Excell()
        {
            Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook sheet = excel.Workbooks.Open("C:\\Users\\Kamil\\Downloads\\graph.xlsx");
            Worksheet x = excel.ActiveSheet as Worksheet;
            for (int  colonne= 2; colonne < 35; colonne++)
            {
                for (int ligne = 2; ligne < 26; ligne++)
                {
                    x.Cells[ligne, colonne] = Tableau[ligne - 2][colonne - 2];
                }
            }          
            sheet.Save();
            sheet.Close(true, Type.Missing, Type.Missing);
            excel.Quit();
        }
        public void Suivant()
        {
            if (nbVoituresSimu == 35) //Fin d'une ligne
            {
                nbVoituresSimu = 1;
                incrementerVitesse();
            }
            else
            {
                incrementerNbVehiculesArret();
            }

            if (nbVoituresSimu == 35 && VitesseMaxSimu == 130)
            {
                Editer_Excell();
                System.Windows.Application.Current.Shutdown();
            }

        }

        public void InitVitessetableau()
        {
            for(int i = 0; i < 26; i++) //Lignes
            {
                Tableau.Add(new List<double>());

                for (int j = 0; j < 35; j++) //Colonnes
                {
                    Tableau[i].Add(0);
                }
            }
            
        }
        private void LancerSimulation(object sender, System.Windows.RoutedEventArgs e)
        {
            ChoixVitesseSimulation.Value = 0.1; //ms
            VitesseTimerGauge = 0.01; //s
            //----------------------INIT VALEURS-----------------------------
            vitessemax = ChoixVitessemax.Value / 3.6 * 0.02 / 0.25;
            acceleration = -0.002 * Math.Log(ChoixAcceleration.Value) + 0.0088;
            deceleration = ChoixDeceleration.Value;
            _distanceEntreVehicule = (int)ChoixDistanceEntreVehicules.Value;
            distancePtcritique = 100;
            distanceAnalyse = 500;
            //----------------------------------------------------------------

            //----------------------INIT GRAPHS-------------------------
            InitialiserGaugeNbvehiculesArret();
            InitialiserGaugeVitesse();
            InitialiserLabelVmoy();
            InitialiserLabelNbVehiculesArret();
            //----------------------------------------------------------

            if (Chargement) InitaliserVoitures();

            InitVitessetableau();
            ChoixNombrevoitures.Value = 1;
            ChoixVitessemax.Value = 10;

            //--------------LANCEMENT TIMERs (à la fin !)---------------
            _simuTimer.Tick += _simuTimer_Tick;
            _simuTimer.Interval = TimeSpan.FromSeconds(1);
            InitTimer();
            _timer1.Start();
            _timer2.Start();
            _simuTimer.Start();
            _timerGauges.Start();
            //------------------------------    ----------------------------

        }
    }
}
