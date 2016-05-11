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
using NumMethods4Lib.MathCore;

namespace NumMethods4.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        public enum IntervalSymbols
        {
            Parenthesis,
            Bracket,
            Inf,
        }

        public ObservableCollection<IFunction> AvailableFunctions { get; }
            = new ObservableCollection<IFunction>
            {
                    new Function1(),
                    new Function2(),
                    new Function3(),
                    new Function4()
            };

        private IFunction SelectedFunction { get; set; } = new Function1();

        public List<string> LeftEndpointSigns { get; }
            = new List<string>
            {
                "(",
                "[",
                "\u221E"
            };

        public List<string> RightEndpointSigns { get; }
            = new List<string>
            {
                ")",
                "]",
                "\u221E"
            };

        private IntervalSymbols _selectedLeftSignType = IntervalSymbols.Bracket;
        public int SelectedLeftSignIndex
        {
            get { return (int)_selectedLeftSignType; }
            set
            {
                _selectedLeftSignType = (IntervalSymbols)value;
                RaisePropertyChanged(() => SelectedLeftSignIndex);
            }
        }

        private IntervalSymbols _selectedRightSignType = IntervalSymbols.Bracket;
        public int SelectedRightSignType
        {
            get { return (int)_selectedRightSignType; }
            set
            {
                _selectedRightSignType = (IntervalSymbols)value;
                RaisePropertyChanged(() => SelectedRightSignType);
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
                IntervalTypes intervalType;
                switch (_selectedLeftSignType)
                {
                    case IntervalSymbols.Parenthesis:
                        switch (_selectedRightSignType)
                        {
                            case IntervalSymbols.Parenthesis:
                                intervalType = IntervalTypes.BothOpen;
                                break;
                            case IntervalSymbols.Bracket:
                                intervalType = IntervalTypes.LeftOpen;
                                break;
                            case IntervalSymbols.Inf:
                                intervalType = IntervalTypes.InfRightLeftOpen;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case IntervalSymbols.Bracket:
                        switch (_selectedRightSignType)
                        {
                            case IntervalSymbols.Parenthesis:
                                intervalType = IntervalTypes.RightOpen;
                                break;
                            case IntervalSymbols.Bracket:
                                intervalType = IntervalTypes.BothClosed;
                                break;
                            case IntervalSymbols.Inf:
                                intervalType = IntervalTypes.InfRightLeftClosed;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case IntervalSymbols.Inf:
                        switch (_selectedRightSignType)
                        {
                            case IntervalSymbols.Parenthesis:
                                intervalType = IntervalTypes.InfLeftRightOpen;
                                break;
                            case IntervalSymbols.Bracket:
                                intervalType = IntervalTypes.InfLeftRightClosed;
                                break;
                            case IntervalSymbols.Inf:
                                intervalType = IntervalTypes.InfBoth;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            if (intervalType == IntervalTypes.InfBoth)
                ResultBind = "\u221E";
            else
                ResultBind = NumCore.SimpsonsMethod(integrateFrom, integrateTo, SelectedFunction, accuracy,intervalType).ToString();
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