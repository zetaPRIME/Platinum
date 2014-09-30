using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

using Fluent.IO;

namespace Platinum
{
	public class Program
	{
		public static string[] args;

		public static string tempModifier = "";

		public static void Main(string[] args)
		{
			Program.args = args;
			ProcessArguments(args);

			FlushTemp();

			using (Core core = Core.instance = new Core())
			{
				core.Run();
			}

			//FlushTemp(); // nope; can't wipe assemblies that are currently loaded
		}

		public static void FlushTemp(string mod = "")
		{
			if (mod == "") mod = tempModifier;
			Path tmp = new Path("tmp").Combine(mod);
			tmp.Combine("Assembly").CreateDirectory().Files("*.dll", false).Delete();
		}

		static void ProcessArguments(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				int ia = i;
				if (args[ia] == "playtest")
				{
					i++;

					tempModifier = "test";

					if (args.Length > i)
					{
						Core.mode = EngineMode.Game;
					}
				}
			}
		}
	}
}
