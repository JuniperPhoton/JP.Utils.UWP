using Microsoft.Xaml.Interactivity;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace JP.Utils.Framework
{
    public class InvokeCommandByKeyDown : DependencyObject, IAction
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandByKeyDown),
                new PropertyMetadata(null));

        public VirtualKey PressedKey
        {
            get { return (VirtualKey)GetValue(PressedKeyProperty); }
            set { SetValue(PressedKeyProperty, value); }
        }

        public static readonly DependencyProperty PressedKeyProperty =
            DependencyProperty.Register("PressedKey", typeof(VirtualKey), typeof(InvokeCommandByKeyDown),
                new PropertyMetadata(VirtualKey.None));

        public object Execute(object sender, object parameter)
        {
            KeyRoutedEventArgs keyPrarm = parameter as KeyRoutedEventArgs;
            if (keyPrarm != null)
            {
                if (keyPrarm.Key == PressedKey)
                {
                    Command.Execute(sender);
                    keyPrarm.Handled = true;
                }
            }
            else
            {
                KeyEventArgs param = parameter as KeyEventArgs;
                if (param != null && param.VirtualKey == PressedKey)
                {
                    Command.Execute(sender);
                    param.Handled = true;
                }
            }
            return null;
        }
    }
}