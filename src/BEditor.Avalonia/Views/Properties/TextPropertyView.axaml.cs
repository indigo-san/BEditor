using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using BEditor.Data;
using BEditor.Data.Property;
using BEditor.Extensions;
using BEditor.Models;
using BEditor.ViewModels.Properties;

namespace BEditor.Views.Properties
{
    public class TextPropertyView : UserControl
    {
        public TextPropertyView()
        {
            InitializeComponent();
        }

        public TextPropertyView(TextProperty property)
        {
            DataContext = new TextPropertyViewModel(property);
            InitializeComponent();
            this.FindControl<TextBox>("TextBox").AddHandler(KeyDownEvent, TextBox_KeyDown, RoutingStrategies.Tunnel);
        }

        public void TextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (DataContext is TextPropertyViewModel vm && sender is TextBox tb)
            {
                vm.TextChanged.Execute(tb.Text);
            }
        }

        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is TextPropertyViewModel vm && sender is TextBox tb)
            {
                vm.LostFocus.Execute(tb.Text);
            }
        }

        public void TextBox_GotFocus(object sender, GotFocusEventArgs e)
        {
            if (DataContext is TextPropertyViewModel vm && sender is TextBox tb)
            {
                vm.GotFocus.Execute(tb.Text);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}