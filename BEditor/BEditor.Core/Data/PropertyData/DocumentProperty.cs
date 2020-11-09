﻿using System;
using System.Runtime.Serialization;

namespace BEditor.Core.Data.PropertyData {
    /// <summary>
    /// 複数行の文字のプロパティを表します
    /// </summary>
    [DataContract(Namespace = "")]
    public sealed class DocumentProperty : PropertyElement {
        private string textProperty;

        /// <summary>
        /// <see cref="DocumentProperty"/> クラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="metadata">このプロパティの <see cref="DocumentPropertyMetadata"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> が <see langword="null"/> です</exception>
        public DocumentProperty(DocumentPropertyMetadata metadata) {
            if (metadata is null) throw new ArgumentNullException(nameof(metadata));

            Text = metadata.DefaultText;
            HeightProperty = metadata.Height;
            PropertyMetadata = metadata;
        }


        /// <summary>
        /// 入力されている文字列を取得または設定します
        /// </summary>
        [DataMember]
        public string Text { get => textProperty; set => SetValue(value, ref textProperty, nameof(Text)); }
        /// <summary>
        /// 高さを取得または設定します
        /// </summary>
        public int? HeightProperty { get; set; }

        /// <inheritdoc/>
        public override string ToString() => $"(Text:{Text} Name:{PropertyMetadata?.Name})";

        #region Commands

        /// <summary>
        /// 文字を変更するコマンド
        /// </summary>
        /// <remarks>このクラスは <see cref="UndoRedoManager.Do(IUndoRedoCommand)"/> と併用することでコマンドを記録できます</remarks>
        public sealed class TextChangeCommand : IUndoRedoCommand {
            private readonly DocumentProperty Document;
            private readonly string newtext;
            private readonly string oldtext;

            /// <summary>
            /// <see cref="TextChangeCommand"/> クラスの新しいインスタンスを初期化します
            /// </summary>
            /// <param name="property"></param>
            /// <param name="text"></param>
            /// <exception cref="ArgumentNullException"><paramref name="property"/> が <see langword="null"/> です</exception>
            public TextChangeCommand(DocumentProperty property, string text) {
                Document = property ?? throw new ArgumentNullException(nameof(property));
                newtext = text;
                oldtext = property.Text;
            }


            /// <inheritdoc/>
            public void Do() => Document.Text = newtext;

            /// <inheritdoc/>
            public void Redo() => Do();

            /// <inheritdoc/>
            public void Undo() => Document.Text = oldtext;
        }

        #endregion
    }

    /// <summary>
    /// <see cref="DocumentProperty"/> のメタデータを表します
    /// </summary>
    public class DocumentPropertyMetadata : PropertyElementMetadata {
        /// <summary>
        /// <see cref="DocumentPropertyMetadata"/> クラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="text">デフォルトのテキスト</param>
        /// <param name="height">高さ</param>
        public DocumentPropertyMetadata(string text, int? height = null) : base("") {
            DefaultText = text;
            Height = height;
        }

        /// <summary>
        /// デフォルトのテキストを取得します
        /// </summary>
        public string DefaultText { get; }
        /// <summary>
        /// 高さを取得します
        /// </summary>
        public int? Height { get; }
    }
}