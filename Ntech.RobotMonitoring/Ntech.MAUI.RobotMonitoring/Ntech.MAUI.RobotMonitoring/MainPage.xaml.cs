using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ntech.MAUI.RobotMonitoring;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private Grid currentModule;
    private bool allowGetWafer = true;
    private bool allowPutWafer = false;
    private double currentRotation = 0;
    private bool isAnimating = false;

    public bool AllowGetWafer
    {
        get => allowGetWafer;
        set
        {
            if (allowGetWafer != value)
            {
                allowGetWafer = value;
                OnPropertyChanged();

                // Cập nhật trực tiếp UI để đảm bảo
                if (BtnGetWafer != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        BtnGetWafer.IsEnabled = value;
                        System.Diagnostics.Debug.WriteLine($"✅ BtnGetWafer.IsEnabled = {value}");
                    });
                }
            }
        }
    }

    public bool AllowPutWafer
    {
        get => allowPutWafer;
        set
        {
            if (allowPutWafer != value)
            {
                allowPutWafer = value;
                OnPropertyChanged();

                // Cập nhật trực tiếp UI để đảm bảo
                if (BtnPutWafer != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        BtnPutWafer.IsEnabled = value;
                        System.Diagnostics.Debug.WriteLine($"✅ BtnPutWafer.IsEnabled = {value}");
                    });
                }
            }
        }
    }

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;

        // Đợi UI load xong
        Loaded += (s, e) =>
        {
            currentModule = PM10BodyWaferTable;
            // Đảm bảo trạng thái ban đầu đúng
            UpdateButtonStates();
        };
    }

    // Phương thức kiểm tra và cập nhật trạng thái nút
    private void UpdateButtonStates()
    {
        // Kiểm tra robot có đang cầm wafer không
        bool robotHasWafer = RobotBodyWaferTable.Children.Contains(Wafer);

        // Kiểm tra module hiện tại có wafer không
        bool currentModuleHasWafer = currentModule != null && currentModule.Children.Contains(Wafer);

        // Debug log để kiểm tra
        System.Diagnostics.Debug.WriteLine("=== UpdateButtonStates ===");
        System.Diagnostics.Debug.WriteLine($"Robot has wafer: {robotHasWafer}");
        System.Diagnostics.Debug.WriteLine($"Current module has wafer: {currentModuleHasWafer}");
        System.Diagnostics.Debug.WriteLine($"Current module: {currentModule?.GetType().Name}");

        if (robotHasWafer)
        {
            // Robot đang cầm wafer -> chỉ cho phép thả
            System.Diagnostics.Debug.WriteLine("→ Robot đang cầm wafer → Cho phép THẢ");
            AllowGetWafer = false;
            AllowPutWafer = true;
        }
        else if (currentModuleHasWafer)
        {
            // Module hiện tại có wafer và robot không cầm -> cho phép gắp
            System.Diagnostics.Debug.WriteLine("→ Module có wafer → Cho phép GẮP");
            AllowGetWafer = true;
            AllowPutWafer = false;
        }
        else
        {
            // Module hiện tại không có wafer và robot không cầm -> không cho phép gì
            System.Diagnostics.Debug.WriteLine("→ Không có wafer → Tắt cả 2 nút");
            AllowGetWafer = false;
            AllowPutWafer = false;
        }

        System.Diagnostics.Debug.WriteLine($"Final state - Get: {AllowGetWafer}, Put: {AllowPutWafer}");
        System.Diagnostics.Debug.WriteLine("========================");
    }

    private async void Rotate_Clicked(object sender, EventArgs e)
    {
        if (isAnimating) return;

        var button = sender as Button;
        if (button == null) return;

        double targetRotation = 0;
        Grid targetModule = null;

        switch (button.Text)
        {
            case "0":
                targetRotation = 0;
                targetModule = PM10BodyWaferTable;
                break;
            case "90":
                targetRotation = 90;
                targetModule = PM01BodyWaferTable;
                break;
            case "180":
                targetRotation = 180;
                targetModule = PM12BodyWaferTable;
                break;
            case "270":
                targetRotation = 270;
                targetModule = PM21BodyWaferTable;
                break;
        }

        System.Diagnostics.Debug.WriteLine($"\n🔄 Rotating to {button.Text}°");

        isAnimating = true;
        DisableAllButtons();

        // CẬP NHẬT currentModule TRƯỚC KHI XOAY
        currentModule = targetModule;
        System.Diagnostics.Debug.WriteLine($"Current module updated: {currentModule?.GetType().Name}");

        // Animate robot rotation
        await RobotArea.RotateTo(targetRotation, 1000, Easing.CubicInOut);

        // Update position indicator
        PositionIndicator.Rotation = targetRotation;

        currentRotation = targetRotation;
        isAnimating = false;
        EnableRotationButtons();

        // Đợi một chút để đảm bảo UI đã hoàn tất
        await Task.Delay(100);

        // Cập nhật trạng thái nút sau khi xoay
        UpdateButtonStates();
    }

    private bool FindWaferInCurrentModule()
    {
        if (currentModule is Grid grid)
        {
            return grid.Children.Contains(Wafer);
        }
        return false;
    }

    private void DisableAllButtons()
    {
        Btn0.IsEnabled = false;
        Btn90.IsEnabled = false;
        Btn180.IsEnabled = false;
        Btn270.IsEnabled = false;
        BtnGetWafer.IsEnabled = false;
        BtnPutWafer.IsEnabled = false;
    }

    private void EnableRotationButtons()
    {
        Btn0.IsEnabled = true;
        Btn90.IsEnabled = true;
        Btn180.IsEnabled = true;
        Btn270.IsEnabled = true;
    }

    private async void GetWafer_Clicked(object sender, EventArgs e)
    {
        if (isAnimating) return;

        System.Diagnostics.Debug.WriteLine("\n🔺 Get Wafer clicked");

        isAnimating = true;
        DisableAllButtons();

        // Extend arm animation - Di chuyển ra ngoài
        await Task.WhenAll(
            RobotArm.TranslateTo(-30, 0, 500, Easing.CubicOut),
            ArmGripper.TranslateTo(65, 0, 500, Easing.CubicOut)
        );

        await Task.Delay(200);

        // Move wafer to robot
        var parentGrid = Wafer.Parent as Grid;
        if (parentGrid != null)
        {
            System.Diagnostics.Debug.WriteLine($"Moving wafer from {parentGrid.GetType().Name} to RobotBodyWaferTable");
            parentGrid.Children.Remove(Wafer);
            RobotBodyWaferTable.Children.Add(Wafer);
            RobotBodyWaferTable.IsVisible = true;
            System.Diagnostics.Debug.WriteLine("✅ Wafer moved to robot");
        }

        await Task.Delay(200);

        // Retract arm - Thu tay vào
        await Task.WhenAll(
            RobotArm.TranslateTo(40, 0, 500, Easing.CubicIn),
            ArmGripper.TranslateTo(95, 0, 500, Easing.CubicIn)
        );

        EnableRotationButtons();
        isAnimating = false;

        // Đợi một chút để đảm bảo UI đã hoàn tất
        await Task.Delay(100);

        // Cập nhật trạng thái sau khi gắp
        UpdateButtonStates();
    }

    private async void PutWafer_Clicked(object sender, EventArgs e)
    {
        if (isAnimating) return;

        System.Diagnostics.Debug.WriteLine("\n🔻 Put Wafer clicked");

        isAnimating = true;
        DisableAllButtons();

        // Extend arm animation - Di chuyển ra ngoài
        await Task.WhenAll(
            RobotArm.TranslateTo(-30, 0, 500, Easing.CubicOut),
            ArmGripper.TranslateTo(65, 0, 500, Easing.CubicOut)
        );

        await Task.Delay(200);

        // Move wafer to current module
        if (RobotBodyWaferTable.Children.Count > 0)
        {
            RobotBodyWaferTable.Children.Remove(Wafer);
            if (currentModule is Grid targetGrid)
            {
                System.Diagnostics.Debug.WriteLine($"Moving wafer from robot to {targetGrid.GetType().Name}");
                targetGrid.Children.Add(Wafer);
                System.Diagnostics.Debug.WriteLine("✅ Wafer moved to module");
            }
            RobotBodyWaferTable.IsVisible = false;
        }

        await Task.Delay(200);

        // Retract arm - Thu tay vào
        await Task.WhenAll(
            RobotArm.TranslateTo(40, 0, 500, Easing.CubicIn),
            ArmGripper.TranslateTo(95, 0, 500, Easing.CubicIn)
        );

        EnableRotationButtons();
        isAnimating = false;

        // Đợi một chút để đảm bảo UI đã hoàn tất
        await Task.Delay(100);

        // Cập nhật trạng thái sau khi thả
        UpdateButtonStates();
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Application.Current?.Quit();
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected new virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}