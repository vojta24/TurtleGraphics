using System.Collections.Generic;

namespace TurtleGraphicsCode {
	public interface IRule {
		Dictionary<char, string> Rules { get; }

		string Axiom { get; }

		double ForwardDistance { get; }

		double TurnAngle { get; }
	}
}
