using System;
using System.Collections.Generic;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using BEditor.Data;
using BEditor.Extensions;
using BEditor.Models;
using BEditor.Properties;

using static BEditor.IMessage;

namespace BEditor.Views
{
    public partial class ObjectViewer : UserControl
    {
        private static readonly IMessage Message = AppModel.Current.Message;

        public ObjectViewer()
        {
            InitializeComponent();
        }

        public static IEnumerable<string> Empty { get; } = Enumerable.Empty<string>();

        private async void CopyID_Click(object sender, RoutedEventArgs e)
        {
            if (this.FindControl<TreeView>("TreeView").SelectedItem is IEditingObject obj)
            {
                await Application.Current.Clipboard.SetTextAsync(obj.ID.ToString());
            }
            else
            {
                await Message.DialogAsync(string.Format(Strings.ErrorObjectViewer2, nameof(IEditingObject)));
            }
        }

        private Scene? GetScene()
        {
            if (this.FindControl<TreeView>("TreeView").SelectedItem is IChild<object> obj) return obj.GetParent<Scene>();
            else throw new IndexOutOfRangeException();
        }

        private ClipElement? GetClip()
        {
            if (this.FindControl<TreeView>("TreeView").SelectedItem is IChild<object> obj) return obj.GetParent<ClipElement>();
            else throw new IndexOutOfRangeException();
        }

        private EffectElement? GetEffect()
        {
            if (this.FindControl<TreeView>("TreeView").SelectedItem is IChild<object> obj) return obj.GetParent<EffectElement>();
            else throw new IndexOutOfRangeException();
        }

        public async void DeleteScene(object sender, RoutedEventArgs e)
        {
            try
            {
                var scene = GetScene();
                if (scene is null) return;
                if (scene is { SceneName: "root" })
                {
                    Message.Snackbar("RootScene は削除することができません");
                    return;
                }

                if (await Message.DialogAsync(
                    Strings.CommandQ1,
                    types: new ButtonType[] { ButtonType.Yes, ButtonType.No }) == ButtonType.Yes)
                {
                    scene.Parent!.PreviewScene = scene.Parent!.SceneList[0];
                    scene.Parent.SceneList.Remove(scene);
                    scene.Unload();

                    scene.ClearDisposable();
                }
            }
            catch (IndexOutOfRangeException)
            {
                Message.Snackbar(string.Format(Strings.ErrorObjectViewer1, nameof(Scene)));
            }
        }

        public void RemoveClip(object sender, RoutedEventArgs e)
        {
            try
            {
                var clip = GetClip();
                if (clip is null) return;
                clip.Parent.RemoveClip(clip).Execute();
            }
            catch (IndexOutOfRangeException)
            {
                Message.Snackbar(string.Format(Strings.ErrorObjectViewer1, nameof(ClipElement)));
            }
        }

        public void RemoveEffect(object sender, RoutedEventArgs e)
        {
            try
            {
                var effect = GetEffect();
                if (effect is null) return;
                effect.Parent!.RemoveEffect(effect).Execute();
            }
            catch (IndexOutOfRangeException)
            {
                Message.Snackbar(string.Format(Strings.ErrorObjectViewer1, nameof(EffectElement)));
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
