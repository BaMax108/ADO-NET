using static HW_163.NotifyPropertyChanged;

namespace HW_163
{
    public class SqlData : INotifyPropertyChanged
    {
        public int ID { get; set; }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private short _productCode;
        public short ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged();
            }
        }

        private string _productName;
        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged();
            }
        }
    }
}
