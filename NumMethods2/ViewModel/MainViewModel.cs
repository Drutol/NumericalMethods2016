using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Gu.Wpf.DataGrid2D;
using Microsoft.Win32;
using Newtonsoft.Json;
using NumMethods2.Exceptions;
using NumMethods2.MatrixMath;

namespace NumMethods2.ViewModel
{
    public interface IMainPageViewInteraction
    {
        DataGrid MatrixGrid { get; }
        DataGrid ResGrid { get; }
    }

    public class MainViewModel : ViewModelBase
    {
        private ICommand _extendMatrixCommand;

        public ICommand ExtendMatrixCommand =>
            _extendMatrixCommand ?? (_extendMatrixCommand = new RelayCommand(ExtendMatrix));

        private ICommand _addMatrixRowCommand;

        private ICommand _submitDataCommand;

        public ICommand SubmitDataCommand =>
            _submitDataCommand ?? (_submitDataCommand = new RelayCommand(() =>
            {
                _matrix = (double[,]) View.MatrixGrid.GetArray2D();
                _resultsGrid = (double[,]) View.ResGrid.GetArray2D();
                _iterationLog = EnableDebugLog ? new List<Tuple<double[,], double[,]>>() : null;
                MatrixSolutions = new List<double>();
                MatrixSolutions = NumCore.FindMatrixSolutions((double[,])_matrix.Clone(), (double[,])_resultsGrid.Clone(),ref _iterationLog);
                RaisePropertyChanged(() => IterationLog);
            }));

        private ICommand _loadFromFileCommand;

        public object LoadFromFileCommand =>
            _loadFromFileCommand ?? (_loadFromFileCommand = new RelayCommand(LoadFromFile));

        private int _currentlySelectedArraySizeIndex = 0;

        public int CurrentlySelectedArraySizeIndex
        {
            get { return _currentlySelectedArraySizeIndex; }
            set
            {
                _currentlySelectedArraySizeIndex = value;
                Matrix = new double[PossibleArraySizes[value],PossibleArraySizes[value]];
                ResultsGrid = new double[PossibleArraySizes[value],1];
                Size = PossibleArraySizes[value];
                RaisePropertyChanged(() => CurrentlySelectedArraySizeIndex);
            }
        }

        public List<int> PossibleArraySizes { get; } = new List<int>
        {
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10
        };

        private bool _enableDebugLog;
        public bool EnableDebugLog
        {
            get { return _enableDebugLog; }
            set
            {
                _enableDebugLog = value;
                RaisePropertyChanged(() => EnableDebugLog);
            }
        }
        /// <summary>
        /// Item 1 is matrix napshot , Item 2 is results snapshot prepared for UI display.
        /// </summary>
        private List<Tuple<double[,],double[,]>> _iterationLog = new List<Tuple<double[,], double[,]>>();
        /// <summary>
        /// Item 1 is matrix snapshot , 
        /// Item 2 is results snapshot , 
        /// Item 3 is iteration counter
        /// </summary>
        public ObservableCollection<Tuple<double[,],double[,], string>> IterationLog
        {
            get
            {
                if(_iterationLog == null)
                    return new ObservableCollection<Tuple<double[,], double[,], string>>();
                var output = new ObservableCollection<Tuple<double[,],double[,],string>>();
                for (int i = 0; i < _iterationLog.Count-1; i++)
                {
                    output.Add(new Tuple<double[,],double[,], string>(_iterationLog[i].Item1,_iterationLog[i].Item2,(i+1).ToString()));
                }
                return output;
            }
        } 


        public IMainPageViewInteraction View { get; set; }

        public int Size { get; set; } = 2;

        private double[,] _matrixBackup;
        private double[,] _matrix { get; set; } = new double[,] { { 0, 0 }, { 0, 0 } };

        public double[,] Matrix
        {
            get { return _matrix; }
            set
            {
                _matrix = value;
                RaisePropertyChanged(() => Matrix);
            }
        }

        private double[,] _resultsGrid = new double[,] { { 0 }, { 0 } };

        public double[,] ResultsGrid
        {
            get
            {
                return _resultsGrid;
            }
            set
            {
                _resultsGrid = value;
                RaisePropertyChanged(() => ResultsGrid);
            }
        }

        private List<double> _matrixSolutions = new List<double>();

        public List<double> MatrixSolutions
        {
            get
            {
                return _matrixSolutions;
            }
            set
            {
                _matrixSolutions = value;
                RaisePropertyChanged(() => MatrixSolutions);
            }
        }



        public void ExtendMatrix()
        {
            List<List<double>> list = new List<List<double>>();
            var flatMatrix = Matrix.Cast<double>();
            for (int i = 0; i < Size; i++)
            {
                var row = flatMatrix.Skip(i * Size).Take(Size).ToList();
                row.Add(0);
                list.Add(row);
            }
            var newRow = new List<double>();
            for (int i = 0; i < Size; i++)
                newRow.Add(0);
            list.Add(newRow);
            Size++;
            ResultsGrid = Utils.ResizeArray(ResultsGrid, Size, 1);
            Matrix = Utils.To2DArray<double>(list);
        }

        private void LoadFromFile()
        {
            var fp = new OpenFileDialog();
            if(fp.ShowDialog() ?? false)
            using (var writer = new StreamReader(fp.OpenFile()))
            {
                try
                {
                    var data = MatrixFileLoadingManager.LoadData(writer.ReadToEnd());
                    Matrix = data.Item1;
                    ResultsGrid = data.Item2;
                    Size = Matrix.GetLength(0);
                }
                catch (InvalidMatrixFileException)
                {
                    //
                }

            }
        }

        public void LoadFromFile(string path)
        {
            using (var writer = new StreamReader(path))
            {
                try
                {
                    var data = MatrixFileLoadingManager.LoadData(writer.ReadToEnd());
                    Matrix = data.Item1;
                    ResultsGrid = data.Item2;
                    Size = Matrix.GetLength(0);
                }
                catch (InvalidMatrixFileException)
                {
                    //
                }

            }
        }

        public MainViewModel()
        {

        }

    }
}