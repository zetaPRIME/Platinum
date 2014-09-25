using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Platinum
{
	public static class Input
	{
		public static List<PlayerInput> players = new List<PlayerInput>();

		public static KeyboardState keyStateNow;
		public static KeyboardState keyStateLast;

		static string keyBuffer = "";
		static string keyBufferComplete = "";
		public static string KeyBuffer { get { return keyBufferComplete; } }

		public static Settings padSettings;

		public static MouseState mouseStateNow;
		public static MouseState mouseStateLast;

		public static Point MousePosition { get { return mouseStateNow.Position; } }
		public static int MouseScroll { get { return (mouseStateNow.ScrollWheelValue - mouseStateLast.ScrollWheelValue) / -120; } }

		public static void Init()
		{
			// for some stupid reason you need to bash reflection against MonoGame if you want to configure controllers
			Type gpt = typeof(GamePad);
			MethodInfo minfo = gpt.GetMethod("PrepSettings", BindingFlags.NonPublic | BindingFlags.Static);
			minfo.Invoke(null, new object[] { });
			FieldInfo finfo = gpt.GetField("settings", BindingFlags.NonPublic | BindingFlags.Static);
			padSettings = finfo.GetValue(null) as Settings;

		}

		public static void Update()
		{
			keyStateLast = keyStateNow;
			keyStateNow = Keyboard.GetState();

			keyBufferComplete = keyBuffer;
			keyBuffer = "";

			mouseStateLast = mouseStateNow;
			mouseStateNow = Mouse.GetState();

			foreach (PlayerInput pi in players) pi.Update();
		}

		public const string AllowedCharacters = "1234567890!@#$%^&*()" + "`-=~_+[]\\{}|;':\",./<>?" +
			"QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm" +
			"" // accents at some point
			+ " \b";
		public static void OnTextInput(Object sender, TextInputEventArgs args)
		{
			//if (args.Character == '\b') return; // no backspace!
			//if (args.Character == '') return; // ctrl+backspace glitch. what
			char chr = args.Character;
			if (chr == '') chr = '\b';
			if (!AllowedCharacters.Contains(chr)) return;
			keyBuffer += chr;

			//Console.WriteLine(keyBuffer);
		}

		public static int OperateText(ref string str, int cursorPos = -1)
		{
			string buffer = keyBufferComplete;
			bool backspace = buffer.Contains('\b') || buffer.Contains('');
			buffer = buffer.Replace("\b", "");
			buffer = buffer.Replace("", ""); // ctrl+backspc char is invisible in VS
			buffer = buffer.Replace("\t", "");
			buffer = buffer.Replace("\v", "");
			buffer = buffer.Replace("\n", "");
			buffer = buffer.Replace("\f", "");

			if (cursorPos == -1) cursorPos = str.Length;
			string ostr = str;
			int opos = cursorPos;
			str = str.Insert(cursorPos, buffer);
			cursorPos += buffer.Length;

			// control keys
			if (KeyPressed(Keys.Left)) cursorPos--;
			if (KeyPressed(Keys.Right)) cursorPos++;
			if (KeyPressed(Keys.Home)) cursorPos = 0;
			if (KeyPressed(Keys.End)) cursorPos = str.Length;
			if (backspace && cursorPos > 0)
			{
				string nstr = str;
				str = nstr.Substring(0, cursorPos - 1);
				if (KeyHeld(Keys.LeftControl)) str = "";
				if (nstr.Length >= cursorPos) str += nstr.Substring(cursorPos);
				cursorPos--;
				if (KeyHeld(Keys.LeftControl)) cursorPos = 0;
			}
			if (KeyPressed(Keys.Delete) && cursorPos < str.Length)
			{
				string nstr = str;
				str = nstr.Substring(0, cursorPos);
				if (nstr.Length > cursorPos && !KeyHeld(Keys.LeftControl)) str += nstr.Substring(cursorPos + 1);
			}

			return cursorPos - opos;
		}

		public static string GetClipboard()
		{
			string clip = "";

			string clipRaw = "";
			#if WINDOWS // .NET
			Thread t = new Thread(() => { clipRaw = System.Windows.Clipboard.GetText(); });
			t.SetApartmentState(ApartmentState.STA);
			t.Start(); t.Join(100);
			#else // mono
			Gtk.Clipboard clipboard = Gtk.Clipboard.Get(Gdk.Atom.Interm("CLIPBOARD", false));
			if (clipboard != null) clipRaw = clipboard.WaitForText();
			#endif
			for (int i = 0; i < clipRaw.Length; i++)
			{
				char c = clipRaw[i];
				if (AllowedCharacters.Contains(c)) clip += c;
			}
			return clip;
		}
		public static void SetClipboard(string str)
		{
			#if WINDOWS // .NET
			Thread t = new Thread(() => { System.Windows.Clipboard.SetText(str); });
			t.SetApartmentState(ApartmentState.STA);
			t.Start(); t.Join(100);
			#else // mono
			Gtk.Clipboard clipboard = Gtk.Clipboard.Get(Gdk.Atom.Interm("CLIPBOARD", false));
			if (clipboard != null) clipboard.Text = str;
			#endif
		}

		public static bool KeyPressed(Keys key)
		{
			return keyStateNow.IsKeyDown(key) && (keyStateLast == null || keyStateLast.IsKeyUp(key));
		}

		public static bool KeyHeld(Keys key)
		{
			return keyStateNow.IsKeyDown(key);
		}

		public static bool KeyReleased(Keys key)
		{
			return keyStateNow.IsKeyUp(key) && (keyStateLast == null || keyStateLast.IsKeyDown(key));
		}
	}

	public enum Button { A, B, X, Y, L, R, Start, Select, Up, Down, Left, Right, END }
}
