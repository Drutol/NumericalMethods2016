using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NumMethods4Lib.MathCore;
using NumMethods5.NumCore;
using NumMethods5.Utils;
using OxyPlot;

namespace NumMethods5.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private void DoMaths()
        {
            PolynomialList.Clear();
            double eps = 0;
            if (true)
            {
                int nodesCount,degree;
                if (!int.TryParse(NodesCountBind, out nodesCount) || !int.TryParse(PolynomialDegreeBind, out degree) ||
                    (!ApproxByExplicitDegree && !double.TryParse(ErrorMarginBind, out eps)))
                    MessageBox.Show(Locale["#MethArgParsing"], Locale["#ParsingErr"], MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                    try
                    {
                        AccuratePlot =
                            NumCore.NumCore.GetAccuratePlotDataPoints(SelectedFunction, DrawInterval).ToList();
                        Polynomial approx;
                        var timer = new Stopwatch();
                        timer.Start();
                        ApproximationCriterium criterium;
                        if (ApproxByExplicitDegree)
                            criterium = new ApproximationByPolynomialLevel(degree, UseCotes);
                        else
                            criterium = new ApproximationByAccuracy(eps, UseCotes);
                        var points =
                            NumCore.NumCore.GetApproximatedPlotDataPoints(SelectedFunction, ApproxInterval, nodesCount,
                                criterium, out approx);
                        timer.Stop();
                        ApproxPlot = points.Select(x => new DataPoint(x.X, x.Y)).ToList();
                        ApproxTime = timer.ElapsedTicks.ToString();
                        Error = NumCore.NumCore.GetError(SelectedFunction, approx).ToString();
                        Polynomial = GetPolynom(approx);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message + Locale["#BadIntervalCont"], Locale["#ParsingErr"],
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
            }
        }

        #region Just locale things.

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
                if ((int) CurrentLocale == Enum.GetNames(typeof(AvailableLocale)).Length - 1)
                    CurrentLocale = 0;
                else
                    CurrentLocale += 1;
            }));

        #endregion

        #region View properties.

        private List<DataPoint> _accuratePlot = new List<DataPoint>();

        public List<DataPoint> AccuratePlot
        {
            get { return _accuratePlot; }
            set
            {
                _accuratePlot = value;
                RaisePropertyChanged(() => AccuratePlot);
            }
        }

        private List<DataPoint> _approxPlot = new List<DataPoint>();

        public List<DataPoint> ApproxPlot
        {
            get { return _approxPlot; }
            set
            {
                _approxPlot = value;
                RaisePropertyChanged(() => ApproxPlot);
            }
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

        private int _functionSelectorSelectedIndex;

        public int FunctionSelectorSelectedIndex
        {
            get { return _functionSelectorSelectedIndex; }
            set
            {
                _functionSelectorSelectedIndex = value;
                SelectedFunction = AvailableFunctions[value];
            }
        }

        private string _approximateFromX = "0";

        public string ApproximateFromXBind
        {
            get { return _approximateFromX; }
            set
            {
                _approximateFromX = value.Replace(".", ",");
                RaisePropertyChanged(() => ApproximateFromXBind);
            }
        }

        private string _drawFromX = "0";

        public string DrawFromXBind
        {
            get { return _drawFromX; }
            set
            {
                _drawFromX = value.Replace(".", ",");
                RaisePropertyChanged(() => DrawFromXBind);
            }
        }

        private string _approximateToX = "10";

        public string ApproximateToXBind
        {
            get { return _approximateToX; }
            set
            {
                _approximateToX = value.Replace(".", ",");
                RaisePropertyChanged(() => ApproximateToXBind);
            }
        }

        private string _drawToX = "10";

        public string DrawToXBind
        {
            get { return _drawToX; }
            set
            {
                _drawToX = value.Replace(".", ",");
                RaisePropertyChanged(() => DrawToXBind);
            }
        }

        private string _nodesCount = "10";

        public string NodesCountBind
        {
            get { return _nodesCount; }
            set
            {
                _nodesCount = value;
                RaisePropertyChanged(() => NodesCountBind);
            }
        }

        private string _polynomialDegree = "7";

        public string PolynomialDegreeBind
        {
            get { return _polynomialDegree; }
            set
            {
                _polynomialDegree = value;
                RaisePropertyChanged(() => PolynomialDegreeBind);
            }
        }

        private string _errorMargin = "1";

        public string ErrorMarginBind
        {
            get { return _errorMargin; }
            set
            {
                _errorMargin = value.Replace(".", ",");
                RaisePropertyChanged(() => ErrorMarginBind);
            }
        }

        private string _approxTime = "--";

        public string ApproxTime
        {
            get { return _approxTime; }
            set
            {
                _approxTime = value;
                RaisePropertyChanged(() => ApproxTime);
            }
        }

        private string _error = "--";

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                RaisePropertyChanged(() => Error);
            }
        }

        private string _polynomial = "--";

        public string Polynomial
        {
            get { return _polynomial; }
            set
            {
                _polynomial = value;
                RaisePropertyChanged(() => Polynomial);
            }
        }

        private Interval ApproxInterval
        {
            get
            {
                try
                {
                    var From = double.Parse(ApproximateFromXBind);
                    var To = double.Parse(ApproximateToXBind);
                    if (From >= To)
                        throw new Exception();
                    return new Interval
                    {
                        From = From,
                        To = To
                    };
                }
                catch (Exception)
                {
                    throw new ArgumentException(Locale["#BadApproxInterval"]);
                }
            }
        }

        private Interval DrawInterval
        {
            get
            {
                try
                {
                    var From = double.Parse(DrawFromXBind);
                    var To = double.Parse(DrawToXBind);
                    if (From >= To)
                        throw new Exception();
                    return new Interval
                    {
                        From = From,
                        To = To
                    };
                }
                catch (Exception)
                {
                    throw new ArgumentException(Locale["#BadDrawingInterval"]);
                }
            }
        }

        public bool ApproxByExplicitDegree { get; set; }

        public bool UseCotes { get; set; }

        public MainViewModel()
        {
            CurrentLocale = AvailableLocale.EN;
        }

        #endregion

        #region The fancyness overload. Aka. UTF-8 chars

        private List<string> PolynomCoefs { get; } = new List<string>
        {
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2070",
            /* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u00B9",
            /* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u00B2",
            /* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u00B3",
            /* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2074",
            /* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2075",
            /* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2076",
            /* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2077",
            /* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2078",
            /* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2079" /* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
        };

        private string GetPolynom(Polynomial approx)
        {
            var result = "";
            var coefs = approx.Coefficients;
            var prec = coefs.Count - 1;
            var pr = prec;
            foreach (var coef in coefs)
            {
                var key = "x" + (pr/10 > 0 ? PolynomCoefs[pr/10] + PolynomCoefs[pr%10] : PolynomCoefs[pr]);
                PolynomialList.Add(new KeyValuePair<string, double>(key, coef));
                pr--;
            }

            for (var i = prec/10; i >= 0; i--)
            {
                for (var j = prec%10; j >= 0; j--)
                {
                    var coef = coefs[10*i + j];
                    if (i == 0 && j == 0)
                        return result + (coef > 0 ? $"+{coef:N2}" : $"-{Math.Abs(coef):N2}");
                    result += coef > 0 ? $"+{coef:N2}x" : $"-{Math.Abs(coef):N2}x";
                    if (i > 0)
                    {
                        for (var k = 1; k <= i; k++)
                        {
                            result += PolynomCoefs[k];
                            prec--;
                        }
                    }
                    result += PolynomCoefs[j];
                }
            }
            return result;
        }

        public ObservableCollection<KeyValuePair<string, double>> PolynomialList { get; set; } =
            new ObservableCollection<KeyValuePair<string, double>>();

        public ICommand CopyToClipboardCommand => new RelayCommand(CopyToClipboard);

        private void CopyToClipboard()
        {
            Clipboard.SetText(string.Join("+", PolynomialList.Select(x => string.Concat(x.Key + "*", x.Value))));
        }

        public ICommand CalculateCommand => new RelayCommand(DoMaths);

        #endregion
    }
}