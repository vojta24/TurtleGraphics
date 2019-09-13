using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Igor.Models;

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for ExceptionDisplay.xaml
	/// </summary>
	public partial class ExceptionDisplay : UserControl, INotifyPropertyChanged {

		#region Notifications

		public event PropertyChangedEventHandler PropertyChanged;

		private void Notify(string prop) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		#endregion

		public ExceptionDisplay() {
			InitializeComponent();
			DataContext = this;
			Grid.SetColumn(this, 1);

			_dismissCommand = new Command(() => {
				MainWindow.Instance.Paths.Children.Remove(this);
				MainWindow.Instance.ShowTurtleCheckBox = _turtleVisibilityBck;
			});
			Loaded += ExceptionDisplay_Loaded;
		}

		private void ExceptionDisplay_Loaded(object sender, System.Windows.RoutedEventArgs e) {
			MainWindow.Instance.ExceptionDialogActive = true;
			_turtleVisibilityBck = MainWindow.Instance.ShowTurtleCheckBox;
			MainWindow.Instance.ShowTurtleCheckBox = false;
		}

		private bool _turtleVisibilityBck;
		private ParsingException _exception;

		private string _exceptionMessage;
		private ICommand _dismissCommand;
		private string _stackTrace;

		public string StackTrace { get => _stackTrace; set { _stackTrace = value; Notify(nameof(StackTrace)); } }
		public ICommand DismissCommand { get => _dismissCommand; set { _dismissCommand = value; Notify(nameof(DismissCommand)); } }
		public string ExceptionMessage { get => _exceptionMessage; set { _exceptionMessage = value + $"{Environment.NewLine}  at line: {Exception.LineText}"; Notify(nameof(ExceptionMessage)); } }

		public ParsingException Exception {
			get => _exception;
			set {
				_exception = value;
				StackTrace = value.StackTrace;
			}
		}

		public void Show() {
			MainWindow.Instance.Paths.Children.Add(this);
		}
	}
}
