using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NumMethods4Lib.MathCore;
using NumMethods5.NumCoreApprox;
using OxyPlot;

namespace NumMethods5.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<IFunction> AvailableFunctions { get; }
            = new ObservableCollection<IFunction>
            {
                    new Function1(),
                    new Function2(),
                    new Function3(),
            };

        private IFunction SelectedFunction { get; set; } = new Function1();

        private int _functionSelectorSelectedIndex;

        public int FunctionSelectorSelectedIndex
        {
            get { return _functionSelectorSelectedIndex; }
            set
            {
                _functionSelectorSelectedIndex = value;
                SelectedFunction = AvailableFunctions[value];
            }
        }

        private string _fromX = "1";

        public string FromXBind
        {
            get { return _fromX;}
            set
            {
                _fromX = value;
                RaisePropertyChanged(() => FromXBind);
            }
        }

        private string _toX = "3";

        public string ToXBind
        {
            get { return _toX; }
            set
            {
                _toX = value;
                RaisePropertyChanged(() => ToXBind);
            }
        }

        private string _nodesCount = "5";

        public string NodesCountBind
        {
            get { return _nodesCount; }
            set
            {
                _nodesCount = value;
                RaisePropertyChanged(() => NodesCountBind);
            }
        }

        private string _maxIter = "100";

        public string MaxIterBind
        {
            get { return _maxIter; }
            set
            {
                _maxIter = value;
                RaisePropertyChanged(() => MaxIterBind);
            }
        }

        private string _precision;

        public string PrecisionBind
        {
            get { return _precision; }
            set
            {
                _precision = value;
                RaisePropertyChanged(() => PrecisionBind);
            }
        }

        private List<DataPoint> _accuratePlot = new List<DataPoint>();

        public List<DataPoint> AccuratePlot
        {
            get
            {
                return _accuratePlot;
            }
            set
            {
                _accuratePlot = value;
                RaisePropertyChanged(() => AccuratePlot);
            }
        }

        private List<DataPoint> _approxPlot = new List<DataPoint>();

        public List<DataPoint> ApproxPlot
        {
            get
            {
                return _approxPlot;
            }
            set
            {
                _approxPlot = value;
                RaisePropertyChanged(() => ApproxPlot);
            }
        }

        public ICommand CalculateCommand => new RelayCommand(DoMaths);

        private void DoMaths()
        {
            try
            {
                AccuratePlot =
                    NumCoreApprox.NumCore.GetAccuratePlotDataPoints(SelectedFunction, CurrentInterval).ToList();

                ApproxPlot =
                    NumCoreApprox.NumCore.GetApproximatedPlotDataPoints(SelectedFunction, CurrentInterval, 5,
                        new ApproximationByPolymonialLevel(5)).Select(x => new DataPoint(x.X, x.Y)).ToList();
            }          
            catch (Exception)
            {

            }

        }

        private Interval CurrentInterval
        {
            get
            {
                try
                {
                    return new Interval
                    {
                        From = double.Parse(FromXBind),
                        To = double.Parse(ToXBind)
                    };
                }
                catch (Exception)
                {
                    throw new ArgumentException();
                }

            }
        }

        public MainViewModel()
        {
            
        }
    }
}