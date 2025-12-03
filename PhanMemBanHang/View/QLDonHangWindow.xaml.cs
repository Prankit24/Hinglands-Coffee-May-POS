using System;
using System.Windows;
using System.Windows.Controls;
using PhanMemBanHang.Model;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class QLDonHangWindow : Window
    {
        private readonly QLDonHangVM _viewModel;

        public QLDonHangWindow()
        {
            InitializeComponent();
            _viewModel = new QLDonHangVM();
            DataContext = _viewModel;
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DgDonHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid grid && grid.SelectedItem is HoaDon hd)
            {
                _viewModel.ChonDonHang(hd);
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.LocDonHang();
        }

        private void ComboBox_TrangThai_Changed(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.LocDonHang();
        }

        private void ComboBox_PhuongThuc_Changed(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.LocDonHang();
        }

        private void ComboBox_LoaiDon_Changed(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.LocDonHang();
        }

        private void ComboBox_NhanVien_Changed(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.LocDonHang();
        }

        private void ComboBox_SapXep_Changed(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.ApDungSapXep();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _viewModel?.Dispose();
            base.OnClosing(e);
        }
    }
}