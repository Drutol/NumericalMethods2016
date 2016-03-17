using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NumMethods1.NumCore;

namespace NumMethods1.ViewModels
{
    public enum ApproxMethodEnum
    {
        Iterations,
        Value
    }

    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public ObservableCollection<IFunction> AvailableFunctions { get; }
        = new ObservableCollection<IFunction>
        {
            new Function1(),
            new Function2(),
            new Function3(),
            new Function4()
        };

        //trying roots collection
        private ListCollectionView _rootsView;
        public ListCollectionView RootsView
        {
            get { return _rootsView; }
            set
            {
                _rootsView = value;
                RaisePropertyChanged(() => RootsView);
            }
        }
        private ObservableCollection<FunctionRoot> RootsCollection { get; } = 
            new ObservableCollection<FunctionRoot>();

        //for command param purposes
        public ApproxMethodEnum Iter => ApproxMethodEnum.Iterations;
        public ApproxMethodEnum Val => ApproxMethodEnum.Value;

        private ApproxMethodEnum SelectedApproxMethod { get; set; } = ApproxMethodEnum.Value;

        public ObservableCollection<KeyValuePair<double, double>> ChartData { get; } =
            new ObservableCollection<KeyValuePair<double, double>>();

        private double FromX { get; set; }
        private double ToX { get; set; }

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
                _fromXValue = value.Replace('.',',');
                RaisePropertyChanged(() => FromXValueBind);
            }
        }

        private string _toXValue = "100";
        public string ToXValueBind
        {
            get { return _toXValue; }
            set
            {
                _toXValue = value.Replace('.','.');
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

        private string _approxValue = "0,5";
        public string ApproxValueBind
        {
            get { return _approxValue; }
            set
            {
                _approxValue = value.Replace('.',',');
                RaisePropertyChanged(() => ApproxValueBind);
            }
        }

        private int _sliderValue = 3;
        public double SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = (int)value;
                RaisePropertyChanged(() => SliderValue);
            }
        }

        private ICommand _submitDataCommand;
        public ICommand SubmitDataCommand =>
            _submitDataCommand ?? (_submitDataCommand = new RelayCommand(SubmitData));

        private ICommand _setApproxMethodCommand;
        public ICommand SetApproxMethodCommand =>
            _setApproxMethodCommand ??
            (_setApproxMethodCommand = new RelayCommand<ApproxMethodEnum>(method => SelectedApproxMethod = method));
        
        #endregion

        
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            FunctionSelectorSelectedItem = AvailableFunctions[0];
        }

        private void UpdateChart()
        {
            int precVal = (int)((Math.Abs(ToX) + Math.Abs(FromX)) * _sliderValue / 100);
            ChartData.Clear();
            for (int i = (int)FromX; i < (int)ToX; i+= precVal)
            {
                ChartData.Add(new KeyValuePair<double, double>(i, FunctionSelectorSelectedItem.GetValue(i)));
            }
        }

        private void SubmitData()
        {
            double from, to, approx;
            if (!double.TryParse(FromXValueBind, out from) || 
                !double.TryParse(ToXValueBind, out to) ||
                !double.TryParse(ApproxValueBind,out approx))
            {
                MessageBox.Show("Provided values cannot be parsed.", "Try setting different values.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (from >= to)
                MessageBox.Show("Upper boundary is smaller than lower one.", "Try setting different values.", MessageBoxButton.OK, MessageBoxImage.Error);

            FromX = from;
            ToX = to;

            try
            {
                RootsCollection.Add(MathCore.GetFunctionRootFalsi(FunctionSelectorSelectedItem, new GetFunctionRootBiArgs
                {
                    FromX = from,
                    ToX = to,
                    Approx = approx,
                    MaxIterations = SelectedApproxMethod == ApproxMethodEnum.Iterations ? _maxIterations : -1
                }));
                RootsCollection.Add(MathCore.GetFunctionRootBi(FunctionSelectorSelectedItem, new GetFunctionRootBiArgs
                {
                    FromX = from,
                    ToX = to,
                    Approx = approx,
                    MaxIterations = SelectedApproxMethod == ApproxMethodEnum.Iterations ? _maxIterations : -1
                }));
                RootsView = new ListCollectionView(RootsCollection);
                RootsView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            }
            catch (ArgumentException)
            {
                MessageBox.Show("For provided arguments function does not have root or has odd amount of them.","Try setting different values.",MessageBoxButton.OK, MessageBoxImage.Error);
            }


            UpdateChart();
        }
        
    }
}