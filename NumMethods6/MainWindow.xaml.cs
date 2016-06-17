using System;
using System.Windows;
using Gu.Wpf.DataGrid2D;
using NumMethods6.ViewModel;

namespace NumMethods6
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel.MatrixRequest += OnMatrixRequest;
        }

        public MainViewModel ViewModel => DataContext as MainViewModel;

        private void OnMatrixRequest()
        {
            try
            {
                ViewModel.VariableMatrix = (double[,]) VarGrid.GetArray2D();
            }
            catch (Exception)
            {
                //some ill things are in there
            }
        }
    }
}