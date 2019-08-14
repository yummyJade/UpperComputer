using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpperComputer2.Core;

namespace UpperComputer2
{
    class XInputController
    {
        Controller controller;
        Gamepad gamepad;
        public bool connected = false;
        public int deadband = 2500;
        public mypoint leftThumb, rightThumb = null;
        public float leftTrigger, rightTrigger;

        public XInputController()
        {
            controller = new Controller(UserIndex.One);
            //controller = new Controller(UserIndex.Two);
            connected = controller.IsConnected;
            leftThumb = new mypoint(0, 0);
            rightThumb = new mypoint(0, 0);
        }
        // Call this method to update all class values
        public void Update()
        {
            if (!connected)
                return;

            gamepad = controller.GetState().Gamepad;
            leftThumb = new mypoint(0, 0);
            rightThumb = new mypoint(0, 0); 
            leftThumb.X = (Math.Abs((float)gamepad.LeftThumbX) < deadband) ? 0 : (float)gamepad.LeftThumbX / short.MinValue * -100;
            leftThumb.Y = (Math.Abs((float)gamepad.LeftThumbY) < deadband) ? 0 : (float)gamepad.LeftThumbY / short.MaxValue * 100;
            rightThumb.Y = (Math.Abs((float)gamepad.RightThumbX) < deadband) ? 0 : (float)gamepad.RightThumbX / short.MaxValue * 100;
            rightThumb.X = (Math.Abs((float)gamepad.RightThumbY) < deadband) ? 0 : (float)gamepad.RightThumbY / short.MaxValue * 100;

            leftTrigger = gamepad.LeftTrigger;
            rightTrigger = gamepad.RightTrigger;
           
        }

        /// <summary>
        /// 读取方向键信息
        /// </summary>
        /// <returns></returns>
        public void GetDirection(out float tLeft, out float tRight, out mypoint pLeft,out mypoint pRight,out string returnFlag)
        {
            pLeft = new mypoint(0, 0);
            pRight = new mypoint(0, 0);
            if (!controller.IsConnected)
                returnFlag =  null;
            gamepad = controller.GetState().Gamepad;

            leftThumb.X = (Math.Abs((float)gamepad.LeftThumbX) < deadband) ? 0 : (float)gamepad.LeftThumbX / short.MinValue * -100;
            leftThumb.Y = (Math.Abs((float)gamepad.LeftThumbY) < deadband) ? 0 : (float)gamepad.LeftThumbY / short.MaxValue * 100;
            rightThumb.Y = (Math.Abs((float)gamepad.RightThumbX) < deadband) ? 0 : (float)gamepad.RightThumbX /  short.MinValue* -100;
            rightThumb.X = (Math.Abs((float)gamepad.RightThumbY) < deadband) ? 0 : (float)gamepad.RightThumbY / short.MaxValue * 100;
           pLeft = leftThumb;
            pRight = rightThumb;
            leftTrigger = (float)Math.Ceiling((float)gamepad.LeftTrigger/256 * 90);
            rightTrigger = (float)Math.Ceiling((float)gamepad.RightTrigger / 256 * 90);
            tLeft = leftTrigger;
            tRight = rightTrigger;
            

            GamepadButtonFlags flag = gamepad.Buttons;
            int resultStart = ((int)flag) & 0x10;
            int resultBack = ((int)flag) & 0x20;
            int resultUp = ((int)flag) & 0x01;
            int resultDown = ((int)flag) & 0x02;
            int resultLeft = ((int)flag) & 0x04;
            int resultRight = ((int)flag) & 0x08;
            int resultLS = ((int)flag) & 0x40;
            int resultRS = ((int)flag) & 0x80;
            int resultLB = ((int)flag) & 0x100;
            int resultRB = ((int)flag) & 0x200;
            int resultA = ((int)flag) & 0x1000;
            int resultB = ((int)flag) & 0x2000;
            int resultX = ((int)flag) & 0x4000;
            int resultY = ((int)flag) & 0x8000;

            // int result
            if (resultStart != 0)
                returnFlag = "start";
            else if (resultBack != 0)
                returnFlag = "back";
            else if (resultUp != 0)
                returnFlag = "up";
            else if (resultDown != 0)
                returnFlag = "down";
            else if (resultLeft != 0)
                returnFlag = "left";
            else if (resultRight != 0)
                returnFlag = "right";
            else if (resultLS != 0)
                returnFlag = "LS";
            else if (resultRS != 0)
                returnFlag = "RS";
            else if (resultLB != 0)
                returnFlag = "OL";
            else if (resultRB != 0)
                returnFlag = "OR";
            else if (resultA != 0)
                returnFlag = "sa";
            else if (resultB != 0)
                returnFlag = "sr";
            else if (resultX != 0)
                returnFlag = "st";
            else if (resultY != 0)
                returnFlag = "li";
            else
                returnFlag = flag.ToString();

        }

     
    }
}
