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
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Thread_Run = new(run) { IsBackground = true };
            Thread_Run.Start();
        }
        [ObservableProperty]
        private Thread thread_Run;
        [ObservableProperty]
        private bool _IsRun = false;
        private CancellationTokenSource _cancellationTokenSource = new ();
        [ObservableProperty]
        private ObservableCollection<Lamp>  _BoolList = new();
        [ObservableProperty]
        private ConcurrentDictionary<int, Lamp> _BoolDict = new();
        public void Update()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                BoolList.Clear();
                foreach (var item in BoolDict)
                {
                    BoolList.Add(item.Value);
                }
            });
            
        }
        public bool AddLamp(Lamp lamp)
        {
            if(BoolDict.TryAdd(lamp.Id, lamp))
            {
                Update();
                return true;
            }
            return false;
        }
        public bool RemoveLamp(Lamp lamp)
        {
            if( BoolDict.TryRemove(lamp.Id, out _))
            {
                Update();
                return true;
            }
            return false;
        }
        [RelayCommand]
        private void Reset()
        {
            foreach (var item in )
            {

            }
        }
        [RelayCommand]
        private void Start()
        {
            IsRun = true;
            _cancellationTokenSource = new ();
        }
        [RelayCommand]
        private void Stop()
        {
            IsRun = false;
            _cancellationTokenSource.Cancel();
        }

        private void run()
        {
            while (true)
            {

                while (IsRun)
                {
                    var token = _cancellationTokenSource.Token;
                    foreach (var item in BoolDict.Values)
                    {
                        if (token.IsCancellationRequested) break;
                        Thread.Sleep(50);
                        item.IsOn = !item.IsOn;

                    }

                }
                Thread.Sleep(1000);
            }
        }
    }
}
