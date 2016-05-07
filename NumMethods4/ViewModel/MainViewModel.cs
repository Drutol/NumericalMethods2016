using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using NumMethods4.MathCore;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace NumMethods4.ViewModel
{
    
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<IFunction> AvailableFunctions { get; }
            = new ObservableCollection<IFunction>
            {
                    new Function1(),
                    new Function2(),
                    new Function3(),
                    new Function4(),
            };

        private IFunction SelectedFunction { get; set; } = new Function1();

        private string _resultBind = "";

        public string ResultBind
        {
            get { return _resultBind; }
            set
            {
                _resultBind = $" {value} + C";
                RaisePropertyChanged(() => ResultBind);
            }
        }

        private string _selectedFunctionText=" |x| dx =";

        public string SelectedFunctionText
        {
            get { return _selectedFunctionText; }
            set
            {
                _selectedFunctionText = $" {value} dx =";
                RaisePropertyChanged(() => SelectedFunctionText);
            }
        }

        private int _functionSelectorSelectedIndex;

        public int FunctionSelectorSelectedIndex
        {
            get { return _functionSelectorSelectedIndex; }
            set
            {
                _functionSelectorSelectedIndex = value;
                SelectedFunctionText = AvailableFunctions[value].TextRepresentation;
                SelectedFunction = AvailableFunctions[value];
            }
        }

        private ICommand _calculateCommand;

        public ICommand CalculateCommand => _calculateCommand ?? (_calculateCommand = new RelayCommand(() =>
            {
                double integrateFrom, integrateTo, accuracy;
                if (!double.TryParse(IntegrateFromX, out integrateFrom) || 
                !double.TryParse(IntegrateToX, out integrateTo) ||
                !double.TryParse(IntegrationAccuracy, out accuracy))
                {
                    MessageBox.Show(/*Locale["#CannotParseMsg"], Locale["#CannotParseTitle"]*/"","", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                if (integrateFrom > integrateTo)
                {
                    MessageBox.Show(/*Locale["#IntervalErrorMsg"], Locale["#IntervalErrorTitle"]*/"","", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                ResultBind = "wynik ³a³";
            }));

        public MainViewModel()
        {
            
        
        }

        private string _integrateFromX = "-10";

        public string IntegrateFromX
        {
            get { return _integrateFromX; }
            set
            {
                _integrateFromX = value;
                RaisePropertyChanged(() => IntegrateFromX);
            }
        }

        private string _integrateToX = "10";

        public string IntegrateToX
        {
            get { return _integrateToX; }
            set
            {
                _integrateToX = value;
                RaisePropertyChanged(() => IntegrateToX);
            }
        }

        private string _integrationAccuracy = "10";

        public string IntegrationAccuracy
        {
            get { return _integrationAccuracy; }
            set
            {
                _integrationAccuracy = value;
                RaisePropertyChanged(() => IntegrationAccuracy);
            }
        }
    }
}