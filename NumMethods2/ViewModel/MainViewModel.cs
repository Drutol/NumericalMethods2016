using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GalaSoft.MvvmLight;

namespace NumMethods2.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public double[,] _matrix { get; set; } = new double[,] { {1,1,0} , {2,2,0} , {3,3,0} , {1,2,0} , {3,1,0} };

        public int Rows { get; set; } = 5; 
        public int Columns { get; set; } = 3;


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


        public MainViewModel()
        {
            AddMatrixColumn();
            AddMatrixColumn();
            AddMatrixColumn();
        }

    }
}