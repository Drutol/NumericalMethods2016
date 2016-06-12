using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NumMethods6.MathCore;
using OxyPlot;


namespace NumMethods6.ViewModel
{
    public class Interval
    {
        public double From { get; set; }
        public double To { get; set; }
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

        private string _integrationStep = "1";
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

        private string _diffDegree = "4";
        public string DiffDegreeBind
        {
            get { return _diffDegree; }
            set
            {
                _diffDegree = value;
                RaisePropertyChanged(() => DiffDegreeBind);
            }
        }

        public double DiffDegree => double.Parse(DiffDegreeBind.Replace(".", ","));

        public ICommand DoMathsCommand => new RelayCommand(DoMaths);



        #endregion

        public const int K = 2;
        public const float R = 0.5f;
        public const float L = 0.001f;
        public const float J1 = 0.3f;
        public const int C = 60;
        public const float Mu = 0.6f;
        public const float J2 = 0.2f;
        public const int M0 = 60;
        public const int Ut = 60;

        private double u(double t, double x, double y, double z, double w)
        {
            return 1;
        }

        private double M0Function(double t, double x, double y, double z, double w)
        {
            return 1;
        }


        private void DoMaths()
        {
            MatrixRequest?.Invoke(); //updates matrix
                                     //TODO : Maths
            double X = 0;

            var fun = new List<RungeKutta.DiffFun>
            {
                (t, variables) => -R/L*variables[0]-K/L*variables[1]+1/L*60,
                (t, variables) => -K/J1*variables[0]-Mu/J1*variables[1]-C/J1*variables[2]-Mu/J1*variables[3],
                (t, variables) => variables[1]-variables[3],
                (t, variables) => Mu/J2*variables[1]+C/J2*variables[2]-Mu/J2*variables[3]-1/J2*60
            };
            var dynFun = new List<RungeKutta.DynamicDiffFun>
            {
                ((d, variables) => variables.y),
                ((d, variables) => -Math.Sin(variables.x) + Math.Sin(5*d))
            };
            var results = new List<DataPoint>();
            var results1 = new List<DataPoint>();
            //var points = new double[]
            //{0, 0 , 0 ,0};
            //while (X < 3)
            //{
            //    X += .1;
            //    var result = RungeKutta.Rk4(X, points, 0.1, fun);
            //    results.Add(new DataPoint(X, result.Skip(3).First()));
            //    results1.Add(new DataPoint(X, result.Skip(2).First()));
            //}
            var points = new double[]
            {1, 1};
            while (X < 100)
            {             
                var result = RungeKutta.Rk4(X, points, .05, dynFun, new List<string> { "x", "y" });
                results.Add(new DataPoint(X, result.First()));
                results1.Add(new DataPoint(X, result.Skip(1).First()));
                X += .05;
            }
            DifferentialDataPoints = results;
            ComparisionDataPoints = results1;
        }
    }
}