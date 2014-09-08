using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public class Program
	{
		public static void Main()
		{
			using (Core core = Core.instance = new Core())
			{
				core.Run();
			}
		}
	}
}
