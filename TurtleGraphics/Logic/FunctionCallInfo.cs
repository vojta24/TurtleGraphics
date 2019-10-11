namespace TurtleGraphics {
	public class FunctionCallInfo {
		public string FunctionName { get; set; }
		public string[] Arguments { get; set; }

		public string GetArg(int i, string line) => Arguments.Length > i ?
			Arguments[i] : throw new ParsingException("Not enogh arguments provided supplied to the function!", line);
	}
}