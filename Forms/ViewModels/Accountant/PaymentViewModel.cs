using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Repositories.Repositories.Entities;
using Services.DTOs.PaymentDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Forms.ViewModels.Accountant
{
    public partial class PaymentViewModel : ObservableObject
    {
        private readonly IPaymentService _paymentService;

        [ObservableProperty]
        private ObservableCollection<ResponsePaymentDTO> totalInvoices = new();

        [ObservableProperty]
        private ObservableCollection<ResponsePaymentDTO> repairInvoices = new();

        [ObservableProperty]
        private ResponsePaymentDTO selectedInvoice;

        [ObservableProperty]
        private int selectedMonth;

        [ObservableProperty]
        private int selectedYear;

        [ObservableProperty]
        private string selectedStatus = "Tất cả";

        [ObservableProperty]
        private Apartment selectedApartment;

        [ObservableProperty]
        private ObservableCollection<Apartment> apartments = new();

        [ObservableProperty]
        private bool isLoading;

        public ObservableCollection<int> Months { get; } = new(Enumerable.Range(1, 12));
        public ObservableCollection<int> Years { get; } = new(Enumerable.Range(2020, 10));
        public ObservableCollection<string> PaymentStatuses { get; } = new() { "Tất cả", "Chưa thanh toán", "Thanh toán một phần", "Đã thanh toán" };

        public PaymentViewModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                selectedMonth = DateTime.Now.Month;
                selectedYear = DateTime.Now.Year;
                await LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadData()
        {
            IsLoading = true;
            try
            {
                await LoadApartments();
                await LoadInvoices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadApartments()
        {
            var apartmentList = await _paymentService.GetApartmentsAsync();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Apartments.Clear();
                foreach (var apartment in apartmentList)
                {
                    Apartments.Add(apartment);
                }
            });
        }

        // ... existing code ...

        private async Task LoadInvoices()
        {
            var invoices = await _paymentService.GetPayments(SelectedMonth, SelectedYear, SelectedStatus);
            Application.Current.Dispatcher.Invoke(() =>
            {
                TotalInvoices.Clear();
                RepairInvoices.Clear();

                foreach (var invoice in invoices)
                {
                    if (invoice.Type == "RepairInvoice")
                    {
                        RepairInvoices.Add(invoice);
                    }
                    else
                    {
                        TotalInvoices.Add(invoice);
                    }
                }
            });
        }

        [RelayCommand]
        private async Task SearchInvoices()
        {
            IsLoading = true;
            try
            {
                var searchText = SelectedApartment?.ApartmentCode ?? string.Empty;
                var invoices = await _paymentService.SearchPayments(searchText, SelectedMonth, SelectedYear, SelectedStatus);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    TotalInvoices.Clear();
                    RepairInvoices.Clear();

                    foreach (var invoice in invoices)
                    {
                        if (invoice.Type == "RepairInvoice")
                        {
                            RepairInvoices.Add(invoice);
                        }
                        else
                        {
                            TotalInvoices.Add(invoice);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ... existing code ...
        [RelayCommand]
        private async Task UpdatePayment(ResponsePaymentDTO invoice)
        {
            if (invoice == null) return;

            try
            {
                var inputDialog = new InputDialog("Nhập số tiền thanh toán:", "Thanh toán hóa đơn");
                if (inputDialog.ShowDialog() == true)
                {
                    if (decimal.TryParse(inputDialog.ResponseText, out decimal paidAmount))
                    {
                        if (paidAmount <= 0)
                        {
                            MessageBox.Show("Số tiền thanh toán phải lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        if (paidAmount > invoice.RemainingAmount)
                        {
                            MessageBox.Show("Số tiền thanh toán không được lớn hơn số tiền còn lại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        await _paymentService.UpdatePaymentStatus(invoice.InvoiceId, paidAmount);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var updatedInvoice = TotalInvoices.FirstOrDefault(i => i.InvoiceId == invoice.InvoiceId);
                            if (updatedInvoice != null)
                            {
                                updatedInvoice.PaidAmount += paidAmount;
                                updatedInvoice.RemainingAmount -= paidAmount;

                                if (updatedInvoice.RemainingAmount <= 0)
                                {
                                    updatedInvoice.Status = "Đã thanh toán";
                                }
                                else if (updatedInvoice.PaidAmount > 0)
                                {
                                    updatedInvoice.Status = "Thanh toán một phần";
                                }

                                var index = TotalInvoices.IndexOf(updatedInvoice);
                                TotalInvoices.RemoveAt(index);
                                TotalInvoices.Insert(index, updatedInvoice);
                            }
                        });

                        MessageBox.Show("Cập nhật thanh toán thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Số tiền không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task UpdateRepairPayment(ResponsePaymentDTO invoice)
        {
            if (invoice == null) return;

            try
            {
                var inputDialog = new InputDialog("Nhập số tiền thanh toán:", "Thanh toán hóa đơn sửa chữa");
                if (inputDialog.ShowDialog() == true)
                {
                    if (decimal.TryParse(inputDialog.ResponseText, out decimal paidAmount))
                    {
                        if (paidAmount <= 0)
                        {
                            MessageBox.Show("Số tiền thanh toán phải lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        if (paidAmount > invoice.RemainingAmount)
                        {
                            MessageBox.Show("Số tiền thanh toán không được lớn hơn số tiền còn lại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Update the database
                        await _paymentService.UpdatePaymentStatus(invoice.InvoiceId, paidAmount, "RepairInvoice");

                        // Update the UI
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            // Find and update the invoice in the collection
                            var updatedInvoice = RepairInvoices.FirstOrDefault(i => i.InvoiceId == invoice.InvoiceId);
                            if (updatedInvoice != null)
                            {
                                // Update the amounts
                                updatedInvoice.PaidAmount += paidAmount;
                                updatedInvoice.RemainingAmount -= paidAmount;

                                // Update the status
                                if (updatedInvoice.RemainingAmount <= 0)
                                {
                                    updatedInvoice.Status = "Đã thanh toán";
                                }
                                else if (updatedInvoice.PaidAmount > 0)
                                {
                                    updatedInvoice.Status = "Thanh toán một phần";
                                }

                                // Force UI refresh
                                var index = RepairInvoices.IndexOf(updatedInvoice);
                                RepairInvoices.RemoveAt(index);
                                RepairInvoices.Insert(index, updatedInvoice);
                            }
                        });

                        MessageBox.Show("Cập nhật thanh toán thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Số tiền không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task ViewInvoiceDetails(ResponsePaymentDTO invoice)
        {
            // TODO: Implement view invoice details
            MessageBox.Show("Chức năng đang được phát triển", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private async Task ViewRepairInvoiceDetails(ResponsePaymentDTO invoice)
        {
            // TODO: Implement view repair invoice details
            MessageBox.Show("Chức năng đang được phát triển", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                SelectedApartment = null;
                SelectedStatus = "Tất cả";
                await LoadData();
                MessageBox.Show("Làm mới dữ liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi làm mới dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        partial void OnSelectedMonthChanged(int value)
        {
            _ = LoadInvoices();
        }

        partial void OnSelectedYearChanged(int value)
        {
            _ = LoadInvoices();
        }

        partial void OnSelectedStatusChanged(string value)
        {
            _ = LoadInvoices();
        }

        partial void OnSelectedApartmentChanged(Apartment value)
        {
            _ = SearchInvoices();
        }
    }

    public class InputDialog : Window
    {
        private TextBox textBox;

        public string ResponseText
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public InputDialog(string question, string title)
        {
            Title = title;
            Width = 300;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.ToolWindow;

            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var questionText = new TextBlock { Text = question, Margin = new Thickness(0, 0, 0, 5) };
            grid.Children.Add(questionText);
            Grid.SetRow(questionText, 0);

            textBox = new TextBox { Margin = new Thickness(0, 0, 0, 15) };
            grid.Children.Add(textBox);
            Grid.SetRow(textBox, 1);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var okButton = new Button { Content = "OK", Width = 60, Margin = new Thickness(0, 0, 10, 0) };
            okButton.Click += (s, e) => { DialogResult = true; };
            var cancelButton = new Button { Content = "Hủy", Width = 60 };
            cancelButton.Click += (s, e) => { DialogResult = false; };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            grid.Children.Add(buttonPanel);
            Grid.SetRow(buttonPanel, 2);

            Content = grid;
        }
    }
}