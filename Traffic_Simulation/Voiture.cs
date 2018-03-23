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
        public bool vm;

        Stopwatch ChronoTempsPasseArret = new Stopwatch();


        public Voiture() //constructeur
        {
            var relativeUri = new Uri("Images/automobile.png", UriKind.Relative);
            Source = new BitmapImage(relativeUri);
            Width = 16;
            Height = 10;
            Xposition = 0;
            Yposition = 0;
            Vitesse = 1;
            Vitessemax = 0;
            Acceleration = 0;
            Lane = 1;
            Vehiculelent = false;
            TempsPasseBouchon = 0;
            dejaArret = false;
            vm = false;
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
            
            if (Vitesse < 0.2 && Xposition>0)
            {
                if (!dejaArret)
                {
                    ChronoTempsPasseArret.Start();
                }
                dejaArret = true;
            }
            else
            {
                if (ChronoTempsPasseArret.IsRunning)
                {
                    ChronoTempsPasseArret.Stop();
                    TimeSpan timeSpanEcoule = ChronoTempsPasseArret.Elapsed;
                    TempsPasseBouchon += timeSpanEcoule.Seconds;
                    ChronoTempsPasseArret.Reset();
                }
                dejaArret = false;
            }
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
                    if (Vitesse <= vitessemax - 0.1) Vitesse += acceleration;
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