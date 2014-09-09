using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public class PackageManager
	{
		public static RootPackage rootPackage;

		public static Dictionary<string, Package> loadedPackages = new Dictionary<string, Package>();
	}
}
