using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NumMethods1.Exceptions;
using NumMethods1.NumCore;
using NumMethods1.Utils;

namespace NumMethods1.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            FunctionSelectorSelectedItem = AvailableFunctions[0];
            CurrentLocale = AvailableLocale.EN; //Setting default language.
        }

        private async void SubmitData()
        {
            double from, to, approx, divRate = 1;
            if (!double.TryParse(FromXValueBind, out from) || !double.TryParse(ToXValueBind, out to) ||
                !double.TryParse(ApproxValueBind, out approx))
            {
                MessageBox.Show(Locale["#ValuesParseException"], Locale["#RecommendDiffrentArgs"], MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            if (from >= to)
            {
                MessageBox.Show(Locale["#IntervalEndpointsException"], Locale["#RecommendDiffrentArgs"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (IsIntervalDivisionEnabled && !double.TryParse(DivisionRateBind, out divRate))
            {
                MessageBox.Show(Locale["#IntervalDivParseException"], Locale["#RecommendDiffrentArgs"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ProgressBarVisibility = (int)divRate != 1 ? Visibility.Visible : Visibility.Collapsed;
            _fromX = from;
            _toX = to;

            //Add results to the list.
            int noRootsCounter = 0, maxIterCounter = 0, divisionsSuccesses = 0;
            var intervalStep = Math.Abs(from - to) / divRate;
            var roots = await Task.Run(() =>
            {
                List<FunctionRoot> output = new List<FunctionRoot>();
                for (var i = from; i < to; i += intervalStep)
                {
                    //Prepare argument.
                    var arg = new GetFunctionRootArgs
                    {
                        FromX = i,
                        ToX = i + intervalStep,
                        Approx = approx,
                        MaxIterations = _maxIterations
                    };
                    try
                    {
                        output.Add(MathCore.GetFunctionRootBi(FunctionSelectorSelectedItem, arg));
                    }
                    catch (Exception e)
                    {
                        //if this function return true , it means that values on the interval's borders are of the same sign
                        if (CatchFunction(e, ref maxIterCounter, ref noRootsCounter,"Bi"))
                            continue;
                    }
                    try
                    {
                        output.Add(MathCore.GetFunctionRootFalsi(FunctionSelectorSelectedItem, arg));
                    }
                    catch (Exception e)
                    {
                        CatchFunction(e, ref maxIterCounter, ref noRootsCounter, "Falsi");
                    }

                }
                return output;
            });
            foreach (var functionRoot in roots)
                RootsCollection.Add(functionRoot);
            
            

            if (maxIterCounter > 0 || noRootsCounter > 0)
            {
                MessageBox.Show(Locale["#DividedIntervalRaport"]
                    .Replace("&s1;", divisionsSuccesses.ToString())
                    .Replace("&s2;", maxIterCounter.ToString())
                    .Replace("&s3;", noRootsCounter.ToString()),
                    Locale["#DividedIntervalRaport1"], MessageBoxButton.OK, MessageBoxImage.Information);
            }


            //Update DataGrid's groups definitions.
            RootsView = new ListCollectionView(RootsCollection);

            //Once we are done we can render the chart.
            UpdateChart();
        }

        private bool CatchFunction(Exception e,ref int counterIter,ref int counterBoundary,string method)
        {
            if (e is BoundaryFunctionValuesOfTheSameSignException)
            {
                if (!IsIntervalDivisionEnabled)
                    MessageBox.Show($"{Locale["#EvenOrNoRootsException1"]}{(e as BoundaryFunctionValuesOfTheSameSignException).LeftValue}\n" +
                                    $"{Locale["#EvenOrNoRootsException2"]}{(e as BoundaryFunctionValuesOfTheSameSignException).RightValue}",
                        Locale["#RecommendDiffrentArgs"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                    counterBoundary++;

                return true;
            }
            if(e is MaxIterationsReachedException)
            {
                if (!IsIntervalDivisionEnabled)
                    MessageBox.Show(Locale["#TooManyIterationsException"], $"{Locale["#RecommendDiffrentArgs"]} - {method}",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                    counterIter++;
            }
            return false;
        }


        private void UpdateChart()
        {
            var precVal = (Math.Abs(_toX) + Math.Abs(_fromX))*_sliderValue/200;
            ChartData.Clear();
            ChartBiRootData.Clear();
            ChartFalsiRootData.Clear();
            for (var i = _fromX; i < _toX; i += precVal)
                ChartData.Add(new KeyValuePair<double, double>(i, FunctionSelectorSelectedItem.GetValue(i)));

            int step = RootsCollection.Count > 100 ? Convert.ToInt32(_divisionRate) / 10 : 1;

            IntervalDivNoteBind = step == 1 ? "" : Locale["#IntervalDivNote"].Replace("&arg;",step.ToString());

            RaisePropertyChanged(() => IntervalDivNoteBind);
            foreach (
                var root in
                    RootsCollection.Where(
                        (root,i) =>
                           (i % step == 0) && (root.SourceId == FunctionSelectorSelectedItem.Id) && (root.X > _fromX) && (root.X < _toX)))
            {
                if (root.Method_Used == "Bi")
                    ChartBiRootData.Add(new KeyValuePair<double, double>(root.X, root.Y));
                else
                    ChartFalsiRootData.Add(new KeyValuePair<double, double>(root.X, root.Y));
            }

            ProgressBarVisibility = Visibility.Collapsed;

        }

        #region Fields

        private double _fromX;
        private double _toX;

        #endregion

        #region Properties

        /// <summary>
        ///     Stores pre-defined phrases.
        /// </summary>
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

        /// <summary>
        ///     Selection of functions in combobox.
        /// </summary>
        public ObservableCollection<IFunction> AvailableFunctions { get; }
            = new ObservableCollection<IFunction>
            {
                new Function1(),
                new Function2(),
                new Function3(),
                new Function4()
            };

        /// <summary>
        ///     List of roots to be displayed by DataGrid.
        /// </summary>
        private ListCollectionView _rootsView;

        public ListCollectionView RootsView
        {
            get { return _rootsView; }
            set
            {
                _rootsView = value;
                _rootsView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
                RaisePropertyChanged(() => RootsView);
            }
        }

        private ObservableCollection<FunctionRoot> RootsCollection { get; } =
            new ObservableCollection<FunctionRoot>();

        /// <summary>
        ///     Data used by WPFToolkit's line series chart.
        /// </summary>
        public ObservableCollection<KeyValuePair<double, double>> ChartData { get; } =
            new ObservableCollection<KeyValuePair<double, double>>();

        public ObservableCollection<KeyValuePair<double, double>> ChartBiRootData { get; } =
            new ObservableCollection<KeyValuePair<double, double>>();

        public ObservableCollection<KeyValuePair<double, double>> ChartFalsiRootData { get; } =
            new ObservableCollection<KeyValuePair<double, double>>();

        /// <summary>
        ///     Curently selected function by user.
        /// </summary>
        private IFunction _functionSelectorSelectedItem;

        public IFunction FunctionSelectorSelectedItem
        {
            get { return _functionSelectorSelectedItem; }
            set
            {
                _functionSelectorSelectedItem = value;
                UpdateChart();
            }
        }

        /// <summary>
        ///     Phrase which indicates which flag image to use.
        /// </summary>
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

        private string _intervalDivNote = "";

        public string IntervalDivNoteBind { get; set; }
        //{
        //    get { return _intervalDivNote; }
        //    set
        //    {
        //        int rate = Convert.ToInt32(_divisionRate);
        //        _intervalDivNote = "sztec" + ;
        //        RaisePropertyChanged(() => IntervalDivNoteBind);
        //    }
        //}
        
        /// <summary>
        ///     Value which is directly bound to corresponding textbox.
        /// </summary>
        private string _fromXValue = "-10";

        public string FromXValueBind
        {
            get { return _fromXValue; }
            set
            {
                _fromXValue = value.Replace('.', ',');
                RaisePropertyChanged(() => FromXValueBind);
            }
        }

        /// <summary>
        ///     Value which is directly bound to corresponding textbox.
        /// </summary>
        private string _toXValue = "10";

        public string ToXValueBind
        {
            get { return _toXValue; }
            set
            {
                _toXValue = value.Replace('.', ',');
                RaisePropertyChanged(() => ToXValueBind);
            }
        }

        /// <summary>
        ///     Value which is bound to corresponding textbox.
        /// </summary>
        private int _maxIterations = 100;

        public string MaxIterations
        {
            get { return _maxIterations.ToString(); }
            set
            {
                int val;
                if (!int.TryParse(value, out val) || val < 1)
                    val = 100;
                _maxIterations = val;
                RaisePropertyChanged(() => MaxIterations);
            }
        }

        /// <summary>
        ///     Value which is directly bound to corresponding textbox.
        /// </summary>
        private string _approxValue = "0,5";

        public string ApproxValueBind
        {
            get { return _approxValue; }
            set
            {
                _approxValue = value.Replace('.', ',');
                RaisePropertyChanged(() => ApproxValueBind);
            }
        }

        /// <summary>
        ///     Current representation of slider's current value.
        /// </summary>
        private int _sliderValue = 3;

        public double SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = (int) value;
                RaisePropertyChanged(() => SliderValue);
            }
        }

        /// <summary>
        ///     Indicates if the base interval sould be divided.
        /// </summary>
        private bool _isIntervalDivisionEnabled;

        public bool IsIntervalDivisionEnabled
        {
            get { return _isIntervalDivisionEnabled; }
            set
            {
                _isIntervalDivisionEnabled = value;
                RaisePropertyChanged(() => IsIntervalDivisionEnabled);
            }
        }

        /// <summary>
        ///     Current ammount subintervals to be created.
        /// </summary>
        private string _divisionRate = "";

        public string DivisionRateBind
        {
            get { return _divisionRate; }
            set
            {
                _divisionRate = value.Replace('.', ',');
                RaisePropertyChanged(() => DivisionRateBind);
            }
        }

        /// <summary>
        ///     Command bound to "Submit" button.
        /// </summary>
        private ICommand _submitDataCommand;

        public ICommand SubmitDataCommand =>
            _submitDataCommand ?? (_submitDataCommand = new RelayCommand(SubmitData));

        /// <summary>
        ///     Command bound to inner "clear results" button.
        /// </summary>
        private ICommand _clearParticularResultsCommand;

        public ICommand ClearParticularResultsCommand =>
            _clearParticularResultsCommand ?? (_clearParticularResultsCommand = new RelayCommand<string>(s =>
            {
                var data = RootsCollection.Where(root => root.Group != s).ToList();
                RootsCollection.Clear();
                ChartBiRootData.Clear();
                ChartFalsiRootData.Clear();
                foreach (var functionRoot in data)
                    RootsCollection.Add(functionRoot);

                RootsView = new ListCollectionView(RootsCollection);

                //Clearing the chart when there is no more data diplayed.
                if (data.Count == 0)
                    ChartData.Clear();
            }));

        /// <summary>
        ///     Command bound to main "clear results" button.
        /// </summary>
        private ICommand _clearResultsCommand;

        public ICommand ClearResultsCommand =>
            _clearResultsCommand ?? (_clearResultsCommand = new RelayCommand(() =>
            {
                RootsCollection.Clear();
                RootsView = new ListCollectionView(RootsCollection);
                ChartData.Clear();
                ChartBiRootData.Clear();
                ChartFalsiRootData.Clear();
            }));

        /// <summary>
        ///     Command bound to "flag" button.
        /// </summary>
        private ICommand _changeLanguageCommand;

        public ICommand ChangeLanguageCommand =>
            _changeLanguageCommand ?? (_changeLanguageCommand = new RelayCommand(() =>
            {
                if ((int) CurrentLocale == Enum.GetNames(typeof (AvailableLocale)).Length - 1)
                    CurrentLocale = 0;
                else
                    CurrentLocale += 1;
            }));

        /// <summary>
        ///     Indicates which language should be used.
        /// </summary>
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

        private Visibility _progressBarVisibility = Visibility.Collapsed;

        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged(() => ProgressBarVisibility);
            }
        }

        #endregion
    }
}