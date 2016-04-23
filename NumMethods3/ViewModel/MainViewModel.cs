using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NumMethods1.NumCore;
using NumMethods3.MathCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NumMethods3.ViewModel
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
            }
        }

        private ObservableCollection<FunctionValue> _functionValues = new ObservableCollection<FunctionValue>();
        public ObservableCollection<FunctionValue> FunctionValues
        {
            get { return _functionValues; }
            set
            {
                _functionValues = value;
                RaisePropertyChanged(() => FunctionValues);
            }
        }

        private ICommand _extendRowCommand;

        public ICommand ExtendRowCommand =>
            _extendRowCommand ?? (_extendRowCommand = new RelayCommand(() => FunctionValues.Add(new FunctionValue())));

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

        private string _toXValue = "";

        public string ToXValueBind
        {
            get { return _toXValue; }
            set
            {
                _toXValue = value.Replace('.', ',');
                RaisePropertyChanged(() => ToXValueBind);
            }
        }

        public ObservableCollection<KeyValuePair<double, double>> ChartData { get; } =
    new ObservableCollection<KeyValuePair<double, double>>();


        public MainViewModel()
        {

        }
    }
}