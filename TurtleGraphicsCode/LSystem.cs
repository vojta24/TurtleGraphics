using System.Collections.Generic;
using System.Text;

namespace TurtleGraphicsCode {
	public class LSystem {

		public string Axiom { get; private set; }
		public string Sentence { get; private set; }
		public Dictionary<char, string> Rules { get; private set; }
		public IRule Rule { get; }

		public LSystem(IRule rule, int generations) {
			Rule = rule;
			Axiom = rule.Axiom;
			Rules = rule.Rules;
			Sentence = Axiom;
			for (int i = 0; i < generations; i++) {
				Sentence = Generate();
			}
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

		public Turtle Draw(bool fullScreen) {
			Turtle t = new Turtle(fullScreen);

			foreach (char c in Sentence) {
				if (Rule.Actions.ContainsKey(c)) {
					Rule.Actions[c].Invoke(t);
				}
			}

			return t;
		}
	}
}
