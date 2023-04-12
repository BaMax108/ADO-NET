using System.Windows;

namespace HW_163
{
    /// <summary>
    /// Логика взаимодействия для NewUser.xaml
    /// </summary>
    public partial class NewUser : Window
    {
        private SqlUser User;

        public NewUser(ref SqlUser user)
        {
            User = user;
            InitializeComponent();
            tbxSecondName.Text = user.SecondName;
            tbxFirstName.Text = user.FirstName;
            tbxLastName.Text = user.LastName;
            tbxPhoneNumber.Text = user.PhoneNumber;
            tbxEmail.Text = user.Email;

            if (user.Email != null) btnCreate.Content = "Обновить";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            User.LastName = tbxLastName.Text;
            User.FirstName = tbxFirstName.Text;
            User.SecondName = tbxSecondName.Text;
            User.PhoneNumber = tbxPhoneNumber.Text;
            User.Email = tbxEmail.Text;

            this.Close();
        }
    }
}
