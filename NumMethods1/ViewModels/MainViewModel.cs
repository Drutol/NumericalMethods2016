using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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

        private string _fromXValue = "-100";
        public string FromXValueBind
        {
            get { return _fromXValue; }
            set
            {
                _fromXValue = value;
                RaisePropertyChanged(() => FromXValueBind);
            }
        }

        public double FromX { get; set; }
        public double ToX { get; set; }

        private string _toXValue = "100";
        public string ToXValueBind
        {
            get { return _toXValue; }
            set
            {
                _toXValue = value;
                RaisePropertyChanged(() => ToXValueBind);
            }
        }

        private int _maxIterations = 100;
        public string MaxIterations
        {
            get { return _maxIterations.ToString(); }
            set
            {
                int val;
                if (!int.TryParse(value, out val) || val <= 0)
                    val = 100;
                _maxIterations = val;
                RaisePropertyChanged(() => MaxIterations);
            }
        }

        private ICommand _submitDataCommand;

        public ICommand SubmitDataCommand =>
            _submitDataCommand ?? (_submitDataCommand = new RelayCommand(SubmitData));

        public int SliderAcc { get; set; }

        private int _sliderValue = 1;

        public int SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;
                RaisePropertyChanged(() => SliderValue);
            }
        }

        private ICommand _chartAccuracyValueChangedCommand;

        public ICommand ChartAccuracyValueChangedCommand =>
            _chartAccuracyValueChangedCommand ?? (_chartAccuracyValueChangedCommand = new RelayCommand(ChartAccuracyValueChanged));
        #endregion


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            FunctionSelectorSelectedItem = AvailableFunctions[0];
        }

        private void ChartAccuracyValueChanged()
        {
            SliderAcc = /*Slider.value*/ 5;
        }

        private void UpdateChart()
        {
            ChartData.Clear();
            for (int i = (int)FromX; i < (int)ToX; i += (int)((Math.Abs(ToX) + Math.Abs(FromX)) * SliderAcc / 100))
            {
                ChartData.Add(new KeyValuePair<double, double>(i, FunctionSelectorSelectedItem.GetValue(i)));
            }
        }

        private void SubmitData()
        {
            double from, to;
            if (!double.TryParse(FromXValueBind, out from) || !double.TryParse(ToXValueBind, out to))
                return; //TODO : Display feedback msg
            if (from >= to)
                return; //TODO: Disp feedback msg.

            FromX = from;
            ToX = to;
            UpdateChart();
        }
    }
}