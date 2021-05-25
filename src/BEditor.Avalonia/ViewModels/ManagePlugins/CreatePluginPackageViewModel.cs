﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Controls;

using BEditor.Models;
using BEditor.Packaging;
using BEditor.Properties;
using BEditor.Views.DialogContent;

using Microsoft.Extensions.Logging;

using Reactive.Bindings;

namespace BEditor.ViewModels.ManagePlugins
{
    public sealed class CreatePluginPackageViewModel
    {
        public CreatePluginPackageViewModel()
        {
            PickDirectory.Subscribe(async () =>
            {
                var dialog = new OpenFolderDialog();
                if (await dialog.ShowAsync(App.GetMainWindow()) is var dir && Directory.Exists(dir))
                {
                    OutputDirectory.Value = dir;
                }
            });

            PickAssemblyFile.Subscribe(async () =>
            {
                var dialog = new OpenFileRecord
                {
                    Filters =
                    {
                        new(Strings.AssemblyFile, new FileExtension[]
                        {
                            new("dll")
                        })
                    }
                };

                if (await AppModel.Current.FileDialog.ShowOpenFileDialogAsync(dialog))
                {
                    AssemblyFile.Value = dialog.FileName;
                }
            });

            Create.Subscribe(async _ =>
            {
                if (!File.Exists(AssemblyFile.Value))
                {
                    await AppModel.Current.Message.DialogAsync(Strings.FileNotFound + $"\n{AssemblyFile.Value}");
                    return;
                }
                if (!Guid.TryParse(Id.Value, out var id))
                {
                    await AppModel.Current.Message.DialogAsync(Strings.InvalidIdCanUseGUIDAndUUID);
                    return;
                }
                var progress = new ProgressDialog();

                _ = progress.ShowDialog(App.GetMainWindow());
                var mainAsm = Path.GetFileName(AssemblyFile.Value);

                await PackageFile.CreatePackageAsync(
                    AssemblyFile.Value,
                    Path.Combine(OutputDirectory.Value, Path.ChangeExtension(mainAsm, ".bepkg")),
                    new()
                    {
                        MainAssembly = mainAsm,
                        Name = Name.Value,
                        Author = Author.Value,
                        HomePage = WebSite.Value,
                        Description = Description.Value,
                        Id = id,
                        License = License.Value
                    },
                    progress);

                progress.Close();
            },
            async e =>
            {
                App.Logger.LogError(e, Strings.CouldNotCreatePackage);
                await AppModel.Current.Message.DialogAsync(Strings.CouldNotCreatePackage + $"\nmessage: {e.Message}");
            });
        }

        public ReactivePropertySlim<string> Name { get; } = new();

        public ReactivePropertySlim<string> Author { get; } = new();

        public ReactivePropertySlim<string> WebSite { get; } = new();

        public ReactivePropertySlim<string> Description { get; } = new();

        public ReactivePropertySlim<string> Id { get; } = new();

        public ReactivePropertySlim<string> License { get; } = new();

        public ReactivePropertySlim<string> OutputDirectory { get; } = new();

        public ReactiveCommand PickDirectory { get; } = new();

        public ReactivePropertySlim<string> AssemblyFile { get; } = new();

        public ReactiveCommand PickAssemblyFile { get; } = new();

        public ReactiveCommand Create { get; } = new();
    }
}