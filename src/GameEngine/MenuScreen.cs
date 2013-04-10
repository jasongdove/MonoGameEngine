using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public abstract class MenuScreen : GameScreen
    {
        private readonly List<MenuEntry> _menuEntries = new List<MenuEntry>();
        private bool _mouseVisible = false;
        private float _titleOpacity = 1.0f;
        private Color _highlighted = Color.White;
        private Color _selected = Color.White;
        private Color _normal = Color.White;
        private Texture2D _descriptionTexture;
        private Color _descriptionColor;
        private Color _descriptionBoxColor;
        private float _descriptionScale;
        private int _selectedEntry = 0;

        protected MenuScreen()
        {
            Removing += MenuScreen_Removing;
        }

        public event EventHandler Cancel;

        public GameScreen Parent { get; set; }

        public SpriteFont SpriteFont { get; set; }

        public Texture2D BackgroundTexture { get; set; }

        public Vector2 BackgroundPosition { get; set; }

        public Texture2D TitleTexture { get; set; }

        public Vector2 InitialTitlePosition { get; set; }

        public Vector2 TitlePosition { get; set; }

        public Texture2D MouseTexture { get; set; }

        public Rectangle MouseBounds { get; private set; }

        public float TitleOpacity
        {
            get { return _titleOpacity; }
            set { _titleOpacity = value; }
        }

        public Vector2 Position { get; set; }

        public Color Highlighted
        {
            get { return _highlighted; }
            set { _highlighted = value; }
        }

        public Color Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        public Color Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }

        public Rectangle DescriptionPosition { get; private set; }

        public Point DescriptionPadding { get; set; }

        public MenuState MenuState { get; private set; }

        public List<MenuEntry> MenuEntries
        {
            get { return _menuEntries; }
        }

        public override bool AcceptsInput
        {
            get { return true; }
        }

        public abstract string PreviousEntryActionName { get; }
        public abstract string NextEntryActionName { get; }
        public abstract string SelectedEntryActionName { get; }
        public abstract string MenuCancelActionName { get; }

        public void SetDescriptionArea(Rectangle position, Color boxColor, Color textColor, float scale)
        {
            SetDescriptionArea(position, boxColor, textColor, new Point(0, 0), scale);
        }

        public void SetDescriptionArea(Rectangle position, Color boxColor, Color textColor, Point padding, float scale)
        {
            DescriptionPosition = position;
            _descriptionTexture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            _descriptionTexture.SetData(new[] { boxColor });
            _descriptionBoxColor = boxColor;
            _descriptionColor = textColor;
            DescriptionPadding = padding;
            _descriptionScale = scale;
        }

        public void EnableMouse(Texture2D texture)
        {
            if (texture != null)
            {
                MouseTexture = texture;
                _mouseVisible = true;
            }
        }

        public void ShowMouse()
        {
            _mouseVisible = true;
        }

        public void HideMouse()
        {
            _mouseVisible = false;
        }

        public virtual void MenuCancel()
        {
            ExitScreen();
            var handler = Cancel;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public override void HandleInput()
        {
            if (InputMap.NewActionPress(PreviousEntryActionName))
            {
                _menuEntries[_selectedEntry].Normal();
                _selectedEntry--;
                if (_selectedEntry < 0)
                {
                    _selectedEntry = _menuEntries.Count - 1;
                }
                _menuEntries[_selectedEntry].Highlight();
            }

            if (InputMap.NewActionPress(NextEntryActionName))
            {
                _menuEntries[_selectedEntry].Normal();
                _selectedEntry++;
                if (_selectedEntry >= _menuEntries.Count)
                {
                    _selectedEntry = 0;
                }
                _menuEntries[_selectedEntry].Highlight();
            }

            if (MouseTexture != null && _mouseVisible)
            {
                for (int i = _menuEntries.Count - 1; i >= 0; i--)
                {
                    if (_menuEntries[i].BoundingRectangle.Intersects(MouseBounds))
                    {
                        _menuEntries[_selectedEntry].Normal();
                        _selectedEntry = i;
                        _menuEntries[_selectedEntry].Highlight();
                    }
                }
            }

            if (InputMap.NewActionPress(SelectedEntryActionName))
            {
                _menuEntries[_selectedEntry].Select();
            }

            if (InputMap.NewActionPress(MenuCancelActionName))
            {
                MenuCancel();
            }
        }

        public abstract override void InitializeScreen();
        public abstract override void LoadContent();

        protected override void UpdateScreen(GameTime gameTime)
        {
            if (_menuEntries.Count == 0)
            {
                return;
            }

            if (_menuEntries[_selectedEntry].State != MenuEntry.EntryState.Highlight)
            {
                _menuEntries[_selectedEntry].Highlight();
            }

            for (int i = 0; i < _menuEntries.Count; i++)
            {
                _menuEntries[i].UpdateEntry(gameTime);
                _menuEntries[_selectedEntry].AnimateHighlighted(gameTime);
            }

            if (MouseTexture != null)
            {
                Vector2 mousePos = InputMap.GetMousePosition();
                MouseBounds = new Rectangle((int)mousePos.X, (int)mousePos.Y, MouseTexture.Width, MouseTexture.Height);
            }
        }

        protected override void DrawScreen(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            
            if (BackgroundTexture != null)
            {
                spriteBatch.Draw(BackgroundTexture, BackgroundPosition, Color.White);
            }

            if (TitleTexture != null)
            {
                spriteBatch.Draw(TitleTexture, TitlePosition, Color.White * _titleOpacity);
            }

            for (int i = 0; i < _menuEntries.Count; i++)
            {
                _menuEntries[i].Draw(gameTime, i == _selectedEntry);
            }

            DrawDescriptionArea(spriteBatch, gameTime);
            DrawMouse(spriteBatch, gameTime);
        }

        private void MenuScreen_Removing(object sender, EventArgs e)
        {
            _menuEntries.Clear();
            if (Parent != null && Parent.State == ScreenState.Frozen)
            {
                Parent.ActivateScreen();
                var ms = Parent as MenuScreen;
                if (ms != null && ms.MouseTexture != null)
                {
                    ms.ShowMouse();
                }
            }
        }

        private void DrawDescriptionArea(SpriteBatch spriteBatch, GameTime gameTime)
        {
            String s = _menuEntries[_selectedEntry].EntryDescription;
            if (!String.IsNullOrEmpty(s) && _descriptionTexture != null)
            {
                spriteBatch.Draw(_descriptionTexture, DescriptionPosition, _descriptionBoxColor);
                _menuEntries[_selectedEntry].DrawDescription(
                    gameTime,
                    DescriptionPosition,
                    DescriptionPadding,
                    _descriptionColor,
                    _descriptionScale);
            }
        }

        private void DrawMouse(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (MouseTexture != null && _mouseVisible)
            {
                spriteBatch.Draw(MouseTexture, InputMap.GetMousePosition(), Color.White);
            }
        }
    }
}