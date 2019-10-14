using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {

		public bool? LaunchFullScreen { get; private set; } = null;

		public TurtleGraphicsCodeData Deserialized { get; private set; }

		protected override void OnStartup(StartupEventArgs e) {
			foreach (string arg in e.Args) {
				if (arg == "-f") {
					LaunchFullScreen = true;
				}
				if (File.Exists(arg)) {
					BinaryFormatter bf = new BinaryFormatter();
					using (FileStream fs = File.OpenRead(arg)) {
						Deserialized = (TurtleGraphicsCodeData)bf.Deserialize(fs);
					}
				}
			}
			base.OnStartup(e);
		}
	}
}
