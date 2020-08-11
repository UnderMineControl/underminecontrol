using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnderMineControl.Menu
{
    using API;
    using API.MenuItems;

    public class MenuUtility : IMenu
    {
        private const int DEFAULT_HEIGHT = 350;
        private const int DEFAULT_WIDTH = 250;

        private int? _id;
        public string Text { get; set; } = "Hello world";
        public Rect WindowLocation { get; private set; }
        public Vector2 ScrollLocation { get; private set; }
        public GUISkin Skin { get; private set; }
        public bool Show { get; set; } = false;
        public int Id => (_id ?? (_id = GetHashCode())).Value;

        private Texture2D _background = null;
        private Texture2D _boarder = null;
        private Color? _textColor = null;
        private List<MenuItem> _items = new List<MenuItem>();
        private IResourceUtility _resources;

        public IMenuItem[] Items => _items.Cast<IMenuItem>().ToArray();

        public MenuUtility(IResourceUtility resource)
        {
            _resources = resource;
            WindowLocation = new Rect
            (
                Screen.width - (DEFAULT_WIDTH * 2),
                (Screen.height / 2) - (DEFAULT_HEIGHT / 2),
                DEFAULT_WIDTH, 
                DEFAULT_HEIGHT
            );
        }

        public IMenu AddItem(IMenuItem item)
        {
            if (!(item is MenuItem mi))
                throw new InvalidCastException("item isn't of type MenuItem");

            _items.Add(mi);
            return this;
        }

        public IMenu SetPosition(float x, float y)
        {
            WindowLocation = new Rect
            (
                x,
                y,
                WindowLocation.width,
                WindowLocation.height
            );
            return this;
        }

        public IMenu SetSize(float width, float height)
        {
            WindowLocation = new Rect
            (
                WindowLocation.x,
                WindowLocation.y,
                width,
                height
            );
            return this;
        }

        public IMenu AddTextBox(string label, Action<string, IMenuTextBox> onChange, string startingText = "", bool multiLine = false)
        {
            return AddTextBox(label, onChange, out _, startingText, multiLine);
        }

        public IMenu AddTextBox(string label, Action<string, IMenuTextBox> onChange, out IMenuTextBox control, string startingText = "", bool multiLine = false)
        {
            var item = new MenuTextBox(label, startingText, multiLine);
            item.OnChange = (v) => onChange((string)v, item);
            _items.Add(item);
            control = item;
            return this;
        }

        public IMenu AddCheckBox(string label, Action<bool, IMenuCheckBox> onChange, bool starting = false)
        {
            return AddCheckBox(label, onChange, out _, starting);
        }

        public IMenu AddCheckBox(string label, Action<bool, IMenuCheckBox> onChange, out IMenuCheckBox control, bool starting = false)
        {
            var item = new MenuCheckBox(label, starting);
            item.OnChange = (v) => onChange((bool)v, item);
            _items.Add(item);
            control = item;
            return this;
        }

        public IMenu AddButton(string text, Action<IMenuButton> onClick)
        {
            return AddButton(text, onClick, out _);
        }

        public IMenu AddButton(string text, Action<IMenuButton> onClick, out IMenuButton control)
        {
            var item = new MenuButton(text);
            item.OnChange = (v) => onClick(item);
            _items.Add(item);
            control = item;
            return this;
        }

        public IMenu AddLabel(string text)
        {
            return AddLabel(text, out _);
        }

        public IMenu AddLabel(string text, out IMenuLabel control)
        {
            var item = new MenuLabel(text);
            _items.Add(item);
            control = item;
            return this;
        }

        public IMenu SetSkin(Texture2D background = null, Texture2D boarder = null, Color? textColor = null)
        {
            _background = background;
            _boarder = boarder;
            _textColor = textColor;

            return this;
        }

        public IMenu SetDefaultSkin()
        {
            _textColor = Color.white;
            return this;
        }

        public void Render()
        {
            if (!Show)
                return;

            PatchSkin();
            WindowLocation = GUILayout.Window(Id, WindowLocation, WindowFunc, Text);
        }

        private void PatchSkin()
        {
            if (Skin != null)
            {
                GUI.skin = Skin;
                return;
            }

            if (_background == null &&
                _boarder == null &&
                _textColor == null)
                return;

            try
            {
                var textColorAct = _textColor ?? Color.white;

                var newSkin = UnityEngine.Object.Instantiate(GUI.skin);
                UnityEngine.Object.DontDestroyOnLoad(newSkin);

                _background = _background ?? _resources.LoadTexture(_resources.GetEmbeddedResource("UnderMineBackground.png"));
                UnityEngine.Object.DontDestroyOnLoad(_background);

                newSkin.box.onNormal.background = null;
                newSkin.box.normal.background = _background;
                newSkin.box.normal.textColor = textColorAct;

                _boarder = _boarder ?? _resources.LoadTexture(_resources.GetEmbeddedResource("UnderMineWindow.png"));
                UnityEngine.Object.DontDestroyOnLoad(_boarder);

                newSkin.window.onNormal.background = null;
                newSkin.window.normal.background = _boarder;
                newSkin.window.padding = new RectOffset(6, 6, 22, 6);
                newSkin.window.border = new RectOffset(10, 10, 20, 10);
                newSkin.window.normal.textColor = textColorAct;

                newSkin.button.padding = new RectOffset(4, 4, 3, 3);
                newSkin.button.normal.textColor = textColorAct;
                newSkin.textField.normal.textColor = textColorAct;
                newSkin.label.normal.textColor = textColorAct;

                Skin = newSkin;
                GUI.skin = Skin;
            }
            catch (Exception ex)
            {
                Debug.LogError("Error occurred while creating skin, using fallback: " + ex);
                Skin = null;
            }
        }

        private void WindowFunc(int id)
        {
            var changed = new List<MenuItem>();

            GUILayout.BeginVertical();
            ScrollLocation = GUILayout.BeginScrollView(ScrollLocation, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUI.skin.textArea);

            foreach (var item in _items)
            {
                if (item.Render())
                    changed.Add(item);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUI.DragWindow();

            foreach (var item in changed)
                item.OnChange(item.UnderlyingValue);
        }
    }
}
