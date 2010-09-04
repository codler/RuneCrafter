using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RuneCrafter
{
    class Input
    {
        Dictionary<string, string> sKey = new Dictionary<string, string>();
        public Dictionary<string, bool> isPressed = new Dictionary<string, bool>();

        public Vector2 mouse = new Vector2(0f,0f);
        public Vector2 prevMouse = new Vector2(0f, 0f);

        public void init(Dictionary<string,string> key)
        {
            sKey = key;
            foreach (KeyValuePair<string,string> kvp in sKey )
            {
                isPressed.Add(kvp.Value, false);
            }
        }

        public void update()
        {
            KeyboardState k = Keyboard.GetState();
            Keys[] key = k.GetPressedKeys();

            foreach (Keys item in key)
            {
                string gvString = "";
                bool gvBool = false;
                if (sKey.TryGetValue(item.ToString(), out gvString))
                {
                    //if (item.ToString() == sKey[item.ToString()])
                    //{
                    if (!isPressed.TryGetValue(sKey[item.ToString()], out gvBool))
                        {
                            isPressed.Add(sKey[item.ToString()], true);
                        }

                        isPressed[sKey[item.ToString()]] = true;
                      //  isPressed[item.ToString()] = true;
                    //}

                    

                }

            }

        }

        public bool IsPressed(string keyName)
        {
            bool gvBool = false;
            if (!isPressed.TryGetValue(keyName, out gvBool)) return false;

            bool pressed = isPressed[keyName];
            isPressed[keyName] = false;
            return pressed;
        }


        public Vector2 mouseMove()
        {
            Vector2 start = new Vector2(0f,0f);
            MouseState m = Mouse.GetState();
            
            // MouseName = RightButton
            if (m.RightButton == ButtonState.Pressed)
            {
                start = new Vector2(m.X, m.Y) - mouse;
                mouse = new Vector2(m.X, m.Y);
            }
            else if (m.RightButton == ButtonState.Released) 
            {
                mouse = new Vector2(m.X, m.Y);
            }

            
            //System.Diagnostics.Trace.WriteLine(start.Y);
            return start;
        }

        public float mouseScroll()
        {
            MouseState m = Mouse.GetState();
            //System.Diagnostics.Trace.WriteLine(m.ScrollWheelValue);
            return m.ScrollWheelValue / GameConstants.scrollSensitive;
            
        }

    }
}
