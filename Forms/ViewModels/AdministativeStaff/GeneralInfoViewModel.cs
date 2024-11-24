using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using MaterialDesignThemes.Wpf;
using Services.DTOs.GeneralInfo.AreaDTO;
using Services.DTOs.GeneralInfo.BlockDTO;
using Services.Interfaces.AdministrativeStaffServices;

using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using System.Diagnostics;
using Services.DTOs.GeneralInfo.FloorDTO;


namespace Forms.ViewModels.AdministativeStaff
{
   public partial class GeneralInfoViewModel : ObservableObject
    {
        private readonly IGeneralInfoService _generalInfoService;
        private DispatcherTimer? _searchTimer;
        private DispatcherTimer? _searchBlockTimer;
        private DispatcherTimer? _searchFloorTimer;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string searchAreaText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<AreaResponseDTO> areas;

        [ObservableProperty]
        private AreaResponseDTO selectedArea;


        // -------------------
        [ObservableProperty]
        private ObservableCollection<BlockResponseDTO> blocks;

        [ObservableProperty]
        private BlockResponseDTO selectedBlock;

        [ObservableProperty]
        private string searchBlockText = string.Empty;
        // -------------------
        [ObservableProperty]
        private ObservableCollection<FloorResponseDTO> floors;

        [ObservableProperty]
        private FloorResponseDTO selectedFloor;

        [ObservableProperty]
        private string searchFloorText = string.Empty;


        public GeneralInfoViewModel(IGeneralInfoService generalInfoService)
        {
            _generalInfoService = generalInfoService;
            Areas = new ObservableCollection<AreaResponseDTO>();
            Blocks = new ObservableCollection<BlockResponseDTO>();

            Floors = new ObservableCollection<FloorResponseDTO>();
            Task.Run(LoadAreas);
        }

