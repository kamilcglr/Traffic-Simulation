using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        //-------------CHOIX SIMULATION--------------
        public bool SimulationTempsPasseRoute = true;
        public bool SimulationVitesseMoyenne = false;
        public bool SimulationTempsPasseArret = false;
        //-------------------------------------------

        private readonly DispatcherTimer _simuTimer = new DispatcherTimer();
        public static bool EnSimulation = false;
        public static int ligneVitesse = 2;
        public static int colonneNbVehicules = 2;

        //Tableaux que l'on complete à chaque _simuTimer_Tick
        public static List<List<double>> Tableau = new List<List<double>>();
        public static List<List<double>> TableauMediane = new List<List<double>>();
        public static List<List<double>> TableauTempsPasseRoute= new List<List<double>>();
        public static List<List<double>> TableauTempsPasseRouteMediane = new List<List<double>>();

        //Liste que l'on remplie pour en faire une moyenne à la fin
        public static List<double> VitesseMoyenneSimulateur = new List<double>();
        public static List<double> TempsPasseMoyenne = new List<double>();

        public void InitTimerSimu()
        {
            _timer1.Tick += timer1_Tick;
            _timer2.Tick += timer2_Tick;
            _timerGauges.Tick += timerGauges_Tick;
            _simuTimer.Tick += _simuTimer_Tick;

            _timer1.Start();
            _timer2.Start();
            _timerGauges.Start();
            _simuTimer.Start(); 


            double VitesseTimerSimulation = 0.1; //ms
            double VitesseTimerGauge = 0.1; //s
            double Lecture = 10; //s

            _timer1.Interval = TimeSpan.FromMilliseconds(VitesseTimerSimulation);
            _timer2.Interval = TimeSpan.FromMilliseconds(VitesseTimerSimulation);
            _timerGauges.Interval = TimeSpan.FromSeconds(VitesseTimerGauge);
            _simuTimer.Interval = TimeSpan.FromSeconds(Lecture);
        }

        private void _simuTimer_Tick(object sender, EventArgs e)
        {
            if (SimulationVitesseMoyenne)
            {
                Tableau[ligneVitesse - 2][colonneNbVehicules - 2] =
                    VitesseMoyenneSimulateur.Sum() / VitesseMoyenneSimulateur.Count;
                TableauMediane[ligneVitesse - 2][colonneNbVehicules - 2] =
                    VitesseMoyenneSimulateur[VitesseMoyenneSimulateur.Count / 2];
                VitesseMoyenneSimulateur.Clear(); //On vide le tableau
                Suivant();
            }
           
            if(SimulationTempsPasseRoute || SimulationTempsPasseArret){
                if (TempsPasseMoyenne.Count > 35)//On prend au moins dix valeurs
                {
                    TableauTempsPasseRoute[ligneVitesse - 2][colonneNbVehicules - 2] =
                        TempsPasseMoyenne.Sum() / TempsPasseMoyenne.Count;
                    TableauTempsPasseRouteMediane[ligneVitesse - 2][colonneNbVehicules - 2] = TempsPasseMoyenne[TempsPasseMoyenne.Count / 2];
                    TempsPasseMoyenne.Clear(); //On vide le tableau
                    Suivant();
                }}
            
        }

        public void Editer_Excell_VmaxetNbArret()
        {
            Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook sheet = excel.Workbooks.Open("C:\\Users\\Kamil\\Downloads\\TempsPasse.xlsx");
            Worksheet x = excel.ActiveSheet as Worksheet;
            if (SimulationTempsPasseRoute || SimulationTempsPasseArret)
            {
                for (int colonne = 2; colonne < 33; colonne++)
                {
                    for (int ligne = 2; ligne < 13; ligne++)
                    {
                        x.Cells[ligne, colonne] = TableauTempsPasseRoute[ligne - 2][colonne - 2];
                    }
                }

                for (int colonne = 2; colonne < 33; colonne++)
                {
                    for (int ligne = 2; ligne < 13; ligne++)
                    {
                        x.Cells[ligne + 15, colonne] = TableauTempsPasseRouteMediane[ligne - 2][colonne - 2];
                    }
                }
            }

            if (SimulationVitesseMoyenne)
            {
                for (int colonne = 2; colonne < 33; colonne++)
                {
                    for (int ligne = 2; ligne < 13; ligne++)
                    {
                        x.Cells[ligne, colonne] = Tableau[ligne - 2][colonne - 2];
                    }
                }

                for (int colonne = 2; colonne < 33; colonne++)
                {
                    for (int ligne = 2; ligne < 13; ligne++)
                    {
                        x.Cells[ligne + 15, colonne] = TableauMediane[ligne - 2][colonne - 2];
                    }
                }
            }
            sheet.Save();
            sheet.Close(true, Type.Missing, Type.Missing);
            excel.Quit();
        }
        public void Suivant()
        {
            if ((int)ChoixVitessemax.Value == 130 && (int)ChoixNombrevoitures.Value == 35)
            {
                Editer_Excell_VmaxetNbArret();
                System.Windows.Application.Current.Shutdown();
            }

            if (ChoixVitessemax.Value == 130) //On remplie tout la colonne jusqu'à 130 km/h
            {
                ChoixVitessemax.Value = 30;
                ligneVitesse = 2;
                ChoixNombrevoitures.Value++;
                colonneNbVehicules++; //On passe à la colonne suivante (ajout de véhicules)
            }
            else
            {
                ChoixVitessemax.Value += 10; // On augmente la vitesse
                ligneVitesse++; //On passe à la ligne suivante (vitesse suivante dans le tableau)
            }
            ModificationNbVehicules();          

        }
        public void InitVitessetableau()
        {
            for(int i = 0; i < 11; i++) //Lignes
            {
                Tableau.Add(new List<double>());

                for (int j = 0; j < 31; j++) //Colonnes
                {
                    Tableau[i].Add(0);
                }
            }
            for (int i = 0; i < 11; i++) //Lignes
            {
                TableauMediane.Add(new List<double>());

                for (int j = 0; j < 31; j++) //Colonnes
                {
                    TableauMediane[i].Add(0);
                }
            }

        }
        public void InitTempsPasseTableau()
        {
            for (int i = 0; i < 11; i++) //Lignes
            {
                TableauTempsPasseRoute.Add(new List<double>());

                for (int j = 0; j < 31; j++) //Colonnes
                {
                    TableauTempsPasseRoute[i].Add(0);
                }
            }
            for (int i = 0; i < 11; i++) //Lignes
            {
                TableauTempsPasseRouteMediane.Add(new List<double>());

                for (int j = 0; j < 31; j++) //Colonnes
                {
                    TableauTempsPasseRouteMediane[i].Add(0);
                }
            }
        }
        
        private void LancerSimulation(object sender, System.Windows.RoutedEventArgs e)
        {
            EnSimulation = true;
            //------------INIT SIMU------------------

            //InitVitessetableau();
            if(SimulationTempsPasseRoute || SimulationTempsPasseArret )InitTempsPasseTableau();
            if(SimulationVitesseMoyenne)InitVitessetableau();
            ChoixNombrevoitures.Value = 5;
            ChoixVitessemax.Value = 30;

            if (!pause)
            {
                //----------------------INIT VALEURS-----------------------------
                vitessemax = ChoixVitessemax.Value / 3.6 * 0.02 / 0.25;
                acceleration = -0.002 * Math.Log(ChoixAcceleration.Value) + 0.0088;
                deceleration = ChoixDeceleration.Value;
                _distanceEntreVehicule = (int)ChoixDistanceEntreVehicules.Value;
                distancePtcritique = 100;
                distanceAnalyse = 500;
                //----------------------------------------------------------------

                //----------------------INIT GRAPHS-------------------------

                InitialiserGaugeVitesse();
                InitialiserGaugeNbvehiculesArret();
                InitialiserGaugeTempsPasseRoute();
                InitialiserGaugeTempsPasseArret();
                //----------------------------------------------------------

                if (Chargement) InitaliserVoitures();

                //--------------LANCEMENT TIMERs (à la fin !)---------------
                InitTimerSimu();

                //----------------------------------------------------------
            }
            else
            {
                InitTimerSimu();
                pause = false;
            }
        }
    }
}
