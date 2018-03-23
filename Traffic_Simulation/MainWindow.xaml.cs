using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LiveCharts.Defaults;
using Microsoft.Office.Interop.Excel;
using Window = System.Windows.Window;
using Excel = Microsoft.Office.Interop.Excel;
namespace Simulateur_0._0._2
{
    /// <summary>
    ///     Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly List<Voiture> Cars = new List<Voiture>();
        private static readonly List<Voiture> Cars2 = new List<Voiture>();

        private static List<Voiture> CarsCopie = new List<Voiture>();
        private static List<Voiture> Cars2Copie = new List<Voiture>();

        //VALEURS CHANGEANTES
        private static double vitessemax;
        private static double acceleration;
        private static double deceleration;
        private static int distancePtcritique;
        private static int distanceAnalyse;


        public static List<ObservableValue> VitesseValeurs = new List<ObservableValue>();
        public static List<ObservableValue> NbVehiculesArretValeurs = new List<ObservableValue>();
        public static List<ObservableValue> TempsPasseRoute = new List<ObservableValue>();
        public static List<ObservableValue> TempsPasseArret = new List<ObservableValue>();


        public static List<HeatPoint> HeatMapValeurs1 = new List<HeatPoint>();
        public static List<HeatPoint> HeatMapValeurs2 = new List<HeatPoint>();


        public static double ValVmoyLabel;
        public static int ValNbArretLabel;
        public static int ValTempsPasseRouteLabel;
        public static int ValTempsPasseArretLabel;


        public static List<double> Vmoy = new List<double>();
        public static List<int> Nbarret = new List<int>();
        public static List<int> MoyLabelTempsPasseRoute = new List<int>();
        public static List<int> MoyLabelTempsPasseArret = new List<int>();

        private readonly int _pointCritique = 900;

        private readonly Random _rand = new Random();
        private readonly DispatcherTimer _timer1 = new DispatcherTimer();
        private readonly DispatcherTimer _timer2 = new DispatcherTimer();
        private readonly DispatcherTimer _timer3 = new DispatcherTimer();
        private readonly DispatcherTimer _timerGauges = new DispatcherTimer();
        private int _distanceEntreVehicule;

        public bool Chargement = true;
        public int Nbvoitures;
        public int PositionL1 = 80;
        public int PositionL2 = 110;


        public static double VitesseTimerSimulation = 5; //ms
        public static double VitesseTimerGauge = 1; //s
        public static double VitesseTimerGraph = 3; //s

        public static bool recherchermesure = true;
        Stopwatch sw = new Stopwatch();


        public MainWindow()
        {
            InitializeComponent();
        }
        public void InitTimer()
        {
            _timer1.Tick += timer1_Tick;
            _timer1.Interval = TimeSpan.FromMilliseconds(VitesseTimerSimulation);
            _timer2.Tick += timer2_Tick;
            _timer2.Interval = TimeSpan.FromMilliseconds(VitesseTimerSimulation);
            _timer3.Tick += timer3_Tick;
            _timer3.Interval = TimeSpan.FromSeconds(VitesseTimerGraph);
            _timerGauges.Tick += timerGauges_Tick;
            _timerGauges.Interval = TimeSpan.FromSeconds(VitesseTimerGauge);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            vitessemax = ChoixVitessemax.Value / 3.6 * 0.02 / 0.25;
            acceleration = -0.002 * Math.Log(ChoixAcceleration.Value) + 0.0088;
            deceleration = ChoixDeceleration.Value;
            _distanceEntreVehicule = (int) ChoixDistanceEntreVehicules.Value;
            distancePtcritique = 100;
            distanceAnalyse = 500;

            Avance_ligne1();
            Retour_vehicules();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (recherchermesure)//On cherche une voiture pour mesure son temps 
            {
                MesureTemps();
            }
            Avance_ligne2();
            if (Cars.Count + Cars2.Count != (int) ChoixNombrevoitures.Value) ModificationNbVehicules();
            NbVoitures1.Content = "Ligne 1 : " + Cars.Count;
            NbVoitures2.Content = "Ligne 1 : " + Cars2.Count;
            
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            UpdateGraphVitesseMoy();
            UpdateGraphNbVehiculesArret();
            UpdateGraphTempsPasseArret();
            UpdateGraphTempsPasseRoute();

            UpdateLabelVitesseMoyenne(GaugeVitesse.Value);
            UpdateLabelNbVehiculesArret((int)GaugeNbvehiculesArret.Value);
            UpdateLabelTempsPasseRoute((int)GaugeTempsPasseRoute.Value);
            UpdateLabelTempsPasseRoute((int)GaugeTempsPasseArret.Value);
        }

