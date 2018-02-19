using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Simulateur_0._0._2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer1 = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer timer2 = new System.Windows.Threading.DispatcherTimer();
        public bool chargement = true;

        static List<Voiture> cars = new List<Voiture>();
        static List<Voiture> cars2 = new List<Voiture>();
        public int nbvoitures=0;
        public int positionL2 = 0;
        public int positionL1 = 100;
        int point_critique = 600;
        int distance_entre_vehicule = 20;

        Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = TimeSpan.FromMilliseconds(10);
            timer2.Tick += new EventHandler(timer2_Tick);
            timer2.Interval = TimeSpan.FromMilliseconds(10);


        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Avance_ligne1();
            Retour_vehicules();
            nb_voitures_1.Content = "Ligne 1 : " + cars.Count.ToString();
            nb_voitures_2.Content = "Ligne 1 : " + cars2.Count.ToString();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            Avance_ligne2();

        }

        public void Avance_ligne1()
        {
            distance_entre_vehicule = (int)choix_distance_entre_vehicules.Value;

            double vitessemax = choix_vitessemax.Value;
            double acceleration = choix_acceleration.Value;
            double deceleration = choix_deceleration.Value;
            for (int i = 0; i < cars.Count; i++)
            {
                if (i == 0) // Pour la première voiture on la fait avancer dans tous les cas
                {
                    Canvas.SetLeft(cars[0], cars[0].Move(vitessemax, acceleration,deceleration));
                    Canvas.SetTop(cars[0], cars[0]._yposition);
                }
                else // Pour les autre on vérifie devant pour freiner ou avancer
                {
                    if ((cars[i]._xposition) < (cars[i - 1]._xposition - distance_entre_vehicule))
                    {
                        Canvas.SetLeft(cars[i], cars[i].Move(vitessemax, acceleration, deceleration));
                        Canvas.SetTop(cars[i], cars[i]._yposition);
                    }
                    else
                    {
                        cars[i]._frein = true;
                        Canvas.SetLeft(cars[i], cars[i].Move(vitessemax, acceleration, deceleration));
                        Canvas.SetTop(cars[i], cars[i]._yposition);
                        cars[i]._frein = false;
                    }
                }
            }
        }
        public void Avance_ligne2()
        {
            distance_entre_vehicule = (int)choix_distance_entre_vehicules.Value;

            double distance_ptcritique = 200;
            double vitessemax = choix_vitessemax.Value;
            double acceleration = choix_acceleration.Value;
            double deceleration = choix_deceleration.Value;

            for (int i = 0; i < cars2.Count; i++)
            {
                if (i == 0)
                {
                    if (cars2[0]._xposition <= point_critique - distance_ptcritique)
                    {
                        Canvas.SetLeft(cars2[0], cars2[0].Move(vitessemax, acceleration, deceleration));
                        Canvas.SetTop(cars2[0], cars2[0]._yposition);
                    }
                    else
                    {
                        int position = Champ_libre(cars2[0]._xposition);
                        if (position != -1)
                        {
                            Changement_ligne(position, 0);

                        }
                        else
                        {
                            cars2[0]._frein = true;
                            Canvas.SetLeft(cars2[0], cars2[0].Move(vitessemax, acceleration, deceleration));
                            Canvas.SetTop(cars2[0], cars2[0]._yposition);
                            cars2[0]._frein = false;
                        }
                    }
                }
                else //On fait avancer les autres voitures en  véfrifant devant
                {
                    if (cars2[i]._xposition <= point_critique - distance_ptcritique)
                    {
                        if ((cars2[i]._xposition < cars2[i - 1]._xposition - distance_entre_vehicule))
                        {
                            Canvas.SetLeft(cars2[i], cars2[i].Move(vitessemax, acceleration, deceleration));
                            Canvas.SetTop(cars2[i], cars2[i]._yposition);
                        }
                        else
                        {
                            cars2[i]._frein = true;
                            Canvas.SetLeft(cars2[i], cars2[i].Move(vitessemax, acceleration, deceleration));
                            Canvas.SetTop(cars2[i], cars2[i]._yposition);
                            cars2[i]._frein = false;
                        }
                    }
                    else
                    {
                        int position = Champ_libre(cars2[i]._xposition);
                        if (position != -1)
                        {
                            Changement_ligne(position, i);
                        }
                        else
                        {
                            cars2[i]._frein = true;
                            Canvas.SetLeft(cars2[i], cars2[i].Move(vitessemax, acceleration, deceleration));
                            Canvas.SetTop(cars2[i], cars2[i]._yposition);
                            cars2[i]._frein = false;
                        }
                    }
                }
            }
        }


        public int Champ_libre(double _xposition)
        {
            distance_entre_vehicule = (int)choix_distance_entre_vehicules.Value;

            int autorise_Champ_libre = -1;
            for (int i = 0; i < cars.Count; i++)
            {
                if ((cars[i]._xposition >= _xposition - distance_entre_vehicule) && (cars[i]._xposition <= _xposition + distance_entre_vehicule))
                {
                    autorise_Champ_libre = -1;
                    break;
                }
                else
                {
                    autorise_Champ_libre = i;
                }
            }
            if (autorise_Champ_libre != -1)
            {
                for (int i = cars.Count - 1; i != 0; i--)
                {
                    if ((cars[i]._xposition < _xposition))
                    {
                        autorise_Champ_libre = 0;
                    }
                    else
                    {
                        autorise_Champ_libre = i;
                        break;
                    }

                }
            }
            return autorise_Champ_libre;
        }
        public void Changement_ligne(int position, int i)
        {
            distance_entre_vehicule = (int)choix_distance_entre_vehicules.Value;

            double vitessemax = choix_vitessemax.Value;
            double acceleration = choix_acceleration.Value;
            double deceleration = choix_deceleration.Value;

            Voiture temp = cars2[i];
            cars2.RemoveAt(i);
            if (position + 1 > cars.Count) //Si on dépasse la valeur 
            {
                cars.Add(temp);
            }
            else
            {
                cars.Insert(position + 1, temp);
            }
            //On affiche cette voiture et on la fait avancer
            cars[position + 1]._yposition = positionL1;
            Canvas.SetLeft(cars[position + 1], cars[position + 1].Move(vitessemax, acceleration, deceleration));
            Canvas.SetTop(cars[position + 1], cars[position + 1]._yposition);
        }
        public void Retour_vehicules()
        {
            double vitessemax = choix_vitessemax.Value;
            double acceleration = choix_acceleration.Value;
            double deceleration = choix_deceleration.Value;

            //----------------- Retour de voitures au début ---------------------------------------------
            if (cars[0]._xposition >= Bordure.ActualWidth - 16)
            {
                Voiture temp = cars[0];
                cars.RemoveAt(0);
                temp._xposition = 0;
                temp._vitesse = vitessemax;
                temp._vehiculelent = false;
                Uri relativeUri = new Uri("Images/car.png", UriKind.Relative);
                temp.Source = new BitmapImage(relativeUri);
                if (rand.Next(100) < 30)
                {
                    temp._vehiculelent = true;
                    Uri relativeUri2 = new Uri("Images/automobile.png", UriKind.Relative);
                    temp.Source = new BitmapImage(relativeUri2);
                }
                if (temp._lane == 2)
                {
                    temp._yposition = positionL2;
                    cars2.Add(temp);
                    Canvas.SetLeft(temp, temp.Move(vitessemax, acceleration, deceleration));
                    Canvas.SetTop(temp, temp._yposition);
                }
                else
                {
                    cars.Add(temp);
                    Canvas.SetLeft(temp, temp.Move(vitessemax, acceleration, deceleration));
                    Canvas.SetTop(temp, temp._yposition);
                }
            }
        }
        public void Remplissage_voies()
        {
            bool relance_timers = false;
            //Si les timers sont actifs (bouton start déjà appuyé) on devra relancer les timers
            if (timer1.IsEnabled || timer2.IsEnabled)
            {
                timer1.Stop();
                timer2.Stop();
                relance_timers = true;
            }

            distance_entre_vehicule = (int)choix_distance_entre_vehicules.Value;

            //Creation des proportions
            int nbvoitures_voiegauche = (int)(Choix_nombrevoitures.Value *( Choix_proportion_voituregauche.Value / 100)) ;
            int nbvoitures_voiedroite = (int)Choix_nombrevoitures.Value - nbvoitures_voiegauche;
            int densite_camion = (int)Choix_densitecamion.Value;

            nbvoitures = cars.Count + cars2.Count;

            //On verifie si le nombre de voitures est plus élevé ou plus faible que les valeurs précédentes
            //Si le nombre de voiture a augmenté, on ajoute le nombre de voiture nécéssaires voitures nécéssaires
            if (nbvoitures < Choix_nombrevoitures.Value)
            {
                int i = nbvoitures_voiegauche;
                while (i != 0)
                {
                    Voiture voiture = new Voiture();
                    if (rand.Next(100) < densite_camion)
                    {
                        voiture._vehiculelent = true;
                        Uri relativeUri = new Uri("Images/automobile.png", UriKind.Relative);
                        voiture.Source = new BitmapImage(relativeUri);
                    }
                    voiture._lane = 2;
                    voiture._vitesse = choix_vitessemax.Value;
                    voiture._yposition = positionL2;
                    voiture._xposition = distance_entre_vehicule * 2 * i;
                    cars2.Add(voiture);
                    affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture._xposition);
                    Canvas.SetTop(voiture, voiture._yposition);
                    i--;
                }
                int j = nbvoitures_voiedroite;
                while (j != 0)
                {
                    Voiture voiture = new Voiture();
                    if (rand.Next(100) < densite_camion)
                    {
                        voiture._vehiculelent = true;
                        Uri relativeUri = new Uri("Images/automobile.png", UriKind.Relative);
                        voiture.Source = new BitmapImage(relativeUri);
                    }
                    voiture._lane = 1;
                    voiture._vitesse = choix_vitessemax.Value;
                    voiture._yposition = positionL1;
                    voiture._xposition = j * distance_entre_vehicule * 2 + Choix_nombrevoitures.Value;
                    cars.Add(voiture);
                    affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture._xposition);
                    Canvas.SetTop(voiture, voiture._yposition);
                    j--;
                }
            }
            //Sinon, le nombre de voitures a baissé, ...
            else
            {
                int i = cars.Count-1;
                while (i > nbvoitures_voiedroite)
                {
                    affichage.Children.Remove(cars[i]);
                    cars.RemoveAt(i);
                    i--;
                }
                int j = cars2.Count-1;
                while (j > nbvoitures_voiedroite)
                {
                    affichage.Children.Remove(cars2[j]);
                    cars2.RemoveAt(j);
                    j--;
                }
            }
            if (relance_timers)
            {
                timer1.Start();
                timer2.Start();
            }

        }

 //UI ELEMENTS
        public void start(object sender, RoutedEventArgs e)
        {
            if (chargement)
            {
                Remplissage_voies();
                chargement = false;
            }
            timer1.Start();
            timer2.Start();
        }

        private void choix_vitesse_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vitessemax_choix_affichage.Content = "Vitesse max : " + Math.Round(choix_vitessemax.Value, 3).ToString();
        }

        private void choix_acceleration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            accelerationmax_choix_affichage.Content = "Accélération max : " + Math.Round(choix_acceleration.Value, 3).ToString();
        }

        private void Bouton_frein_Click(object sender, RoutedEventArgs e)
        {

            if (cars[0]._frein == false)
            {
                cars[0]._frein = true;
            }
            else
            {
                cars[0]._frein = false;
            }
        }

        private void Choix_nombrevoitures_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!chargement)
            {
                Remplissage_voies();
            }            
            Nombrevehicules_choix_affichage.Content = "Nombre de véhicules : " + Choix_nombrevoitures.Value.ToString("F0");
        }

        private void Choix_proportion_voituregauche_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!chargement)
            {
                Remplissage_voies();
            }
            proportion_voiegauche_choix_affichage.Content = "Proportion véhicules file de gauche" +  Choix_proportion_voituregauche.Value.ToString("F0") + " %";
        }

        private void choix_distance_entre_vehicules_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Distance_securite_affichage.Content = "Distance entre veh : " + choix_distance_entre_vehicules.Value.ToString();
        }

        private void choix_deceleration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Deceleration_choix_affichage.Content = "Deceleration : " + Math.Round(choix_deceleration.Value, 3).ToString();
        }

        private void Choix_densitecamion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!chargement)
            {
                Remplissage_voies();
            }
            densitecamion_choix_affichage.Content = "Proportion de camions " + Choix_densitecamion.Value.ToString() + " %";

        }
    }
}