        // Trong ViewModel
        private async Task LoadInitialData()
        {
            try
            {
                IsLoading = true;
                await LoadAreas();
            }
            catch (BusinessException bex)
            {
                MessageBox.Show(bex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi tải dữ liệu");
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        partial void OnSelectedAreaChanged(AreaResponseDTO? value)
        {
            if (value == null)
            {
                Blocks?.Clear();
                return;
            }

            // Use Task.Run to avoid UI thread blocking
            Task.Run(async () =>
            {
                try
                {
                    await Application.Current.Dispatcher.InvokeAsync(() => IsLoading = true);
                    var blocks = await _generalInfoService.GetBlocksByAreaAsync(value.AreaId);
                    
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Blocks = new ObservableCollection<BlockResponseDTO>(blocks);
                        IsLoading = false;
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading blocks: {ex.Message}");
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        MessageBox.Show("Error loading blocks. Please try again.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        IsLoading = false;
                    });
                }
            });
        }

     [RelayCommand]
        private async Task LoadBlocks(int areaId)
        {
            try
            {
                IsLoading = true;
                var blocks = await _generalInfoService.GetBlocksByAreaAsync(areaId);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Blocks = new ObservableCollection<BlockResponseDTO>(blocks);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading blocks: {ex.Message}");
                MessageBox.Show("Error loading blocks. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SearchBlock()
        {
            if (SelectedArea == null) return;

            try
            {
                IsLoading = true;
                var results = await _generalInfoService.SearchBlocksAsync(SelectedArea.AreaId, SearchBlockText);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Blocks = new ObservableCollection<BlockResponseDTO>(results);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching blocks: {ex.Message}");
                MessageBox.Show("Error searching blocks. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSearchBlockTextChanged(string value)
        {
            _searchBlockTimer?.Stop();
            
            _searchBlockTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            _searchBlockTimer.Tick += async (s, e) =>
            {
                _searchBlockTimer.Stop();
                await SearchBlock();
            };
            _searchBlockTimer.Start();
        }

        [RelayCommand]
        private async Task LoadAreas()
        {
            try
            {
                IsLoading = true;
                var areas = await _generalInfoService.GetAllAreasAsync();
                Areas = new ObservableCollection<AreaResponseDTO>(areas);
            }
            catch (BusinessException ex)
            {
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RefreshData()
        {
            try 
            {
                IsLoading = true;
                await LoadAreas();
                
                if (SelectedArea != null)
                {
                    await LoadBlocks(SelectedArea.AreaId);
                }
                
                if (SelectedBlock != null)
                {
                    await LoadFloorsForBlock(SelectedBlock.BlockId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing data: {ex.Message}");
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show("Có lỗi xảy ra khi làm mới dữ liệu"));
            }
            finally
            {
                IsLoading = false;
            }
        }

        // [RelayCommand]
        private async Task SearchArea()
        {
            if (string.IsNullOrWhiteSpace(SearchAreaText))
            {
                await LoadAreas();
                return;
            }
            
            try
            {
                IsLoading = true;
                var results = await _generalInfoService.SearchAreasAsync(SearchAreaText);
                Areas = new ObservableCollection<AreaResponseDTO>(results);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Search error: {ex.Message}");
                MessageBox.Show("Không thể tìm kiếm. Vui lòng thử lại.");
            }
            finally
            {
                IsLoading = false;
            }
        }


        partial void OnSearchAreaTextChanged(string value)
        {
            if (_searchTimer != null)
            {
                _searchTimer.Stop();
            }

            _searchTimer = new DispatcherTimer 
            { 
                Interval = TimeSpan.FromMilliseconds(300) 
            };
            
            _searchTimer.Tick += async (s, e) =>
            {
                _searchTimer.Stop();
                await SearchArea();
            };
            
            _searchTimer.Start();
        }

        // -------------------

        partial void OnSelectedBlockChanged(BlockResponseDTO? value)
        {
            if (value == null)
            {
                Application.Current.Dispatcher.Invoke(() => Floors?.Clear());
                return;
            }

            Task.Run(async () => 
            {
                try
                {
                    await Application.Current.Dispatcher.InvokeAsync(() => IsLoading = true);
                    var floors = await _generalInfoService.GetFloorsByBlockAsync(value.BlockId);
                    await Application.Current.Dispatcher.InvokeAsync(() => 
                    {
                        Floors = new ObservableCollection<FloorResponseDTO>(floors);
                        IsLoading = false;
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading floors: {ex.Message}");
                    await Application.Current.Dispatcher.InvokeAsync(() => 
                    {
                        MessageBox.Show("Error loading floors. Please try again.");
                        IsLoading = false;
                    });
                }
            });
        }

        private async Task LoadFloorsForBlock(int blockId)
        {
            try
            {
                IsLoading = true;
                var floors = await _generalInfoService.GetFloorsByBlockAsync(blockId);
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Floors = new ObservableCollection<FloorResponseDTO>(floors);
                });
            }
            catch (BusinessException bex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show(bex.Message));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading floors: {ex.Message}");
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show("Có lỗi xảy ra khi tải danh sách tầng"));
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSearchFloorTextChanged(string value)
        {
            if (_searchFloorTimer != null)
            {
                _searchFloorTimer.Stop();
            }
            _searchFloorTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            _searchFloorTimer.Tick += async (s, e) =>
            {
                _searchFloorTimer.Stop();
                await SearchFloor();
            };
            _searchFloorTimer.Start();
        }

        
        private async Task SearchFloor()
        {
            if (SelectedBlock == null) return;

            try
            {
                IsLoading = true;
                var results = await _generalInfoService.SearchFloorsAsync(
                    SelectedBlock.BlockId, 
                    SearchFloorText);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Floors = new ObservableCollection<FloorResponseDTO>(results);
                });
            }
            catch (BusinessException bex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show(bex.Message));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching floors: {ex.Message}");
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show("Có lỗi xảy ra khi tìm kiếm tầng"));
            }
            finally
            {
                IsLoading = false;
            }
        }

    }
}
