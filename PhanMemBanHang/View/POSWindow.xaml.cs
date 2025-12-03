using System.Windows;
using System.Windows.Controls;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class POSWindow : Window
    {
        public POSWindow()
        {
            InitializeComponent();
            DataContext = new POSVM();
        }

        private void RadioButton_SizeChanged(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag is SanPhamItemVM sp)
            {
                sp.SizeHienTai = rb.Content?.ToString();
            }
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            var ql = new AdminWindow();
            ql.Show();
            this.Close();
        }
    }
}
