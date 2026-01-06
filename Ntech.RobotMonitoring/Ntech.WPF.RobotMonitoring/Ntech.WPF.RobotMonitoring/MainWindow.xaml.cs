using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ntech.WPF.RobotMonitoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Panel currentModule;
        private bool allowGetWafer = true;
        private bool allowPutWafer = false;

        public bool AllowGetWafer
        {
            set
            {
                this.allowGetWafer = value;
                this.OnPropertyChanged();
            }
            get { return this.allowGetWafer; }
        }

        public bool AllowPutWafer
        {
            set
            {
                this.allowPutWafer = value;
                this.OnPropertyChanged();
            }
            get { return this.allowPutWafer; }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.currentModule = this.PM10BodyWaferTable;
        }

        private void Rotate(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var storyBoardName = string.Format("Rotate{0}", btn.Content.ToString());
                var storyboardGetOut = this.TryFindResource(storyBoardName) as Storyboard;
                if (storyboardGetOut != null)
                {
                    btn.IsEnabled = false;
                    storyboardGetOut.Completed += (o, args) => btn.IsEnabled = true;
                    storyboardGetOut.Begin();
                }
            }

            switch (btn.Content.ToString())
            {
                case "0":
                    this.currentModule = this.PM10BodyWaferTable;
                    break;
                case "90":
                    this.currentModule = this.PM01BodyWaferTable;
                    break;
                case "180":
                    this.currentModule = this.PM12BodyWaferTable;
                    break;
                case "270":
                    this.currentModule = this.PM21BodyWaferTable;
                    break;
                case "360":
                    this.currentModule = this.PM10BodyWaferTable;
                    break;
            }

            if (this.FindWaferInCurrentModule())
            {
                this.AllowGetWafer = true;
            }
            else
            {
                this.AllowGetWafer = false;
            }
        }

        private bool FindWaferInCurrentModule()
        {
            return this.currentModule.Children.Contains(this.Wafer);
        }

        private void GetWafer(object sender, RoutedEventArgs e)
        {
            const string storyBoardName = "PickUpWafer";
            var storyboardGetOut = this.TryFindResource(storyBoardName) as Storyboard;
            if (storyboardGetOut != null)
            {
                var wafer = this.Wafer;
                var parentGrid = wafer.Parent as Grid;
                if (parentGrid != null)
                {
                    parentGrid.Children.Remove(wafer);
                    this.RobotBodyWaferTable.Children.Add(wafer);
                    storyboardGetOut.Completed += (o, args) =>
                    {
                        this.AllowPutWafer = true;
                    };

                    this.AllowGetWafer = false;
                    storyboardGetOut.Begin();
                }
            }
        }

        private void PutWafer(object sender, RoutedEventArgs e)
        {
            const string storyBoardName = "PutWafer01";
            var storyboardGetOut = this.TryFindResource(storyBoardName) as Storyboard;
            if (storyboardGetOut != null)
            {
                storyboardGetOut.Completed -= StoryboardGetOutOnCompleted;
                storyboardGetOut.Completed += StoryboardGetOutOnCompleted;
                this.AllowPutWafer = false;
                storyboardGetOut.Begin();
            }
        }

        private void StoryboardGetOutOnCompleted(object sender, EventArgs eventArgs)
        {
            var wafer = this.Wafer;
            var parentGrid = wafer.Parent as Grid;
            if (parentGrid != null)
            {
                parentGrid.Children.Remove(wafer);
                this.currentModule.Children.Add(wafer);
                this.AllowGetWafer = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}