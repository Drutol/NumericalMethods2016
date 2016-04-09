using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Gu.Wpf.DataGrid2D;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace NumMethods2.ViewModel
{
    public interface IMainPageViewInteraction
    {
        DataGrid MatrixGrid { get; }
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
                MatrixSolutions = MatrixMath.NumCore.FindMatrixSolutions(_matrix, _resultsGrid);
            }));

        private ICommand _loadFromFileCommand;

        public object LoadFromFileCommand =>
            _loadFromFileCommand ?? (_loadFromFileCommand = new RelayCommand(LoadFromFile));

        public IMainPageViewInteraction View { get; set; }

        public int Rows { get; set; } = 2;
        public int Columns { get; set; } = 2;

        public double[,] _matrix { get; set; } = new double[,] { { 0, 0 }, { 0, 0 } };

        public double[,] Matrix
        {
            get { return _matrix; }
            set
            {
                _matrix = value;
                RaisePropertyChanged(() => Matrix);
            }
        }

        private double[,] _resultsGrid = new double[,] { { 0}, { 0 } };

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
            for (int i = 0; i < Rows; i++)
            {
                var row = flatMatrix.Skip(i * Columns).Take(Columns).ToList();
                row.Add(0);
                list.Add(row);
            }
            var newRow = new List<double>();
            for (int i = 0; i < Columns; i++)
                newRow.Add(0);
            list.Add(newRow);
            Columns++;
            Rows++;
            ResultsGrid = Utils.ResizeArray(ResultsGrid, Rows, 1);
            Matrix = Utils.To2DArray<double>(list);
        }

        private void LoadFromFile()
        {
            var fp = new OpenFileDialog();
            if(fp.ShowDialog() ?? false)
            using (var writer = new StreamReader(fp.OpenFile()))
            {
                var data = JsonConvert.DeserializeObject<Tuple<double[,], double[,]>>(writer.ReadToEnd());
                Matrix = data.Item1;
                ResultsGrid = data.Item2;
                Rows = Columns = Matrix.GetLength(0);
                
            }

        }

        public MainViewModel()
        {

        }

    }
}