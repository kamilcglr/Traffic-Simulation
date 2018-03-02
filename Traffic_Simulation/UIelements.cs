using System;
using System.Windows;
using System.Windows.Controls;
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
            if (Chargement) // On effectue cette étape si c'est la première fois qu'est pressé le bouton
            {
                var nbvoituresVoiegauche =
                    (int) (ChoixNombrevoitures.Value * (ChoixProportionVoituregauche.Value / 100));
                var nbvoituresVoiedroite = (int) ChoixNombrevoitures.Value - nbvoituresVoiegauche;
                var densiteCamion = (int) ChoixDensitecamion.Value;
                _distanceEntreVehicule = (int) ChoixDistanceEntreVehicules.Value;

                var i = nbvoituresVoiegauche;
                while (i != 0)
                {
                    var voiture = new Voiture();
                    if (_rand.Next(100) < densiteCamion && Chargement
                    ) //Pas besoin de faire la densité de camion pour les autres cas
                        Ajoutcamion(voiture);

                    voiture.Lane = 2;
                    voiture.Vitesse = ChoixVitessemax.Value / 3.6 * 0.02 / 0.25;
                    voiture.Yposition = PositionL2;
                    voiture.Xposition = _distanceEntreVehicule * 2 * i;
                    Cars2.Add(voiture);
                    Affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture.Xposition);
                    Canvas.SetBottom(voiture, voiture.Yposition);
                    i--;
                }

                var j = nbvoituresVoiedroite;
                while (j != 0)
                {
                    var voiture = new Voiture();
                    if (_rand.Next(100) < densiteCamion && Chargement
                    ) //Pas besoin de faire la densité de camion pour les autres cas
                        Ajoutcamion(voiture);

                    voiture.Lane = 1;
                    voiture.Vitesse = ChoixVitessemax.Value / 3.6 * 0.02 / 0.25;
                    voiture.Yposition = PositionL1;
                    voiture.Xposition = j * _distanceEntreVehicule * 2 + ChoixNombrevoitures.Value;
                    Cars.Add(voiture);
                    Affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture.Xposition);
                    Canvas.SetBottom(voiture, voiture.Yposition);
                    j--;
                }
                InitialiserGraphVitesse();
                Chargement = false;
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
                PointGeometrySize = 0
                
            });
            _timer1.Start();
            _timer2.Start();
            _timer3.Start();
        }
        //----------------------------------GRAPHES----------------------------------------------------
        public void InitialiserGraphVitesse()
        {
            for (int i = 0; i < 20; i++) //Creation de 20 Points
            {
                var point = new ObservableValue(0);
                VitesseValeurs.Add(point);
            }
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
            VitessemaxChoixAffichage.Content = "Vitesse max : " + Math.Round(ChoixVitessemax.Value, 3) + "km/h";
        }

        private void Choix_acceleration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Acceleration renvoyée en pixel/20ms2
            //On exprime va afficher ici le 0 à 100
            AccelerationmaxChoixAffichage.Content =
                "Accélération max (0 à 100) : " + Math.Round(ChoixAcceleration.Value, 3) + "s";
        }

        private void Bouton_frein_Click(object sender, RoutedEventArgs e)
        {
            /*if (Cars[0].Frein == false)
                Cars[0].Frein = true;
            else
                Cars[0].Frein = false;*/
        }

        private void Choix_proportion_voituregauche_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            ProportionVoiegaucheChoixAffichage.Content = "Proportion véhicules file de gauche" +
                                                         ChoixProportionVoituregauche.Value.ToString("F0") + " %";
        }

        private void choix_distance_entre_vehicules_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            DistanceSecuriteAffichage.Content =
                "Distance entre veh : " + ChoixDistanceEntreVehicules.Value.ToString("F0");
        }

        private void choix_deceleration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DecelerationChoixAffichage.Content = "Deceleration : " + Math.Round(ChoixDeceleration.Value, 3);
        }

        private void Choix_densitecamion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DensitecamionChoixAffichage.Content =
                "Proportion de camions " + ChoixDensitecamion.Value.ToString("F0") + " %";
        }

        private void Choix_nombrevoitures_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            NombrevehiculesChoixAffichage.Content =
                "Nombre de véhicules : " + ChoixNombrevoitures.Value.ToString("F0");
        }
    }
}