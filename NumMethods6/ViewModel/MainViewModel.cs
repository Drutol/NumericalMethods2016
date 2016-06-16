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

    public enum DiffMethod
    {
        RK4,
        Ralston
    }

    public delegate void RequestVariableMatrix();

    public class MainViewModel : ViewModelBase
    {
        public event RequestVariableMatrix MatrixRequest;

        public MainViewModel()
        {
            CurrentlySelectedFunction = AvailableFunctions[0];
        }

        #region Props
        private DiffMethod CurrentlySelectedMethod { get; set; } = DiffMethod.RK4;

        public ICommand ChooseMethodCommand =>
            new RelayCommand<string>(s => CurrentlySelectedMethod = (DiffMethod)int.Parse(s));

        private IFunction _currentlySelectedFunction;

        public IFunction CurrentlySelectedFunction
        {
            get { return _currentlySelectedFunction; }
            set
            {
                _currentlySelectedFunction = value;
                RaisePropertyChanged(() => CurrentlySelectedFunction);
                var cmp = DifferentialDataPoints.Select(d => new DataPoint(d.X, CurrentlySelectedFunction.GetValue(d.X))).ToList();
                ComparisionDataPoints = cmp;
            }
        }
        
        private double IntegrationStep => double.Parse(IntegrationStepBind.Replace(".", ","));

        public ICommand DoMathsCommand => new RelayCommand(DoMaths);
        
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
                if ( value < 0 )
                    return;
                DifferentialDataPoints = CoeffFunctionsAvaibleToDraw[value].Item2;
                _selecedCoeffFunctionIndex = value;
                RaisePropertyChanged(() => DifferentialDataPoints);
            }
        }

        public ObservableCollection<IFunction> AvailableFunctions { get; }
            = new ObservableCollection<IFunction>
            {
                new Function1 {EnableWeight = false},
                new Function2 {EnableWeight = false},
                new Function3 {EnableWeight = false}
            };

        private double[,] _matrix { get; set; } = {
            {-R/L, -K/L, 0, 0, 1/L*60},
            {K/J1, -Mu/J1, -C/J1, Mu/J1, 0},
            {0, 1, 0, -1, 0},
            {0, Mu/J2, C/J2, -Mu/J2, -1/J2*60}
        };

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
            { 0 },
            { 0 },
            { 0 },
            { 0 }
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

        private string _integrationStep = "0.001";
        public string IntegrationStepBind
        {
            get { return _integrationStep; }
            set
            {
                _integrationStep = value;
                RaisePropertyChanged(() => IntegrationStepBind);
            }
        }

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

        private string _toX = "5";

        public string ToXBind
        {
            get { return _toX; }
            set
            {
                _toX = value.Replace(".", ",");
                RaisePropertyChanged(() => ToXBind);
            }
        }

        private Interval DrawingInterval
        {
            get
            {
                try
                {
                    var from = double.Parse(_fromX);
                    var to = double.Parse(_toX);
                    if(from>to)
                        throw new Exception("\"From t\" is bigger than \"To t\".");
                    return new Interval
                    {
                        From = from,
                        To = to
                    };
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
        #endregion

        #region constants
        private const int K = 2;
        private const float R = 0.5f;
        private const float L = 0.001f;
        private const float J1 = 0.3f;
        private const int C = 60;
        private const float Mu = 0.6f;
        private const float J2 = 0.2f;
        #endregion


        private void DoMaths()
        {
            MatrixRequest?.Invoke(); //updates matrix
            double t, toX, step;
            IEnumerable<double> points;
            double[,] ode;
            DiffMethod method; 
            try
            {
                t = DrawingInterval.From;
                toX = DrawingInterval.To;
                step = IntegrationStep;
                method = CurrentlySelectedMethod;
                points = VariableMatrix.Cast<double>();
                ode = ConstMatrix;
            }
            catch (Exception e)
            {
                MessageBox.Show("Couldn't parse values." + e.Message, "Parse error.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dynFun = new List<DifferentialService.DynamicDiffFun>
            {
                (dt, vars) => ode[0,0]*vars.x+ode[0,1]*vars.y+ode[0,2]*vars.z+ode[0,3]*vars.w+ode[0,4],
                (dt, vars) => ode[1,0]*vars.x+ode[1,1]*vars.y+ode[1,2]*vars.z+ode[1,3]*vars.w+ode[1,4],
                (dt, vars) => ode[2,0]*vars.x+ode[2,1]*vars.y+ode[2,2]*vars.z+ode[2,3]*vars.w+ode[2,4],
                (dt, vars) => ode[3,0]*vars.x+ode[3,1]*vars.y+ode[3,2]*vars.z+ode[3,3]*vars.w+ode[3,4]
            };
            
            var coeffFunctions = new List<string> { "x", "y", "z", "w" };

            //Test case:
            // *Produces beautiful e^ 2x
            //*
            //*
            // var dynFun = new List<DifferentialService.DynamicDiffFun>
            //{
            //     (dt, variables) => -2*variables.x+4*variables.y,
            //     (dt, variables) => 3*variables.x-variables.y
            //};
            // var coeffFunctions = new List<string> { "x", "y", };
            // var points = new double[] { 1, 1 };

            var cmp = new List<DataPoint>();
            List<List<DataPoint>> results = coeffFunctions.Select(coeffFunction => new List<DataPoint>()).ToList();
            while (t < toX)
            {
                //kuttas
                IEnumerable<double> result = new List<double>();
                switch (method)
                {
                    case DiffMethod.RK4:
                        result = DifferentialService.Rk4(t, points, step, dynFun, coeffFunctions);
                        break;
                    case DiffMethod.Ralston:
                        result = DifferentialService.Ralston(t, points, step, dynFun, coeffFunctions);
                        break;
                }
                
                int i = 0;
                foreach (var d in result)
                    results[i++].Add(new DataPoint(t, d));

                if (t + step > toX)
                    step = toX - t;
                points = result.ToArray();

                //draw compare
                if(CurrentlySelectedFunction != null)
                    cmp.Add(new DataPoint(t, CurrentlySelectedFunction.GetValue(t)));
                t += step;
            }
            CoeffFunctionsAvaibleToDraw = coeffFunctions.Select((coeff, i) => new Tuple<string, List<DataPoint>>(coeff, results[i])).ToList();
            ComparisionDataPoints = cmp;
            SelectedCoeffFunctionIndex = 0;
        }
    }
}