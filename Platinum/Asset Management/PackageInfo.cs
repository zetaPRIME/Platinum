using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LitJson;

namespace Platinum
{
	public class PackageInfo
	{
		public bool isZip = false;
		public PackageType type = PackageType.Library;
		public string path;

		public JsonData def;
	}
}
