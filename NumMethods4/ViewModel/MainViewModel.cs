using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NumMethods4.MathCore;

namespace NumMethods4.ViewModel
{
    
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<IFunction> AvailableFunctions { get; }
            = new ObservableCollection<IFunction>
            {
                    new Function1(),
                    new Function2(),
                    new Function3(),
                    new Function4(),
            };
        public MainViewModel()
        {
            
        
        }
    }
}