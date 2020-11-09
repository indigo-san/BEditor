﻿using System;
using System.Runtime.Serialization;

using BEditor.Core.Data.PropertyData.EasingSetting;

namespace BEditor.Core.Data.PropertyData {
    /// <summary>
    /// ファイルを選択するプロパティを表します
    /// </summary>
    [DataContract(Namespace = "")]
    public sealed class FileProperty : PropertyElement, IEasingSetting {
        private string file;

        /// <summary>
        /// <see cref="FileProperty"/> クラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="metadata">このプロパティの <see cref="FilePropertyMetadata"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> が <see langword="null"/> です</exception>
        public FileProperty(FilePropertyMetadata metadata) {
            PropertyMetadata = metadata??throw new ArgumentNullException(nameof(metadata));
            File = metadata.DefaultFile;
        }

        /// <summary>
        /// ファイルの名前を取得または設定します
        /// </summary>
        [DataMember]
        public string File { get => file; set => SetValue(value, ref file, nameof(File)); }

        /// <inheritdoc/>
        public override string ToString() => $"(File:{File} Name:{PropertyMetadata?.Name})";


        #region Commands

        /// <summary>
        /// ファイルの名前を変更するコマンド
        /// </summary>
        /// <remarks>このクラスは <see cref="UndoRedoManager.Do(IUndoRedoCommand)"/> と併用することでコマンドを記録できます</remarks>
        public sealed class ChangeFileCommand : IUndoRedoCommand {
            private readonly FileProperty FileSetting;
            private readonly string path;
            private readonly string oldpath;

            /// <summary>
            /// <see cref="ChangeFileCommand"/> クラスの新しいインスタンスを初期化します
            /// </summary>
            /// <param name="property">対象の <see cref="FileProperty"/></param>
            /// <param name="path">新しい値</param>
            /// <exception cref="ArgumentNullException"><paramref name="property"/> が <see langword="null"/> です</exception>
            public ChangeFileCommand(FileProperty property, string path) {
                FileSetting = property ?? throw new ArgumentNullException(nameof(property));
                this.path = path;
                oldpath = FileSetting.File;
            }


            /// <inheritdoc/>
            public void Do() => FileSetting.File = path;

            /// <inheritdoc/>
            public void Redo() => Do();

            /// <inheritdoc/>
            public void Undo() => FileSetting.File = oldpath;
        }

        #endregion
    }

    /// <summary>
    /// <see cref="FileProperty"/> のメタデータを表します
    /// </summary>
    public class FilePropertyMetadata : PropertyElementMetadata {
        /// <summary>
        /// <see cref="FilePropertyMetadata"/> クラスの新しいインスタンスを初期化します
        /// </summary>
        public FilePropertyMetadata(string name, string defaultfile=null, string filter=null, string filtername=null) : base(name) {
            DefaultFile = defaultfile;
            Filter = filter;
            FilterName = filtername;
        }

        /// <summary>
        /// デフォルトのファイル名を取得します
        /// </summary>
        public string DefaultFile { get; }
        /// <summary>
        /// フィルターを取得します
        /// </summary>
        public string Filter { get; }
        /// <summary>
        /// フィルターの名前を取得します
        /// </summary>
        public string FilterName { get; }
    }
}