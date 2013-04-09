using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public abstract class MenuScreen : GameScreen
    {
        protected MenuScreen()
        {
            Removing += MenuScreen_Removing;
        }

        public event EventHandler Cancel;

        public GameScreen Parent { get; set; }
        public Texture2D BackgroundTexture { get; set; }
        public Vector2 BackgroundPosition { get; set; }
        public MenuState MenuState { get; private set; }

        public override bool AcceptsInput
        {
            get { return true; }
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

        protected override void DrawScreen(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            if (BackgroundTexture != null)
            {
                spriteBatch.Draw(BackgroundTexture, BackgroundPosition, Color.White);
            }
        }

        private void MenuScreen_Removing(object sender, EventArgs e)
        {
            if (Parent != null && Parent.State == ScreenState.Frozen)
            {
                Parent.ActivateScreen();
                ////var ms = Parent as MenuScreen;
                ////if (ms != null && ms.MouseTexture != null)
                ////{
                ////    ms.ShowMouse();
                ////}
            }
        }
    }
}