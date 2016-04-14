using System.Windows;
using System.Windows.Controls;
using NumMethods2.ViewModel;

namespace NumMethods2
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainPageViewInteraction
    {
        public MainWindow()
        {
            InitializeComponent();
            (DataContext as MainViewModel).View = this;
        }

        public DataGrid MatrixGrid => Grid;
        public DataGrid ResGrid => ResultsGrid;


        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                (DataContext as MainViewModel).LoadFromFile(files[0]);
            }
        }
    }
}