﻿using System.ComponentModel;

using BEditor.Plugin;

namespace TestPlugin2
{
    public record Setting(
        [property: DisplayName("整数")]
        int Integer,
        [property: DisplayName("浮動小数点")]
        float Single,
        [property: DisplayName("文字列")]
        string Text) : SettingRecord;
}