using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NumMethods3.MathCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using OxyPlot;
using NumMethods3.NumCore;
using NumMethods3.Utils;

namespace NumMethods3.ViewModel
{
    public class RawInputSelection : ISelectable
    {
        public string TextRepresentation => "Input values manually.";
    }

    public class InterpolationDataPack
    {
        public List<FunctionValue> Nodes { get; set; }
        public List<FunctionValue> Interpolated { get; set; }
        public List<FunctionValue> InterpolationResults { get; set; }
        public List<FunctionValue> FunctionValues { get; }
        public int InterpolationNodesCount { get; }
        public double InterpolateFromX { get; }
        public double InterpolateToX { get; }
        public double DrawFromX { get; set; }
        public double DrawToX { get; set; }
        public IFunction SelectedFunction { get; }

        public InterpolationDataPack(double interpolateFromXVal, ObservableCollection<FunctionValue> funcVals, 
            int interpolationNodesCount, double interpolateToXVal, double drawFromX, double drawToX, IFunction selFunc)
        {
            InterpolateFromX = interpolateFromXVal;
            FunctionValues = funcVals.ToList();
            InterpolationNodesCount = interpolationNodesCount;
            InterpolateToX = interpolateToXVal;
            SelectedFunction = selFunc;
            DrawFromX =drawFromX;
            DrawToX =drawToX;
        }
    }

    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<ISelectable> AvailableFunctions { get; }
            = new ObservableCollection<ISelectable>
            {
                    new Function1(),
                    new Function2(),
                    new Function3(),
                    new Function4(),
                    new RawInputSelection()
            };

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
            }));

        /// <summary>
        ///     Curently selected function by user.
        /// </summary>
        private IFunction SelectedFunction { get; set; } = new Function1();

        private int _functionSelectorSelectedIndex;

        public int FunctionSelectorSelectedIndex
        {
            get { return _functionSelectorSelectedIndex; }
            set
            {
                _functionSelectorSelectedIndex = value;
                SelectedFunction = AvailableFunctions[value] as IFunction;
                FunctionModeVisibility = value < AvailableFunctions.Count - 1 ? Visibility.Visible : Visibility.Collapsed;
                
            }
        }

        private ObservableCollection<FunctionValue> _functionValues = new ObservableCollection<FunctionValue>
        {
            new FunctionValue {X = -10,Y= 10},
            new FunctionValue {X = -5,Y= 12},
            new FunctionValue {X = -7,Y= 45},
            new FunctionValue {X = -1,Y= 2},
            new FunctionValue {X = 10,Y= 15},
        };
        public ObservableCollection<FunctionValue> FunctionValues
        {
            get { return _functionValues; }
            set
            {
                _functionValues = value;
                RaisePropertyChanged(() => FunctionValues);
            }
        }

        private ICommand _doMathsCommand;

        public ICommand DoMathsCommand =>
            _doMathsCommand ?? (_doMathsCommand = new RelayCommand(() =>
            {
                double interFrom, interTo, drawFrom, drawTo;
                int nCount;
                if (!double.TryParse(InterpolateFromXValueBind, out interFrom) || !double.TryParse(InterpolateToXValueBind, out interTo) || 
                !double.TryParse(DrawFromXValueBind, out drawFrom) || !double.TryParse(DrawToXValueBind, out drawTo) ||
                !int.TryParse(InterpolationNodesCount, out nCount))
                {
                    MessageBox.Show(Locale["#CannotParseMsg"], Locale["#CannotParseTitle"], MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                var data = new InterpolationDataPack(interFrom, _functionValues, nCount, interTo, drawFrom, drawTo, SelectedFunction);
                bool from=false, to=false;
                if (data.DrawFromX > data.InterpolateFromX)
                {
                    data.DrawFromX = data.InterpolateFromX;
                    from = true;
                }
                if (data.DrawToX < data.InterpolateToX)
                {
                    data.DrawToX = data.InterpolateToX;
                    to = true;
                }
                if (to || from)
                    MessageBox.Show(Locale["#DrawingIntervalChangeMsg"], Locale["#DrawingIntervalChangeTitle"], MessageBoxButton.OK, MessageBoxImage.Information);

                try
                {
                    data = MathCore.NumCore.GetInterpolatedFunctionData(data);
                }
                catch (Exception e)
                {
                    if(e is ArgumentException)
                        MessageBox.Show(Locale["#NodesAmmountExceptionMsg"], Locale["#NodesAmmountExceptionTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show(Locale["#UnexpectedExceptionMsg"], Locale["#UnexpectedExceptionTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    NodeChartData = data.Nodes.Take(data.Nodes.Count).Select(value => value.ToDataPoint).ToList();
                    ChartDataInterpolated = data.Interpolated.Select(value => value.ToDataPoint).ToList();
                    ChartDataInterpolation = data.InterpolationResults.Select(value => value.ToDataPoint).ToList();
                    RaisePropertyChanged(() => NodeChartData);
                    RaisePropertyChanged(() => ChartDataInterpolated);
                    RaisePropertyChanged(() => ChartDataInterpolation);
                }
             }));

        public MainViewModel()
        {
            CurrentLocale = AvailableLocale.EN;
        }

        private string _drawFromXValue = "-10";

        public string DrawFromXValueBind
        {
            get { return _drawFromXValue; }
            set
            {
                _drawFromXValue = value.Replace('.', ',');
                RaisePropertyChanged(() => DrawFromXValueBind);
            }
        }

        private string _drawToXValue = "10";

        public string DrawToXValueBind
        {
            get { return _drawToXValue; }
            set
            {
                _drawToXValue = value.Replace('.', ',');
                RaisePropertyChanged(() => DrawToXValueBind);
            }
        }

        private string _interpolateFromXValue = "-10";

        public string InterpolateFromXValueBind
        {
            get { return _interpolateFromXValue; }
            set
            {
                _interpolateFromXValue = value.Replace('.', ',');
                RaisePropertyChanged(() => InterpolateFromXValueBind);
            }
        }

        private string _interpolateToXValue = "10";

        public string InterpolateToXValueBind
        {
            get { return _interpolateToXValue; }
            set
            {
                _interpolateToXValue = value.Replace('.', ',');
                RaisePropertyChanged(() => InterpolateToXValueBind);
            }
        }

        private string _interpolationNodesCount = "10";

        public string InterpolationNodesCount
        {
            get { return _interpolationNodesCount; }
            set
            {
                _interpolationNodesCount = value;
                RaisePropertyChanged(() => InterpolationNodesCount);
            }
        }

        private Visibility _functionModeVisibility;

        public Visibility FunctionModeVisibility
        {
            get { return _functionModeVisibility; }
            set
            {
                _functionModeVisibility = value;
                RaisePropertyChanged(() => FunctionModeVisibility);
            }
        }
    
        public List<DataPoint> ChartDataInterpolation { get; set; } =
            new List<DataPoint>();

        public List<DataPoint> ChartDataInterpolated { get; set; } =
            new List<DataPoint>();

        public List<DataPoint> NodeChartData { get; set; } =
            new List<DataPoint>();
    }
}