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
                if ((int)CurrentLocale == Enum.GetNames(typeof(AvailableLocale)).Length - 1)
                    CurrentLocale = 0;
                else
                    CurrentLocale += 1;
            }));

        #endregion

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

        #region View properties.

        private string _approximateFromX = "-10";

        public string ApproximateFromXBind
        {
            get { return _approximateFromX;}
            set
            {
                _approximateFromX = value.Replace(".",",");
                RaisePropertyChanged(() => ApproximateFromXBind);
            }
        }

        private string _drawFromX = "-20";

        public string DrawFromXBind
        {
            get { return _drawFromX; }
            set
            {
                _drawFromX = value.Replace(".",",");
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

        private string _drawToX = "20";

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

        //private string _maxIter = "100";

        //public string MaxIterBind
        //{
        //    get { return _maxIter; }
        //    set
        //    {
        //        _maxIter = value;
        //        RaisePropertyChanged(() => MaxIterBind);
        //    }
        //}

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

#endregion

        private List<DataPoint> _accuratePlot = new List<DataPoint>();

        public List<DataPoint> AccuratePlot
        {
            get{ return _accuratePlot; }
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

        #region The fancyness overload.
        private List<string> PolynomCoefs { get; } = new List<string>
        {
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2070",/* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u00B9",/* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u00B2",/* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u00B3",/* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2074",/* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2075",/* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2076",/* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2077",/* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2078",/* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
            /*(ﾉ◕ヮ◕)ﾉ*:･ﾟ✧ */"\u2079",/* ✧ﾟ･: *ヽ(◕ヮ◕ヽ)*/
        };

        private string GetPolynom(Polynomial approx,int prec)
        {
            string result="";
            List<double> coefs = approx.Coefficients;
            var pr = prec;
            foreach (var coef in coefs)
            {
                string key = "x" + (pr/10 > 0 ? PolynomCoefs[pr/10]+PolynomCoefs[pr%10] : PolynomCoefs[pr]);
                PolynomialList.Add(new KeyValuePair<string, double>(key,coef));
                pr--;
            }

            for ( int i=prec/10;i>=0;i--)
            {
                for (int j = prec%10; j >=0; j--)
                {
                    var coef = coefs[10*i + j];
                    if (i == 0 && j == 0)
                        return result += coef > 0 ? $"+{coef:N2}" : $"-{Math.Abs(coef):N2}";
                    result += coef > 0 ? $"+{coef:N2}x" : $"-{Math.Abs(coef):N2}x";
                    if (i > 0)
                    {
                        for (int k = 1; k <= i; k++)
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
        public ObservableCollection<KeyValuePair<string, double>> PolynomialList { get; set; } = new ObservableCollection<KeyValuePair<string, double>>();

        #endregion

        public ICommand CopyToClipboardCommand => new RelayCommand(CopyToClipboard);

        private void CopyToClipboard()
        {
            
        }

        public ICommand CalculateCommand => new RelayCommand(DoMaths);

        private void DoMaths()
        {
            PolynomialList.Clear();
            int nodesCount, prec;
            if (PrecModeBind)
            {
                if (!int.TryParse(NodesCountBind, out nodesCount) || !int.TryParse(PolynomialDegreeBind, out prec))
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
                        ApproxPlot =
                            NumCore.NumCore.GetApproximatedPlotDataPoints(SelectedFunction, ApproxInterval, nodesCount,
                                new ApproximationByPolynomialLevel(prec, UseCotes), out approx)
                                .Select(x => new DataPoint(x.X, x.Y))
                                .ToList();
                        timer.Stop();
                        ApproxTime = timer.ElapsedTicks.ToString();
                        Error = NumCore.NumCore.GetError(SelectedFunction, approx).ToString();
                        Polynomial = GetPolynom(approx, prec);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message + Locale["#BadIntervalCont"], Locale["#ParsingErr"],
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
            }
            else
            {
                double eps = 0;
                if (!int.TryParse(NodesCountBind, out nodesCount) || !double.TryParse(ErrorMarginBind, out eps))
                    MessageBox.Show(Locale["#MethArgParsing"], Locale["#ParsingErr"], MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                    try
                    {
                        prec = 0;
                        AccuratePlot =
                            NumCore.NumCore.GetAccuratePlotDataPoints(SelectedFunction, DrawInterval).ToList();
                        Polynomial approx;
                        double err;
                        var timer = new Stopwatch();
                        timer.Start();
                        do
                        {
                            ApproxPlot =
                                NumCore.NumCore.GetApproximatedPlotDataPoints(SelectedFunction, ApproxInterval,
                                    nodesCount,
                                    new ApproximationByPolynomialLevel(prec++, UseCotes), out approx)
                                    .Select(x => new DataPoint(x.X, x.Y))
                                    .ToList();
                            err = NumCore.NumCore.GetError(SelectedFunction, approx);
                            if (double.IsNaN(err))
                                MessageBox.Show(
                                    $"Couldn't get that little error and received NaN value at {prec} degree Polynomial.",
                                    "Too little error margin!", MessageBoxButton.OK, MessageBoxImage.Error);
                        } while (err-Math.Abs(eps)>0);
                        timer.Stop();
                        ApproxTime = timer.ElapsedTicks.ToString();
                        Error = err.ToString();
                        Polynomial = GetPolynom(approx, prec-1);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message + Locale["#BadIntervalCont"], Locale["#ParsingErr"], MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
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
                     To = To,   
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

        public bool PrecModeBind { get; set; }

        public bool UseCotes { get; set; }

        public MainViewModel()
        {
            CurrentLocale = AvailableLocale.EN;
        }
    }
}