using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _000_BoolLammp
{
    public partial class Lamp : ObservableObject
    {
        [ObservableProperty]
        private int _Id = 0;
        [ObservableProperty]
        private bool _IsOn = false;
        [RelayCommand]
        private void Toggle()
        {
            IsOn = !IsOn;
        }
    }
}
