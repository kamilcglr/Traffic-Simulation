using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Simulateur_0._0._2
{   
    class Voiture : System.Windows.Controls.Image
    {
        public double _xposition;
        public double _yposition;

        public double _vitesse;
        public double _vitessemax;
        public double _deceleration;
        public double _acceleration;

        public bool _vehiculelent;
        public bool _frein= false;

        public int _lane;

        public Voiture()//constructeur
        {
            Uri relativeUri = new Uri("Images/car.png", UriKind.Relative);
            this.Source = new BitmapImage(relativeUri);
            this.Height = 16;
            this.Width = 16;
            this._xposition = 0;
            this._yposition = 0;
            this._vitesse = 1;
            this._vitessemax = 0;
            this._acceleration = 0;
            this._lane = 1;
            this._vehiculelent = false;
    }
        public double Move(double _vitessemax, double _acceleration, double _deceleration)
        {
            if (_frein)
            {
                _vitesse = _vitesse/_deceleration;
                _xposition =_xposition + _vitesse;
            }
            else
            {   if (_vitesse < 1)
                    {
                        _vitesse += 0.25;
                    }
                if(_vehiculelent)
                {
                    if (_vitesse <= _vitessemax-0.5)
                    {
                        _vitesse = _vitesse * _acceleration;
                    }
                }
                else 
                {
                    _vitesse = _vitesse * _acceleration;
                }
                _xposition = _xposition + _vitesse;
            }
            return _xposition;

        }

    }
}
