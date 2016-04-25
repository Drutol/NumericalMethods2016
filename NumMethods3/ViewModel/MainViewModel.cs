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

                    /*
                         *  doublenewton_interpolation(constXF *xf,unsigned intxf_size,doublex)
                         *  {
                         *  doubleresult = xf[0].f;
                         *  double*a =new double[xf_size];
                         *  for(int i = 0; i < xf_size; i++)
                         *  a[i] = xf[i].f;
                         *  for(inti = 1; i < xf_size; i++)
                         *  for(intj = 0; j < i; j++)
                         *  a[i] = (a[i] - a[j]) / (xf[i].x - xf[j].x);
                         *  for(inti = 1; i < xf_size; i++)
                         *  {
                         *  double f = 1.0;
                         *  for(int j = 0; j < i; j++)
                         *  f *= x - xf[j].x;
                         *  result += a[i] * f;
                         *  } 
                         *  return result;
                         *  }
                         * 
                         * 
                         * 
                         */
                          }

                var progressives = NumCore.ProgressiveSubs(nodeCount, nodes.Select(value => value.Y).ToList());
                if (SelectedFunction != null)
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
                else
                {
                    List<double> coeficientes = new List<double>();
                    int k;
                    int puntos = nodeCount;
                    double valor;
                    coeficientes.Add(nodes[0].Y);
                    for (int j = 1; j < puntos; j++)
                    {
                        k = (nodes.Count - puntos + j);
                        for (int i = 1; i < puntos - (j - 1); i++, k++)
                        {
                            valor = (nodes[k].Y - nodes[k - 1].Y)/(nodes[i + j - 1].X - nodes[i - 1].X);
                            nodes.Add(new FunctionValue {X=0,Y=valor});
                            if (i == 1)
                            {
                                coeficientes.Add(valor);
                            }
                        }
                    }
                    for (double x = 0; x < precision; x++)
                    {
                        List<double> dif = new List<double>();
                        valor = coeficientes[0];
                        var val = fromX + x * (toX - fromX) / precision;
                        dif.Add(val - nodes[0].X);

                        for (int i = 0; i < puntos - 1; i++)
                        {
                            dif.Add((val - nodes[i + 1].X) * (dif[i]));
                        }
                        for (int i = 0; i < coeficientes.Count - 1; i++)
                        {
                            valor = valor + (coeficientes[1 + i] * dif[i]);
                        }
                        interpolationResults.Add(new FunctionValue {X = val,Y=valor});
                    }

                }


                NodeChartData = nodes.Take(nodeCount).Select(value => value.ToDataPoint).ToList();
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