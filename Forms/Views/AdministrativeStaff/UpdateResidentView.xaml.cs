using Core;
using Repositories.Repositories.Entities;
using Services.DTOs.ResidentDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for UpdateResidentView.xaml
    /// </summary>
    public partial class UpdateResidentView : Window
    {
        private readonly IApartmentService _apartmentService;
        private readonly string _residentId;

        public UpdateResidentView(IApartmentService apartmentService, string residentId)
        {
            InitializeComponent();
            _apartmentService = apartmentService;
            _residentId = residentId;

            LoadResidentData();
        }

        private async void LoadResidentData()
        {
            Resident resident = await _apartmentService.GetResidentById(_residentId);
            ResidentIdInput.Text = resident.ResidentId;
            FullNameInput.Text = resident.FullName;
            GenderInput.SelectedIndex = resident.Gender!.Trim() == "Nam" ? 0 : 1;
            DateOfBirthInput.SelectedDate = resident.DateOfBirth?.ToDateTime(new TimeOnly());
            RelationShipInput.Text = resident.RelationShipWithOwner;
        }

        private async void DeleteResident_Click(object sender, RoutedEventArgs e)
        {
            string id = ResidentIdInput.Text.Trim();
            if (DateOfBirthInput.SelectedDate == null)
            {
                throw new BusinessException("Không được để trống ngày sinh");
            }
            await _apartmentService.MoveResidentOut(id);
            MessageBox.Show("Cập nhật thành công");
            this.Close();
        }

        private async void UpdateResident_Click(object sender, RoutedEventArgs e)
        {
            string id = ResidentIdInput.Text.Trim();
            if(DateOfBirthInput.SelectedDate == null)
            {
                throw new BusinessException("Không được để trống ngày sinh");
            }
            UpdateResidentDTO dto = new()
            {
                FullName = FullNameInput.Text.Trim(),
                DateOfBirth = DateOnly.FromDateTime(DateOfBirthInput.SelectedDate!.Value),
                Gender = (GenderInput.SelectedItem as ComboBoxItem)?.Content.ToString(),
                RelationShipWithOwner = RelationShipInput.Text.Trim()
            };
            await _apartmentService.UpdateResident(id, dto);
            MessageBox.Show("Cập nhật thành công");
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void MoveIn_Click(object sender, RoutedEventArgs e)
        {
            string id = ResidentIdInput.Text.Trim();
            if (DateOfBirthInput.SelectedDate == null)
            {
                throw new BusinessException("Không được để trống ngày sinh");
            }
            await _apartmentService.MoveResidentIn(id);
            MessageBox.Show("Cập nhật thành công");
            this.Close();
        }
    }
}
