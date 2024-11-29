using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.RegulationDTO;
using Services.DTOs.ViolationDTO;
using Services.Interfaces.AdministrativeStaffServices;
using Services.Services.SharedServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forms.ViewModels.AdministativeStaff
{
    public partial class RegulationViewModel : ObservableObject
    {
        private readonly IRegulationService _regulationService;

        [ObservableProperty]
        private ObservableCollection<RegulationResponseDTO> _regulations = new();

        [ObservableProperty]
        private RegulationResponseDTO? _selectedRegulation;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string? _selectedCategory;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private ObservableCollection<string> _categories = new();

        [ObservableProperty]
        private ObservableCollection<string> _priorityLevels = new();

        public RegulationViewModel(IRegulationService regulationService)
        {
            _regulationService = regulationService;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadCategories();
            await LoadPriorityLevels();
            await LoadRegulationsAsync();
        }

        private async Task LoadCategories()
        {
            var categories = await _regulationService.GetCategoriesAsync();
            Categories = new ObservableCollection<string>(categories);
        }

        private async Task LoadPriorityLevels()
        {
            var levels = await _regulationService.GetPriorityLevelsAsync();
            PriorityLevels = new ObservableCollection<string>(levels);
        }

        [RelayCommand]
        private async Task LoadRegulationsAsync()
        {
            if (IsLoading) return; // Prevent multiple concurrent loads
            
            try
            {
                IsLoading = true;
                var regulations = await _regulationService.GetAllAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Regulations = new ObservableCollection<RegulationResponseDTO>(regulations);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Không thể tải danh sách nội quy: " + ex.Message);
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadRegulationsAsync();
                return;
            }

            try
            {
                IsLoading = true;
                var results = await _regulationService.SearchAsync(SearchText);
                Regulations = new ObservableCollection<RegulationResponseDTO>(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tìm kiếm: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void CreateNew()
        {
            SelectedRegulation = new RegulationResponseDTO
            {
                Title = string.Empty,
                Content = string.Empty,
                CreatedDate = DateTime.Now,
                Category = Categories.FirstOrDefault() ?? string.Empty,
                Priority = PriorityLevels.FirstOrDefault() ?? string.Empty,
                IsActive = true
            };
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (SelectedRegulation == null)
            {
                MessageBox.Show("Vui lòng nhập thông tin nội quy");
                return;
            }

            try
            {
                IsLoading = true;

                if (SelectedRegulation.RegulationId == 0) // Create new
                {
                    var createDto = new CreateRegulationDTO
                    {
                        Title = SelectedRegulation.Title,
                        Content = SelectedRegulation.Content,
                        Category = SelectedRegulation.Category,
                        Priority = SelectedRegulation.Priority,
                        IsActive = SelectedRegulation.IsActive
                    };

                    var result = await _regulationService.CreateAsync(createDto);
                    Regulations.Add(result);
                    MessageBox.Show("Thêm nội quy thành công!");
                }
                else // Update
                {
                    var result = await _regulationService.UpdateAsync(SelectedRegulation);
                    var index = Regulations.IndexOf(Regulations.First(x => x.RegulationId == result.RegulationId));
                    Regulations[index] = result;
                    MessageBox.Show("Cập nhật nội quy thành công!");
                }
            }
            catch (BusinessException bex)
            {
                MessageBox.Show(bex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi lưu: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSelectedCategoryChanged(string? value)
        {
            if (value == null) return;
            // Gọi async method một cách an toàn
            _ = Task.Run(async () => 
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await FilterRegulationsAsync();
                });
            });
        }

        private async Task FilterRegulationsAsync()
        {
            if (SelectedCategory == null) return;
            
            await LoadRegulationsAsync();
            Regulations = new ObservableCollection<RegulationResponseDTO>(
                Regulations.Where(r => r.Category == SelectedCategory)
            );
        }
    }
}
