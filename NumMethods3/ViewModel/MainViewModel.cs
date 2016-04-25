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
    public class RawInputSelection : ISelectable
    {
        public string TextRepresentation => "Input values manually.";
    }

    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<ISelectable> AvailableFunctions { get; }
            = new ObservableCollection<ISelectable>
            {
                        new Function1(),
                        new Function2(),
                        new Function3(),
                        new Function4(),
                        new RawInputSelection()
            };

        /// <summary>
        ///     Curently selected function by user.
        /// </summary>
        private IFunction SelectedFunction { get; set; } = new Function1();

        private int _functionSelectorSelectedIndex;

        public int FunctionSelectorSelectedIndex
        {
            get { return _functionSelectorSelectedIndex; }
            set
            {
                _functionSelectorSelectedIndex = value;
                SelectedFunction = AvailableFunctions[value] as IFunction;
                FunctionModeVisibility = value < AvailableFunctions.Count - 1 ? Visibility.Visible : Visibility.Collapsed;
                
            }
        }

        private ObservableCollection<FunctionValue> _functionValues = new ObservableCollection<FunctionValue>
        {
            new FunctionValue {X = -10,Y= 10},
            new FunctionValue {X = -5,Y= 12},
            new FunctionValue {X = -7,Y= 45},
            new FunctionValue {X = -1,Y= 2},
            new FunctionValue {X = 10,Y= 15},
        };
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
                int nodeCount;
                double toX, fromX;
                double nodeDist;
                int precision = 20000;
                var nodes = new List<FunctionValue>();
                var interpolated = new List<FunctionValue>();
                var interpolationResults = new List<FunctionValue>();
                if (SelectedFunction != null)
                {
                    nodeCount = int.Parse(InterpolationNodesCount);
                    toX = double.Parse(ToXValueBind);
                    fromX = double.Parse(FromXValueBind);
                    nodeDist = Math.Abs((fromX - toX) / (nodeCount - 1));

                    for (int i = 0; i < nodeCount; i++)
                    {
                        nodes.Add(new FunctionValue
                        {
                            X = i == nodeCount - 1 ? toX : fromX + i * nodeDist,
                        });
                        nodes[i].Y = SelectedFunction.GetValue(nodes[i].X);
                    }
                }

                else
                {
                    var nodesInPreparation = FunctionValues.ToList();
                    if(nodesInPreparation.Select(value => value.X).Distinct().ToList().Count != nodesInPreparation.Count)
                        return; // doubled x value

                    nodes = nodesInPreparation.OrderBy( value => value.X ).ToList();
                    nodeCount = nodesInPreparation.Count;
                    toX = nodes.Last().X;
                    fromX = nodes.First().X;
                    nodeDist = Math.Abs((fromX - toX) / (nodeCount - 1));
                }

                var progressives = NumCore.ProgressiveSubs(nodeCount, nodes.Select(value => value.Y).ToList());

                for (double i = 0; i < precision; i++)
                {
                    var current = new FunctionValue();
                    current.X = fromX + i*(toX - fromX)/precision;
                    double t = (current.X - nodes[0].X)/nodeDist;
                    current.Y = NumCore.NewtonsInterpolation(t, progressives);
                    interpolationResults.Add(current);
                    if (SelectedFunction != null)
                        interpolated.Add(new FunctionValue
                        {
                            X = current.X,
                            Y = SelectedFunction.GetValue(current.X)
                        });
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

        private Visibility _functionModeVisibility;

        public Visibility FunctionModeVisibility
        {
            get { return _functionModeVisibility; }
            set
            {
                _functionModeVisibility = value;
                RaisePropertyChanged(() => FunctionModeVisibility);
            }
        }
    
        public List<DataPoint> ChartDataInterpolation { get; set; } =
            new List<DataPoint>();

        public List<DataPoint> ChartDataInterpolated { get; set; } =
            new List<DataPoint>();

        public List<DataPoint> NodeChartData { get; set; } =
            new List<DataPoint>();
    }
}