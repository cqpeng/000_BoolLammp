using _000_BoolLammp.Helper.TypeDefine;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _000_BoolLammp
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Thread_Run = new(MainRun) { IsBackground = true };
            Thread_Run.Start();
        }
        [ObservableProperty]
        private Thread thread_Run;
        [ObservableProperty]
        private bool isRun = false;
        private CancellationTokenSource cancellationTokenSource = new ();
        [ObservableProperty]
        private ObservableCollection<Lamp>  lampList = new();
        [ObservableProperty]
        private ConcurrentDictionary<int, Lamp> lampDict = new();
        [ObservableProperty]
        private Mode mode = Mode.Single;
        [ObservableProperty]
        private int delayTime = 10;
        [ObservableProperty]
        private int lampCount = 200;
        [RelayCommand]
        private async Task InitializeLampList(object obj)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var ret = await InitializeLampListAsync(cts.Token);
            if(ret != 0)
            {
                MessageBox.Show($"初始化失败,错误代码：{ret}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async Task<int> InitializeLampListAsync(CancellationToken cancellationToken)
        {
            return await Task.Run(int() => InitializeLampList(), cancellationToken);
        }
        public int InitializeLampList()
        {

            for (int i = 0; i < LampCount; i++)
            {
                Lamp lamp = new ()
                {
                    Id = i,
                    IsOn = false
                };
                if(!AddLamp(lamp))
                    return -1;
            }
            return 0;
        }
        [RelayCommand]
        private async Task ClearLampList(object obj)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var ret = await ClearLampListAsync(cts.Token);
            if (ret != 0)
            {
                MessageBox.Show($"清空失败,错误代码：{ret}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       public async Task<int> ClearLampListAsync(CancellationToken cancellationToken)
       {
            return await Task.Run(int () => ClearLampList(), cancellationToken);
        }
        public int ClearLampList()
        {

            foreach (Lamp lamp in LampDict.Values)
            {
                if (!RemoveLamp(lamp))
                    return -1;
            }
            return 0;
        }
        public void Update()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LampList.Clear();
                foreach (var item in LampDict)
                {
                    LampList.Add(item.Value);
                }
            });
            
        }
        public bool AddLamp(Lamp lamp)
        {
            if(LampDict.TryAdd(lamp.Id, lamp))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LampList.Add(lamp);
                });
                //Update();
                return true;
            }
            return false;
        }
        public bool RemoveLamp(Lamp lamp)
        {
            if( LampDict.TryRemove(lamp.Id, out _))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LampList.Remove(lamp);
                });
                //Update();
                return true;
            }
            return false;
        }
        [RelayCommand]
        private void Reset(object obj)
        {
            Reset();
        }
        public void Reset()
        {
            foreach (var item in LampDict.Values)
            {
                item.IsOn = false;
            }
        }
        [RelayCommand]
        private void Start(object obj)
        {
            if(LampDict.Count == 0)
            {
                MessageBox.Show("启动失败，请先初始化一定数量的灯", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            IsRun = true;
            cancellationTokenSource = new ();
        }
        [RelayCommand]
        private void Stop(object obj)
        {
            IsRun = false;
            cancellationTokenSource.Cancel();
            MessageBox.Show("已停止", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MainRun()
        {
            while (true)
            {

                while (IsRun)
                {
                    var token = cancellationTokenSource.Token;
                    Lamp lastLamp = null;
                    Lamp culLamp = null;
                    switch (Mode)
                    {
                        case Mode.Single:
                            foreach (var item in LampDict.Values)
                            {
                                if (token.IsCancellationRequested) break;
                                culLamp = item;
                                item.IsOn = !item.IsOn;
                                if (lastLamp != null)
                                    lastLamp.IsOn = false;
                                lastLamp = item;
                                Thread.Sleep(DelayTime);
                            }
                            Reset();

                            break;
                        case Mode.SingleLoop:
                            foreach (var item in LampDict.Values)
                            {
                                if (token.IsCancellationRequested) break;

                                culLamp = item;
                                item.IsOn = !item.IsOn;
                                if (lastLamp != null)
                                    lastLamp.IsOn = false;
                                lastLamp = item;
                                Thread.Sleep(DelayTime);
                            }
                            var sorketKeys = LampDict.Keys.OrderByDescending(k => k );
                            foreach (var key in sorketKeys)
                            {
                                if (token.IsCancellationRequested) break;
                                if (LampDict.TryGetValue(key, out var item))
                                {
                                    culLamp = item;
                                    item.IsOn = !item.IsOn;
                                    if (lastLamp != null)
                                        lastLamp.IsOn = false;
                                    lastLamp = item;
                                    Thread.Sleep(DelayTime);
                                }
                                
                                
                            }
                            Reset();
                            break;
                        case Mode.DirectionShow:
                            foreach (var item in LampDict.Values)
                            {
                                if (token.IsCancellationRequested) break;
                                item.IsOn = !item.IsOn;
                                Thread.Sleep(DelayTime);
                            }
                            Reset();
                            break;
                        case Mode.TowDirectionShow:
                            var sorketKeysLeft = LampDict.Keys.OrderBy(k => k < LampCount / 2);
                            var sorketKeysRight = LampDict.Keys.OrderBy(k => k >= LampCount / 2);
                            for (int i = 0;i < LampDict.Count; i++)
                            {
                                if (LampDict.TryGetValue(i, out var itemLeft))
                                {
                                    itemLeft.IsOn = !itemLeft.IsOn;
                                }
                                if (LampDict.TryGetValue(LampDict.Count - 1 - i, out var itemRight))
                                {
                                    itemRight.IsOn = !itemRight.IsOn;
                                }
                                if (token.IsCancellationRequested) break;
                                Thread.Sleep(DelayTime);
                            }
                            Reset();
                            break;
                        default:
                            break;
                    }

                   
                }
                Thread.Sleep(1000);
            }
        }
    }
}
