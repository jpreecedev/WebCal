﻿namespace TachographReader.Behaviours
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    public class WindowBehavior
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [AttachedPropertyBrowsableForType(typeof (Window))]
        public static bool GetHideCloseButton(Window obj)
        {
            return (bool) obj.GetValue(HideCloseButtonProperty);
        }

        [AttachedPropertyBrowsableForType(typeof (Window))]
        public static void SetHideCloseButton(Window obj, bool value)
        {
            obj.SetValue(HideCloseButtonProperty, value);
        }

        private static void HideCloseButtonChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null)
            {
                return;
            }

            var hideCloseButton = (bool) e.NewValue;
            if (hideCloseButton && !GetIsHiddenCloseButton(window))
            {
                if (!window.IsLoaded)
                {
                    window.Loaded += HideWhenLoadedDelegate;
                }
                else
                {
                    HideCloseButton(window);
                }
                SetIsHiddenCloseButton(window, true);
            }
            else if (!hideCloseButton && GetIsHiddenCloseButton(window))
            {
                if (!window.IsLoaded)
                {
                    window.Loaded -= ShowWhenLoadedDelegate;
                }
                else
                {
                    ShowCloseButton(window);
                }
                SetIsHiddenCloseButton(window, false);
            }
        }

        private static void HideCloseButton(Window w)
        {
            IntPtr hwnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private static void ShowCloseButton(Window w)
        {
            IntPtr hwnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) | WS_SYSMENU);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [AttachedPropertyBrowsableForType(typeof (Window))]
        public static bool GetIsHiddenCloseButton(Window obj)
        {
            return (bool) obj.GetValue(IsHiddenCloseButtonProperty);
        }

        private static void SetIsHiddenCloseButton(Window obj, bool value)
        {
            obj.SetValue(IsHiddenCloseButtonKey, value);
        }

        private static readonly Type OwnerType = typeof (WindowBehavior);

        public static readonly DependencyProperty HideCloseButtonProperty =
            DependencyProperty.RegisterAttached(
                "HideCloseButton",
                typeof (bool),
                OwnerType,
                new FrameworkPropertyMetadata(false, HideCloseButtonChangedCallback));

        private static readonly RoutedEventHandler HideWhenLoadedDelegate = (sender, args) =>
        {
            if (sender is Window == false)
            {
                return;
            }
            var w = (Window) sender;
            HideCloseButton(w);
            w.Loaded -= HideWhenLoadedDelegate;
        };

        private static readonly RoutedEventHandler ShowWhenLoadedDelegate = (sender, args) =>
        {
            if (sender is Window == false)
            {
                return;
            }
            var w = (Window) sender;
            HideCloseButton(w);
            w.Loaded -= ShowWhenLoadedDelegate;
        };

        private static readonly DependencyPropertyKey IsHiddenCloseButtonKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "IsHiddenCloseButton",
                typeof (bool),
                OwnerType,
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsHiddenCloseButtonProperty =
            IsHiddenCloseButtonKey.DependencyProperty;
    }
}