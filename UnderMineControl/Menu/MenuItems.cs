using System;
using UnityEngine;

namespace UnderMineControl.Menu
{
    using API.MenuItems;

    public abstract class MenuItem : IMenuItem
    {
        public object UnderlyingValue { get; set; }
        public Action<object> OnChange { get; set; }

        public abstract bool Render();
    }

    public class MenuButton : MenuItem, IMenuButton
    {
        public string Text { get; private set; }

        public MenuButton(string text)
        {
            Text = text;
        }

        public override bool Render()
        {
            return GUILayout.Button(Text);
        }
    }

    public class MenuCheckBox : MenuItem, IMenuCheckBox
    {
        public string Text { get; private set; }

        public bool Value
        {
            get => (bool)UnderlyingValue;
            set => UnderlyingValue = value;
        }

        public MenuCheckBox(string text, bool starting)
        {
            UnderlyingValue = starting;
            Text = text;
        }

        public override bool Render()
        {
            var old = Value;
            Value = GUILayout.Toggle(Value, Text);

            return old != Value;
        }
    }

    public class MenuTextBox : MenuItem, IMenuTextBox
    {
        public string LabelText { get; private set; }
        public string StartingText { get; private set; }
        public bool MultiLine { get; private set; }

        public string Value
        {
            get => (string)UnderlyingValue;
            set => UnderlyingValue = value;
        }

        public MenuTextBox(string label, string starting, bool multiline = false)
        {
            LabelText = label;
            StartingText = starting;
            MultiLine = multiline;
            Value = StartingText;
        }

        public override bool Render()
        {
            GUILayout.Label(LabelText);

            var before = Value;

            Value = MultiLine ?
                GUILayout.TextArea(Value) :
                GUILayout.TextField(Value);

            return before != Value;
        }
    }

    public class MenuLabel : MenuItem, IMenuLabel
    {
        public string Text { get; set; }

        public MenuLabel(string label)
        {
            Text = label;
            UnderlyingValue = label;
        }

        public override bool Render()
        {
            GUILayout.Label(Text);
            return false;
        }
    }
}
