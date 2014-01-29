using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Isometric.GameComponents
{
    

    public class Input
    {
        public enum EMouseButton
        {
            LeftButton,
            MiddleButton,
            RightButton
        }

        private MouseState mousestate;
        private KeyboardState keyboardstate;

        private Point lastPosition;
        private Point currentPosition;

        private bool[] currentMouseButtonStates;
        private bool[] lastMouseButtonStates;

        private Keys[] currentPressedKeys;
        private Keys[] lastPressedKeys;

        private int currentScrollValue;
        private int lastScrollValue;

        public Input()
        {
            mousestate = Mouse.GetState();
            keyboardstate = Keyboard.GetState();

            lastPosition = new Point();
            currentPosition = new Point();

            currentMouseButtonStates = new bool[4];
            lastMouseButtonStates = new bool[4];

            currentPressedKeys = new Keys[0];
            lastPressedKeys = new Keys[0];

            currentScrollValue = mousestate.ScrollWheelValue;
            lastScrollValue = currentScrollValue;
        }

        public void Update()
        {
            mousestate = Mouse.GetState();
            keyboardstate = Keyboard.GetState();

            lastPosition = currentPosition;
            currentPosition = new Point(mousestate.X, mousestate.Y);

            for (int i = 0; i < currentMouseButtonStates.Length; i++)
            {
                lastMouseButtonStates[i] = currentMouseButtonStates[i];
            }

            currentMouseButtonStates[(int)EMouseButton.LeftButton] = mousestate.LeftButton == ButtonState.Pressed;
            currentMouseButtonStates[(int)EMouseButton.MiddleButton] = mousestate.MiddleButton == ButtonState.Pressed;
            currentMouseButtonStates[(int)EMouseButton.RightButton] = mousestate.RightButton == ButtonState.Pressed;

            lastScrollValue = currentScrollValue;
            currentScrollValue = mousestate.ScrollWheelValue;

            lastPressedKeys = currentPressedKeys;
            currentPressedKeys = keyboardstate.GetPressedKeys();
        }

        public bool hasMouseMoved()
        {
            return lastPosition != currentPosition;
        }

        public Point mousePosition()
        {
            return currentPosition;
        }

        public Vector2 mousePositionV2()
        {
            return new Vector2(currentPosition.X, currentPosition.Y);
        }

        public bool mouseButtonClicked(EMouseButton button)
        {
            return currentMouseButtonStates[(int)button] && !lastMouseButtonStates[(int)button];
        }
        public bool mouseButtonReleased(EMouseButton button)
        {
            return !currentMouseButtonStates[(int)button] && lastMouseButtonStates[(int)button];
        }
        public bool mouseButtonHold(EMouseButton button)
        {
            return currentMouseButtonStates[(int)button] && lastMouseButtonStates[(int)button];
        }
        public bool mouseButtonPressed(EMouseButton button)
        {
            return currentMouseButtonStates[(int)button];
        }
        
        private static bool contains<T>(T elem, T[] array)
        {
            foreach (T e in array)
            {
                if (e.Equals(elem))
                {
                    return true;
                }
            }
            return false;
        }

        public bool isKeyDown(Keys k)
        {
            return contains<Keys>(k, currentPressedKeys);
        }
        public bool wasKeyDown(Keys k)
        {
            return contains<Keys>(k, lastPressedKeys);
        }

        public bool keyClicked(Keys k)
        {
            return isKeyDown(k) && !wasKeyDown(k);
        }
        public bool keyReleased(Keys k)
        {
            return !isKeyDown(k) && wasKeyDown(k);
        }
        public bool keyHold(Keys k)
        {
            return isKeyDown(k) && wasKeyDown(k);
        }
        public bool keyPressed(Keys k)
        {
            return isKeyDown(k);
        }

        public bool hasScrolled()
        {
            return scrollChange() != 0;
        }

        public bool scrolledUp()
        {
            return scrollChange() > 0;
        }

        public bool scrolledDown()
        {
            return scrollChange() < 0;
        }

        public int scrollChange()
        {
            return currentScrollValue - lastScrollValue;
        }
    }
}
