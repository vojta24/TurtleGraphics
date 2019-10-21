using System.Collections.Generic;
using System.Text;

namespace TurtleGraphicsCode {
	public class LSystem {

		public string Axiom { get; private set; }
		public string Sentence { get; private set; }
		public Dictionary<char, string> Rules { get; private set; }
		public IRule Rule { get; }

		public LSystem(IRule rule) {
			Rule = rule;
			Axiom = rule.Axiom;
			Rules = rule.Rules;
			Sentence = Axiom;
		}

		public string Generate() {
			StringBuilder newSentence = new StringBuilder();

			foreach (char c in Sentence) {
				if (Rules.ContainsKey(c)) {
					newSentence.Append(Rules[c]);
				}
				else {
					newSentence.Append(c);
				}
			}
			Sentence = newSentence.ToString();
			return Sentence;
		}
	}
}
