using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Simulateur_0._0._2
{
    /// <summary>
    ///     Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly List<Voiture> Cars = new List<Voiture>();
        private static readonly List<Voiture> Cars2 = new List<Voiture>();
        public bool Chargement = true;
        private int _distanceEntreVehicule = 20;
        public int Nbvoitures;
        private readonly int _pointCritique = 800;
        public int PositionL1 = 540;
        public int PositionL2 = 570;

        private readonly Random _rand = new Random();
        private readonly DispatcherTimer _timer1 = new DispatcherTimer();
        private readonly DispatcherTimer _timer2 = new DispatcherTimer();



        public MainWindow()
        {
            InitializeComponent();
            _timer1.Tick += timer1_Tick;
            _timer1.Interval = TimeSpan.FromMilliseconds(20);
            _timer2.Tick += timer2_Tick;
            _timer2.Interval = TimeSpan.FromMilliseconds(20);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Avance_ligne1();
            Retour_vehicules();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Avance_ligne2();
            if (Cars.Count + Cars2.Count != (int) ChoixNombrevoitures.Value) ModificationNbVehicules();
            NbVoitures1.Content = "Ligne 1 : " + Cars.Count;
            NbVoitures2.Content = "Ligne 1 : " + Cars2.Count;
        }

        public void Avance_ligne1()
        {
            _distanceEntreVehicule = (int) ChoixDistanceEntreVehicules.Value;

            var vitessemax = ((ChoixVitessemax.Value / 3.6) * 0.02) / 0.25;
            var acceleration =  -0.002* Math.Log(ChoixAcceleration.Value) + 0.0088;
            var deceleration = ChoixDeceleration.Value;
            for (var i = 0; i < Cars.Count; i++)
                if (i == 0) // Pour la première voiture on la fait avancer dans tous les cas
                {
                    Canvas.SetLeft(Cars[0], Cars[0].Move(vitessemax, acceleration, deceleration));
                    Canvas.SetBottom(Cars[0], Cars[0].Yposition);
                }
                else // Pour les autre on vérifie devant pour freiner ou avancer
                {
                    if (Cars[i].Xposition + Cars[i].Width < Cars[i - 1].Xposition - _distanceEntreVehicule)
                    {
                        Canvas.SetLeft(Cars[i], Cars[i].Move(vitessemax, acceleration, deceleration));
                        Canvas.SetBottom(Cars[i], Cars[i].Yposition);
                    }
                    else
                    {
                        Cars[i].Frein = true;
                        Canvas.SetLeft(Cars[i], Cars[i].Move(vitessemax, acceleration, deceleration));
                        Canvas.SetBottom(Cars[i], Cars[i].Yposition);
                        Cars[i].Frein = false;
                    }
                }
        }

        public void Avance_ligne2()
        {
            _distanceEntreVehicule = (int) ChoixDistanceEntreVehicules.Value;

            double distancePtcritique = 100;
            var vitessemax = ((ChoixVitessemax.Value/3.6)* 0.02) / 0.25;
            var acceleration = -0.002 * Math.Log(ChoixAcceleration.Value) + 0.0088;
            var deceleration = ChoixDeceleration.Value;

            for (var i = 0; i < Cars2.Count; i++)
                if (i == 0)
                {
                    if (Cars2[0].Xposition <= _pointCritique - distancePtcritique)
                    {
                        Canvas.SetLeft(Cars2[0], Cars2[0].Move(vitessemax, acceleration, deceleration));
                        Canvas.SetBottom(Cars2[0], Cars2[0].Yposition);
                    }
                    else
                    {
                        var position = Champ_libre(Cars2[0].Xposition);
                        if (position != -1)
                        {
                            Changement_ligne(position, 0);
                        }
                        else
                        {
                            Cars2[0].Frein = true;
                            Canvas.SetLeft(Cars2[0], Cars2[0].Move(vitessemax, acceleration, deceleration));
                            Canvas.SetBottom(Cars2[0], Cars2[0].Yposition);
                            Cars2[0].Frein = false;
                        }
                    }
                }
                else //On fait avancer les autres voitures en  véfrifant devant
                {
                    if (Cars2[i].Xposition <= _pointCritique - distancePtcritique)
                    {
                        if (Cars2[i].Xposition + Cars2[i].Width < Cars2[i - 1].Xposition - _distanceEntreVehicule)
                        {
                            Canvas.SetLeft(Cars2[i], Cars2[i].Move(vitessemax, acceleration, deceleration));
                            Canvas.SetBottom(Cars2[i], Cars2[i].Yposition);
                        }
                        else
                        {
                            Cars2[i].Frein = true;
                            Canvas.SetLeft(Cars2[i], Cars2[i].Move(vitessemax, acceleration, deceleration));
                            Canvas.SetBottom(Cars2[i], Cars2[i].Yposition);
                            Cars2[i].Frein = false;
                        }
                    }
                    else
                    {
                        var position = Champ_libre(Cars2[i].Xposition);
                        if (position != -1)
                        {
                            Changement_ligne(position, i);
                        }
                        else
                        {
                            Cars2[i].Frein = true;
                            Canvas.SetLeft(Cars2[i], Cars2[i].Move(vitessemax, acceleration, deceleration));
                            Canvas.SetBottom(Cars2[i], Cars2[i].Yposition);
                            Cars2[i].Frein = false;
                        }
                    }
                }
        }

        public int Champ_libre(double xposition)
        {
            _distanceEntreVehicule = (int) ChoixDistanceEntreVehicules.Value;

            var autoriseChampLibre = -1;
            for (var i = 0; i < Cars.Count; i++)
                if (Cars[i].Xposition >= xposition - _distanceEntreVehicule &&
                    Cars[i].Xposition <= xposition + _distanceEntreVehicule)
                {
                    autoriseChampLibre = -1;
                    break;
                }
                else
                {
                    autoriseChampLibre = i;
                }

            if (autoriseChampLibre != -1)
                for (var i = Cars.Count - 1; i != 0; i--)
                    if (Cars[i].Xposition < xposition)
                    {
                        autoriseChampLibre = 0;
                    }
                    else
                    {
                        autoriseChampLibre = i;
                        break;
                    }

            return autoriseChampLibre;
        }

        public void Changement_ligne(int position, int i)
        {
            _distanceEntreVehicule = (int) ChoixDistanceEntreVehicules.Value;

            var vitessemax = ((ChoixVitessemax.Value / 3.6) * 0.02) / 0.25;
            var acceleration = -0.002 * Math.Log(ChoixAcceleration.Value) + 0.0088;
            var deceleration = ChoixDeceleration.Value;

            Voiture temp = Cars2[i];
            Cars2.RemoveAt(i);
            if (position + 1 > Cars.Count) //Si on dépasse la valeur 
                Cars.Add(temp);
            else
                Cars.Insert(position + 1, temp);
            //On affiche cette voiture et on la fait avancer
            Cars[position + 1].Yposition = PositionL1;
            Canvas.SetLeft(Cars[position + 1], Cars[position + 1].Move(vitessemax, acceleration, deceleration));
            Canvas.SetBottom(Cars[position + 1], Cars[position + 1].Yposition);
        }

        public void Retour_vehicules()
        {
            var vitessemax = ((ChoixVitessemax.Value / 3.6) * 0.02) / 0.25;
            var acceleration = -0.002 * Math.Log(ChoixAcceleration.Value) + 0.0088;
            var deceleration = ChoixDeceleration.Value;

            if (Cars[0].Xposition >= Bordure.ActualWidth - 16)
            {
                Voiture temp = Cars[0];
                Cars.RemoveAt(0);
                temp.Xposition = 0;
                temp.Vitesse = vitessemax;
                temp.Vehiculelent = false;
                var relativeUri = new Uri("Images/automobile.png", UriKind.Relative);
                temp.Source = new BitmapImage(relativeUri);
                if (_rand.Next(100) < ChoixProportionVoituregauche.Value)
                    temp.Lane = 2;
                else
                    temp.Lane = 1;
                if (_rand.Next(100) < ChoixDensitecamion.Value)
                {
                    Ajoutcamion(temp);
                }
                else
                {
                    temp.Width = 16;
                }
                if (temp.Lane == 2)
                {
                    temp.Yposition = PositionL2;
                    Cars2.Add(temp);
                    Canvas.SetLeft(temp, temp.Move(vitessemax, acceleration, deceleration));
                    Canvas.SetBottom(temp, temp.Yposition);
                }
                else
                {
                    Cars.Add(temp);
                    Canvas.SetLeft(temp, temp.Move(vitessemax, acceleration, deceleration));
                    Canvas.SetBottom(temp, temp.Yposition);
                }
            }
        }

        public void ModificationNbVehicules()
        {
            var relanceTimers = false;
            //Si les timers sont actifs (bouton start déjà appuyé une fois) on arrete les timers et on devra relancer les timers  à la fin
            if (_timer1.IsEnabled || _timer2.IsEnabled)
            {
                _timer1.Stop();
                _timer2.Stop();
                relanceTimers = true;
            }

            _distanceEntreVehicule = (int) ChoixDistanceEntreVehicules.Value;
            var densiteCamion = (int) ChoixDensitecamion.Value;

            Nbvoitures = Cars.Count + Cars2.Count;

            var nbajout = (int) ChoixNombrevoitures.Value - Nbvoitures;
            //Arrondit pour respecter le choix de proportion de chaque côté
            var nbajoutVoiegauche =  (int) Math.Round(nbajout *  (ChoixProportionVoituregauche.Value / 100), 0);
            var nbajoutVoiedroite = nbajout - nbajoutVoiegauche;

            if (nbajout > 0)
            {
                //Proportion en plus dans la voie gauche
                var i = nbajoutVoiegauche;
                while (i != 0)
                {
                    double positionDernier = Cars2.Last().Xposition;
                    var voiture = new Voiture
                    {
                        Lane = 2,
                        Vitesse = ((ChoixVitessemax.Value / 3.6) * 0.02) / 0.25,
                        Yposition = PositionL2,
                        Xposition = positionDernier - 3*_distanceEntreVehicule //A VOIR
                    };
                    Cars2.Add(voiture);
                    Affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture.Xposition);
                    Canvas.SetBottom(voiture, voiture.Yposition);
                    i--;
                }

                var j = nbajoutVoiedroite;
                while (j != 0)
                {
                    double positionDernier = Cars.Last().Xposition;
                    Voiture voiture = new Voiture
                    {
                        Lane = 1,
                        Vitesse =  ((ChoixVitessemax.Value / 3.6) * 0.02) / 0.25,
                        Yposition = PositionL1,
                        Xposition = positionDernier - 3*_distanceEntreVehicule
                    };
                    Cars.Add(voiture);
                    Affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture.Xposition);
                    Canvas.SetBottom(voiture, voiture.Yposition);
                    j--;
                }
            }
            //Sinon, le nombre de voitures a baissé, ...
            else
            {
                for (var i = 0; i > nbajoutVoiegauche; i--)
                {
                    Affichage.Children.Remove(Cars2[Cars2.Count - 1]);
                    Cars2.RemoveAt(Cars2.Count - 1);
                }

                for (var i = 0; i > nbajoutVoiedroite; i--)
                {
                    Affichage.Children.Remove(Cars[Cars.Count - 1]);
                    Cars.RemoveAt(Cars.Count - 1);
                }
            }

            if (relanceTimers)
            {
                _timer1.Start();
                _timer2.Start();
            }
        }

        //UI ELEMENTS
        public void Start(object sender, RoutedEventArgs e)
        {
            if (Chargement)
            {
                var nbvoituresVoiegauche =
                    (int) (ChoixNombrevoitures.Value * (ChoixProportionVoituregauche.Value / 100));
                var nbvoituresVoiedroite = (int) ChoixNombrevoitures.Value - nbvoituresVoiegauche;
                var densiteCamion = (int) ChoixDensitecamion.Value;
                _distanceEntreVehicule = (int) ChoixDistanceEntreVehicules.Value;

                var i = nbvoituresVoiegauche;
                while (i != 0)
                {
                    Voiture voiture = new Voiture();
                    if (_rand.Next(100) < densiteCamion && Chargement) //Pas besoin de faire la densité de camion pour les autres cas
                    {
                        Ajoutcamion(voiture);
                    }
                    voiture.Lane = 2;
                    voiture.Vitesse = ((ChoixVitessemax.Value / 3.6) * 0.02) / 0.25;
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
                    Voiture voiture = new Voiture();
                    if (_rand.Next(100) < densiteCamion && Chargement
                    ) //Pas besoin de faire la densité de camion pour les autres cas
                    {
                        Ajoutcamion(voiture);
                    }
                    voiture.Lane = 1;
                    voiture.Vitesse = ((ChoixVitessemax.Value / 3.6) * 0.02) / 0.25;
                    voiture.Yposition = PositionL1;
                    voiture.Xposition = j * _distanceEntreVehicule * 2 + ChoixNombrevoitures.Value;
                    Cars.Add(voiture);
                    Affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture.Xposition);
                    Canvas.SetBottom(voiture, voiture.Yposition);
                    j--;
                }
                Chargement = false;
            }

            _timer1.Start();
            _timer2.Start();
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
            VitessemaxChoixAffichage.Content = "Vitesse max : " + Math.Round(ChoixVitessemax.Value, 3) +"km/h";
        }

        private void Choix_acceleration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Acceleration renvoyée en pixel/20ms2
            //On exprime va afficher ici le 0 à 100
            AccelerationmaxChoixAffichage.Content = "Accélération max (0 à 100) : " + Math.Round(ChoixAcceleration.Value, 3) +"s";
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