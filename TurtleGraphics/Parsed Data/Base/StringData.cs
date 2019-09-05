namespace TurtleGraphics {
	public class StringData : ParsedData {
		public string[] Parameters { get; set; }

		public string Arg1 => Parameters[0];

		public StringData(params string[] parameters) {
			Parameters = parameters;
		}
	}
}
