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
        public float _xposition;
        public float _yposition;

        public float _vitesse;
        public float _vitessemax;

        public float _acceleration;
        public bool _frein= false;

        public int _lane;

        public Voiture()//constructeur
        {   
            this.Source = new BitmapImage(new Uri("C:/Users/Kamil/source/repos/Traffic-Simulation/Traffic_Simulation/Images/car.png"));
            this.Height = 16;
            this.Width = 16;
            this._xposition = 0;
            this._yposition = 0;
            this._vitesse = 0;
            this._vitessemax = 0;
            this._acceleration = 0;
            this._lane = 1;
            

    }
        public float Move(float vmax, float a)
        {
            if (_frein)
            {
                _vitesse = (float ) (_vitesse / 1.5);
                _xposition += _vitesse;
            }
            else
            {
                if (_vitesse < 1)
                {
                    _vitesse = 1;
                }
                if ((_vitesse < vmax))
                {
                    _vitesse = _vitesse * a;
                }
                _xposition += _vitesse;
            }
            return _xposition;

        }

    }
}
