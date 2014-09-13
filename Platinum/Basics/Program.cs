using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fluent.IO;

namespace Platinum
{
	public class Program
	{
		public static void Main()
		{
			FlushTemp();

			using (Core core = Core.instance = new Core())
			{
				core.Run();
			}

			//FlushTemp(); // nope; can't wipe assemblies that are currently loaded
		}

		static void FlushTemp()
		{
			Path tmp = new Path("tmp");
			tmp.Combine("Assembly").CreateDirectory().Files("*.dll", false).Delete();
		}
	}
}
