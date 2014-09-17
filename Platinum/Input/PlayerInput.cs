using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Platinum
{
	public class PlayerInput
	{
		public BitField32 down = 0;
		public BitField32 last = 0;
		public BitField32 press = 0;
		public BitField32 release = 0;

		const int maxButtons = (int)Button.END;

		public Dictionary<Keys, Button> mapKeys = new Dictionary<Keys, Button>();
		public Dictionary<Buttons, Button> mapPad = new Dictionary<Buttons, Button>();
		public int padNum = -1;

		const float stickThreshold = 0.5f;

		public void Update()
		{
			last = down;
			down = 0;

			// determine by stuff
			KeyboardState keys = Keyboard.GetState();
			foreach (KeyValuePair<Keys, Button> kvp in mapKeys)
			{
				if (keys.IsKeyDown(kvp.Key)) down[(int)kvp.Value] = true;
			}

			if (padNum > -1 && padNum < 4)
			{
				GamePadState pad = GamePad.GetState((PlayerIndex)padNum);

				// stick/dpad input
				Vector2 lstick = pad.ThumbSticks.Left;

				if (lstick.Y >= stickThreshold) down[(int)Button.Up] = true;
				if (lstick.Y <= -stickThreshold) down[(int)Button.Down] = true;
				if (lstick.X <= -stickThreshold) down[(int)Button.Left] = true;
				if (lstick.X >= stickThreshold) down[(int)Button.Right] = true;

				if (pad.DPad.Up == ButtonState.Pressed) down[(int)Button.Up] = true;
				if (pad.DPad.Down == ButtonState.Pressed) down[(int)Button.Down] = true;
				if (pad.DPad.Left == ButtonState.Pressed) down[(int)Button.Left] = true;
				if (pad.DPad.Right == ButtonState.Pressed) down[(int)Button.Right] = true;

				foreach (KeyValuePair<Buttons, Button> kvp in mapPad)
				{
					if (pad.IsButtonDown(kvp.Key)) down[(int)kvp.Value] = true;
				}
			}

			press = down & (last ^ UInt32.MaxValue);
			release = last & (down ^ UInt32.MaxValue);
		}

		public void LoadDefaults(int player)
		{
			mapKeys.Clear();
			mapPad.Clear();

			if (player == 0)
			{
				mapKeys.Add(Keys.Up, Button.Up);
				mapKeys.Add(Keys.Down, Button.Down);
				mapKeys.Add(Keys.Left, Button.Left);
				mapKeys.Add(Keys.Right, Button.Right);

				mapKeys.Add(Keys.X, Button.A);
				mapKeys.Add(Keys.Z, Button.B);
				mapKeys.Add(Keys.S, Button.X);
				mapKeys.Add(Keys.A, Button.Y);

				mapKeys.Add(Keys.D, Button.L);
				mapKeys.Add(Keys.C, Button.R);

				mapKeys.Add(Keys.Enter, Button.Start);
				mapKeys.Add(Keys.RightShift, Button.Select);
			}

			// gamepad it up yo in the hizzouse
			if (player > -1 && player < 4)
			{
				padNum = player;

				mapPad.Add(Buttons.A, Button.B);
				mapPad.Add(Buttons.B, Button.A);
				mapPad.Add(Buttons.X, Button.Y);
				mapPad.Add(Buttons.Y, Button.X);

				mapPad.Add(Buttons.LeftShoulder, Button.L);
				mapPad.Add(Buttons.RightShoulder, Button.R);
				mapPad.Add(Buttons.LeftTrigger, Button.L);
				mapPad.Add(Buttons.RightTrigger, Button.R);

				mapPad.Add(Buttons.Start, Button.Start);
				mapPad.Add(Buttons.Back, Button.Select);
			}
		}
	}
}