        private void timerGauges_Tick(object sender, EventArgs e)
        {
            CarsCopie = Cars;
            Cars2Copie = Cars2;
            GaugeVitesse.Value = Vitessemoyenne();
            VitesseMoyenneSimulateur.Add(GaugeVitesse.Value);
            GaugeTempsPasseArret.Value = TempsPasseBouchon();
            GaugeNbvehiculesArret.Value = NbVehiculesArret();

            UpdateHeatMap();

        }


        public void UpdateHeatMap()
        {
            for (var j = 0; j < 6; j++) HeatMapValeurs1[j].Weight = 0;
            for (var j = 0; j < 5; j++) HeatMapValeurs2[j].Weight = 0;
            for (var i = 0; i < CarsCopie.Count; i++)
            for (var j = 0; j < 6; j++)
                if (CarsCopie[i].Xposition < (j + 1) * 166)
                {
                    HeatMapValeurs1[j].Weight++;
                    break;
                }

            if (Cars2Copie.Count == 0) //Si pas de voitures dans la voie 2 tout vider
                for (var j = 0; j < 5; j++)
                    HeatMapValeurs2[j].Weight = 0;
            else
                for (var i = 0; i < Cars2Copie.Count; i++)
                for (var j = 0; j < 5; j++)
                    if (Cars2Copie[i].Xposition < (j + 1) * 200)
                    {
                        HeatMapValeurs2[j].Weight++;
                        break;
                    }
        }


        //----------------PannelVitesse-------------------------
        public double Vitessemoyenne()
        {
            double vitessemoy = 0.1;
            var i = 0;
            if (CarsCopie.Count != 0)
            {
                if (CarsCopie[0].Xposition > 0)
                {
                    while (CarsCopie[i].Xposition > 0)
                    {
                        vitessemoy += CarsCopie[i].Vitesse;
                        i++;
                        if (i == CarsCopie.Count) break;
                    }
                }
                else
                {
                    i = 1; //Eviter division par zero
                }

            }
            var j = 0;
            if (Cars2Copie.Count != 0)
                while (Cars2Copie[j].Xposition > 0)
                {
                    vitessemoy += Cars2Copie[j].Vitesse;
                    j++;
                    if (j == Cars2Copie.Count) break;
                }
            vitessemoy = vitessemoy / (i + j);
            vitessemoy = (((vitessemoy * 0.25) / 0.02) * 3.6);
            return vitessemoy;
        }
        public void UpdateLabelVitesseMoyenne(double ajoutvitesse)
        {
            Vmoy.RemoveAt(0);
            Vmoy.Add(ajoutvitesse);
            ValVmoyLabel = Vmoy.Sum();
            ValVmoyLabel = ValVmoyLabel / Vmoy.Count;
            LabelVitesseMoyenne.Content = Math.Round(ValVmoyLabel, 0) + " km/h";
        }
        public void UpdateGraphVitesseMoy()
        {
            for (var i = 0; i < 19; i++) VitesseValeurs[i].Value = VitesseValeurs[i + 1].Value;
            VitesseValeurs[19].Value = GaugeVitesse.Value; //On prend la valeur que l'on a déjà calculé précédement
        }

