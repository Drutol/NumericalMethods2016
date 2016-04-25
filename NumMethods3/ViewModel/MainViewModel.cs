using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NumMethods1.NumCore;
using NumMethods3.MathCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using OxyPlot;

namespace NumMethods3.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        public ObservableCollection<IFunction> AvailableFunctions { get; }
            = new ObservableCollection<IFunction>
            {
                        new Function1(),
                        new Function2(),
                        new Function3(),
                        new Function4()
            };

        /// <summary>
        ///     Curently selected function by user.
        /// </summary>
        private IFunction _functionSelectorSelectedItem;

        public IFunction FunctionSelectorSelectedItem
        {
            get { return _functionSelectorSelectedItem ?? (_functionSelectorSelectedItem = AvailableFunctions[0]); }
            set
            {
                _functionSelectorSelectedItem = value;
            }
        }

        private ObservableCollection<FunctionValue> _functionValues = new ObservableCollection<FunctionValue>();
        public ObservableCollection<FunctionValue> FunctionValues
        {
            get { return _functionValues; }
            set
            {
                _functionValues = value;
                RaisePropertyChanged(() => FunctionValues);
            }
        }

        private ICommand _doMathsCommand;

        public ICommand DoMathsCommand =>
            _doMathsCommand ?? (_doMathsCommand = new RelayCommand(() =>
            {
                int nodeCount = int.Parse(InterpolationNodesCount);
                double toX = double.Parse(ToXValueBind), fromX = double.Parse(FromXValueBind);
                double nodeDist = Math.Abs((fromX - toX)/(nodeCount - 1));
                int precision = (int)Math.Pow(10,(_precValue/2)+1);
                var nodes = new List<FunctionValue>();
                var interpolated = new List<FunctionValue>();
                var interpolationResults = new List<FunctionValue>();
                for (int i = 0; i < nodeCount; i++)
                {
                    nodes.Add(new FunctionValue
                    {
                        X = i == nodeCount - 1 ? toX : fromX + i*nodeDist,
                    });
                    nodes[i].Y = FunctionSelectorSelectedItem.GetValue(nodes[i].X);
                }

                var progressives = NumCore.ProgressiveSubs(nodeCount, nodes.Select(value => value.Y).ToList());

                if (!_isDataEntryEnabled)
                {
                    for (double i = 0; i < precision; i++)
                    {
                        var current = new FunctionValue();
                        current.X = fromX + i*(toX - fromX)/precision;
                        double t = (current.X - nodes[0].X)/nodeDist;
                        current.Y = NumCore.NewtonsInterpolation(t, progressives);
                        interpolationResults.Add(current);
                        interpolated.Add(new FunctionValue
                        {
                            X = current.X,
                            Y = FunctionSelectorSelectedItem.GetValue(current.X)
                        });
                    }
                }
                else
                {
                    var vals = new List<FunctionValue>(_functionValues.ToList());
                    foreach (var x in vals)
                    {
                        interpolated.Add(new FunctionValue
                        {
                            X = x.X,
                            Y = x.Y
                        });
                        double t = (x.X - nodes[0].X) / nodeDist;
                        x.Y = NumCore.NewtonsInterpolation(t, progressives);
                        interpolationResults.Add(x);
                    }
                }
                NodeChartData = nodes.Select(value => value.ToDataPoint).ToList();
                ChartDataInterpolated = interpolated.Select(value => value.ToDataPoint).ToList();
                ChartDataInterpolation = interpolationResults.Select(value => value.ToDataPoint).ToList();
                RaisePropertyChanged(() => NodeChartData);
                RaisePropertyChanged(() => ChartDataInterpolated);
                RaisePropertyChanged(() => ChartDataInterpolation);

            }));

        private string _fromXValue = "-10";

        public string FromXValueBind
        {
            get { return _fromXValue; }
            set
            {
                _fromXValue = value.Replace('.', ',');
                RaisePropertyChanged(() => FromXValueBind);
            }
        }

        private string _toXValue = "10";

        public string ToXValueBind
        {
            get { return _toXValue; }
            set
            {
                _toXValue = value.Replace('.', ',');
                RaisePropertyChanged(() => ToXValueBind);
            }
        }

        private string _interpolationNodesCount = "10";

        public string InterpolationNodesCount
        {
            get { return _interpolationNodesCount; }
            set
            {
                _interpolationNodesCount = value;
                RaisePropertyChanged(() => InterpolationNodesCount);
            }
        }

        private int _precValue = 3;

        public int PrecValueBind
        {
            get { return _precValue; }
            set
            {
                _precValue = value;
                RaisePropertyChanged(() => PrecValueBind);
            }
        }

        private bool _isDataEntryEnabled= true;

        public bool IsDataGridEnabled
        {
            get { return _isDataEntryEnabled; }
            set
            {
                _isDataEntryEnabled = value;
                RaisePropertyChanged(() => IsDataGridEnabled);
                RaisePropertyChanged(() => IsComboBoxEnabled);
            }
        }

        public bool IsComboBoxEnabled
        {
            get { return !_isDataEntryEnabled; }
        }



        public List<DataPoint> ChartDataInterpolation { get; set; } =
            new List<DataPoint>();

        public List<DataPoint> ChartDataInterpolated { get; set; } =
            new List<DataPoint>();

        public List<DataPoint> NodeChartData { get; set; } =
            new List<DataPoint>();

        public MainViewModel()
        {

        }
    }
}