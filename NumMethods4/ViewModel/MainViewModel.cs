using System;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
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
                    new Function4()
            };

        private IFunction SelectedFunction { get; set; } = new Function1();

        public ObservableCollection<string> LeftEndpointSigns { get; }
            = new ObservableCollection<string>
            {
                "(",
                "[",
                "\u221E"
            };

        private string _selectedLeftSign = "[ ";

        public string SelectedLeftSign
        {
            get { return _selectedLeftSign; }
            set
            {
                if (value != "\u221E")
                {
                    _selectedLeftSign = value + " ";
                    RaisePropertyChanged(() => SelectedLeftSign);
                }
                else
                {
                    _selectedLeftSign = "( ";
                    _integrateFromX = "\u221E";
                    RaisePropertyChanged(() => IntegrateFromX);
                    RaisePropertyChanged(() => SelectedLeftSign);
                }
            }
        }

        public ObservableCollection<string> RightEndpointSigns { get; }
            = new ObservableCollection<string>
            {
                ")",
                "]",
                "\u221E"
            };

        private string _selectedRightSign = " ]";

        public string SelectedRightSign
        {
            get { return _selectedRightSign; }
            set
            {
                if (value != "\u221E")
                {
                    _selectedRightSign = " " + value;
                    RaisePropertyChanged(() => SelectedRightSign);
                }
                else
                {
                    _selectedRightSign = " )";
                    _integrateToX = "\u221E";
                    RaisePropertyChanged(() => IntegrateToX);
                    RaisePropertyChanged(() => SelectedRightSign);
                }
            }
        }

        private string _resultBind = "";

        public string ResultBind
        {
            get { return _resultBind; }
            private set
            {
                _resultBind = value != null ? $" {value} + C" : "";
                RaisePropertyChanged(() => ResultBind);
            }
        }

        private string _selectedFunctionText = " |x| dx =";

        public string SelectedFunctionText
        {
            get { return _selectedFunctionText; }
            private set
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
                ResultBind = null;
                _functionSelectorSelectedIndex = value;
                SelectedFunctionText = AvailableFunctions[value].TextRepresentation;
                SelectedFunction = AvailableFunctions[value];
            }
        }

        private ICommand _calculateCommand;

        public ICommand CalculateCommand => _calculateCommand ?? (_calculateCommand = new RelayCommand(() =>
        {
            if (IntegrateFromX != "\u221E" && IntegrateToX != "\u221E")
            {
                double integrateFrom, integrateTo, accuracy;
                if (!double.TryParse(IntegrateFromX, out integrateFrom) ||
                    !double.TryParse(IntegrateToX, out integrateTo) ||
                    !double.TryParse(IntegrationAccuracy, out accuracy))
                {
                    MessageBox.Show( /*Locale["#CannotParseMsg"], Locale["#CannotParseTitle"]*/
                        "parse", "", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                if (integrateFrom > integrateTo)
                {
                    MessageBox.Show( /*Locale["#IntervalErrorMsg"], Locale["#IntervalErrorTitle"]*/
                        "from>to", "", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                double includingEndpointsResult = integrateFrom < integrateTo
                    ? NumCore.SimpsonsMethod(integrateFrom, integrateTo, SelectedFunction, accuracy)
                    : SelectedFunction.GetValue(integrateFrom);
                bool excludeLeft = false, excludeRight = false;
                if (SelectedLeftSign == "( ")
                    excludeLeft = true;
                if (SelectedRightSign == " )")
                    excludeRight = true;
                if (excludeLeft || excludeRight)
                {
                    ResultBind = NumCore.ExcludingEndpointsIntegration(integrateFrom, integrateTo, excludeLeft,
                            excludeRight, SelectedFunction, includingEndpointsResult).ToString();
                }
                else
                    ResultBind = includingEndpointsResult.ToString();
            }
            else
            {
                ResultBind = "\u221E";
            }
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