        //----------------PannelNbArret-------------------------
        public int NbVehiculesArret()
        {
            var n = 0;
            if (CarsCopie.Count != 0)
            {
                var i = 0;
                while (CarsCopie[i].Xposition >= 0)
                {
                    if (CarsCopie[i].Vitesse < 0.2)
                    {
                        n++; //0,2 vaut moins de dix km/h
                    }
                    i++;
                    if (i == CarsCopie.Count) break;
                }
            }

            if (Cars2Copie.Count != 0)
            {
                var j = 0;
                while (Cars2Copie[j].Xposition > 0)
                {
                    if (Cars2Copie[j].Vitesse < 0.2)
                    {
                        n++;
                    }
                    j++;
                    if (j == Cars2Copie.Count) break;
                }
            }

            return n;
        }
        public void UpdateGraphNbVehiculesArret()
        {
            for (var i = 0; i < 19; i++) NbVehiculesArretValeurs[i].Value = NbVehiculesArretValeurs[i + 1].Value;
            NbVehiculesArretValeurs[19].Value =
                GaugeNbvehiculesArret.Value; //On prend la valeur que l'on a déjà calculé précédement
        }
        public void UpdateLabelNbVehiculesArret(int ajoutarret)
        {
            Nbarret.RemoveAt(0);
            Nbarret.Add(ajoutarret);
            ValNbArretLabel = Nbarret.Sum();
            ValNbArretLabel = ValNbArretLabel / Nbarret.Count;
            LabelNbVehiculesArret.Content = ValNbArretLabel + " véhicules";
        }

        //----------------PannelTempsPasseRoute-------------------------
        public void MesureTemps() //relance la mesure du temps
        {
            for (int i = 0; i < Cars.Count; i++)
            {
                if (Cars[i].Xposition > 0 && Cars[i].Xposition < 5)
                {
                    sw.Start();
                    Cars[i].vm = true;
                    recherchermesure = false;
                    break;
                }
            }
        }
        public void UpdateGraphTempsPasseRoute()
        {
            for (var i = 0; i < 19; i++) TempsPasseRoute[i].Value = TempsPasseRoute[i + 1].Value;
            TempsPasseRoute[19].Value = GaugeTempsPasseRoute.Value; //On prend la valeur que l'on a déjà calculé précédement
        }
        public void UpdateLabelTempsPasseRoute(int ajouttemps)
        {
            MoyLabelTempsPasseRoute.RemoveAt(0);
            MoyLabelTempsPasseRoute.Add(ajouttemps);
            ValTempsPasseRouteLabel = MoyLabelTempsPasseRoute.Sum();
            ValTempsPasseRouteLabel = ValTempsPasseRouteLabel / MoyLabelTempsPasseRoute.Count;
            LabelTempsPasseRoute.Content = ValTempsPasseRouteLabel + " secondes";
        }

        //----------------PannelTempsPasseArret-------------------------

        public void UpdateGraphTempsPasseArret()
        {
            for (var i = 0; i < 19; i++) TempsPasseRoute[i].Value = TempsPasseRoute[i + 1].Value;
            TempsPasseRoute[19].Value = GaugeTempsPasseRoute.Value;
        }
        public double TempsPasseBouchon()//Mettre dans jauge
        {
            if (CarsCopie.Count != 0)
            {
                double tp = 0;
                for (int i = 0; i < CarsCopie.Count; i++)
                {
                    tp += CarsCopie[i].TempsPasseBouchon;
                }

                for (int i = 0; i < Cars2Copie.Count; i++)
                {
                    tp += Cars2Copie[i].TempsPasseBouchon;
                }

                tp = tp / (Cars2Copie.Count + CarsCopie.Count); //Temps passé en 20 milisecondes (tp *20ms) 
                tp = (tp * (VitesseTimerSimulation)) / 1000;
                return Math.Round(tp, 1); // On passe de millisecondes à secondes
            }
            else return 0;
        }
        public void UpdateLabelTempsPasseArret(int ajouttemps)
        {
            MoyLabelTempsPasseArret.RemoveAt(0);
            MoyLabelTempsPasseArret.Add(ajouttemps);
            ValTempsPasseArretLabel = MoyLabelTempsPasseArret.Sum();
            ValTempsPasseArretLabel = ValTempsPasseArretLabel / MoyLabelTempsPasseArret.Count;
            LabelTempsPasseRoute.Content = ValTempsPasseArretLabel + " secondes";
        }


