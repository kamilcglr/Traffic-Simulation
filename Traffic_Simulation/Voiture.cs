using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Simulateur_0._0._2
{
    internal class Voiture : Image
    {
        public double _acceleration;
        public double _deceleration;
        public bool _frein = false;

        public int _lane;

        public bool _vehiculelent;

        public double _vitesse;
        public double _vitessemax;
        public double _xposition;
        public double _yposition;

        public Voiture() //constructeur
        {
            var relativeUri = new Uri("Images/car.png", UriKind.Relative);
            Source = new BitmapImage(relativeUri);
            Height = 16;
            Width = 16;
            _xposition = 0;
            _yposition = 0;
            _vitesse = 1;
            _vitessemax = 0;
            _acceleration = 0;
            _lane = 1;
            _vehiculelent = false;
        }

        public double Move(double _vitessemax, double _acceleration, double _deceleration)
        {
            if (_frein)
            {
                _vitesse -= _deceleration;

                if (_vitesse > 0)
                    _xposition = _xposition + _vitesse;
                else
                    _vitesse = 0;
            }
            else
            {
                if (_vehiculelent)
                {
                    if (_vitesse <= _vitessemax - 0.1) _vitesse += _acceleration;
                }
                else
                {
                    if (_vitesse <= _vitessemax) _vitesse += _acceleration;
                }

                _xposition = _xposition + _vitesse;
            }

            return _xposition;
        }
    }
}