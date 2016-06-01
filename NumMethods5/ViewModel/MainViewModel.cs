﻿using System;
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

        private string _precision = "7";

        public string PrecisionBind
        {
            get { return _precision; }
            set
            {
                _precision = value.Replace(".", ",");
                RaisePropertyChanged(() => PrecisionBind);
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

        private string GetPolynom(int prec)
        {
            string result="";
            List<double> coefs = NumCore.NumCore.GetPolynomialCoeffs(prec).ToList();
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
        #endregion

        public ICommand CalculateCommand => new RelayCommand(DoMaths);

        private void DoMaths()
        {
            int nodesCount, prec;
            if (!int.TryParse(NodesCountBind, out nodesCount) || !int.TryParse(PrecisionBind, out prec)) 
                MessageBox.Show("Method arguments could not be parsed.", "Parsing error!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            else
                try
                {
                    AccuratePlot =
                        NumCore.NumCore.GetAccuratePlotDataPoints(SelectedFunction, DrawInterval).ToList();
                    double error;
                    var timer = new Stopwatch();
                    timer.Start();
                    ApproxPlot =
                        NumCore.NumCore.GetApproximatedPlotDataPoints(SelectedFunction, ApproxInterval, nodesCount,
                            new ApproximationByPolynomialLevel(prec), out error)
                            .Select(x => new DataPoint(x.X, x.Y))
                            .ToList();
                    timer.Stop();
                    ApproxTime = timer.ElapsedTicks.ToString();
                    Error = error.ToString();
                    Polynomial = GetPolynom(prec);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "could not be parsed.\n (._.) ( l: ) ( .-. ) ( :l ) (._.)", "Parsing error!", MessageBoxButton.OK,
                        MessageBoxImage.Error);
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
                        throw new ArgumentException("Approximation interval data ");
                    return new Interval
                    {
                     From = From,
                     To = To,   
                    };
                }
                catch (Exception)
                {
                    throw new ArgumentException("Approximation interval data ");
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
                        throw new ArgumentException("Drawing interval data ");
                    return new Interval
                    {
                        From = From,
                        To = To
                    };
                }
                catch (Exception)
                {
                    throw new ArgumentException("Drawing interval data ");
                }
            }
        }

        public MainViewModel()
        {
            CurrentLocale = AvailableLocale.EN;
        }
    }
}