using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public class EventParams
	{
		Dictionary<string, int> _int = new Dictionary<string, int>();
		Dictionary<string, float> _float = new Dictionary<string, float>();
		Dictionary<string, string> _string = new Dictionary<string, string>();
		Dictionary<string, object> _obj = new Dictionary<string, object>();

		public int Int(string name) { if (!_int.ContainsKey(name)) return 0; return _int[name]; }
		public float Float(string name) { if (!_float.ContainsKey(name)) return 0; return _float[name]; }
		public string String(string name) { if (!_string.ContainsKey(name)) return null; return _string[name]; }
		public object Object(string name) { if (!_obj.ContainsKey(name)) return null; return _obj[name]; }

		public EventParams Add(string name, int entry) { _int.Add(name, entry); return this; }
		public EventParams Add(string name, float entry) { _float.Add(name, entry); return this; }
		public EventParams Add(string name, string entry) { _string.Add(name, entry); return this; }
		public EventParams Add(string name, object entry) { _obj.Add(name, entry); return this; }
	}
}
