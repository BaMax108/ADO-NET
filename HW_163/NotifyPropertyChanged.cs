using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HW_163
{
    public class NotifyPropertyChanged
    {
        public class INotifyPropertyChanged : System.ComponentModel.INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}