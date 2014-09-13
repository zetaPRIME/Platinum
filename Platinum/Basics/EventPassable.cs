using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public class EventPassable : IEventPassable
	{
		Dictionary<string, Action<EventParams>> events = new Dictionary<string, Action<EventParams>>();

		public void Call(string name) { Call(name, new EventParams()); }
		public void Call(string name, EventParams param) { if (events.ContainsKey(name)) events[name](param); }
	}
}
