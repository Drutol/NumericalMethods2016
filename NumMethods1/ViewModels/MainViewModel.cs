using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
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
            for (int i = -10; i < 15; i++)
            {
                ChartData.Add(new KeyValuePair<double, double>(i,FunctionSelectorSelectedItem.GetValue(i)));
            }
        }
    }
}