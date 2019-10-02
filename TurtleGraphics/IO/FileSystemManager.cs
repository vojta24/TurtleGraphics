using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TurtleGraphics {
	public class FileSystemManager {

		public const string EXTENSION = ".tgs";
		public string SavedDataPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedData");

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


		public async Task<SavedData> Load() {
			LoadSaveDataDialog d = new LoadSaveDataDialog {
				Path = SavedDataPath
			};
			Grid.SetColumn(d, 1);
			MainWindow.Instance.Paths.Children.Add(d);
			return await d.Select();
		}
	}
}
