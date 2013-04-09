using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameEngine
{
    public class DebugScreen : GameScreen
    {
        private readonly World _world;
        private readonly Rectangle _screenBounds;
        private DebugViewXNA _debug;

        public DebugScreen(World world, Rectangle screenBounds)
        {
            _world = world;
            _screenBounds = screenBounds;
        }

        public override bool AcceptsInput
        {
            get { return false; }
        }

        public override void InitializeScreen()
        {
        }

        public override void LoadContent()
        {
            _debug = new DebugViewXNA(_world);
            _debug.LoadContent(ScreenManager.GraphicsDevice, ScreenManager.Content);
            _debug.AppendFlags(FarseerPhysics.DebugViewFlags.Shape);
            _debug.AppendFlags(FarseerPhysics.DebugViewFlags.PolygonPoints);
        }

        protected override void UpdateScreen(GameTime gameTime)
        {
        }

        protected override void DrawScreen(GameTime gameTime)
        {
            // Debug
            Matrix proj = Matrix.CreateOrthographic(
                ConvertUnits.ToSimUnits(_screenBounds.Width),
                -ConvertUnits.ToSimUnits(_screenBounds.Height),
                0,
                1000000);
            var campos = new Vector3();
            campos.X = ConvertUnits.ToSimUnits(-_screenBounds.Width / 2f);
            campos.Y = ConvertUnits.ToSimUnits(-_screenBounds.Height / 2f);
            campos.Z = 0;
            Matrix tran = Matrix.Identity;
            tran.Translation = campos;
            Matrix view = tran;

            _debug.RenderDebugData(ref proj, ref view);
        }
    }
}