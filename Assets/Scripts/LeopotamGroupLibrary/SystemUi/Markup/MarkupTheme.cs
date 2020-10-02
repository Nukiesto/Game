// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2019 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.EditorHelpers;
using UnityEngine;

namespace LeopotamGroup.SystemUi.Markup
{
    public sealed class MarkupTheme : ScriptableObject
    {
        [SerializeField]
        private string _name = "NewMarkupTheme";

        [SerializeField]
        private Sprite _buttonNormalSprite;

        [SerializeField]
        private Sprite _buttonPressedSprite;

        [SerializeField]
        private Sprite _buttonHighlightedSprite;

        [SerializeField]
        private Sprite _buttonDisabledSprite;

        [HtmlColor]
        [SerializeField]
        private Color _buttonNormalColor = Color.white;

        [HtmlColor]
        [SerializeField]
        private Color _buttonPressedColor = Color.white;

        [HtmlColor]
        [SerializeField]
        private Color _buttonHighlightedColor = Color.white;

        [HtmlColor]
        [SerializeField]
        private Color _buttonDisabledColor = Color.gray;

        [SerializeField]
        private Sprite _sliderBackgroundSprite;

        [SerializeField]
        private Sprite _sliderForegroundSprite;

        [SerializeField]
        private Sprite _sliderHandleSprite;

        [HtmlColor]
        [SerializeField]
        private Color _sliderBackgroundColor = Color.gray;

        [HtmlColor]
        [SerializeField]
        private Color _sliderForegroundColor = Color.white;

        [HtmlColor]
        [SerializeField]
        private Color _sliderHandleColor = Color.white;

        [SerializeField]
        private Sprite _toggleBackgroundSprite;

        [SerializeField]
        private Sprite _toggleForegroundSprite;

        [HtmlColor]
        [SerializeField]
        private Color _toggleBackgroundColor = Color.white;

        [HtmlColor]
        [SerializeField]
        private Color _toggleForegroundColor = Color.white;

        [SerializeField]
        private Vector2 _toggleBackgroundSize = Vector2.one * 24f;

        [SerializeField]
        private Vector2 _toggleForegroundSize = Vector2.one * 24f;

        [SerializeField]
        private Sprite _scrollbarBackgroundSprite;

        [SerializeField]
        private Sprite _scrollbarHandleSprite;

        [HtmlColor]
        [SerializeField]
        private Color _scrollbarBackgroundColor = Color.gray;

        [HtmlColor]
        [SerializeField]
        private Color _scrollbarHandleColor = Color.white;

        [SerializeField]
        private float _scrollbarWidth = 16f;

        [SerializeField]
        private Sprite _inputBackgroundSprite;

        [HtmlColor]
        [SerializeField]
        private Color _inputBackgroundColor = Color.white;

        [HtmlColor]
        [SerializeField]
        private Color _inputPlaceholderColor = Color.gray;

        [HtmlColor]
        [SerializeField]
        private Color _inputSelectionColor = new Color32(168, 206, 255, 192);

        [SerializeField]
        private FontStyle _inputPlaceholderStyle = FontStyle.Italic;

        [SerializeField]
        private float _inputMargin = 10f;

        public enum ButtonState
        {
            Normal,
            Pressed,
            Highlighted,
            Disabled
        }

        public enum SliderState
        {
            Background,
            Foreground,
            Handle
        }

        public enum ToggleState
        {
            Background,
            Foreground
        }

        public enum ScrollbarState
        {
            Background,
            Handle
        }

        public enum InputState
        {
            Background,
            Placeholder,
            Selection
        }

        public string GetName()
        {
            return _name;
        }

        public Sprite GetButtonSprite(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Normal:
                    return _buttonNormalSprite;

                case ButtonState.Pressed:
                    return _buttonPressedSprite;

                case ButtonState.Highlighted:
                    return _buttonHighlightedSprite;

                case ButtonState.Disabled:
                    return _buttonDisabledSprite;

                default:
                    return null;
            }
        }

        public Color GetButtonColor(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Normal:
                    return _buttonNormalColor;

                case ButtonState.Pressed:
                    return _buttonPressedColor;

                case ButtonState.Highlighted:
                    return _buttonHighlightedColor;

                case ButtonState.Disabled:
                    return _buttonDisabledColor;

                default:
                    return Color.black;
            }
        }

        public Sprite GetSliderSprite(SliderState state)
        {
            switch (state)
            {
                case SliderState.Background:
                    return _sliderBackgroundSprite;

                case SliderState.Foreground:
                    return _sliderForegroundSprite;

                case SliderState.Handle:
                    return _sliderHandleSprite;

                default:
                    return null;
            }
        }

        public Color GetSliderColor(SliderState state)
        {
            switch (state)
            {
                case SliderState.Background:
                    return _sliderBackgroundColor;

                case SliderState.Foreground:
                    return _sliderForegroundColor;

                case SliderState.Handle:
                    return _sliderHandleColor;

                default:
                    return Color.black;
            }
        }

        public Sprite GetToggleSprite(ToggleState state)
        {
            switch (state)
            {
                case ToggleState.Background:
                    return _toggleBackgroundSprite;

                case ToggleState.Foreground:
                    return _toggleForegroundSprite;

                default:
                    return null;
            }
        }

        public Color GetToggleColor(ToggleState state)
        {
            switch (state)
            {
                case ToggleState.Background:
                    return _toggleBackgroundColor;

                case ToggleState.Foreground:
                    return _toggleForegroundColor;

                default:
                    return Color.black;
            }
        }

        public Vector2 GetToggleSize(ToggleState state)
        {
            switch (state)
            {
                case ToggleState.Background:
                    return _toggleBackgroundSize;

                case ToggleState.Foreground:
                    return _toggleForegroundSize;

                default:
                    return Vector2.zero;
            }
        }

        public Sprite GetScrollbarSprite(ScrollbarState state)
        {
            switch (state)
            {
                case ScrollbarState.Background:
                    return _scrollbarBackgroundSprite;

                case ScrollbarState.Handle:
                    return _scrollbarHandleSprite;

                default:
                    return null;
            }
        }

        public Color GetScrollbarColor(ScrollbarState state)
        {
            switch (state)
            {
                case ScrollbarState.Background:
                    return _scrollbarBackgroundColor;

                case ScrollbarState.Handle:
                    return _scrollbarHandleColor;

                default:
                    return Color.black;
            }
        }

        public float GetScrollbarWidth()
        {
            return _scrollbarWidth;
        }

        public Sprite GetInputSprite()
        {
            return _inputBackgroundSprite;
        }

        public Color GetInputColor(InputState state)
        {
            switch (state)
            {
                case InputState.Background:
                    return _inputBackgroundColor;

                case InputState.Placeholder:
                    return _inputPlaceholderColor;

                case InputState.Selection:
                    return _inputSelectionColor;

                default:
                    return Color.black;
            }
        }

        public float GetInputMargin()
        {
            return _inputMargin;
        }

        public FontStyle GetInputPlaceholderStyle()
        {
            return _inputPlaceholderStyle;
        }
    }
}