using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using GalaSoft.MvvmLight;
using NumMethods4Lib.MathCore;

namespace NumMethods5.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<IFunction> AvailableFunctions { get; }
            = new ObservableCollection<IFunction>
            {
                    new Function1(),
                    new Function2(),
                    new Function3(),
                    //new Function4()
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

        private string _fromX = "-10";

        public string FromXBind
        {
            get { return _fromX;}
            set
            {
                _fromX = value;
                RaisePropertyChanged(() => FromXBind);
            }
        }

        private string _toX = "10";

        public string ToXBind
        {
            get { return _toX; }
            set
            {
                _toX = value;
                RaisePropertyChanged(() => ToXBind);
            }
        }

        private string _nodesCount = "5";

        public string NodesCountBind
        {
            get { return _nodesCount; }
            set
            {
                _nodesCount = value;
                RaisePropertyChanged(() => NodesCountBind);
            }
        }

        private string _maxIter = "100";

        public string MaxIterBind
        {
            get { return _maxIter; }
            set
            {
                _maxIter = value;
                RaisePropertyChanged(() => MaxIterBind);
            }
        }

        private string _precision;

        public string PrecisionBind
        {
            get { return _precision; }
            set
            {
                _precision = value;
                RaisePropertyChanged(() => PrecisionBind);
            }
        }

        public MainViewModel()
        {
            
        }
    }
}