        //----------------Affichage et mouvement----------------------
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
                if (i == 0) //Voiture de tête
                {
                    //Pour la première voiture on vérifie seulement si elle est dans zone d'analyse
                    if (Cars2[0].Xposition <= _pointCritique - distanceAnalyse)
                    {
                        Canvas.SetLeft(Cars2[0], Cars2[0].Move(vitessemax, acceleration, deceleration));
                        Canvas.SetBottom(Cars2[0], Cars2[0].Yposition);
                    }
                    else //La voiture est dans la zone d'analyse
                    {
                        var force = false;
                        //On freine si on est dans la zone critique
                        if (Cars2[0].Xposition > _pointCritique - distancePtcritique
                        ) //On regarde si on est dans la zone critique
                        {
                            //Si c'est le cas, on cherche une postion ou on freine
                            force = true;
                            var position = Champ_libre(Cars2[0].Xposition, force);
                            if (position != -1) //On peut changer de voie en forcant
                            {
                                Changement_ligne(position, i);
                            }
                            else //pas de place même en forçant, donc on freine
                            {
                                Cars2[0].Frein = true;
                                Canvas.SetLeft(Cars2[0], Cars2[0].Move(vitessemax, acceleration, deceleration));
                                Canvas.SetBottom(Cars2[0], Cars2[0].Yposition);
                                Cars2[0].Frein = false;
                            }
                        }
                        else //on n'est pas dans la zone critique
                        {
                            force = false; //On recherchera une position sans forcer
                            var position = Champ_libre(Cars2[i].Xposition, force);
                            if (position != -1)
                            {
                                Changement_ligne(position, i); //On change sans forcer
                            }
                            else
                            {
                                //si on est pas dans la distance critique on peut continuer à avancer
                                Canvas.SetLeft(Cars2[0], Cars2[0].Move(vitessemax, acceleration, deceleration));
                                Canvas.SetBottom(Cars2[0], Cars2[0].Yposition);
                            }
                        }
                    }
                }
                else //On fait avancer les autres voitures en  véfrifant devant
                {
                    if (Cars2[i].Xposition <= _pointCritique - distanceAnalyse) //Verification zone non analyse
                    {
                        //Avance normalement
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
                    {
                        //Recherde de position libre sur l'autre voie
                        var force = false;
                        if (Cars2[i].Xposition > _pointCritique - distancePtcritique
                        ) //On regarde si on est dans la zone critique
                        {
                            //Si c'est le cas, on cherche une postion ou on freine
                            force = true;
                            var position = Champ_libre(Cars2[i].Xposition, force);
                            if (position != -1) //On peut changer de voie en forcant
                            {
                                Changement_ligne(position, i);
                            }
                            else //pas de place même en forçant, donc on freine
                            {
                                Cars2[i].Frein = true;
                                Canvas.SetLeft(Cars2[i], Cars2[i].Move(vitessemax, acceleration, deceleration));
                                Canvas.SetBottom(Cars2[i], Cars2[i].Yposition);
                                Cars2[i].Frein = false;
                            }
                        }
                        else //on n'est pas dans la zone critique
                        {
                            force = false; //On recherchera une position sans forcer
                            var position = Champ_libre(Cars2[i].Xposition, force);
                            if (position != -1)
                            {
                                Changement_ligne(position, i);
                            }
                            else
                            {
                                //si on n'est pas dans la distance critique on peut continuer à avancer
                                Canvas.SetLeft(Cars2[i], Cars2[i].Move(vitessemax, acceleration, deceleration));
                                Canvas.SetBottom(Cars2[i], Cars2[i].Yposition);
                            }
                        }
                    }
                }
        }

