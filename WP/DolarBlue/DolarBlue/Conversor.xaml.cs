using Microsoft.Phone.Controls;

namespace DolarBlue
{
    public partial class Conversor : PhoneApplicationPage
    {
        public Conversor()
        {
            InitializeComponent();

            DataContext = App.ViewModel;
        }
    }
}