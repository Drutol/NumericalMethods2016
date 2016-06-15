using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NumMethods4Lib.MathCore;
using NumMethods6.MathCore;
using OxyPlot;


namespace NumMethods6.ViewModel
{
    public class Interval
    {
        public double From { get; set; }
        public double To { get; set; }
    }

    public enum Method
    {
        RK4,
        Ralston,
        User
    }

    public delegate void RequestVariableMatrix();

    public class MainViewModel : ViewModelBase
    {
        public event RequestVariableMatrix MatrixRequest;

        #region Props
        private List<DataPoint> _differentialDataPoints = new List<DataPoint>();

        public List<DataPoint> DifferentialDataPoints
        {
            get { return _differentialDataPoints; }
            set
            {
                _differentialDataPoints = value;
                RaisePropertyChanged(() => DifferentialDataPoints);
            }
        }

        private List<DataPoint> _comparisionDataPoints = new List<DataPoint>();

        public List<DataPoint> ComparisionDataPoints
        {
            get { return _comparisionDataPoints; }
            set
            {
                _comparisionDataPoints = value;
                RaisePropertyChanged(() => ComparisionDataPoints);
            }
        }

        private List<Tuple<string, List<DataPoint>>> _coeffFunctionsAvaibleToDraw ;

        public List<Tuple<string, List<DataPoint>>> CoeffFunctionsAvaibleToDraw
        {
            get { return _coeffFunctionsAvaibleToDraw; }
            set
            {
                _coeffFunctionsAvaibleToDraw = value;
                RaisePropertyChanged(() => CoeffFunctionsAvaibleToDraw);
            }
        }

        private int _selecedCoeffFunctionIndex;

        public int SelectedCoeffFunctionIndex
        {
            get { return _selecedCoeffFunctionIndex; }
            set
            {
                DifferentialDataPoints = CoeffFunctionsAvaibleToDraw[value].Item2;
                _selecedCoeffFunctionIndex = value;
            }
        }

        public ObservableCollection<IFunction> AvailableFunctions { get; }
            = new ObservableCollection<IFunction>
            {
                new Function1(),
                new Function2(),
                new Function3()
            };

        public IFunction CurrentlySelectedFunction { get; set; } = new Function1();

        private double[,] _matrix { get; set; } = {
            {-InitialConditions.R/InitialConditions.L, -InitialConditions.K/InitialConditions.L, 0, 0},
            {InitialConditions.K/InitialConditions.J1,-InitialConditions.Mu/InitialConditions.J1,-InitialConditions.C/InitialConditions.J1,InitialConditions.Mu/InitialConditions.J1},
            {0,1,0,-1},
            {0,InitialConditions.Mu/InitialConditions.J2,InitialConditions.C/InitialConditions.J2,-InitialConditions.Mu/InitialConditions.J2} };

        public double[,] ConstMatrix
        {
            get { return _matrix; }
            set
            {
                _matrix = value;
                RaisePropertyChanged(() => ConstMatrix);
            }
        }

        private double[,] _variableMatrix { get; set; } = {
            { 1 },
            { 1 },
            { 1 },
            { 1 }
        };

        public double[,] VariableMatrix
        {
            get { return _variableMatrix; }
            set
            {
                _variableMatrix = value;
                RaisePropertyChanged(() => VariableMatrix);
            }
        }

        private string _integrationStep = "10";
        public string IntegrationStepBind
        {
            get { return _integrationStep; }
            set
            {
                _integrationStep = value;
                RaisePropertyChanged(() => IntegrationStepBind);
            }
        }

        public double IntegrationStep => double.Parse(IntegrationStepBind.Replace(".", ","));

        //private string _diffDegree = "4";
        //public string DiffDegreeBind
        //{
        //    get { return _diffDegree; }
        //    set
        //    {
        //        _diffDegree = value;
        //        RaisePropertyChanged(() => DiffDegreeBind);
        //    }
        //}

        //public double DiffDegree => double.Parse(DiffDegreeBind.Replace(".", ","));

        public ICommand DoMathsCommand => new RelayCommand(DoMaths);

        private string _fromX = "0";

        public string FromXBind
        {
            get { return _fromX; }
            set
            {
                _fromX = value.Replace(".",",");
                RaisePropertyChanged(() => FromXBind);
            }
        }

        private string _toX = "10";

        public string ToXBind
        {
            get { return _toX; }
            set
            {
                _toX = value.Replace(".", ",");
                RaisePropertyChanged(() => ToXBind);
            }
        }

        public Interval DrawingInterval
        {
            get
            {
                try
                {
                    var from = int.Parse(_fromX);
                    var to = int.Parse(_toX);
                    if(from>to)
                        throw new Exception("From is bigger than to.");
                    return new Interval
                    {
                        From = from,
                        To = to
                    };
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "Change interval.", "Interval parsing error.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }

        #endregion

        public const int K = 2;
        public const float R = 0.5f;
        public const float L = 0.001f;
        public const float J1 = 0.3f;
        public const int C = 60;
        public const float Mu = 0.6f;
        public const float J2 = 0.2f;
        
        private void DoMaths()
        {
            MatrixRequest?.Invoke(); //updates matrix
                                     //TODO : Maths
            double X = DrawingInterval.From;
            double step;

            if (double.TryParse(_integrationStep, out step))
            {
                if (step != 0)
                    step = 1/step;
            }
            else
            {
                MessageBox.Show("Bad step value.", "step balue parse err", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var fun = new List<RungeKutta.DiffFun>
            {
                (t, variables) => -R/L*variables[0]-K/L*variables[1]+1/L*60,
                (t, variables) => -K/J1*variables[0]-Mu/J1*variables[1]-C/J1*variables[2]-Mu/J1*variables[3],
                (t, variables) => variables[1]-variables[3],
                (t, variables) => Mu/J2*variables[1]+C/J2*variables[2]-Mu/J2*variables[3]-1/J2*60
            };
            var dynFun = new List<RungeKutta.DynamicDiffFun>
            {
                (t, variables) => -R/L*variables.x-K/L*variables.y+1/L*60,
                (t, variables) => -K/J1*variables.x-Mu/J1*variables.y-C/J1*variables.z-Mu/J1*variables.w,
                (t, variables) => variables.y-variables.w,
                (t, variables) => Mu/J2*variables.y+C/J2*variables.z-Mu/J2*variables.w-1/J2*60
            };
            //var points = new double[]
            //{0, 0 , 0 ,0};
            //while (X < DrawingInterval.To)
            //{
            //    X += step;
            //    var result = RungeKutta.Rk4(X, points, step, fun);
            //    results.Add(new DataPoint(X, result.Skip(3).First()));
            //    results1.Add(new DataPoint(X, result.Skip(2).First()));
            //}
            var points = new double[]
            { 0,0,0,0};
            var coeffFunctions = new List<string> { "x", "y", "z", "w" };
            List<List<DataPoint>> results = coeffFunctions.Select(coeffFunction => new List<DataPoint>()).ToList();
            while (X < DrawingInterval.To)
            {             
                var result = RungeKutta.Rk4(X, points, 0.001, dynFun, coeffFunctions);
                int i =0;
                foreach (var d in result)
                    results[i++].Add(new DataPoint(X,d));

                //results.Add(new DataPoint(X, result.First()));
                //results1.Add(new DataPoint(X, result.Skip(1).First()));
                X += step;
            }
            CoeffFunctionsAvaibleToDraw =
                coeffFunctions.Select((coeff, i) => new Tuple<string,List<DataPoint>>(coeff, results[i])).ToList();
            SelectedCoeffFunctionIndex = 0;
        }
    }
}