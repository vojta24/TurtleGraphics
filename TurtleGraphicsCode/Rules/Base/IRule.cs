using System;
using System.Collections.Generic;

namespace TurtleGraphicsCode {
	public interface IRule {
		Dictionary<char, string> Rules { get; }

		Dictionary<char, Action<Turtle>> Actions { get; }

		string Axiom { get; }
	}
}
