﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using BEditor.Data;
using BEditor.Models;
using BEditor.Plugin;
using BEditor.Properties;
using BEditor.ViewModels;
using BEditor.ViewModels.CreatePage;
using BEditor.Views;
using BEditor.Views.CreatePage;

using MahApps.Metro.Controls;

using MaterialDesignThemes.Wpf;

using Reactive.Bindings;

using Expression = System.Linq.Expressions.Expression;

namespace BEditor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        enum ShowHideState : byte
        {
            Show,
            Hide
        }
        private static readonly Func<PluginManager, List<(string, IEnumerable<ICustomMenu>)>> GetMenus;
        private ShowHideState TimelineIsShown = ShowHideState.Show;
        private ShowHideState PropertyIsShown = ShowHideState.Show;

        static MainWindow()
        {
            var type = typeof(PluginManager);

            var param = Expression.Parameter(type);
            var expression = Expression.Lambda<Func<PluginManager, List<(string, IEnumerable<ICustomMenu>)>>>(
                Expression.Field(param, "_menus"), param);

            GetMenus = expression.Compile();
        }
        public MainWindow()
        {
            InitializeComponent();

            Activated += (_, _) => MainWindowViewModel.Current.MainWindowColor.Value = (System.Windows.Media.Brush)FindResource("PrimaryHueMidBrush");
            Deactivated += (_, _) => MainWindowViewModel.Current.MainWindowColor.Value = (System.Windows.Media.Brush)FindResource("PrimaryHueDarkBrush");
            EditModel.Current.ClipCreate += EditModel_ClipCreate;
            EditModel.Current.SceneCreate += EditModel_SceneCreate;
            EditModel.Current.EffectAddTo += EditModel_EffectAddTo;

            Focus();

            SetRecentUsedFiles();
            SetPluginMenu();
        }

        private void EditModel_EffectAddTo(object? sender, ClipElement c)
        {
            var viewmodel = new EffectAddPageViewModel()
            {
                Scene =
                {
                    Value = c.Parent
                }
            };
            var dialog = new EffectAddPage(viewmodel);

            foreach (var i in viewmodel.ClipItems.Value)
            {
                i.IsSelected.Value = false;
                if (i.Clip == c)
                {
                    i.IsSelected.Value = true;
                }
            }

            new NoneDialog()
            {
                Content = dialog,
                MaxWidth = double.PositiveInfinity
            }.ShowDialog();

            viewmodel.Dispose();
        }
        private void EditModel_SceneCreate(object? sender, EventArgs e)
        {
            var view = new SceneCreatePage();
            new NoneDialog()
            {
                Content = view,
                MaxWidth = double.PositiveInfinity,
            }.ShowDialog();

            if (view.DataContext is IDisposable disposable) disposable.Dispose();
        }
        private void EditModel_ClipCreate(object? sender, EventArgs e)
        {
            var viewmodel = new ClipCreatePageViewModel()
            {
                Scene =
                {
                    Value = AppData.Current.Project!.PreviewScene
                }
            };
            var dialog = new ClipCreatePage(viewmodel);

            new NoneDialog()
            {
                Content = dialog,
                MaxWidth = double.PositiveInfinity
            }.ShowDialog();

            viewmodel.Dispose();
        }

        private void SetRecentUsedFiles()
        {
            static async Task ProjectOpenCommand(string name)
            {
                try
                {
                    await ProjectModel.DirectOpen(name);
                }
                catch
                {
                    Debug.Assert(false);
                    AppData.Current.Message.Snackbar(string.Format(Strings.FailedToLoad, "Project"));
                }
            }

            foreach (var file in Settings.Default.RecentFiles)
            {
                var menu = new MenuItem()
                {
                    Header = file
                };
                menu.Click += async (s, e) => await ProjectOpenCommand(((s as MenuItem)!.Header as string)!);

                UsedFiles.Items.Insert(0, menu);
            }

            Settings.Default.RecentFiles.CollectionChanged += (s, e) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    if (s is null) return;
                    if (e.Action is NotifyCollectionChangedAction.Add)
                    {
                        var menu = new MenuItem()
                        {
                            Header = (s as ObservableCollection<string>)![e.NewStartingIndex]
                        };
                        menu.Click += async (s, e) => await ProjectOpenCommand(((s as MenuItem)!.Header as string)!);

                        UsedFiles.Items.Insert(0, menu);
                    }
                    else if (e.Action is NotifyCollectionChangedAction.Remove)
                    {
                        var file = e.OldItems![0] as string;

                        foreach (var item in UsedFiles.Items)
                        {
                            if (item is MenuItem menuItem && menuItem.Header is string header && header == file)
                            {
                                UsedFiles.Items.Remove(item);

                                return;
                            }
                        }
                    }
                });
            };
        }
        private void SetPluginMenu()
        {
            var menus = GetMenus(PluginManager.Default);

            foreach (var item in menus)
            {
                var menu = new MenuItem()
                {
                    Header = item.Item1
                };

                foreach (var m in item.Item2)
                {
                    var command = new ReactiveCommand();
                    command.Subscribe(m.Execute);

                    var newItem = new MenuItem()
                    {
                        Command = command,
                        Header = m.Name
                    };

                    menu.Items.Add(newItem);
                }

                PluginMenu.Items.Add(menu);
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e) { }

        private void ObjectMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var packIcon = (PackIcon)sender;
                Func<ObjectMetadata> s = () => ClipTypeIconConverter.ToClipMetadata(packIcon.Kind);
                var dataObject = new DataObject(typeof(Func<ObjectMetadata>), s);
                // ドラッグ開始
                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;

            if (btn.ContextMenu == null) return;

            btn.ContextMenu.IsOpen = true;
            btn.ContextMenu.PlacementTarget = btn;
            btn.ContextMenu.Placement = PlacementMode.Bottom;
        }

        private void LoadedObjectMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var text = (TextBlock)sender;
                Func<ObjectMetadata> s = () => (ObjectMetadata)text.DataContext;
                var dataObject = new DataObject(typeof(Func<ObjectMetadata>), s);
                // ドラッグ開始
                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
            }
        }

        private void TimelineShowHide(object sender, RoutedEventArgs e)
        {
            if (TimelineIsShown == ShowHideState.Show)
            {
                TimelineGrid.Height = new GridLength(0);
                TimelineIsShown = ShowHideState.Hide;
            }
            else
            {
                TimelineGrid.Height = new GridLength(1, GridUnitType.Star);
                TimelineIsShown = ShowHideState.Show;
            }
        }
        private void PropertyShowHide(object sender, RoutedEventArgs e)
        {
            if (PropertyIsShown == ShowHideState.Show)
            {
                PropertyGrid.Width = new GridLength(0);
                PropertyIsShown = ShowHideState.Hide;
            }
            else
            {
                PropertyGrid.Width = new GridLength(425);
                PropertyIsShown = ShowHideState.Show;
            }
        }
    }
}