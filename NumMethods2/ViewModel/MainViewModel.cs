using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Gu.Wpf.DataGrid2D;

namespace NumMethods2.ViewModel
{
    public interface IMainPageViewInteraction
    {
        DataGrid MatrixGrid { get; }
    }

    public class MainViewModel : ViewModelBase
    {
        private ICommand _addMatrixColumnCommand;

        public ICommand AddMatrixColumnCommand =>
            _addMatrixColumnCommand ?? (_addMatrixColumnCommand = new RelayCommand(AddMatrixColumn));

        private ICommand _addMatrixRowCommand;

        public ICommand AddMatrixRowCommand =>
            _addMatrixRowCommand ?? (_addMatrixRowCommand = new RelayCommand(AddMatrixRow));

        private ICommand _submitDataCommand;

        public ICommand SubmitDataCommand =>
            _submitDataCommand ?? (_submitDataCommand = new RelayCommand(() =>
            {
                _matrix = (double[,]) View.MatrixGrid.GetArray2D();
            }));

        public IMainPageViewInteraction View { get; set; }

        public int Rows { get; set; } = 5;
        public int Columns { get; set; } = 3;

        public double[,] _matrix { get; set; } = new double[,] { {1,1,0} , {2,2,0} , {3,3,0} , {1,2,0} , {3,1,0} };

        public double[,] Matrix
        {
            get { return _matrix; }
            set
            {
                _matrix = value;
                RaisePropertyChanged(() => Matrix);
            }
        }

        public void AddMatrixColumn()
        {
            
            List<List<double>> list = new List<List<double>>();
            var flatMatrix = Matrix.Cast<double>();
            for (int i = 0; i < Rows; i++)
            {
                var row = flatMatrix.Skip(i*Columns).Take(Columns).ToList();
                row.Add(0);
                list.Add(row);
            }
            Columns++;
            Matrix = Utils.To2DArray<double>(list);
        }

        public void AddMatrixRow()
        {
            List<List<double>> list = new List<List<double>>();
            var flatMatrix = Matrix.Cast<double>().ToList();
            for (int i = 0; i < Rows; i++)
            {
                var row = flatMatrix.Skip(i * Columns).Take(Columns).ToList();
                list.Add(row);
            }
            var newRow = new List<double>();
            for (int i = 0; i < Columns; i++)
                newRow.Add(0);
            list.Add(newRow);
            Rows++;
            Matrix = Utils.To2DArray<double>(list);
        }


        public MainViewModel()
        {

        }

    }
}