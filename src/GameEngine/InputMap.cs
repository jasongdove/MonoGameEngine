using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine
{
    public class InputMap
    {
        private readonly Dictionary<string, List<Input>> _keybinds;

        public InputMap()
        {
            _keybinds = new Dictionary<string, List<Input>>();
        }

        public void NewAction(string action, Input input)
        {
            if (_keybinds.ContainsKey(action))
            {
                _keybinds[action].Add(input);
            }
            else
            {
                _keybinds.Add(action, new List<Input>(new[] { input }));
            }
        }

        public void NewAction(string action, Keys k)
        {
            var input = new Input { Key = k };
            NewAction(action, input);
        }

        public void NewAction(string action, Buttons b)
        {
            var input = new Input { Button = b };
            NewAction(action, input);
        }

        public void NewAction(string action, Triggers t)
        {
            if (t == Triggers.None)
            {
                return;
            }

            var input = new Input { Trigger = t };
            NewAction(action, input);
        }

        public void NewAction(string action, MousePresses m)
        {
            if (m == MousePresses.None)
            {
                return;
            }

            var input = new Input { MousePresses = m };
            NewAction(action, input);
        }

        public List<Input> GetKeybinds(string actionName)
        {
            return _keybinds[actionName];
        }

        public Vector2 GetMousePosition()
        {
            return InputSystem.MousePosition();
        }

        public float GetTriggerValue(Triggers t)
        {
            if (t == Triggers.LeftTrigger)
            {
                return InputSystem.LeftTrigger();
            }

            if (t == Triggers.RightTrigger)
            {
                return InputSystem.RightTrigger();
            }

            return 0.0f;
        }

        public bool ActionPressed(string actionName)
        {
            if (_keybinds.ContainsKey(actionName))
            {
                foreach (var input in _keybinds[actionName])
                {
                    float triggerValue = GetTriggerValue(input.Trigger);
                    if (InputSystem.IsPressedKey(input.Key)
                        || InputSystem.IsPressedButton(input.Button)
                        || InputSystem.IsPressedMouse(input.MousePresses)
                        || triggerValue > 0.200000002980232)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool NewActionPress(string actionName)
        {
            if (_keybinds.ContainsKey(actionName))
            {
                foreach (var input in _keybinds[actionName])
                {
                    float triggerValue = GetTriggerValue(input.Trigger);
                    if (InputSystem.IsNewKeyPress(input.Key)
                        || InputSystem.IsNewButtonPress(input.Button)
                        || InputSystem.IsNewMousePress(input.MousePresses)
                        || triggerValue > 0.200000002980232)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HeldAction(string actionName)
        {
            if (_keybinds.ContainsKey(actionName))
            {
                foreach (var input in _keybinds[actionName])
                {
                    float triggerValue = GetTriggerValue(input.Trigger);
                    if (InputSystem.IsHeldKey(input.Key)
                        || InputSystem.IsHeldButton(input.Button)
                        || InputSystem.IsHeldMousePress(input.MousePresses)
                        || triggerValue > 0.200000002980232)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public struct Input
        {
            public Keys Key { get; set; }
            public Buttons Button { get; set; }
            public Triggers Trigger { get; set; }
            public MousePresses MousePresses { get; set; }
        }
    }
}