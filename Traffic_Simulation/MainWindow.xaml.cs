using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Wpf.Gauges;

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
        public int Nbvoitures;
        private readonly int _pointCritique = 800;
        public int PositionL1 = 80;
        public int PositionL2 = 110;

        private readonly Random _rand = new Random();
        private readonly DispatcherTimer _timer1 = new DispatcherTimer();
        private readonly DispatcherTimer _timer2 = new DispatcherTimer();
        private readonly DispatcherTimer _timer3 = new DispatcherTimer();
        private readonly DispatcherTimer _timerGauges = new DispatcherTimer();

        //VALEURS CHANGEANTES
        private static double vitessemax = 0;
        private static double acceleration = 0;
        private static double deceleration = 0;
        private int _distanceEntreVehicule = 0;
        private static int distancePtcritique = 0;
        private static int distanceAnalyse = 0;



        public static List<ObservableValue> VitesseValeurs = new List<ObservableValue>() ;
        public static List<ObservableValue> NbVehiculesArretValeurs = new List<ObservableValue>();


        public MainWindow()
        {
            InitializeComponent();
            _timer1.Tick += timer1_Tick;
            _timer1.Interval = TimeSpan.FromMilliseconds(20);
            _timer2.Tick += timer2_Tick;
            _timer2.Interval = TimeSpan.FromMilliseconds(20);
            _timer3.Tick += timer3_Tick;
            _timer3.Interval = TimeSpan.FromSeconds(3);
            _timerGauges.Tick += timerGauges_Tick;
            _timerGauges.Interval = TimeSpan.FromSeconds(1);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            vitessemax = ((ChoixVitessemax.Value / 3.6) * 0.02) / 0.25;
            acceleration = -0.002 * Math.Log(ChoixAcceleration.Value) + 0.0088;
            deceleration = ChoixDeceleration.Value;
            _distanceEntreVehicule = (int)ChoixDistanceEntreVehicules.Value;
            distancePtcritique = 100;
            distanceAnalyse = 500;


            Avance_ligne1();
            Retour_vehicules();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            Avance_ligne2();
            if (Cars.Count + Cars2.Count != (int)ChoixNombrevoitures.Value) ModificationNbVehicules();
            NbVoitures1.Content = "Ligne 1 : " + Cars.Count;
            NbVoitures2.Content = "Ligne 1 : " + Cars2.Count;
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            MiseajourVitesseMoy();
            MiseajourNbVehiculesArret();
        }

        private void timerGauges_Tick(object sender, EventArgs e)
        {
            Gaugetest.Value = Vitessemoyenne();
            GaugeNbvehiculesArret.Value = NbVehiculesArret();
        }

        /*public void ProgressBarcoloration()
        {
            PourcentageVitesse.Value = (Vitessemoyenne()*100)/ChoixVitessemax.Value;
            byte rouge = Convert.ToByte( ((100 - PourcentageVitesse.Value) / 100) * 255);
            byte vert = Convert.ToByte(((PourcentageVitesse.Value / 100) * 255));
            PourcentageVitesse.Foreground = new SolidColorBrush(Color.FromArgb(255, rouge, vert, 0));
        }*/

        public void MiseajourVitesseMoy()
        {
            for (int i = 0; i < 19; i++)
            {
                VitesseValeurs[i].Value = VitesseValeurs[i + 1].Value;
            }
            VitesseValeurs[19].Value = Gaugetest.Value; //On prend la valeur que l'on a déjà calculé précédement
        }
        public void MiseajourNbVehiculesArret()
        {
            for (int i = 0; i < 19; i++)
            {
                NbVehiculesArretValeurs[i].Value = NbVehiculesArretValeurs[i + 1].Value;
            }
            NbVehiculesArretValeurs[19].Value = GaugeNbvehiculesArret.Value; //On prend la valeur que l'on a déjà calculé précédement
        }

        public int NbVehiculesArret()
        {
            int n=0;
            for (int i =0; i< Cars.Count ; i++)
            {
                if (Cars[i].Vitesse < 0.2)
                {
                    n++;
                }
            }
            for (int i = 0; i < Cars2.Count ; i++)
            {
                if (Cars2[i].Vitesse <0.2)
                {
                    n++;
                }
            }
            return n;
        }
        
        public void Avance_ligne1()
        {
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
            for (var i = 0; i < Cars2.Count; i++)
                if (i == 0)
                {   //Pour la première voiture on vérifie seulement si elle est dans le point critique
                    if (Cars2[0].Xposition <= _pointCritique - distanceAnalyse)
                    {
                        Canvas.SetLeft(Cars2[0], Cars2[0].Move(vitessemax, acceleration, deceleration));
                        Canvas.SetBottom(Cars2[0], Cars2[0].Yposition);
                    }
                    else //La voiture est dans la zone de changement
                    {
                        var position = Champ_libre(Cars2[0].Xposition);
                        if (position != -1 )
                        {
                            Changement_ligne(position, 0);
                        }
                        else
                        {//On freine si on est dans la zone critique
                            if ((Cars2[0].Xposition > _pointCritique - distancePtcritique))
                            {
                                Cars2[0].Frein = true;
                                Canvas.SetLeft(Cars2[0], Cars2[0].Move(vitessemax, acceleration, deceleration));
                                Canvas.SetBottom(Cars2[0], Cars2[0].Yposition);
                                Cars2[0].Frein = false;
                            }
                            else
                            {//si on est pas dans la distance critique on peut continuer à avancer
                                Canvas.SetLeft(Cars2[0], Cars2[0].Move(vitessemax, acceleration, deceleration));
                                Canvas.SetBottom(Cars2[0], Cars2[0].Yposition);
                            }
                        }
                    }
                }
                else //On fait avancer les autres voitures en  véfrifant devant
                {   
                    if (Cars2[i].Xposition <= _pointCritique - distanceAnalyse)//Verification zone analyse
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
                    //Sinon le vehicule est dans la zone d'analyse
                    else
                    {//Recherde de position libre sur l'autre voie
                        var position = Champ_libre(Cars2[i].Xposition);
                        if (position != -1)
                        {
                            Changement_ligne(position, i);
                        }
                        else
                        {//On freine si on est dans la zone critique
                            if ((Cars2[i].Xposition > _pointCritique - distancePtcritique))
                            {
                                Cars2[i].Frein = true;
                                Canvas.SetLeft(Cars2[i], Cars2[i].Move(vitessemax, acceleration, deceleration));
                                Canvas.SetBottom(Cars2[i], Cars2[i].Yposition);
                                Cars2[i].Frein = false;
                            }
                            else {//si on n'est pas dans la distance critique on peut continuer à avancer
                                Canvas.SetLeft(Cars2[i], Cars2[i].Move(vitessemax, acceleration, deceleration));
                                Canvas.SetBottom(Cars2[i], Cars2[i].Yposition);
                            }
                        }
                    }
                }
        }

        public int Champ_libre(double xposition)
        {
            var autoriseChampLibre = -1;
            for (var i = 0; i < Cars.Count; i++)//Quelqu'un sur la voie voie opposée à cette voiture sur l'autre partie de la route
                if (Cars[i].Xposition >= xposition - _distanceEntreVehicule &&
                    Cars[i].Xposition <= xposition + 2* _distanceEntreVehicule)
                {
                    autoriseChampLibre = -1;
                    break; //On peut arreter de chercher il y a deja un vehicule
                }
                else
                {
                    autoriseChampLibre = i;
                }

            if (autoriseChampLibre != -1)//Personne, alors on prend la place de la voiture jsute derriere celle qui avait la position de cars2
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
            Voiture temp = Cars2[i];
            Cars2.RemoveAt(i);
            if (position + 1 > Cars.Count) //Si on dépasse la valeur 
                Cars.Add(temp);
            else
                Cars.Insert(position + 1, temp);

            //On affiche cette voiture et on la fait avancer
            Cars[position+1].ChangementL = true;//On active le dépacement en Y
            Canvas.SetLeft(Cars[position + 1], Cars[position + 1].Move(vitessemax, acceleration, deceleration));
            Canvas.SetBottom(Cars[position + 1], Cars[position + 1].Yposition);
        }

        public void Retour_vehicules()
        {
            if (Cars[0].Xposition >= Colonne1.ActualWidth - 16)
            {
                Voiture temp = Cars[0];
                Cars.RemoveAt(0);
                temp.Xposition = -10; //On place les voitures hors cadre pour éviter les voitures entassées à gauche
                temp.Vitesse = vitessemax;
                temp.Vehiculelent = false;
                var relativeUri = new Uri("Images/automobile.png", UriKind.Relative);
                temp.Source = new BitmapImage(relativeUri);
                //On réevalue la voie du véhicule 
                if (_rand.Next(100) < ChoixProportionVoituregauche.Value)
                    temp.Lane = 2;
                else
                    temp.Lane = 1;

                //Densité de camion
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
                    temp.Yposition = PositionL2;//Mettre la position verticale
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
            var relanceTimers = false;//Bool pour relancer les timers
            //Si les timers sont actifs (bouton start déjà appuyé une fois) on arrete les timers et on devra relancer les timers  à la fin
            if (_timer1.IsEnabled || _timer2.IsEnabled)
            {
                _timer1.Stop();
                _timer2.Stop();
                relanceTimers = true;
            }
            var densiteCamion = (int) ChoixDensitecamion.Value;
            Nbvoitures = Cars.Count + Cars2.Count;

            var nbajout = (int) ChoixNombrevoitures.Value - Nbvoitures;
            //Arrondit pour respecter le choix de proportion de chaque côté
            var nbajoutVoiegauche = (int) Math.Round(nbajout * (ChoixProportionVoituregauche.Value / 100), 0);
            var nbajoutVoiedroite = nbajout - nbajoutVoiegauche;

            if (nbajout > 0)
            {
                //Proportion en plus dans la voie gauche
                var i = nbajoutVoiegauche;
                while (i != 0)
                {
                    double positionDernier;
                    //On recupere la positiion de la derniere voiture 
                    if (Cars2.Count == 0)
                    {
                    positionDernier = 0;
                    }
                    else
                    {
                    positionDernier = Cars2.Last().Xposition;
                    }

                var voiture = new Voiture
                    {
                        Lane = 2,
                        Vitesse = ((ChoixVitessemax.Value / 3.6) * 0.02) / 0.25,
                        Yposition = PositionL2,
                        Xposition = positionDernier - 3 * _distanceEntreVehicule //Valeur arbitraire, on laisse assez de place en cas de freinage
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
                    double positionDernier;
                    if (Cars.Count == 0)
                    {
                        positionDernier = 0;
                    }
                    else
                    {
                        positionDernier = Cars.Last().Xposition;
                    }

                    Voiture voiture = new Voiture
                    {
                        Lane = 1,
                        Vitesse = ((ChoixVitessemax.Value / 3.6) * 0.02) / 0.25,
                        Yposition = PositionL1,
                        Xposition = positionDernier - 3 * _distanceEntreVehicule
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
                    if (Cars2.Count == 0)//Il n'y a pas assez de vehicules à enlever sur cette voie
                    {
                        nbajoutVoiedroite = nbajoutVoiedroite - nbajoutVoiegauche; //On enlève les voitures sur l'autre voie
                        break;
                    }
                    Affichage.Children.Remove(Cars2[Cars2.Count - 1]);
                    Cars2.RemoveAt(Cars2.Count - 1);
                }

                for (var i = 0; i > nbajoutVoiedroite; i--)
                {
                    if (Cars.Count == 0)//Pas de voiture à enlever sur cette voie
                    {
                        break;
                    }
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

        public double Vitessemoyenne()
        {
            double vitessemoy = 0;
            for (int i = 0; i < Cars.Count; i++)
            {
                vitessemoy += Cars[i].Vitesse;
            }
            for (int i = 0; i < Cars2.Count; i++)
            {
                vitessemoy += Cars2[i].Vitesse;
            }

            vitessemoy = vitessemoy / (Cars.Count + Cars2.Count);
            vitessemoy = (((vitessemoy * 0.25) / 0.02) * 3.6);
            return vitessemoy;
        }


    }

}

