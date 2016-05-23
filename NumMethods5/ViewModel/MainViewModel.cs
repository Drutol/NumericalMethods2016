using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
                _fromX = value.Replace(".",",");
                RaisePropertyChanged(() => FromXBind);
            }
        }

        private string _fromDrawX = "1";

        public string FromXDrawBind
        {
            get { return _fromDrawX; }
            set
            {
                _fromDrawX = value.Replace(".",",");
                RaisePropertyChanged(() => FromXDrawBind);
            }
        }

        private string _toX = "3";

        public string ToXBind
        {
            get { return _toX; }
            set
            {
                _toX = value.Replace(".", ",");
                RaisePropertyChanged(() => ToXBind);
            }
        }

        private string _toDrawX = "3";

        public string ToXDrawBind
        {
            get { return _toDrawX; }
            set
            {
                _toDrawX = value.Replace(".", ",");
                RaisePropertyChanged(() => ToXDrawBind);
            }
        }

        private string _nodesCount = "10";

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

        private string _precision = "5";

        public string PrecisionBind
        {
            get { return _precision; }
            set
            {
                _precision = value.Replace(".", ",");
                RaisePropertyChanged(() => PrecisionBind);
            }
        }

        private string _approxTime = "--";

        public string ApproxTime
        {
            get { return _approxTime; }
            set
            {
                _approxTime = value;
                RaisePropertyChanged(() => ApproxTime);
            }
        }

        private string _error = "--";

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                RaisePropertyChanged(() => Error);
            }
        }

        private string _polynomial = "--";

        public string Polynomial
        {
            get { return _polynomial; }
            set
            {
                _polynomial = value;
                RaisePropertyChanged(() => Polynomial);
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
                    NumCoreApprox.NumCore.GetAccuratePlotDataPoints(SelectedFunction, DrawInterval).ToList();
                double error;
                var timer = new Stopwatch();
                timer.Start();
                ApproxPlot =
                    NumCoreApprox.NumCore.GetApproximatedPlotDataPoints(SelectedFunction, ApproxInterval, int.Parse(NodesCountBind),
                        new ApproximationByPolymonialLevel(int.Parse(PrecisionBind), UseCotes),out error).Select(x => new DataPoint(x.X, x.Y)).ToList();
                timer.Stop();
                ApproxTime = timer.ElapsedTicks.ToString();
                Error = error.ToString();
                Polynomial = string.Join("x^?+", NumCoreApprox.NumCore.GetPolynomialCoeffs(int.Parse(PrecisionBind)));
            }          
            catch (Exception)
            {

            }

        }

        private Interval ApproxInterval
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

        private Interval DrawInterval
        {
            get
            {
                try
                {
                    return new Interval
                    {
                        From = double.Parse(FromXDrawBind),
                        To = double.Parse(ToXDrawBind)
                    };
                }
                catch (Exception)
                {
                    throw new ArgumentException();
                }

            }
        }

        public bool UseCotes { get; set; }

        public MainViewModel()
        {
            
        }
    }
}