        public int Champ_libre(double xposition, bool force)
        {
            var autoriseChampLibre = -1;
            double distancesecu = _distanceEntreVehicule;
            if (!force) //la distance de securite sera plus elevee si on ne force pas, on augmente donc la distance
                distancesecu = _distanceEntreVehicule * 2;
            for (var i = 0;
                i < Cars.Count;
                i++) //Quelqu'un sur la voie voie opposée à cette voiture sur l'autre partie de la route
                if (Cars[i].Xposition >= xposition - _distanceEntreVehicule &&
                    Cars[i].Xposition <= xposition + distancesecu)
                {
                    autoriseChampLibre = -1;
                    break; //On peut arreter de chercher il y a deja un vehicule
                }
                else
                {
                    autoriseChampLibre = i;
                }

            if (autoriseChampLibre != -1
            ) //Personne, alors on prend la place de la voiture jsute derriere celle qui avait la position de cars2
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
            var temp = Cars2[i];
            Cars2.RemoveAt(i);
            if (position + 1 > Cars.Count) //Si on dépasse la valeur 
                Cars.Add(temp);
            else
                Cars.Insert(position + 1, temp);

            //On affiche cette voiture et on la fait avancer
            Cars[position + 1].ChangementL = true; //On active le dépacement en Y
            Canvas.SetLeft(Cars[position + 1], Cars[position + 1].Move(vitessemax, acceleration, deceleration));
            Canvas.SetBottom(Cars[position + 1], Cars[position + 1].Yposition);
        }

        public void Retour_vehicules()
        {
            if(Cars.Count != 0)
            {
                if (Cars[0].Xposition >= Colonne1.ActualWidth - 16)
                {
                    //MESURE TEMPS
                    if(Cars[0].vm)
                    {
                        Cars[0].vm = false;
                        TimeSpan ts = sw.Elapsed;
                        sw.Reset();
                        GaugeTempsPasseRoute.Value = ts.Seconds;
                        recherchermesure = true;
                    }

                //ATTTTTEEENTION A BIEN REMETTRE LES VALEURS INCREMENTEES A ZERO !!!!!
                    Cars[0].TempsPasseBouchon = 0;

                    //--------------------------
                    var temp = Cars[0]; //on crée une nouvelle voiture temporaire qui va être rajoutée à la fin de la liste
                    Cars.RemoveAt(0);
                    temp.Xposition = -100; //On place les voitures hors cadre pour éviter les voitures entassées à gauche
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
                        Ajoutcamion(temp);
                    else
                        temp.Width = 16;

                    if (temp.Lane == 2)
                    {
                        temp.Yposition = PositionL2; //Mettre la position verticale
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
        }

        public void ModificationNbVehicules()
        {
            var relanceTimers = false; //Bool pour relancer les timers
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
                        positionDernier = 0;
                    else
                        positionDernier = Cars2.Last().Xposition;

                    var voiture = new Voiture
                    {
                        Lane = 2,
                        Vitesse = ChoixVitessemax.Value / 3.6 * 0.02 / 0.25,
                        Yposition = PositionL2,
                        Xposition =
                            positionDernier -
                            3 * _distanceEntreVehicule //Valeur arbitraire, on laisse assez de place en cas de freinage
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
                        positionDernier = 0;
                    else
                        positionDernier = Cars.Last().Xposition;

                    var voiture = new Voiture
                    {
                        Lane = 1,
                        Vitesse = ChoixVitessemax.Value / 3.6 * 0.02 / 0.25,
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
                    if (Cars2.Count == 0) //Il n'y a pas assez de vehicules à enlever sur cette voie
                    {
                        nbajoutVoiedroite =
                            nbajoutVoiedroite - nbajoutVoiegauche; //On enlève les voitures sur l'autre voie
                        break;
                    }

                    Affichage.Children.Remove(Cars2[Cars2.Count - 1]);
                    Cars2.RemoveAt(Cars2.Count - 1);
                }

                for (var i = 0; i > nbajoutVoiedroite; i--)
                {
                    if (Cars.Count == 0) //Pas de voiture à enlever sur cette voie
                        break;
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

        private void Eteindre(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

    }
}