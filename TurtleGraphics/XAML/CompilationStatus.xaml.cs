using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for CompilationStatus.xaml
	/// </summary>
	public partial class CompilationStatus : UserControl, INotifyPropertyChanged {


		#region Notifications

		public event PropertyChangedEventHandler PropertyChanged;

		private void Notify(string prop) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		#endregion


		public CompilationStatus() {
			InitializeComponent();
			DataContext = this;
			
			Loaded += CompilationStatus_Loaded;
			Grid.SetColumn(this, 1);
		}


		private string _status = "Compiling...";

		public string Status { get => _status; set { _status = value; Notify(nameof(Status)); } }
		public bool Rotate { get; set; } = true;
		private bool _turtleVisibleBck;

		private void CompilationStatus_Loaded(object sender, System.Windows.RoutedEventArgs e) {
			_turtleVisibleBck = MainWindow.Instance.ShowTurtleCheckBox;
			MainWindow.Instance.ShowTurtleCheckBox = false;

			Task.Run(async () => {
				while (Rotate) {
					Dispatcher.Invoke(() => {
						Rotation.Angle += 5;
					});
					await Task.Delay(1);
				}
			});
		}

		public void Start() {
			Rotate = true;
			MainWindow.Instance.Paths.Children.Add(this);
		}

		public void Stop() {
			Rotate = false;
			MainWindow.Instance.Paths.Children.Remove(this);
			MainWindow.Instance.ShowTurtleCheckBox = _turtleVisibleBck;
		}
	}
}
