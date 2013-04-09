using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public abstract class GameScreen
    {
        public delegate void TransitionEventHandler(object sender, TransitionEventArgs e);

        public event TransitionEventHandler Entering;
        public event TransitionEventHandler Exiting;
        public event EventHandler Removing;

        private float _transitionDirection = 1f;
        private float _transitionMultiplier = 1f;
        private TransitionState _transitionState;
        private bool _isContentLoaded;
        private bool _isContentUnloaded;
        private bool _isInitialized;
        private Texture2D _fadeTexture;
        private Color _fadeColor;
        private float _fadeAmount;
        private bool _fadeIsEnabled;

        public TimeSpan TransitionOnTime { get; set; }

        public TimeSpan TransitionOffTime { get; set; }

        public float TransitionPercent { get; private set; }

        public float TransitionDirection
        {
            get { return _transitionDirection; }
        }

        public float TransitionMultiplier
        {
            get { return _transitionMultiplier; }
            set { _transitionMultiplier = value; }
        }

        public TransitionState TransitionState
        {
            get { return _transitionState; }
        }

        public virtual float ScreenAlpha
        {
            get { return 1f; }
        }

        public ScreenState State
        {
            get;
            protected set;
        }

        public bool IsContentLoaded
        {
            get { return _isContentLoaded; }
        }

        public bool IsContentUnloaded
        {
            get { return _isContentUnloaded; }
        }

        public bool IsActive
        {
            get
            {
                return State == ScreenState.TransitionOn
                    || State == ScreenState.TransitionOff
                    || State == ScreenState.Active;
            }
        }

        public ScreenManager ScreenManager { get; set; }

        public abstract bool AcceptsInput { get; }

        public InputMap InputMap { get; private set; }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            InputMap = new InputMap();
            _isInitialized = true;
        }

        public void Update(GameTime gameTime)
        {
            InputSystem.Update(gameTime);

            if (State == ScreenState.Frozen || State == ScreenState.Inactive)
            {
                return;
            }

            if (State == ScreenState.TransitionOn)
            {
                float currentTransitionTime = CalculateTransitionTime(TransitionOnTime, gameTime);

                TransitionPercent += currentTransitionTime * _transitionMultiplier;

                if (TransitionPercent >= 1.0)
                {
                    TransitionPercent = 1.0f;
                    State = ScreenState.Active;
                }
                else
                {
                    var handler = Entering;
                    if (handler != null)
                    {
                        handler(this, new TransitionEventArgs(TransitionPercent, currentTransitionTime));
                    }
                }
            }
            else if (State == ScreenState.TransitionOff)
            {
                float currentTransitionTime = CalculateTransitionTime(TransitionOffTime, gameTime);

                TransitionPercent -= currentTransitionTime * _transitionMultiplier;

                if (TransitionPercent <= 0)
                {
                    TransitionPercent = 0;
                    Remove();
                }
                else
                {
                    var handler = Exiting;
                    if (handler != null)
                    {
                        handler(this, new TransitionEventArgs(TransitionPercent, currentTransitionTime));
                    }
                }
            }
            else if (State == ScreenState.Active || State == ScreenState.Hidden)
            {
                UpdateScreen(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (State == ScreenState.Inactive || State == ScreenState.Hidden)
            {
                return;
            }

            var spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            if (_fadeIsEnabled)
            {
                Viewport view = ScreenManager.Viewport;
                spriteBatch.Draw(_fadeTexture, new Rectangle(0, 0, view.Width, view.Height), _fadeColor);
            }
            DrawScreen(gameTime);
            spriteBatch.End();
        }

        public void ExitScreen()
        {
            State = ScreenState.TransitionOff;
            _transitionDirection = -1;
        }

        public void FreezeScreen()
        {
            State = ScreenState.Frozen;
        }

        public void ActivateScreen()
        {
            if (State != ScreenState.Inactive && State != ScreenState.Active)
            {
                State = ScreenState.Active;
            }
        }

        public void EnableFade(Color c, float percentage)
        {
            percentage = MathHelper.Clamp(percentage, 0, 1);
            _fadeAmount = percentage;
            _fadeTexture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            _fadeTexture.SetData(new[] { c });
            _fadeColor = c * percentage;
            _fadeIsEnabled = true;
        }

        public void DisableFade()
        {
            _fadeTexture = null;
            _fadeColor = Color.White;
            _fadeIsEnabled = false;
        }

        public virtual void LoadContent()
        {
            _isContentLoaded = true;
        }

        public virtual void UnloadContent()
        {
            _isContentUnloaded = true;
        }

        public virtual void HandleInput()
        {
        }

        public abstract void InitializeScreen();

        protected abstract void UpdateScreen(GameTime gameTime);

        protected abstract void DrawScreen(GameTime gameTime);

        private float CalculateTransitionTime(TimeSpan transitionTime, GameTime gameTime)
        {
            if (transitionTime == TimeSpan.Zero)
            {
                return 1;
            }

            return (float)(gameTime.ElapsedGameTime.TotalSeconds / transitionTime.TotalSeconds);
        }

        private void Remove()
        {
            ScreenManager.RemoveScreen(this);
            var handler = Removing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}