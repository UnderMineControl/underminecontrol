using System;
using UnityEngine;

namespace UnderMineControl.API
{
    using MenuItems;

    public interface IMenu
    {
        int Id { get; }
        string Text { get; set; }
        Rect WindowLocation { get; }
        Vector2 ScrollLocation { get; }
        bool Show { get; set; }

        IMenuItem[] Items { get; }

        IMenu AddItem(IMenuItem item);

        IMenu AddTextBox(string label, Action<string, IMenuTextBox> onChange, string startingText = "", bool multiline = false);

        IMenu AddTextBox(string label, Action<string, IMenuTextBox> onChange, out IMenuTextBox control, string startingText = "", bool multiLine = false);

        IMenu AddCheckBox(string label, Action<bool, IMenuCheckBox> onChange, bool starting = false);

        IMenu AddCheckBox(string label, Action<bool, IMenuCheckBox> onChange, out IMenuCheckBox control, bool starting = false);

        IMenu AddButton(string text, Action<IMenuButton> onClick);

        IMenu AddButton(string text, Action<IMenuButton> onClick, out IMenuButton control);

        IMenu AddLabel(string text);

        IMenu AddLabel(string text, out IMenuLabel control);

        IMenu SetSkin(Texture2D background = null, Texture2D boarder = null, Color? textColor = null);

        IMenu SetDefaultSkin();

        IMenu SetPosition(float x, float y);

        IMenu SetSize(float width, float height);

        void Render();
    }
}
