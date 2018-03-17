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
       
        public static int ligneVitesse = 2;
        public static int colonneNbVehicules = 2;

        public static List<List<double>> Tableau = new List<List<double>>();
        public static List<List<double>> TableauMediane = new List<List<double>>();

        public static List<double> VitesseMoyenneSimulateur = new List<double>();

        private void _simuTimer_Tick(object sender, EventArgs e)
        {   //Ajout dans le tableau d'une valeur de vitesse moyenne
            Tableau[ligneVitesse-2][colonneNbVehicules-2] =
                VitesseMoyenneSimulateur.Sum()/VitesseMoyenneSimulateur.Count;
            TableauMediane[ligneVitesse - 2][colonneNbVehicules - 2] =
                VitesseMoyenneSimulateur[VitesseMoyenneSimulateur.Count / 2];
            VitesseMoyenneSimulateur.Clear(); //On vide le tableau
            Suivant();
        }
        public void Editer_Excell_VmaxetNbArret()
        {
            Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook sheet = excel.Workbooks.Open("C:\\Users\\Kamil\\Downloads\\graph.xlsx");
            Worksheet x = excel.ActiveSheet as Worksheet;
            for (int  colonne= 2; colonne < 33; colonne++)
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
                    x.Cells[ligne+15, colonne] = TableauMediane[ligne - 2][colonne - 2];
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
        private void LancerSimulation(object sender, System.Windows.RoutedEventArgs e)
        {
            //------------INIT SIMU------------------
            InitVitessetableau();
            ChoixNombrevoitures.Value = 5;
            ChoixVitessemax.Value = 30;

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
            //----------------------------------------------------------

            if (Chargement) InitaliserVoitures();

            //--------------LANCEMENT TIMERs (à la fin !)---------------

            _simuTimer.Tick += _simuTimer_Tick;
            _simuTimer.Interval = TimeSpan.FromSeconds(5);
            InitTimer();
            _timer1.Start();
            _timer2.Start();
            _simuTimer.Start();
            _timerGauges.Start();
            //------------------------------    ----------------------------

        }
    }
}
