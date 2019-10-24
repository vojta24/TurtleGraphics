using System.Collections.Generic;
using System.Text;

namespace TurtleGraphicsCode {
	public class LSystem {

		public string Sentence { get; private set; }
		public IRule Setup { get; }

		public LSystem(IRule setup, int generations) {
			Setup = setup;
			Sentence = Setup.Axiom;
			for (int i = 0; i < generations; i++) {
				Sentence = Generate();
			}
		}

		public string Generate() {
			StringBuilder newSentence = new StringBuilder();

			foreach (char c in Sentence) {
				if (Setup.Rules.ContainsKey(c)) {
					newSentence.Append(Setup.Rules[c]);
				}
				else {
					newSentence.Append(c);
				}
			}
			Sentence = newSentence.ToString();
			return Sentence;
		}

		public Turtle Draw(bool fullScreen) {
			Turtle t = Setup.Turtle ?? new Turtle();
			t.FullScreen = fullScreen;

			foreach (char c in Sentence) {
				if (Setup.Actions.ContainsKey(c)) {
					Setup.Actions[c].Invoke(t);
				}
			}

			return t;
		}
	}
}
