using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TurtleGraphics {
	public class InteliCommandsHandler {

		#region InteliCommands
		private Dictionary<string, string> _inteliCommands = new Dictionary<string, string> {
			{ "for", " (int i = 0; i < ; i++) {" + Environment.NewLine + Environment.NewLine + "}" },
			{ "if", " () {" + Environment.NewLine + Environment.NewLine + "}" },
			{ "R", "otate();" },
			{ "M", "oveTo();" },
			{ "F", "orward();" },
			{ "PenU", "p();" },
			{ "PenD", "own();" },
			{ "SetB", "rushSize();" },
			{ "SetC", "olor();" },
		};

		private Dictionary<string, int> _inteliCommandsIndexes = new Dictionary<string, int> {
			{ "for", 17  },
			{ "if", 2 },
			{ "R", 6 },
			{ "M", 6 },
			{ "F", 7 },
			{ "PenU", 2 },
			{ "PenD", 4 },
			{ "SetB", 9 },
			{ "SetC", 5 },
		};

		public string GetInteliCommand(string value) {
			foreach (string key in _inteliCommands.Keys) {
				if (value == key) {
					return value + _inteliCommands[key];
				}
			}
			return value;
		}

		public int GetIndexForCaret(string value) {
			foreach (string key in _inteliCommands.Keys) {
				if (value == key) {
					return value.Length + _inteliCommandsIndexes[key];
				}
			}
			throw new NotImplementedException();
		}
		#endregion

		public InteliCommandsState State { get; set; } = InteliCommandsState.Normal;
		private string _triggerCommand;
		private int _addedLinesCount;
		private int _addedLinesIndex;
		private bool _ignoreEvent = false;
		private int _textLength;

		public void Handle(MainWindow window, TextBox inputControl) {
			if (_ignoreEvent) {
				_ignoreEvent = false;
				return;
			}
			string newText = inputControl.Text;

			if (State == InteliCommandsState.Normal) {
				int carret = inputControl.CaretIndex;
				if (IsValidCommand(newText, carret, out (int start, int length) values)) {
					_triggerCommand = newText.Substring(values.start, values.length);

					string commandFull = GetInteliCommand(_triggerCommand);
					_addedLinesCount = (commandFull.Split('\n').Length - 1) * Environment.NewLine.Length;
					_addedLinesIndex = inputControl.CaretIndex;
					State = InteliCommandsState.IsSuggesting;

					window.InteliCommandsText = newText.Remove(values.start, values.length).Insert(values.start, commandFull);
					_ignoreEvent = true;
					window.CommandsText = newText.Insert(_addedLinesIndex, string.Concat(Enumerable.Repeat(Environment.NewLine, _addedLinesCount / Environment.NewLine.Length)));
					_ignoreEvent = true;
					inputControl.CaretIndex = carret;
				}
				else {
					window.InteliCommandsText = newText;
					window.CommandsText = newText;
				}
				_textLength = window.CommandsText.Length;
			}
			else {
				int carret = inputControl.CaretIndex;
				int lastChar = inputControl.CaretIndex - 1;

				if (lastChar > 0 && inputControl.SelectionLength == 0 && newText[lastChar] == '\t' && carret == _addedLinesIndex + 1) {
					_ignoreEvent = true;
					window.CommandsText = window.InteliCommandsText;
					_ignoreEvent = true;
					inputControl.CaretIndex = lastChar - _triggerCommand.Length + GetIndexForCaret(_triggerCommand);
				}
				else {
					if (lastChar < 0) {
						State = InteliCommandsState.Normal;
						window.InteliCommandsText = newText;
						window.CommandsText = newText;
						return;
					}

					int selectionLen = inputControl.SelectionLength;
					int selectionIndex = inputControl.SelectionStart;
					_ignoreEvent = true;
					if (_textLength > newText.Length/* == newText.Length - _addedLinesCount + 1*/) {
						window.InteliCommandsText = newText.Remove(_addedLinesIndex - 1, _addedLinesCount);
						window.CommandsText = newText.Remove(_addedLinesIndex - 1, _addedLinesCount);
					}
					else if (_textLength < newText.Length/*_addedLinesIndex == newText.Length - _addedLinesCount - 1*/) {
						window.InteliCommandsText = newText.Remove(_addedLinesIndex + 1, _addedLinesCount);
						window.CommandsText = newText.Remove(_addedLinesIndex + 1, _addedLinesCount);
					}
					else {
						window.InteliCommandsText = newText.Remove(_addedLinesIndex, _addedLinesCount);
						window.CommandsText = newText.Remove(_addedLinesIndex, _addedLinesCount);
					}
					if (selectionLen != 0) {
						_ignoreEvent = true;
						inputControl.SelectionStart = selectionIndex;
						_ignoreEvent = true;
						inputControl.SelectionLength = selectionLen;
					}
					else {
						_ignoreEvent = true;
						inputControl.CaretIndex = carret;
					}
				}
				State = InteliCommandsState.Normal;
			}
		}

		private bool IsValidCommand(string value, int carret, out (int, int) substringInfo) {
			substringInfo = (0, 0);

			int lastChar = carret - 1;


			if (lastChar < 0) {
				return false;
			}

			while (lastChar >= 0 && !char.IsWhiteSpace(value[lastChar])) {
				lastChar--;
			}

			if (lastChar >= 0 && value[lastChar] != '\n') {
				return false;
			}

			lastChar++;

			string sub = value.Substring(lastChar, carret - lastChar);

			if (carret < value.Length) {
				if (!char.IsWhiteSpace(value[carret])) {
					return false;
				}
			}

			if (_inteliCommands.ContainsKey(sub)) {
				substringInfo = (lastChar, carret - lastChar);
				return true;
			}
			return false;
		}
	}
}