using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public struct Range
	{
		public float min, max;

		public Range(float min, float max) { this.min = min; this.max = max; }
		public Range(float value) : this(value, value) { }

		public bool Overlaps(Range other)
		{
			return !(min > other.max || other.min > max);
		}

		public Range Extend(float value)
		{
			float nmin = min, nmax = max;
			if (value < nmin) nmin = value;
			else if (value > nmax) nmax = value;
			return new Range(nmin, nmax);
		}
	}
}
