using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace _000_BoolLammp
{
    public partial class Lamp : ObservableObject
    {
        [ObservableProperty]
        private int _Id = 0;
        [ObservableProperty]
        private bool isOn = false;
        [RelayCommand]
        private void Toggle()
        {
            IsOn = !IsOn;
        }
    }
}
