using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using NumMethods1.NumCore;

namespace NumMethods1.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public ObservableCollection<IFunction> AvailableFunctions { get; }
        = new ObservableCollection<IFunction>
        {
            new Function1(),
            new Function2(),
            new Function3()           
        };

        public ObservableCollection<KeyValuePair<double, double>> ChartData { get; } =
            new ObservableCollection<KeyValuePair<double, double>>();

        private IFunction _functionSelectorSelectedItem;
        public IFunction FunctionSelectorSelectedItem
        {
            get { return _functionSelectorSelectedItem; }
            set
            {
                _functionSelectorSelectedItem = value;
                UpdateChart();
            }
        }

        private double _fromXValue = -100;
        public double FromXValue
        {
            get { return _fromXValue; }
            set
            {
                _fromXValue = value;
                RaisePropertyChanged(() => FromXValue);
            }
        }

        private double _toXValue = -100;
        public double ToXValue
        {
            get { return _toXValue; }
            set
            {
                _toXValue = value;
                RaisePropertyChanged(() => ToXValue);
            }
        }

        private double _maxIterValue = -100;
        public double MaxIterValue
        {
            get { return _maxIterValue; }
            set
            {
                _maxIterValue = value;
                RaisePropertyChanged(() => MaxIterValue);
            }
        }
        #endregion


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

        }

        private void UpdateChart()
        {
            ChartData.Clear();
            for (int i = -10; i < 10; i++)
            {
                ChartData.Add(new KeyValuePair<double, double>(i,FunctionSelectorSelectedItem.GetValue(i)));
            }
        }
    }
}