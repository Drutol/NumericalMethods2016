using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gu.Wpf.DataGrid2D;
using NumMethods6.ViewModel;

namespace NumMethods6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel.MatrixRequest += OnMatrixRequest;
        }

        private void OnMatrixRequest()
        {
            try
            {
                ViewModel.VariableMatrix = (double[,])VarGrid.GetArray2D();
            }
            catch (Exception)
            {
                //some ill things are in there
            }
            
        }
    }
}
