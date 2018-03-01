using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LiveCharts;
using Wpf.Gauges;
using System.Threading;
using System.Threading.Tasks;
using LiveCharts.Configurations;

namespace Wpf.Gauges
{
    public partial class AngularGaugeExmple : UserControl, INotifyPropertyChanged
    {
        private double _value;

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


