using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public class PackageManager
	{
		public RootPackage rootPackage;

		public Dictionary<string, Package> loadedPackages = new Dictionary<string, Package>();
	}
}
