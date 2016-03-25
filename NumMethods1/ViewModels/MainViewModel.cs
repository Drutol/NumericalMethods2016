using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using NumMethods1.NumCore;
using NumMethods1.Utils;

namespace NumMethods1.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        #region Fields

        private double _fromX;
        private double _toX;

        #endregion

        #region Properties

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
                if (!int.TryParse(value, out val) || val <= 1)
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
                _sliderValue = (int)value;
                RaisePropertyChanged(() => SliderValue);
            }
        }

        /// <summary>
        ///     Command bound to "Submit" button.
        /// </summary>
        private ICommand _submitDataCommand;

        public ICommand SubmitDataCommand =>
            _submitDataCommand ?? (_submitDataCommand = new RelayCommand(SubmitData));

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

        private ICommand _changeLanguageCommand;

        public ICommand ChangeLanguageCommand =>
            _changeLanguageCommand ?? (_changeLanguageCommand = new RelayCommand(() =>
            {
                if ((int)CurrentLocale == Enum.GetNames(typeof (AvailableLocale)).Length -1 )
                    CurrentLocale = 0;
                else
                    CurrentLocale += 1;
            }));

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
                LangImgSourceBind = value.ToString();
            }
        }

        #endregion

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            FunctionSelectorSelectedItem = AvailableFunctions[0];
            CurrentLocale = AvailableLocale.EN;
        }

        private void UpdateChart()
        {
            var precVal = (Math.Abs(_toX) + Math.Abs(_fromX))*_sliderValue/200;
            ChartData.Clear();
            ChartBiRootData.Clear();
            ChartFalsiRootData.Clear();
            for (double i = _fromX; i < _toX; i += precVal)
                ChartData.Add(new KeyValuePair<double, double>(i, FunctionSelectorSelectedItem.GetValue(i)));

            foreach (var root in RootsCollection.Where(root => root.SourceId == FunctionSelectorSelectedItem.Id))
            {
                if (root.Method_Used == "Bi")
                    ChartBiRootData.Add(new KeyValuePair<double, double>(root.X, root.Y));
                else
                    ChartFalsiRootData.Add(new KeyValuePair<double, double>(root.X, root.Y));
            }
        }

        private void SubmitData()
        {
            double from, to, approx;
            if (!double.TryParse(FromXValueBind, out from) || !double.TryParse(ToXValueBind, out to) || !double.TryParse(ApproxValueBind, out approx))
            {
                MessageBox.Show(ViewModelLocator.Main.Locale["#CannotParseException"], ViewModelLocator.Main.Locale["#RecommendDiffrentArgs"], MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (from >= to)
            {
                MessageBox.Show(ViewModelLocator.Main.Locale["#IntervalEndpointsException"], ViewModelLocator.Main.Locale["#RecommendDiffrentArgs"], MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _fromX = from;
            _toX = to;

            try
            {
                //Prepare argument.
                var arg = new GetFunctionRootArgs
                {
                    FromX = from, ToX = to, Approx = approx, MaxIterations = _maxIterations
                };
                //Add results to the list.
                RootsCollection.Add(MathCore.GetFunctionRootFalsi(FunctionSelectorSelectedItem, arg));
                RootsCollection.Add(MathCore.GetFunctionRootBi(FunctionSelectorSelectedItem, arg));
                //Update DataGrid's groups definitions.
                RootsView = new ListCollectionView(RootsCollection);
            }
            catch (ArgumentException)
            {
                MessageBox.Show(ViewModelLocator.Main.Locale["#EvenOrNoRootsException"],ViewModelLocator.Main.Locale["#RecommendDiffrentArgs"],MessageBoxButton.OK,MessageBoxImage.Error);
            }

            //Once we are done we can render the chart.
            UpdateChart();
        }
    }
}