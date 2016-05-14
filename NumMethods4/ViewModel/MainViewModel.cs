using System;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using NumMethods4Lib.MathCore;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using NumMethods4.Utils;

namespace NumMethods4.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private enum IntervalSymbols
        {
            Parenthesis,
            Bracket,
            Inf,
        }

        private enum CalculationMethod
        {
            NewtonCortes,
            Laguerre,
            Comparison,
        }

        public ObservableCollection<IFunction> AvailableFunctions { get; }
            = new ObservableCollection<IFunction>
            {
                    new Function1(),
                    new Function2(),
                    new Function3(),
                    //new Function4()
            };

        private IFunction SelectedFunction { get; set; } = new Function1();

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
                SecondResult = null;
                _functionSelectorSelectedIndex = value;
                SelectedFunctionText = AvailableFunctions[value].TextRepresentation;
                SelectedFunction = AvailableFunctions[value];
            }
        }

        private bool? _laguerreVisiblity;

        public bool? LaguerreVisibility => _laguerreVisiblity;

        private bool? _newtonVisiblity = true;

        public bool? NewtonVisibility => _newtonVisiblity;

        private string _laguerreNodes = "5";

        public string LaguerreNodes
        {
            get { return _laguerreNodes;}
            set
            {
                _laguerreNodes = value;
                RaisePropertyChanged(() => LaguerreNodes);
            }
        }

        private CalculationMethod _selectedCalculationMethod= CalculationMethod.NewtonCortes;

        public string SelectedCalculationMethod
        {
            get
            {
                switch (_selectedCalculationMethod)
                {
                    case CalculationMethod.NewtonCortes:
                        return Locale["#NCResults"];
                    case CalculationMethod.Laguerre:
                        return Locale["#LResults"];
                    case CalculationMethod.Comparison:
                        return Locale["#NCLResults"];
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public ICommand SelectCalculationMethodCommand => new RelayCommand<string>(s =>
        {
            switch (int.Parse(s))
            {
                case 1:
                    IntegrateFromX = "0";
                    IntegrateToX = "\u221E";
                    _secondResult = null;
                    _laguerreVisiblity = true;
                    _newtonVisiblity = null;
                    break;
                case 2:
                    IntegrateFromX = "0";
                    IntegrateToX = "\u221E";
                    _secondResult = "";
                    _laguerreVisiblity = true;
                    _newtonVisiblity = null;
                    break;
                default:
                    IntegrateFromX = "-10";
                    IntegrateToX = "10";
                    SelectedLeftSignIndex = 1;
                    SelectedRightSignType = 1;
                    _secondResult = null;
                    _laguerreVisiblity = null;
                    _newtonVisiblity = true;
                    break;
            }
            _result = "";
            _selectedCalculationMethod = (CalculationMethod) int.Parse(s);
            RaisePropertyChanged(() => SelectedCalculationMethod);
            RaisePropertyChanged(() => ResultBind);
            RaisePropertyChanged(() => SecondResult);
            RaisePropertyChanged(() => LaguerreVisibility);
            RaisePropertyChanged(() => NewtonVisibility);
        });

        public List<string> LeftEndpointSigns { get; } = new List<string>
        {
            "(",
            "[",
            "-\u221E"
        };

        public List<string> RightEndpointSigns { get; } = new List<string>
        {
            ")",
            "]",
            "\u221E"
        };

        private IntervalSymbols _selectedLeftSignType = IntervalSymbols.Bracket;

        public int SelectedLeftSignIndex
        {
            get { return (int) _selectedLeftSignType; }
            set
            {
                if (value == 2)
                {
                    _integrateFromX = "-\u221E";
                    RaisePropertyChanged(() => IntegrateFromX);
                }
                else if (_integrateFromX == "-\u221E")
                {
                    _integrateFromX = "-10";
                    RaisePropertyChanged(() => IntegrateFromX);
                }
                _selectedLeftSignType = (IntervalSymbols) value;
                RaisePropertyChanged(() => SelectedLeftSignIndex);
            }
        }

        private IntervalSymbols _selectedRightSignType = IntervalSymbols.Bracket;

        public int SelectedRightSignType
        {
            get { return (int) _selectedRightSignType; }
            set
            {
                if (value == 2)
                {
                    _integrateToX = "\u221E";
                    RaisePropertyChanged(() => IntegrateToX);
                }
                else if (_integrateToX == "\u221E")
                {
                    _integrateToX = "10";
                    RaisePropertyChanged(() => IntegrateToX);
                }
                _selectedRightSignType = (IntervalSymbols) value;
                RaisePropertyChanged(() => SelectedRightSignType);
            }
        }

        private string _secondResult;

        public string SecondResult
        {
            get {return _secondResult;}
            set
            {
                _secondResult = _secondResult != null ? value!= null ? $" {value} + C" : "" : null;
                RaisePropertyChanged(() => SecondResult);
            }
        }

        private string _result="";

        public string ResultBind
        {
            get { return _result; }
            private set
            {
                _result = value != null ? $" {value} + C" : "";
                RaisePropertyChanged(() => ResultBind);
            }
        }

        

        private ICommand _calculateCommand;

        public ICommand CalculateCommand => _calculateCommand ?? (_calculateCommand = new RelayCommand(() =>
        {
            double integrateFrom, integrateTo, accuracy;
            int maxIter, lNodes;
            if (!double.TryParse(IntegrateFromX, out integrateFrom) || 
            !double.TryParse(IntegrateToX, out integrateTo) || 
            !double.TryParse(IntegrationAccuracy, out accuracy) || 
            !int.TryParse(_maxIter, out maxIter) ||
            !int.TryParse(LaguerreNodes, out lNodes))
            {
                MessageBox.Show( /*Locale["#CannotParseMsg"], Locale["#CannotParseTitle"]*/
                    "Can't parse, change intput values.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (integrateFrom > integrateTo)
            {
                MessageBox.Show( /*Locale["#IntervalErrorMsg"], Locale["#IntervalErrorTitle"]*/
                    "Interval data was input incorrectly, \nappartently left endpioint is bigger than right endpoint.", "", MessageBoxButton.OK, MessageBoxImage.Error);
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
            maxIter = maxIter%2 != 1 ? maxIter-1 : maxIter ;
            if (intervalType == IntervalTypes.InfBoth)
                ResultBind = "\u221E";
            else
                try
                {
                    switch (_selectedCalculationMethod)
                    {
                        case CalculationMethod.NewtonCortes:
                            SelectedFunction.EnableWeight = false;
                            //ResultBind = NumCore.NewtonikKotesik(integrateFrom, integrateTo, SelectedFunction, accuracy, maxIter, intervalType).ToString();
                            ResultBind = NumCore.NewNewtonCotes(integrateFrom, integrateTo, maxIter, SelectedFunction, intervalType).ToString();
                            break;
                        case CalculationMethod.Laguerre:
                            SelectedFunction.EnableWeight = false;
                            ResultBind = NumCore.LaguerreIntegration(SelectedFunction, lNodes).ToString();
                            break;
                        case CalculationMethod.Comparison:
                            SelectedFunction.EnableWeight = true;
                            //ResultBind = NumCore.NewtonikKotesik(integrateFrom, double.PositiveInfinity, SelectedFunction, accuracy, maxIter, intervalType).ToString();
                            ResultBind = NumCore.NewNewtonCotes(integrateFrom, double.PositiveInfinity, maxIter, SelectedFunction, IntervalTypes.BothClosed).ToString();
                            SelectedFunction.EnableWeight = false;
                            //SecondResult = NumCore.NewtonikKotesik(integrateFrom, double.PositiveInfinity, SelectedFunction, accuracy, maxIter, intervalType).ToString();
                            SecondResult = NumCore.LaguerreIntegration(SelectedFunction, lNodes).ToString();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message,"", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }));

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

        private string _integrationAccuracy = "0,01";

        public string IntegrationAccuracy
        {
            get { return _integrationAccuracy; }
            set
            {
                _integrationAccuracy = value.Replace(".", ",");
                RaisePropertyChanged(() => IntegrationAccuracy);
            }
        }

        private string _maxIter = "100";

        public string MaxIterBind
        {
            get { return _maxIter; }
            set
            {
                _maxIter = value.Replace(".", ",");
                RaisePropertyChanged(() => MaxIterBind);
            }
        }

        #region

        private Dictionary<string, string> _locale;

        public Dictionary<string, string> Locale
        {
            get { return _locale; }
            set
            {
                _locale = value;
                RaisePropertyChanged(() => Locale);
            }
        }

        private AvailableLocale _currentLocale;

        public AvailableLocale CurrentLocale
        {
            get { return _currentLocale; }
            set
            {
                _currentLocale = value;
                switch (value)
                {
                    case AvailableLocale.PL:
                        Locale = LocalizationManager.PlDictionary;
                        break;
                    case AvailableLocale.EN:
                        Locale = LocalizationManager.EnDictionary;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                //sets flag of next locale
                LangImgSourceBind = LocalizationManager.GetNextLocale(value).ToString();
            }
        }

        public MainViewModel()
        {
            CurrentLocale = AvailableLocale.EN;
        }

        private string _langImgSource;

        public string LangImgSourceBind
        {
            get { return _langImgSource; }
            set
            {
                _langImgSource = $@"../Localization/{value}.png";
                RaisePropertyChanged(() => LangImgSourceBind);
            }
        }

        private ICommand _changeLanguageCommand;

        public ICommand ChangeLanguageCommand =>
            _changeLanguageCommand ?? (_changeLanguageCommand = new RelayCommand(() =>
            {
                if ((int)CurrentLocale == Enum.GetNames(typeof(AvailableLocale)).Length - 1)
                    CurrentLocale = 0;
                else
                    CurrentLocale += 1;
                RaisePropertyChanged(() => SelectedCalculationMethod);
            }));

        #endregion
    }
}