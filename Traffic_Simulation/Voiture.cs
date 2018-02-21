using System;
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

        public Voiture() //constructeur
        {
            var relativeUri = new Uri("Images/car.png", UriKind.Relative);
            Source = new BitmapImage(relativeUri);
            Width = 16;
            Height = 6;
            Xposition = 0;
            Yposition = 0;
            Vitesse = 1;
            Vitessemax = 0;
            Acceleration = 0;
            Lane = 1;
            Vehiculelent = false;
        }

        public double Move(double vitessemax, double acceleration, double deceleration)
        {
            if (Frein)
            {
                Vitesse -= deceleration;

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