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
		public float GetOverlap(Range other)
		{
			return Math.Min(other.max - min, max - other.min);
		}

		public Range Extend(float value)
		{
			float nmin = min, nmax = max;
			if (value < nmin) nmin = value;
			else if (value > nmax) nmax = value;
			return new Range(nmin, nmax);
		}

		public Range Pad(float value) { return new Range(min - value, max + value); }

		public static Range operator +(Range r, float f) { return new Range(r.min + f, r.max + f); }
		public static Range operator -(Range r, float f) { return r + (-f); }
		
	}
}
