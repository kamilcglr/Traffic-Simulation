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

        static List<Voiture> cars = new List<Voiture>();
        static List<Voiture> cars2 = new List<Voiture>();
        public bool ligneoccupee = false;
        public int dernieroccupe = 0;
        int point_critique = 400;
        int distance_entre_vehicule = 30;
        
        Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = TimeSpan.FromMilliseconds(20);
            
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            float s = (float)colonne1.ActualWidth;
            float vitessemax = (float)choix_vitessemax.Value;
            float acceleration = (float)choix_acceleration.Value ;
            
            for (int i = 0; i < cars.Count; i++)
            {
                if (i == 0) // Pour la première voiture on la fait avancer dans tous les cas
                {
                    Canvas.SetLeft(cars[0], cars[0].Move(vitessemax, acceleration));
                    Canvas.SetTop(cars[0], cars[0]._yposition);
                }
                else // Pour les autre on vérifie devant pour freiner ou avancer
                {
                    if ((cars[i]._xposition < cars[i - 1]._xposition - distance_entre_vehicule))
                    {
                        Canvas.SetLeft(cars[i], cars[i].Move(vitessemax, acceleration));
                        Canvas.SetTop(cars[i], cars[i]._yposition);
                    }
                    else
                    {
                        cars[i]._frein = true;
                        Canvas.SetLeft(cars[i], cars[i].Move(vitessemax, acceleration));
                        Canvas.SetTop(cars[i], cars[i]._yposition);
                        cars[i]._frein = false; 
                    }
                }
            }    
            //TEST SI LIGNE OCCUPEE
            for (int i = 0; i < cars.Count; i++)
            {
                if ((cars[i]._xposition >= 350) && (cars[i]._xposition <= 400))
                {
                    ligneoccupee = true;
                    dernieroccupe = i;
                    
                    break;
                }
                else
                {
                    ligneoccupee = false;
                }
            }

            for (int i = 0; i < cars2.Count; i++)
            {
                if (i == 0)
                {
                    if (cars2[0]._xposition <= point_critique - 20)
                    {
                        Canvas.SetLeft(cars2[0], cars2[0].Move(vitessemax, acceleration));
                        Canvas.SetTop(cars2[0], cars2[0]._yposition);
                    }
                    else
                    {
                        cars2[0]._frein = true;
                        Canvas.SetLeft(cars2[0], cars2[0].Move(vitessemax, acceleration));
                        Canvas.SetTop(cars2[0], cars2[0]._yposition);
                        cars2[0]._frein = false;
                    }
                }
                else //On fait avancer les autres voitures en  véfrifant devant
                {
                    if ((cars2[i]._xposition < cars2[i - 1]._xposition - distance_entre_vehicule))
                    {

                        Canvas.SetLeft(cars2[i], cars2[i].Move(vitessemax, acceleration));
                        Canvas.SetTop(cars2[i], cars2[i]._yposition);
                    }
                    else
                    {
                        cars2[i]._frein = true;
                        Canvas.SetLeft(cars2[i], cars2[i].Move(vitessemax, acceleration));
                        Canvas.SetTop(cars2[i], cars2[i]._yposition);
                        cars2[i]._frein = false;
                    }
                }
            }
            // VOITURE TETE LIGNE 2  Si on atteint le point critique ET champs libre : on change de ligne ATTENTION SI DEVIENT VOITURE DE TETE
            if (cars2.Count != 0)
            {
                if ((cars2[0]._xposition >= point_critique - 50) && !(ligneoccupee))
                {
                    Voiture temp = cars2[0];
                    cars2.RemoveAt(0);
                    if (dernieroccupe+1 > cars.Count) //Si on dépasse la valeur 
                    {
                        cars.Add(temp);
                    }
                    else
                    {
                         cars.Insert(dernieroccupe + 1, temp);
                    }
                    //On affiche cette voiture et on la fait avancer
                    cars[dernieroccupe + 1]._yposition = 0;
                    Canvas.SetLeft(cars[dernieroccupe + 1], cars[dernieroccupe + 1].Move(vitessemax, acceleration));
                    Canvas.SetTop(cars[dernieroccupe + 1], cars[dernieroccupe + 1]._yposition);
                }
            }

            //----------------- Retour de voitures au début ---------------------------------------------
            if (cars[0]._xposition >= colonne1.ActualWidth)
            {
                Voiture temp = cars[0];
                cars.RemoveAt(0);
                temp._xposition = 0;
                if (temp._lane == 2)
                {
                    temp._yposition = 100;
                    cars2.Add(temp);
                    Canvas.SetLeft(temp, temp.Move(vitessemax, acceleration));
                    Canvas.SetTop(temp, temp._yposition);
                }
                else
                {
                    cars.Add(temp);
                    Canvas.SetLeft(temp, temp.Move(vitessemax, acceleration));
                    Canvas.SetTop(temp, temp._yposition);
                }
            }
            nb_voitures_1.Content = "Ligne 1 : " + cars.Count.ToString();
            nb_voitures_2.Content = "Ligne 1 : " + cars2.Count.ToString();

        }

        public void start(object sender, RoutedEventArgs e)
        { 
            timer1.Start();
        }

        public void Ajout_voiture_Click(object sender, RoutedEventArgs e)
        {
            Voiture voiture = new Voiture();
            cars.Add(voiture);
            affichage.Children.Add(voiture);
            Canvas.SetLeft(voiture, voiture._xposition);
            Canvas.SetTop(voiture, voiture._yposition);

        }


        private void choix_vitesse_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vitessemax_choix_affichage.Content = "Vitesse max : " + Math.Round(choix_vitessemax.Value, 3).ToString();
        }

        private void choix_acceleration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            accelerationmax_choix_affichage.Content = "Accélération max : " + Math.Round(choix_acceleration.Value,3).ToString();
        }

        private void Bouton_frein_Click(object sender, RoutedEventArgs e)
        {
            if (cars[0]._frein == false) {
                cars[0]._frein = true;
            }
            else
            {
                cars[0]._frein = false;
            }
        }

        private void Choix_nombrevoitures_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (cars.Count < Choix_nombrevoitures.Value)
            {
                
                int i = (int) Choix_nombrevoitures.Value/3;
                while (i !=0) {
                   
                    Voiture voiture = new Voiture();

                    /*if (rand.Next(100) > 40)
                    {
                        voiture._lane = 1;
                        voiture._yposition = 0;
                        cars.Add(voiture);
                    }
                    else
                    {
                        voiture._lane = 2;
                        voiture._yposition = 100;
                        cars2.Add(voiture);
                    }*/
                    voiture._lane = 2;
                    voiture._yposition = 100;
                    cars2.Add(voiture);
                    affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture._xposition);
                    Canvas.SetTop(voiture, voiture._yposition);
                    i--;
                }
                int j = (int)Choix_nombrevoitures.Value - (int)(Choix_nombrevoitures.Value / 3);
                while (j != 0)
                {
                    Voiture voiture = new Voiture();
                    voiture._lane = 1;
                    voiture._vitesse = (float) choix_vitessemax.Value/2; //TEST
                    voiture._yposition = 0;
                    cars.Add(voiture);
                    affichage.Children.Add(voiture);
                    Canvas.SetLeft(voiture, voiture._xposition);
                    Canvas.SetTop(voiture, voiture._yposition);
                    j--;
                }
            }
            else {//TODO
                timer1.Stop();
                int i = cars.Count-1;
                while (i> (int)Choix_nombrevoitures.Value - (int)(Choix_nombrevoitures.Value / 3))
                {
                    
                        affichage.Children.Remove(cars[i]);
                        cars.RemoveAt(i);
                        i--;
                    
                }
                int j = cars2.Count - 1;
                while(j> (int)Choix_nombrevoitures.Value / 3)
                {
                    affichage.Children.Remove(cars2[j]);
                    cars2.RemoveAt(j);
                    j--;
                }
                timer1.Start();

            }

        }
    }
}
