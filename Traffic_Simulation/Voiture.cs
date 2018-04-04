using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Simulateur_0._0._2
{
    internal class Voiture : Image
    {
        public double Acceleration;
        public double Deceleration;
        public bool Frein = false;

        public int Lane;

        public bool Vehiculelent;

        public double Vitesse;
        public double Vitessemax;
        public double Xposition;
        public double Yposition;

        public double TempsPasseBouchon;
        public bool dejaArret;
        public bool ChangementL;
        public int nombredarret;
        public Stopwatch ChronoTempsPasseArret = new Stopwatch();
        public Stopwatch ChronoTempsPasse = new Stopwatch();


        public Voiture() //constructeur
        {
            var relativeUri = new Uri("Images/automobile.png", UriKind.Relative);
            Source = new BitmapImage(relativeUri);
            Width = 16;
            Height = 10;
            Xposition = 0;
            Yposition = 0;
            Vitesse = 2; 
            Vitessemax = 0;
            Acceleration = 0;
            Lane = 1;
            Vehiculelent = false;
            TempsPasseBouchon = 0;
            dejaArret = false;
            nombredarret = 0;
        }

        public double Move(double vitessemax, double acceleration, double deceleration)
        {
            if (ChangementL)//Si il y a changement de ligne alors on fait translater la voiture
            {
                if (Yposition < 80)
                {
                    Yposition = 80;
                    ChangementL = false;
                }
                else
                {
                    Yposition--;
                }
            }

            if (Xposition > 0) //On fait ces actions seulement si la voiture est dans le parcours 
            {
                ChronoTempsPasse.Start(); //Pas de problemes si on start meme si deja start
                if (Vitesse < 0.2) //Moins de 10km/h
                {
                    if (!dejaArret)
                    {
                        nombredarret++;
                        ChronoTempsPasseArret.Start();
                    }
                    dejaArret = true;
                }
                else
                {
                    if (ChronoTempsPasseArret.IsRunning)
                    {
                        ChronoTempsPasseArret.Stop();
                        TempsPasseBouchon += ChronoTempsPasseArret.ElapsedMilliseconds;
                        ChronoTempsPasseArret.Reset();
                    }
                    dejaArret = false;
                }
            }

            //---------FREINAGE AVANCE ACCELERATION--------------------
            if (Frein)
            {
                Vitesse = Vitesse / deceleration;

                if (Vitesse > 0)
                    Xposition = Xposition + Vitesse;
                else
                    Vitesse = 0;
            }
            else
            {   
                if (Vehiculelent)
                {
                    if (Vitesse <= vitessemax)
                        if (Vitesse <= 2) //Correspond à 90km/h
                        {
                            Vitesse += acceleration;
                        }
                }
                else
                {
                    if (Vitesse <= vitessemax) Vitesse += acceleration;
                }

                Xposition = Xposition + Vitesse;
            }

            return Xposition;
        }
    }
}