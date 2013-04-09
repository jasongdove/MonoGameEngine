using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine
{
    public class InputSystem
    {
        public InputSystem()
        {
            CurrentKeyboardState = new KeyboardState();
            PreviousKeyboardState = new KeyboardState();
            CurrentGamepadState = new GamePadState();
            PreviousGamepadState = new GamePadState();
            CurrentMouseState = new MouseState();
            PreviousMouseState = new MouseState();
        }

        public static KeyboardState CurrentKeyboardState { get; private set; }
        public static KeyboardState PreviousKeyboardState { get; private set; }
        public static GamePadState CurrentGamepadState { get; private set; }
        public static GamePadState PreviousGamepadState { get; private set; }
        public static MouseState CurrentMouseState { get; private set; }
        public static MouseState PreviousMouseState { get; private set; }

        public static bool IsPressedKey(Keys k)
        {
            return CurrentKeyboardState.IsKeyDown(k);
        }

        public static bool IsNewKeyPress(Keys k)
        {
            return CurrentKeyboardState.IsKeyDown(k) && PreviousKeyboardState.IsKeyUp(k);
        }

        public static bool IsHeldKey(Keys k)
        {
            return CurrentKeyboardState.IsKeyDown(k) && PreviousKeyboardState.IsKeyDown(k);
        }

        public static bool IsPressedButton(Buttons b)
        {
            return b != 0 && CurrentGamepadState.IsButtonDown(b);
        }

        public static bool IsNewButtonPress(Buttons b)
        {
            return b != 0 && CurrentGamepadState.IsButtonDown(b) && PreviousGamepadState.IsButtonUp(b);
        }

        public static bool IsHeldButton(Buttons b)
        {
            return b != 0 && CurrentGamepadState.IsButtonDown(b) && PreviousGamepadState.IsButtonDown(b);
        }

        public static bool IsPressedMouse(MousePresses m)
        {
            if (m == MousePresses.LeftMouse)
            {
                return CurrentMouseState.LeftButton == ButtonState.Pressed;
            }

            if (m == MousePresses.RightMouse)
            {
                return CurrentMouseState.RightButton == ButtonState.Pressed;
            }

            return false;
        }

        public static bool IsNewMousePress(MousePresses m)
        {
            if (m == MousePresses.LeftMouse)
            {
                return CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;
            }

            if (m == MousePresses.RightMouse)
            {
                return CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released;
            }

            return false;
        }

        public static bool IsHeldMousePress(MousePresses m)
        {
            if (m == MousePresses.LeftMouse)
            {
                return CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Pressed;
            }

            if (m == MousePresses.RightMouse)
            {
                return CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Pressed;
            }

            return false;
        }

        public static float LeftTrigger()
        {
            return CurrentGamepadState.Triggers.Left;
        }

        public static float RightTrigger()
        {
            return CurrentGamepadState.Triggers.Right;
        }

        public static Vector2 LeftThumbStick()
        {
            return CurrentGamepadState.ThumbSticks.Left;
        }

        public static Vector2 MousePosition()
        {
            return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
        }

        public static void Update(GameTime gameTime)
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            PreviousGamepadState = CurrentGamepadState;
            CurrentGamepadState = GamePad.GetState(PlayerIndex.One);
        }
    }
}