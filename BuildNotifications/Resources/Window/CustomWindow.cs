using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Win32;
using Button = System.Windows.Controls.Button;
using MouseEventHandler = System.Windows.Input.MouseEventHandler;

// copied class. If template resolutions fail, the entire application is unusable. So null reference exceptions here are fine.
#nullable disable
namespace BuildNotifications.Resources.Window;

// Source https://github.com/NikolayVasilev/wpf-custom-window
public partial class CustomWindow : System.Windows.Window
{
    static CustomWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow),
            new FrameworkPropertyMetadata(typeof(CustomWindow)));
    }

    protected CustomWindow()
    {
        var currentDpiScaleFactor = SystemHelper.GetCurrentDpiScaleFactor();
        var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
        SizeChanged += OnSizeChanged;
        StateChanged += OnStateChanged;
        Loaded += OnLoaded;
        var workingArea = screen.WorkingArea;
        MaxHeight = (workingArea.Height + 16) / currentDpiScaleFactor;
        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseButtonUp), true);
        AddHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMove));
    }

    public bool DisplayIcon
    {
        get => (bool)GetValue(DisplayIconProperty);
        set => SetValue(DisplayIconProperty, value);
    }

    public bool DisplaySystemButtons
    {
        get => (bool)GetValue(DisplaySystemButtonsProperty);
        set => SetValue(DisplaySystemButtonsProperty, value);
    }

    public object LeftToButtonsContent
    {
        get => GetValue(LeftToButtonsContentProperty);
        set => SetValue(LeftToButtonsContentProperty, value);
    }

    public object RightToTitleContent
    {
        get => GetValue(RightToTitleContentProperty);
        set => SetValue(RightToTitleContentProperty, value);
    }

    protected virtual bool AllowSystemContextMenu => true;

    public override void OnApplyTemplate()
    {
        GetRequiredTemplateChild<Grid>("WindowRoot");
        _layoutRoot = GetRequiredTemplateChild<Grid>("LayoutRoot");
        _minimizeButton = GetRequiredTemplateChild<Button>("MinimizeButton");
        _maximizeButton = GetRequiredTemplateChild<Button>("MaximizeButton");
        _restoreButton = GetRequiredTemplateChild<Button>("RestoreButton");
        _closeButton = GetRequiredTemplateChild<Button>("CloseButton");
        _headerBar = GetRequiredTemplateChild<Grid>("PART_HeaderBar");

        _rightToTitleContentPresenter = GetRequiredTemplateChild<ContentPresenter>("PART_RightToTitleContentPresenter");
        if (_rightToTitleContentPresenter != null)
            _rightToTitleContentPresenter.Content = RightToTitleContent;

        _leftToButtonsContentPresenter = GetRequiredTemplateChild<ContentPresenter>("PART_LeftToButtonsContentPresenter");
        if (_leftToButtonsContentPresenter != null)
            _leftToButtonsContentPresenter.Content = LeftToButtonsContent;

        if (_layoutRoot != null && WindowState == WindowState.Maximized)
            _layoutRoot.Margin = GetDefaultMarginForDpi();

        if (_closeButton != null)
            _closeButton.Click += CloseButton_Click;

        if (_minimizeButton != null)
            _minimizeButton.Click += MinimizeButton_Click;

        if (_restoreButton != null)
            _restoreButton.Click += RestoreButton_Click;

        if (_maximizeButton != null)
            _maximizeButton.Click += MaximizeButton_Click;

        _headerBar?.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnHeaderBarMouseLeftButtonDown));

        base.OnApplyTemplate();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private T GetRequiredTemplateChild<T>(string childName)
        where T : DependencyObject => (T)GetTemplateChild(childName);

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleWindowState();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void OnHeaderBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_isManualDrag)
            return;

        var position = e.GetPosition(this);
        const int headerBarHeight = 36;
        const int leftmostClickableOffset = 50;

        if (position.X - _layoutRoot.Margin.Left <= leftmostClickableOffset && position.Y <= headerBarHeight && AllowSystemContextMenu)
        {
            if (e.ClickCount != 2)
                OpenSystemContextMenu(e);
            else
                Close();

            e.Handled = true;
            return;
        }

        if (e.ClickCount == 2 && ResizeMode == ResizeMode.CanResize)
        {
            ToggleWindowState();
            return;
        }

        if (WindowState == WindowState.Maximized)
        {
            _isMouseButtonDown = true;
            _mouseDownPosition = position;
        }
        else
        {
            try
            {
                _positionBeforeDrag = new Point(Left, Top);
                DragMove();
            }
            catch
            {
                // ignored
            }
        }
    }

    private void OpenSystemContextMenu(MouseButtonEventArgs e)
    {
        var position = e.GetPosition(this);
        var screen = PointToScreen(position);

        const int num = 36;
        if (position.Y >= num)
            return;

        var handle = new WindowInteropHelper(this).Handle;
        var systemMenu = NativeMethods.GetSystemMenu(handle, false);

        var uEnable = WindowState != WindowState.Maximized ? (uint)0 : 1;
        NativeMethods.EnableMenuItem(systemMenu, 61488, uEnable);

        var num1 = NativeMethods.TrackPopupMenuEx(systemMenu, NativeMethods.TPM_LEFTALIGN | NativeMethods.TPM_RETURNCMD, Convert.ToInt32(screen.X + 2), Convert.ToInt32(screen.Y + 2), handle, IntPtr.Zero);
        if (num1 == 0)
            return;

        NativeMethods.PostMessage(handle, 274, new IntPtr(num1), IntPtr.Zero);
    }

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CustomWindow customWindow)
            return;

        switch (e.Property.Name)
        {
            case nameof(RightToTitleContent):
                if (customWindow._rightToTitleContentPresenter != null)
                    customWindow._rightToTitleContentPresenter.Content = e.NewValue;
                break;
            case nameof(LeftToButtonsContent):
                if (customWindow._leftToButtonsContentPresenter != null)
                    customWindow._leftToButtonsContentPresenter.Content = e.NewValue;
                break;
        }
    }

    private void RestoreButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleWindowState();
    }

    private void SetMaximizeButtonsVisibility(Visibility maximizeButtonVisibility, Visibility reverseMaximizeButtonVisibility)
    {
        if (_maximizeButton != null)
            _maximizeButton.Visibility = maximizeButtonVisibility;

        if (_restoreButton != null)
            _restoreButton.Visibility = reverseMaximizeButtonVisibility;
    }

    private void ToggleWindowState()
    {
        WindowState = WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
    }

    private bool _isMouseButtonDown;
    private bool _isManualDrag;
    private Point _mouseDownPosition;
    private Point _positionBeforeDrag;
    private Point _previousScreenBounds;

    private Grid _layoutRoot;
    private Button _minimizeButton;
    private Button _maximizeButton;
    private Button _restoreButton;
    private Button _closeButton;
    private Grid _headerBar;
    private double _widthBeforeMaximize;
    private WindowState _previousState;
    private ContentPresenter _rightToTitleContentPresenter;
    private ContentPresenter _leftToButtonsContentPresenter;

    public static readonly DependencyProperty DisplayIconProperty = DependencyProperty.Register(
        "DisplayIcon", typeof(bool), typeof(CustomWindow), new PropertyMetadata(true));

    public static readonly DependencyProperty DisplaySystemButtonsProperty = DependencyProperty.Register(
        "DisplaySystemButtons", typeof(bool), typeof(CustomWindow), new PropertyMetadata(true));

    public static readonly DependencyProperty RightToTitleContentProperty = DependencyProperty.Register(
        "RightToTitleContent", typeof(object), typeof(CustomWindow), new PropertyMetadata(default, PropertyChangedCallback));

    public static readonly DependencyProperty LeftToButtonsContentProperty = DependencyProperty.Register(
        "LeftToButtonsContent", typeof(object), typeof(CustomWindow), new PropertyMetadata(default(object)));
}
#nullable enable