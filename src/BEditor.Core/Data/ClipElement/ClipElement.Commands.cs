﻿using System;

using BEditor.Command;
using BEditor.Media;
using BEditor.Resources;

namespace BEditor.Data
{
    /// <summary>
    /// Represents a data of a clip to be placed in the timeline.
    /// </summary>
    public partial class ClipElement
    {
        /// <summary>
        /// 指定したシーンに新しいクリップを追加するコマンドを表します.
        /// </summary>
        internal sealed class AddCommand : IRecordCommand
        {
            private readonly Scene _scene;

            /// <summary>
            /// Initializes a new instance of the <see cref="AddCommand"/> class.
            /// </summary>
            /// <param name="scene">新しいクリップを追加するシーンです.</param>
            /// <param name="startFrame">新しいクリップの開始フレームです.</param>
            /// <param name="layer">新しいクリップの配置レイヤーです.</param>
            /// <param name="metadata">新しいクリップのオブジェクトのメタデータです.</param>
            public AddCommand(Scene scene, Frame startFrame, int layer, ObjectMetadata metadata)
            {
                _scene = scene ?? throw new ArgumentNullException(nameof(scene));
                if (startFrame < Frame.Zero) throw new ArgumentOutOfRangeException(nameof(startFrame));
                if (layer < 0) throw new ArgumentOutOfRangeException(nameof(layer));
                if (metadata is null) throw new ArgumentNullException(nameof(metadata));

                // オブジェクトの情報
                Clip = new ClipElement(startFrame, startFrame + 180, layer, scene, metadata);
            }

            /// <summary>
            /// Gets the clip to add.
            /// </summary>
            public ClipElement Clip { get; private set; }

            /// <inheritdoc/>
            public string Name => Strings.AddClip;

            /// <inheritdoc/>
            public void Do()
            {
                Clip.Load();
                _scene.Add(Clip);
                _scene.SetCurrentClip(Clip);
            }

            /// <inheritdoc/>
            public void Redo()
            {
                Do();
            }

            /// <inheritdoc/>
            public void Undo()
            {
                _scene.Remove(Clip);
                Clip.Unload();

                // 存在する場合
                if (_scene.SelectItems.Contains(Clip))
                {
                    _scene.SelectItems.Remove(Clip);

                    if (_scene.SelectItem == Clip)
                    {
                        _scene.SelectItem = null;
                    }
                }
            }
        }

        /// <summary>
        /// クリップを削除するコマンドを表します.
        /// </summary>
        internal sealed class RemoveCommand : IRecordCommand
        {
            private readonly ClipElement _clip;

            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveCommand"/> class.
            /// </summary>
            /// <param name="clip">削除するクリップです.</param>
            public RemoveCommand(ClipElement clip)
            {
                _clip = clip ?? throw new ArgumentNullException(nameof(clip));
            }

            /// <inheritdoc/>
            public string Name => Strings.RemoveClip;

            /// <inheritdoc/>
            public void Do()
            {
                if (!_clip.Parent.Remove(_clip))
                {
                    // Message.Snackbar("削除できませんでした");
                }
                else
                {
                    _clip.Unload();

                    // 存在する場合
                    if (_clip.Parent.SelectItems.Contains(_clip))
                    {
                        _clip.Parent.SelectItems.Remove(_clip);

                        if (_clip.Parent.SelectItem == _clip)
                        {
                            if (_clip.Parent.SelectItems.Count == 0)
                            {
                                _clip.Parent.SelectItem = null;
                            }
                            else
                            {
                                _clip.Parent.SelectItem = _clip.Parent.SelectItems[0];
                            }
                        }
                    }
                }
            }

            /// <inheritdoc/>
            public void Redo()
            {
                Do();
            }

            /// <inheritdoc/>
            public void Undo()
            {
                _clip.Load();
                _clip.Parent.Add(_clip);
            }
        }

        /// <summary>
        /// クリップを移動するコマンドを表します.
        /// </summary>
        private sealed class MoveCommand : IRecordCommand
        {
            private readonly ClipElement _clip;
            private readonly Frame _newFrame;
            private readonly Frame _oldFrame;
            private readonly int _newLayer;
            private readonly int _oldLayer;

