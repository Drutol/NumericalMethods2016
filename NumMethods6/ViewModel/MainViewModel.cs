using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NumMethods4Lib.MathCore;
using NumMethods6.MathCore;
using OxyPlot;


namespace NumMethods6.ViewModel
{
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
                new Function3(),
            };

        public IFunction CurrentlySelectedFunction { get; set; }

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

        private void DoMaths()
        {
            MatrixRequest?.Invoke(); //updates matrix
            //TODO : Maths
        }

        #endregion




    }
}