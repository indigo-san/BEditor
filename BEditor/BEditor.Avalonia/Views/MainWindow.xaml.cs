﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using Material.Styles;

namespace BEditor.Views
{
    public class MainWindow : Window
    {
        public static MainWindow? Current;

        public MainWindow()
        {
            InitializeComponent();

            Current = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void MenuButton_Click(object sender, RoutedEventArgs args)
        {
            var button = (Button)sender;

            button.ContextMenu?.Open(button);
        }
    }
}