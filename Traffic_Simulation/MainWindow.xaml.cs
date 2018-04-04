using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Configurations;
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
        private static bool pause = false;
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
        private static double distancesecuriteDepassementNonForce;


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

        public bool isKeyPressed = false;
        public Stopwatch ChronoBlocage = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            _timer1.Tick += timer1_Tick;
            _timer2.Tick += timer2_Tick;
            _timer3.Tick += timer3_Tick;
            _timerGauges.Tick += timerGauges_Tick;

            //this.PreviewKeyDown += (s1, e1) => { if (e1.Key == Key.LeftCtrl) isKeyPressed = true; };
            //this.PreviewKeyUp += (s2, e2) => { if (e2.Key == Key.LeftCtrl) isKeyPressed = false; };
            //this.PreviewMouseLeftButtonDown += (s, e) => { if (isKeyPressed) DragMove(); };
        }

        public void InitTimer()
        {
            double VitesseTimerSimulation = ChoixVitesseSimulation.Value; //ms
            double VitesseTimerGauge = 1; //s
            double VitesseTimerGraph = 1; //s
            _timer1.Interval = TimeSpan.FromMilliseconds(VitesseTimerSimulation);
            _timer2.Interval = TimeSpan.FromMilliseconds(VitesseTimerSimulation);
            _timer3.Interval = TimeSpan.FromSeconds(VitesseTimerGraph);
            _timerGauges.Interval = TimeSpan.FromSeconds(VitesseTimerGauge);
        }

        private bool Verifblocage() //Renvoie true si la voiture de gauche peut changer de voie
        {
            if (Cars.Count != 0)
            {
                int indice = 0;
                //Trouver l'indice de la voiture qui est bloquée 
                for (int i = 0; i < Cars.Count; i++)
                {
                    if (Cars[i].Xposition <= _pointCritique && Cars[i].Xposition > _pointCritique - 150)
                    {
                        indice = i;
                        break; //On a trouvé la voiture qui est potentiellement bloquée
                    }
                }
                //On vérifie si cette voiture est à l'arret depuis combien de temps elle est bloquée par rapport
                //à la voiture de tête de la ligne de gauche
                if (Cars[indice].ChronoTempsPasseArret.ElapsedMilliseconds> Cars2[0].ChronoTempsPasseArret.ElapsedMilliseconds)
                {
                    return false; //La voiture de droite etait à l'arret depuis plus longtemps
                }
                else
                {
                    return true; //Elle est à l'arret depuis moins longtemps, elle va donc laisser passer celle de gauche 
                }
            }
            else
            {
                return true; //il n'y a aucue voiture sur la voie de droite
            }
            
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
            NbVoitures1.Content = "Voie gauche : " + Cars.Count;
            NbVoitures2.Content = "Voie droite : " + Cars2.Count;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {           
            //-----------ACTUALISATION VALEURS--------------------
            vitessemax = ChoixVitessemax.Value / 3.6 * 0.02 / 0.25;
            acceleration = -0.002 * Math.Log(ChoixAcceleration.Value) + 0.0088;
            deceleration = ChoixDeceleration.Value;
            _distanceEntreVehicule = (int)ChoixDistanceEntreVehicules.Value;
            distancePtcritique = 100; //Plus on augmente la valeur, plus les voitures vont forcer tôt
            distanceAnalyse = 700; //Plus on augmente la valeur, plus les voitures peuvent changer de ligne tôt
            distancesecuriteDepassementNonForce = ChoixDistanceEntreVehicules.Value*2; //On a ici une grande distance de sécurite si on ne force pas 

            //-----------MISE A JOUR DES GRAPHIQUES--------------------
            UpdateGraphVitesseMoy();
            UpdateGraphNbVehiculesArret();
            UpdateGraphTempsPasseArret();
            UpdateGraphTempsPasseRoute();
            UpdateLabelVitesseMoyenne(GaugeVitesse.Value);
            UpdateLabelNbVehiculesArret((int)GaugeNbvehiculesArret.Value);
            UpdateLabelTempsPasseRoute((int)GaugeTempsPasseRoute.Value);
            UpdateLabelTempsPasseArret((int)GaugeTempsPasseArret.Value);
            //UpdateHeatMap();
        }

        private void timerGauges_Tick(object sender, EventArgs e)
        {
            CarsCopie = Cars;
            Cars2Copie = Cars2;
            GaugeVitesse.Value = Vitessemoyenne();
            GaugeNbvehiculesArret.Value = NbVehiculesArret();
            if (EnSimulation && SimulationVitesseMoyenne)
            {
                VitesseMoyenneSimulateur.Add(GaugeVitesse.Value);
            }
        }

        /*public void UpdateHeatMap()
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
        */

        //----------------Affichage et mouvement----------------------
        public void Avance_ligne1()
        {
            for (var i = 0; i < Cars.Count; i++)
                if (i == 0) // Pour la première voiture on la fait avancer dans tous les cas
                {
                    AvanceL1(0);
                }
                else // Pour les autre on vérifie devant pour freiner ou avancer
                {
                    if (Cars[i].Xposition + Cars[i].Width < Cars[i - 1].Xposition - _distanceEntreVehicule)
                    {
                        AvanceL1(i);
                    }
                    else
                    {
                        FreiangeL1(i);
                    }
                }
        }

        public void Avance_ligne2()
        {
            for (var i = 0; i < Cars2.Count; i++)
                if (i == 0) //CAS PARTICULIR Voiture de tête
                {
                    //Pour la première voiture on vérifie seulement si elle est dans zone d'analyse
                    //On avance tout le temps comme il n'y a pas de voiture devant
                    if (Cars2[0].Xposition <= _pointCritique - distanceAnalyse)
                    {
                        AvanceL2(0);
                    }
                    else //La voiture est dans la zone d'analyse
                    {
                        var force = false;
                        //On freine si on est dans la zone critique
                        if (Cars2[0].Xposition > _pointCritique - distancePtcritique
                        ) //On regarde si on est dans la ZONE CRITIQUE 
                        {
                            if (Verifblocage())
                            {
                                //Si c'est le cas, on cherche une postion ou on freine
                                force = true;
                                var position = Champ_libre_voiture_tete(Cars2[0].Xposition, force);
                                if (position != -1 ) //On peut changer de voie en forcant 
                                {
                                    Changement_ligne(position, i);
                                }
                                else
                                {
                                    FreinageL2(0);
                                }
                            }
                            else //pas de place même en forçant, donc on freine
                                {
                                FreinageL2(0);
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
                                AvanceL2(0);
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
                            AvanceL2(i);
                        }
                        else
                        {
                            FreinageL2(i);
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
                                FreinageL2(i);
                                
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
                                //Avance normalement
                                if (Cars2[i].Xposition + Cars2[i].Width < Cars2[i - 1].Xposition - _distanceEntreVehicule)
                                {
                                   AvanceL2(i);
                                }
                                else
                                {
                                    FreinageL2(i);
                                }
                            }
                        }
                    }
                }
        }

        public void AvanceL1(int numVoiture)
        {
            Canvas.SetLeft(Cars[numVoiture], Cars[numVoiture].Move(vitessemax, acceleration, deceleration));
            Canvas.SetBottom(Cars[numVoiture], Cars[numVoiture].Yposition);
        }
        public void FreiangeL1(int numVoiture)
        {
            Cars[numVoiture].Frein = true;
            Canvas.SetLeft(Cars[numVoiture], Cars[numVoiture].Move(vitessemax, acceleration, deceleration));
            Canvas.SetBottom(Cars[numVoiture], Cars[numVoiture].Yposition);
            Cars[numVoiture].Frein = false;
        }
        public void AvanceL2(int numVoiture)
        {
            Canvas.SetLeft(Cars2[numVoiture], Cars2[numVoiture].Move(vitessemax, acceleration, deceleration));
            Canvas.SetBottom(Cars2[numVoiture], Cars2[numVoiture].Yposition);
        }
        public void FreinageL2(int numVoiture)
        {
            Cars2[numVoiture].Frein = true;
            Canvas.SetLeft(Cars2[numVoiture], Cars2[numVoiture].Move(vitessemax, acceleration, deceleration));
            Canvas.SetBottom(Cars2[numVoiture], Cars2[numVoiture].Yposition);
            Cars2[numVoiture].Frein = false;
        }
        public int Champ_libre_voiture_tete(double xposition, bool force)
        {
                var autoriseChampLibre = -1;
                double
                distancesecu = distancesecuriteDepassementNonForce; 
                //cette distance est plus elevee que la securite normale, on la choisit dans timer1tick
                if (force) //la distance de securite sera plus elevee si on ne force pas, on augmente donc la distance
                    distancesecu = ChoixDistanceEntreVehicules.Value *2;
            if (Cars.Count != 0)
            {
                for (var i = 0;
                        i < Cars.Count;
                        i++)
                    //Quelqu'un sur la voie voie opposée à cette voiture sur l'autre partie de la route
                    if (Cars[i].Xposition >= xposition - distancesecu &&
                        Cars[i].Xposition <= xposition + distancesecu)
                    {
                        autoriseChampLibre = -1;
                        break; //On peut arreter de chercher il y a deja un vehicule
                    }
                    else
                    {
                        autoriseChampLibre = i;
                    }

                if (autoriseChampLibre != -1)
                    //Personne, alors on prend la place de la voiture jsute derriere celle qui avait la position de cars2
                {
                    for (var i = Cars.Count - 1; i != 0; i--)
                    {
                        if (Cars[i].Xposition < xposition)
                        {
                            autoriseChampLibre = 0; //On prend la tête 
                        }
                        else
                        {
                            autoriseChampLibre = i; //On se place derriere la derniere voiture 
                            break;
                        }
                    }
                }
            }
            else
            {
                
                    autoriseChampLibre = 0;
                 //Pas de voiture sur la ligne 1
            }

            return autoriseChampLibre;
           

        }
        public int Champ_libre(double xposition, bool force)
        {
            var autoriseChampLibre = -1;
            double distancesecu = distancesecuriteDepassementNonForce;//cette distance est plus elevee que la securite normale, on la choisit dans timer1tick
            if (force) //la distance de securite sera plus elevee si on ne force pas, on augmente donc la distance
                distancesecu = ChoixDistanceEntreVehicules.Value;
            if (Cars.Count != 0)
            {
                for (var i = 0;
                    i < Cars.Count;
                    i++) //Quelqu'un sur la voie voie opposée à cette voiture sur l'autre partie de la route
                    if (Cars[i].Xposition >= xposition - distancesecu &&
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
                            autoriseChampLibre = 0; //On prend la tête 
                        }
                        else
                        {
                            autoriseChampLibre = i; //On se place derriere la derniere voiture 
                            break;
                        }
            }
            else
            {
                autoriseChampLibre = 0;
            } //Pas de voiture sur la ligne 1

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
            if(Cars.Count == 1) {
                Cars[0].ChangementL = true; //On active le dépacement en Y
                Canvas.SetLeft(Cars[0], Cars[0].Move(vitessemax, acceleration, deceleration));
                Canvas.SetBottom(Cars[0], Cars[0].Yposition);
            }
            else{
                Cars[position + 1].ChangementL = true; //On active le dépacement en Y
                Canvas.SetLeft(Cars[position + 1], Cars[position + 1].Move(vitessemax, acceleration, deceleration));
                Canvas.SetBottom(Cars[position + 1], Cars[position + 1].Yposition);
            }


        }

        public void Retour_vehicules()
        {
            if(Cars.Count != 0)
            {
                if (Cars[0].Xposition >= Colonne1.ActualWidth - 16)
                {
                    
                    if (EnSimulation)
                    {
                        if (SimulationTempsPasseRoute)
                        {
                            TempsPasseMoyenne.Add(Cars[0].ChronoTempsPasse.ElapsedMilliseconds);
                        }
                        else
                        {
                            if(SimulationTempsPasseArret)
                            TempsPasseMoyenne.Add(Cars[0].TempsPasseBouchon);
                            else
                            {
                                VitesseMoyenneSimulateur.Add(Cars[0].nombredarret);
                            }
                        }
                    }
                    GaugeTempsPasseRoute.Value = Math.Round((double)(Cars[0].ChronoTempsPasse.ElapsedMilliseconds / 1000), 1 );
                    GaugeTempsPasseArret.Value = Math.Round((Cars[0].TempsPasseBouchon / 1000),1);
                    Cars[0].ChronoTempsPasse.Reset();
                    //ATTTTTEEENTION A BIEN REMETTRE LES VALEURS INCREMENTEES A ZERO !!!!!
                    Cars[0].TempsPasseBouchon = 0;
                    Cars[0].nombredarret = 0;

                    //--------------------------
                    var temp = Cars[0]; //on crée une nouvelle voiture temporaire qui va être rajoutée à la fin de la liste
                    Cars.RemoveAt(0);
                    temp.Xposition = -200; //On place les voitures hors cadre pour éviter les voitures entassées à gauche
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
                            nbajoutVoiedroite + nbajoutVoiegauche; //On enlève les voitures sur l'autre voie
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

        private void Pause(object sender, System.Windows.RoutedEventArgs e)
        {
            pause = true;
            _timer1.Stop();
            _timer2.Stop();
            _timer3.Stop();
            _timerGauges.Stop();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            try
            {
                this.DragMove();
            }
            catch{ }

        }
    }
}