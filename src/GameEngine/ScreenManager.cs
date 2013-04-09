using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public sealed class ScreenManager : DrawableGameComponent
    {
        private readonly List<GameScreen> _screens = new List<GameScreen>();
        private readonly List<GameScreen> _screensToUpdate = new List<GameScreen>();
        private readonly bool _isInitialized;

        public ScreenManager(Game game)
            : base(game)
        {
            Initialize();
            _isInitialized = true;
        }

        public SpriteBatch SpriteBatch { get; private set; }

        public ContentManager Content
        {
            get { return Game.Content; }
        }

        public Viewport Viewport
        {
            get { return GraphicsDevice.Viewport; }
        }

        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;

            if (_isInitialized)
            {
                screen.Initialize();
                screen.InitializeScreen();
                screen.LoadContent();
            }

            _screens.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            if (_isInitialized && !screen.IsContentUnloaded)
            {
                screen.UnloadContent();
            }

            _screens.Remove(screen);
        }

        public override void Update(GameTime gameTime)
        {
            _screensToUpdate.Clear();
            if (_screens.Count == 0)
            {
                Game.Exit();
            }

            _screensToUpdate.AddRange(_screens);

            if (!Game.IsActive)
            {
                return;
            }

            while (_screensToUpdate.Count > 0)
            {
                // TODO: Stack?
                var screen = _screensToUpdate[_screensToUpdate.Count - 1];
                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                if (screen.State != ScreenState.Frozen && screen.State != ScreenState.Inactive)
                {
                    screen.Update(gameTime);
                }

                if (screen.IsActive && screen.AcceptsInput)
                {
                    screen.HandleInput();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Game.IsActive)
            {
                return;
            }

            foreach (var screen in _screens)
            {
                if (screen.State != ScreenState.Hidden)
                {
                    screen.Draw(gameTime);
                }
            }
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            foreach (var screen in _screens)
            {
                if (!screen.IsContentLoaded)
                {
                    screen.LoadContent();
                }
            }
        }

        protected override void UnloadContent()
        {
            foreach (var screen in _screens)
            {
                if (!screen.IsContentUnloaded)
                {
                    screen.UnloadContent();
                }
            }
        }
    }
}