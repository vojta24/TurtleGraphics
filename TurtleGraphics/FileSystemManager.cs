using System.Windows.Controls;
using System.IO;
using System;
using Microsoft.Win32;

namespace TurtleGraphics {
	public class FileSystemManager {

		private const string EXTENSION = ".tgs";
		public string SavedDataPath => Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SavedData");

		public FileSystemManager() {
			if (!Directory.Exists(SavedDataPath)) {
				Directory.CreateDirectory(SavedDataPath);
			}
		}

		public void Save(string saveFileName, string code) {
			string original = saveFileName;
			if (saveFileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) {
				foreach (char c in Path.GetInvalidFileNameChars()) {
					saveFileName = saveFileName.Replace(c, '_');
				}
			}
			File.WriteAllText(Path.Combine(SavedDataPath, saveFileName + EXTENSION), string.Join(Environment.NewLine, original, code));
		}


		public SavedData Load() {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.DefaultExt = ".tgs";
			dialog.InitialDirectory = SavedDataPath;
			bool? res = dialog.ShowDialog();

			if (res.HasValue && res.Value) {
				string lines = File.ReadAllText(dialog.FileName);
				int lineIndex = lines.IndexOf('\r');
				return new SavedData() { Name = lines.Substring(0, lineIndex), Code = lines.Substring(lineIndex + 2) };
			}
			return new SavedData() { Name = null };
		}
	}
}