            /// <summary>
            /// Initializes a new instance of the <see cref="MoveCommand"/> class.
            /// </summary>
            /// <param name="clip">移動するクリップです.</param>
            /// <param name="newframe">新しい開始フレームです.</param>
            /// <param name="newlayer">新しい配置レイヤーです.</param>
            public MoveCommand(ClipElement clip, Frame newframe, int newlayer)
            {
                _clip = clip ?? throw new ArgumentNullException(nameof(clip));
                _newFrame = (newframe < Frame.Zero) ? throw new ArgumentOutOfRangeException(nameof(newframe)) : newframe;
                _oldFrame = clip.Start;
                _newLayer = (newlayer < 0) ? throw new ArgumentOutOfRangeException(nameof(newlayer)) : newlayer;
                _oldLayer = clip.Layer;
            }

            /// <inheritdoc/>
            public string Name => Strings.MoveClip;
            private Scene Scene => _clip.Parent;

            /// <inheritdoc/>
            public void Do()
            {
                _clip.MoveTo(_newFrame);

                _clip.Layer = _newLayer;

                if (_clip.End > Scene.TotalFrame)
                {
                    Scene.TotalFrame = _clip.End;
                }
            }

            /// <inheritdoc/>
            public void Redo()
            {
                Do();
            }

            /// <inheritdoc/>
            public void Undo()
            {
                _clip.MoveTo(_oldFrame);

                _clip.Layer = _oldLayer;
            }
        }

        /// <summary>
        /// クリップの長さを変更するコマンドを表します.
        /// </summary>
        private sealed class LengthChangeCommand : IRecordCommand
        {
            private readonly ClipElement _clip;
            private readonly Frame _newStart;
            private readonly Frame _newEnd;
            private readonly Frame _oldStart;
            private readonly Frame _oldEnd;

            /// <summary>
            /// Initializes a new instance of the <see cref="LengthChangeCommand"/> class.
            /// </summary>
            /// <param name="clip">長さを変更するクリップです.</param>
            /// <param name="start">クリップの新しい開始フレームです.</param>
            /// <param name="end">クリップの新しい終了フレームです.</param>
            public LengthChangeCommand(ClipElement clip, Frame start, Frame end)
            {
                _clip = clip ?? throw new ArgumentNullException(nameof(clip));
                _newStart = (start < Frame.Zero) ? throw new ArgumentOutOfRangeException(nameof(start)) : start;
                _newEnd = (end < Frame.Zero) ? throw new ArgumentOutOfRangeException(nameof(end)) : end;
                _oldStart = clip.Start;
                _oldEnd = clip.End;
            }

            /// <inheritdoc/>
            public string Name => Strings.ChangeLength;

            /// <inheritdoc/>
            public void Do()
            {
                _clip.Start = _newStart;
                _clip.End = _newEnd;
            }

            /// <inheritdoc/>
            public void Redo()
            {
                Do();
            }

            /// <inheritdoc/>
            public void Undo()
            {
                _clip.Start = _oldStart;
                _clip.End = _oldEnd;
            }
        }

        /// <summary>
        /// クリップを分割するコマンドを表します.
        /// </summary>
        private sealed class SplitCommand : IRecordCommand
        {
            private readonly ClipElement _source;
            private readonly Scene _scene;

            /// <summary>
            /// Initializes a new instance of the <see cref="SplitCommand"/> class.
            /// </summary>
            /// <param name="clip">分割するクリップです.</param>
            /// <param name="frame">分割するフレームです.</param>
            public SplitCommand(ClipElement clip, Frame frame)
            {
                _source = clip;
                _scene = clip.Parent;
                Before = clip.Clone();
                After = clip.Clone();

                Before.End = frame;
                After.Start = frame;
            }

            /// <summary>
            /// Gets the clip before the split frame.
            /// </summary>
            public ClipElement Before { get; }

            /// <summary>
            /// Gets the clip after the split frame.
            /// </summary>
            public ClipElement After { get; }

            /// <inheritdoc/>
            public string Name => Strings.SplitClip;

            /// <inheritdoc/>
            public void Do()
            {
                After.Load();
                Before.Load();

                new RemoveCommand(_source).Do();
                After.SetID(Guid.NewGuid());
                _scene.Add(After);
                Before.SetID(Guid.NewGuid());
                _scene.Add(Before);
            }

            /// <inheritdoc/>
            public void Redo()
            {
                Do();
            }

            /// <inheritdoc/>
            public void Undo()
            {
                Before.Unload();
                After.Unload();
                _source.Load();

                _scene.Remove(Before);
                _scene.Remove(After);
                _scene.Add(_source);
            }
        }
    }
}