using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public abstract class MenuEntry
    {
        private EntryState _state = EntryState.Normal;
        private Dictionary<bool, Rectangle> _displayRegion;
        private bool _hasSubMenu;
        private Color _color = Color.White;
        private float _scale = 1.0f;
        private float _opacity = 1.0f;

        protected MenuEntry()
        {
            BoundingRectangle = new Rectangle(0, 0, 0, 0);
        }

        protected MenuEntry(MenuScreen screen, string title)
        {
            Color = screen.Normal;
            ParentMenu = screen;
            EntryTitle = title;
            BoundingRectangle = new Rectangle(0, 0, 0, 0);
        }

        protected MenuEntry(MenuScreen screen, Texture2D texture)
        {
            Color = screen.Normal;
            ParentMenu = screen;
            EntryTexture = texture;
            BoundingRectangle = new Rectangle(0, 0, 0, 0);
        }

        protected MenuEntry(MenuScreen screen, string title, Texture2D texture)
        {
            Color = screen.Normal;
            ParentMenu = screen;
            EntryTitle = title;
            EntryTexture = texture;
            BoundingRectangle = new Rectangle(0, 0, 0, 0);
        }

        public event EventHandler OnHighlight;
        public event EventHandler OnNormal;
        public event EventHandler Selected;

        public EntryState State
        {
            get { return _state; }
        }

        public string EntryTitle { get; set; }

        public string EntryDescription { get; set; }

        public Point EntryPadding { get; private set; }

        public Texture2D EntryTexture { get; private set; }

        public Rectangle BoundingRectangle { get; private set; }

        public Point NumberOfEntries { get; set; }

        public bool IsSheet
        {
            get { return NumberOfEntries.X == 1 && NumberOfEntries.Y == 1; }
        }

        public Vector2 DefaultPosition { get; private set; }

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; }

        public MenuScreen ParentMenu { get; set; }

        public MenuScreen SubMenu { get; private set; }

        public bool HasSubMenu
        {
            get { return _hasSubMenu; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public float Opacity
        {
            get { return _opacity; }
            set { _opacity = value; }
        }

        public void UpdateEntry(GameTime gameTime)
        {
            Update(gameTime);

            SpriteFont spriteFont = ParentMenu.SpriteFont;
            Vector2 measure = spriteFont == null ? Vector2.Zero : spriteFont.MeasureString(EntryTitle);
            var width = (int)(EntryTexture == null ? measure.X : EntryTexture.Width);
            var height = (int)(EntryTexture == null ? measure.Y : EntryTexture.Height);
            BoundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }

        public void AddSubMenu(MenuScreen subMenu)
        {
            _hasSubMenu = true;
            SubMenu = subMenu;
        }

        public void SetRelativePosition(Vector2 relativePosition, MenuEntry entry, bool defaultPosition)
        {
            var position = new Vector2(entry.Position.X, entry.Position.Y);
            if (entry.EntryTexture != null)
            {
                position.Y += entry.EntryTexture.Height;
            }

            SetPosition(Vector2.Add(position, relativePosition), defaultPosition);
        }

        public void SetRelativePosition(Vector2 relativeDefaultPosition, Vector2 relativeCurrentPosition, MenuEntry entry)
        {
            var defaultPosition = new Vector2(entry.DefaultPosition.X, entry.DefaultPosition.Y);
            var currentPosition = new Vector2(entry.Position.X, entry.Position.Y);
            if (entry.EntryTexture != null)
            {
                defaultPosition.Y += entry.EntryTexture.Height;
                currentPosition.Y += entry.EntryTexture.Height;
            }

            SetPosition(Vector2.Add(defaultPosition, relativeDefaultPosition), Vector2.Add(currentPosition, relativeCurrentPosition));
        }

        public void SetPosition(Vector2 position, bool defaultPosition)
        {
            if (defaultPosition)
            {
                DefaultPosition = position;
            }

            Position = position;
        }

        public void SetPosition(Vector2 defaultPosition, Vector2 current)
        {
            DefaultPosition = defaultPosition;
            Position = current;
        }

        public void AddTexture(Texture2D texture)
        {
            EntryTexture = texture;
            NumberOfEntries = new Point(1, 1);
        }

        public void AddTexture(Texture2D texture, int numX, int numY, Rectangle selectedEntry, Rectangle normalEntry)
        {
            EntryTexture = texture;
            NumberOfEntries = new Point(numX, numY);
            _displayRegion = new Dictionary<bool, Rectangle>();
            _displayRegion.Add(true, selectedEntry);
            _displayRegion.Add(false, normalEntry);
        }

        public void AddPadding(int all)
        {
            EntryPadding = new Point(all, all);
        }

        public void AddPadding(int left, int top)
        {
            EntryPadding = new Point(left, top);
        }

        public virtual void Highlight()
        {
            Color = ParentMenu.Highlighted;
            _state = EntryState.Highlight;
            var handler = OnHighlight;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public virtual void Normal()
        {
            Color = ParentMenu.Normal;
            _state = EntryState.Normal;
            var handler = OnNormal;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public virtual void Select()
        {
            if (SubMenu != null)
            {
                _state = EntryState.Selected;
                Color = ParentMenu.Selected;
                SubMenu.ActivateScreen();
                ParentMenu.ScreenManager.AddScreen(SubMenu);
                ParentMenu.FreezeScreen();
                ParentMenu.HideMouse();
                SubMenu.EnableMouse(ParentMenu.MouseTexture);
            }

            var handler = Selected;
            if (handler != null)
            {
                _state = EntryState.Selected;
                Color = ParentMenu.Selected;
                Selected(this, EventArgs.Empty);
            }
        }

        public virtual void Draw(GameTime gameTime, bool isSelected)
        {
            SpriteBatch spriteBatch = ParentMenu.ScreenManager.SpriteBatch;
            SpriteFont spriteFont = ParentMenu.SpriteFont;

            var entryPosition = new Vector2(Position.X, Position.Y);

            if (EntryTexture != null)
            {
                if (IsSheet)
                {
                    spriteBatch.Draw(EntryTexture, Position, Color.White * _opacity);
                }
                else if (_displayRegion != null)
                {
                    spriteBatch.Draw(EntryTexture, Position, _displayRegion[isSelected], Color.White * _opacity);
                }

                if (spriteFont != null && EntryTitle.Length > 0)
                {
                    Vector2 textDims = spriteFont.MeasureString(EntryTitle);

                    float x = EntryPadding.X == 0 ? EntryTexture.Width / 2f - textDims.X / 2f : EntryPadding.X;
                    float y = EntryPadding.Y == 0 ? EntryTexture.Height / 2f - textDims.Y / 2f : EntryPadding.Y;

                    entryPosition += new Vector2(x, y);

                    spriteBatch.DrawString(spriteFont, EntryTitle, entryPosition, _color * _opacity);
                }
            }
            else if (spriteFont != null && EntryTitle.Length > 0)
            {
                spriteBatch.DrawString(spriteFont, EntryTitle, entryPosition, _color * _opacity, 0, Vector2.Zero, _scale, SpriteEffects.None, 0.0f);
            }
        }

        public virtual void DrawDescription(GameTime gameTime, Rectangle boxPosition, Point padding, Color textColor, float scale)
        {
            SpriteBatch spriteBatch = ParentMenu.ScreenManager.SpriteBatch;
            SpriteFont spriteFont = ParentMenu.SpriteFont;

            if (spriteFont != null)
            {
                //Vector2 textDims = spriteFont.MeasureString(EntryDescription) * _scale;

                float x = padding.X;
                float y = padding.Y;

                var descriptionPosition = new Vector2(boxPosition.X + x, boxPosition.Y + y);
                spriteBatch.DrawString(spriteFont, EntryDescription, descriptionPosition, textColor, 0.0f, Vector2.Zero, _scale, SpriteEffects.None, 1.0f);
            }
        }

        public abstract void Update(GameTime gameTime);

        public abstract void AnimateHighlighted(GameTime gameTime);

        public enum EntryState
        {
            Normal,
            Highlight,
            Selected
        }
    }
}