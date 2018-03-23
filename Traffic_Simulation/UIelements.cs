using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace Simulateur_0._0._2
{
    public partial class MainWindow : Window
    {
        //UI ELEMENTS
        public void Start(object sender, RoutedEventArgs e)
        {
            //----------------------INIT VALEURS-----------------------------
            vitessemax = ChoixVitessemax.Value / 3.6 * 0.02 / 0.25;
            acceleration = -0.002 * Math.Log(ChoixAcceleration.Value) + 0.0088;
            deceleration = ChoixDeceleration.Value;
            _distanceEntreVehicule = (int) ChoixDistanceEntreVehicules.Value;
            distancePtcritique = 100;
            distanceAnalyse = 500;
            //----------------------------------------------------------------

            //----------------------INIT GRAPHS-------------------------
            InitialiserHeatMap();

            InitialiserGaugeVitesse();
            InitialiserGraphVitesse();
            InitialiserLabelVmoy();

            InitialiserGaugeNbvehiculesArret();
            InitialiserGraphNbVehiculesArret();
            InitialiserLabelNbVehiculesArret();

            InitialiserGaugeTempsPasseRoute();
            InitialiserGraphTempsPasseRoute();
            InitialiserLabelTempsPasseRoute();
            
            InitialiserGaugeTempsPasseArret();
            InitialiserGraphTempsPasseArret();
            InitialiserLabelTempsPasseArret();
            //----------------------------------------------------------

            if (Chargement) InitaliserVoitures();

            //--------------LANCEMENT TIMERs (à la fin !)---------------
            InitTimer();
            _timer1.Start();
            _timer2.Start();
            _timer3.Start();
            _timerGauges.Start();
            //----------------------------------------------------------

        }

        //----------------------------------GRAPHES----------------------------------------------
        public void InitialiserGraphVitesse()
        {
            for (var i = 0; i < 20; i++) //Creation de 20 Points
            {
                var point = new ObservableValue(0);
                VitesseValeurs.Add(point);
            }

            Graphtest.Series.Add(new LineSeries
            {
                Values = new ChartValues<ObservableValue>
                {
                    VitesseValeurs[0],
                    VitesseValeurs[1],
                    VitesseValeurs[2],
                    VitesseValeurs[3],
                    VitesseValeurs[4],
                    VitesseValeurs[5],
                    VitesseValeurs[6],
                    VitesseValeurs[7],
                    VitesseValeurs[8],
                    VitesseValeurs[9],
                    VitesseValeurs[10],
                    VitesseValeurs[11],
                    VitesseValeurs[12],
                    VitesseValeurs[13],
                    VitesseValeurs[14],
                    VitesseValeurs[15],
                    VitesseValeurs[16],
                    VitesseValeurs[17],
                    VitesseValeurs[18],
                    VitesseValeurs[19]
                },
                PointGeometrySize = 0,
                Fill = new SolidColorBrush(Color.FromRgb(117, 171, 188)),
                StrokeThickness = 0
            });
            Graphtest.HideTooltip();
            Graphtest.DataTooltip = null;
            Graphtest.Hoverable = false;
        }
        public void InitialiserGraphTempsPasseRoute()
        {
            for (var i = 0; i < 20; i++) //Creation de 20 Points
            {
                var point = new ObservableValue(0);
                TempsPasseRoute.Add(point);
            }

            GraphTempsPasseRoute.Series.Add(new LineSeries
            {
                Values = new ChartValues<ObservableValue>
                {
                    TempsPasseRoute[0],
                    TempsPasseRoute[1],
                    TempsPasseRoute[2],
                    TempsPasseRoute[3],
                    TempsPasseRoute[4],
                    TempsPasseRoute[5],
                    TempsPasseRoute[6],
                    TempsPasseRoute[7],
                    TempsPasseRoute[8],
                    TempsPasseRoute[9],
                    TempsPasseRoute[10],
                    TempsPasseRoute[11],
                    TempsPasseRoute[12],
                    TempsPasseRoute[13],
                    TempsPasseRoute[14],
                    TempsPasseRoute[15],
                    TempsPasseRoute[16],
                    TempsPasseRoute[17],
                    TempsPasseRoute[18],
                    TempsPasseRoute[19]
                },
                PointGeometrySize = 0,
                Fill = new SolidColorBrush(Color.FromRgb(204, 164, 59)),
                StrokeThickness = 0
            });
            Graphtest.HideTooltip();
            Graphtest.DataTooltip = null;
            Graphtest.Hoverable = false;
        }
        public void InitialiserGaugeTempsPasseRoute()
        {
            GaugeTempsPasseRoute.ToValue = 60;
        }
        
        public void InitialiserGraphNbVehiculesArret()
        {
            for (var i = 0; i < 20; i++) //Creation de 20 Points
            {
                var point = new ObservableValue(0);
                NbVehiculesArretValeurs.Add(point);
            }

            GraphNbVehiculesArret.Series.Add(new ColumnSeries
            {
                Values = new ChartValues<ObservableValue>
                {
                    NbVehiculesArretValeurs[0],
                    NbVehiculesArretValeurs[1],
                    NbVehiculesArretValeurs[2],
                    NbVehiculesArretValeurs[3],
                    NbVehiculesArretValeurs[4],
                    NbVehiculesArretValeurs[5],
                    NbVehiculesArretValeurs[6],
                    NbVehiculesArretValeurs[7],
                    NbVehiculesArretValeurs[8],
                    NbVehiculesArretValeurs[9],
                    NbVehiculesArretValeurs[10],
                    NbVehiculesArretValeurs[11],
                    NbVehiculesArretValeurs[12],
                    NbVehiculesArretValeurs[13],
                    NbVehiculesArretValeurs[14],
                    NbVehiculesArretValeurs[15],
                    NbVehiculesArretValeurs[16],
                    NbVehiculesArretValeurs[17],
                    NbVehiculesArretValeurs[18],
                    NbVehiculesArretValeurs[19]
                },
                Fill = new SolidColorBrush(Color.FromRgb(77, 111, 150))
            });
            GraphNbVehiculesArret.HideTooltip();
            GraphNbVehiculesArret.DataTooltip = null;
            GraphNbVehiculesArret.Hoverable = false;
        }
        public void InitialiserGraphTempsPasseArret()
        {
            for (var i = 0; i < 20; i++) //Creation de 20 Points
            {
                var point = new ObservableValue(0);
                TempsPasseArret.Add(point);
            }

            GraphTempsPasseArret.Series.Add(new ColumnSeries
            {
                Values = new ChartValues<ObservableValue>
                {
                    TempsPasseArret[0],
                    TempsPasseArret[1],
                    TempsPasseArret[2],
                    TempsPasseArret[3],
                    TempsPasseArret[4],
                    TempsPasseArret[5],
                    TempsPasseArret[6],
                    TempsPasseArret[7],
                    TempsPasseArret[8],
                    TempsPasseArret[9],
                    TempsPasseArret[10],
                    TempsPasseArret[11],
                    TempsPasseArret[12],
                    TempsPasseArret[13],
                    TempsPasseArret[14],
                    TempsPasseArret[15],
                    TempsPasseArret[16],
                    TempsPasseArret[17],
                    TempsPasseArret[18],
                    TempsPasseArret[19]
                },
                Fill = new SolidColorBrush(Color.FromRgb(152, 68, 71))
            });
            GraphTempsPasseArret.HideTooltip();
            GraphTempsPasseArret.DataTooltip = null;
            GraphTempsPasseArret.Hoverable = false;
        }

        public void InitialiserGaugeNbvehiculesArret()
        {
            GaugeNbvehiculesArret.To = ChoixNombrevoitures.Value;
            GaugeNbvehiculesArret.GaugeActiveFill = new SolidColorBrush(Color.FromRgb(77, 111, 150));
        }
        public void InitialiserGaugeTempsPasseArret()
        {
            GaugeTempsPasseArret.To = 60;
            GaugeTempsPasseArret.GaugeActiveFill = new SolidColorBrush(Color.FromRgb(152, 68, 71));
        }

        public void InitialiserGaugeVitesse()
        {
            var vitesses = GaugeVitesse.ToValue;
            var decoupe = vitesses / 7;

            Sec1.FromValue = vitesses - decoupe;
            Sec1.ToValue = vitesses;

            Sec2.FromValue = vitesses - decoupe * 2;
            Sec2.ToValue = Sec1.FromValue;

            Sec3.FromValue = vitesses - decoupe * 3;
            Sec3.ToValue = Sec2.FromValue;

            Sec4.FromValue = vitesses - decoupe * 4;
            Sec4.ToValue = Sec3.FromValue;

            Sec5.FromValue = vitesses - decoupe * 5;
            Sec5.ToValue = Sec4.FromValue;

            Sec6.FromValue = vitesses - decoupe * 6;
            Sec6.ToValue = Sec5.FromValue;

            Sec7.FromValue = 0;
            Sec7.ToValue = Sec6.FromValue;
        }

        public void InitialiserHeatMap()
        {
            for (var i = 0; i < 6; i++) //Creation de 20 Points
            {
                var point = new HeatPoint(i, 0, 0);
                HeatMapValeurs1.Add(point);
            }

            HeatMap1.Series.Add(new HeatSeries
            {
                Values = new ChartValues<HeatPoint>
                {
                    HeatMapValeurs1[0],
                    HeatMapValeurs1[1],
                    HeatMapValeurs1[2],
                    HeatMapValeurs1[3],
                    HeatMapValeurs1[4],
                    HeatMapValeurs1[5]
                },
                GradientStopCollection = new GradientStopCollection
                {

                    new GradientStop(Colors.LightSkyBlue, 0),
                    new GradientStop(Colors.SkyBlue, .25),
                    new GradientStop(Colors.DodgerBlue, .50),
                    new GradientStop(Colors.MediumBlue, .75),
                    new GradientStop(Colors.DarkBlue, 1)
                },
                DrawsHeatRange = false
            });
            for (var i = 0; i < 5; i++) //Creation de 20 Points
            {
                var point = new HeatPoint(i, 1, 0);
                HeatMapValeurs2.Add(point);
            }

            HeatMap1.Series.Add(new HeatSeries
            {
                Values = new ChartValues<HeatPoint>
                {
                    HeatMapValeurs2[0],
                    HeatMapValeurs2[1],
                    HeatMapValeurs2[2],
                    HeatMapValeurs2[3],
                    HeatMapValeurs2[4]
                },
                GradientStopCollection = new GradientStopCollection
                {
                    /*
                    new GradientStop(Colors.SeaGreen, 0),
                    new GradientStop(Colors.YellowGreen, .25),
                    new GradientStop(Colors.DarkGoldenrod, .50),
                    new GradientStop(Colors.OrangeRed, .75),
                    new GradientStop(Colors.DarkRed, 1)
                    */
                    new GradientStop(Colors.LightSkyBlue, 0),
                    new GradientStop(Colors.SkyBlue, .25),
                    new GradientStop(Colors.DodgerBlue, .50),
                    new GradientStop(Colors.MediumBlue, .75),
                    new GradientStop(Colors.DarkBlue, 1)
                    /*
                    new GradientStop(Color.FromRgb(239,243,255), 0),
                    new GradientStop(Color.FromRgb(189,215,231), .25),
                    new GradientStop(Color.FromRgb(107,174,214), .50),
                    new GradientStop(Color.FromRgb(49,130,189), .75),
                    new GradientStop(Color.FromRgb(8,81,156), 1)
                    */
                    /*
                     * new GradientStop(Color.FromRgb(0,104,55), 0),
                    new GradientStop(Color.FromRgb(0,104,55), .10),
                    new GradientStop(Color.FromRgb(26,152,80), .20),
                    new GradientStop(Color.FromRgb(102,189,99), .30),
                    new GradientStop(Color.FromRgb(166,217,106), .40),
                    new GradientStop(Color.FromRgb(217,239,139), .50),
                    new GradientStop(Color.FromRgb(254,224,139), .60),
                    new GradientStop(Color.FromRgb(253,174,97), .70),
                    new GradientStop(Color.FromRgb(244,109,67), .80),
                    new GradientStop(Color.FromRgb(215,48,39), .90),
                    new GradientStop(Color.FromRgb(165,0,38), 1)
                     */
                },
                DrawsHeatRange = false
            });
            HeatMap1.HideTooltip();
            HeatMap1.DataTooltip = null;
            HeatMap1.Hoverable = false;
        }

        public void InitaliserVoitures()
        {
            // On effectue cette étape si c'est la première fois qu'est pressé le bouton
            {
                var nbvoituresVoiegauche =
                    (int) (ChoixNombrevoitures.Value * (ChoixProportionVoituregauche.Value / 100));
                var nbvoituresVoiedroite = (int) ChoixNombrevoitures.Value - nbvoituresVoiegauche;
                var densiteCamion = (int) ChoixDensitecamion.Value;
                _distanceEntreVehicule = (int) ChoixDistanceEntreVehicules.Value;

                var i = nbvoituresVoiegauche;
                var decoupageL1 = 800;
                if (i != 0) decoupageL1 = 800 / i;
                while (i != 0)
                {
                    var voiture = new Voiture();
                    if (_rand.Next(100) < densiteCamion && Chargement
                    ) //Pas besoin de faire la densité de camion pour les autres cas
                        Ajoutcamion(voiture);

                    voiture.Lane = 2;
                    voiture.Vitesse = vitessemax;
                    voiture.Yposition = PositionL2;
                    voiture.Xposition = decoupageL1 * i - 800;
                    Cars2.Add(voiture);
                    Affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture.Xposition);
                    Canvas.SetBottom(voiture, voiture.Yposition);
                    i--;
                }

                var j = nbvoituresVoiedroite;
                var decoupageL2 = 800 / j;
                while (j != 0)
                {
                    var voiture = new Voiture();
                    if (_rand.Next(100) < densiteCamion && Chargement
                    ) //Pas besoin de faire la densité de camion pour les autres cas
                        Ajoutcamion(voiture);

                    voiture.Lane = 1;
                    voiture.Vitesse = vitessemax;
                    voiture.Yposition = PositionL1;
                    voiture.Xposition = decoupageL2 * j - 800;
                    Cars.Add(voiture);
                    Affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture.Xposition);
                    Canvas.SetBottom(voiture, voiture.Yposition);
                    j--;
                }

                Chargement = false;
            }
        }

        public void InitialiserLabelNbVehiculesArret()
        {
            for (var i = 0; i < 20; i++) Nbarret.Add(0);
        }

        public void InitialiserLabelVmoy()
        {
            for (var i = 0; i < 120; i++) Vmoy.Add(ChoixVitessemax.Value);
        }

        public void InitialiserLabelTempsPasseRoute()
        {
            for (var i = 0; i < 20; i++) MoyLabelTempsPasseRoute.Add(0);
        }
        public void InitialiserLabelTempsPasseArret()
        {
            for (var i = 0; i < 20; i++) MoyLabelTempsPasseArret.Add(0);
        }
        
        private void Ajoutcamion(Voiture voiture)
        {
            voiture.Vehiculelent = true;
            var relativeUri = new Uri("Images/truck.png", UriKind.Relative);
            voiture.Source = new BitmapImage(relativeUri);
            voiture.Width = 32;
            voiture.Height = 10;
        }

        private void Choix_vitesse_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VitessemaxChoixAffichage.Content = Math.Round(ChoixVitessemax.Value, 0) + " km/h";
            //Respecter cet ordre, ToValue réutilisé dans InitialiserGauge
            GaugeVitesse.ToValue = ChoixVitessemax.Value + 20;
            InitialiserGaugeVitesse();
        }

        private void Choix_acceleration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Acceleration renvoyée en pixel/20ms2
            //On exprime va afficher ici le 0 à 100
            AccelerationmaxChoixAffichage.Content = Math.Round(ChoixAcceleration.Value, 1) + " s";
        }

        private void Choix_proportion_voituregauche_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            ProportionVoiegaucheChoixAffichage.Content = ChoixProportionVoituregauche.Value.ToString("F0") + " %";
        }

        private void choix_distance_entre_vehicules_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            DistanceSecuriteAffichage.Content =
                ChoixDistanceEntreVehicules.Value.ToString("F0");
        }

        private void choix_deceleration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DecelerationChoixAffichage.Content = Math.Round(ChoixDeceleration.Value, 3);
        }

        private void Choix_densitecamion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DensitecamionChoixAffichage.Content = ChoixDensitecamion.Value.ToString("F0") + " %";
        }

        private void Choix_nombrevoitures_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            NombrevehiculesChoixAffichage.Content = ChoixNombrevoitures.Value.ToString("F0");
            InitialiserGaugeNbvehiculesArret();
        }
    }
}