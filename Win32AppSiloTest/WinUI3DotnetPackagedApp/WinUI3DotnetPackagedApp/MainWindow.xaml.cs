using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3DotnetPackagedApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            var inOutAppContainerLabel = Win32ApiUtilitiy.GetTokenIsAppContainer() ? "in" : "out";
            appContainerLabel.Text = $"current {inOutAppContainerLabel} app container";
            var installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            filePath.Text = installedLocation.Path;
        }

        private async void ReadUsingUWPFilePicker(object sender, RoutedEventArgs e)
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                WinRT.Interop.InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
                var file = await picker.PickSingleFileAsync();
                var content = await FileIO.ReadTextAsync(file);
                outputContent.Text = content;
            }
            catch (Exception err)
            {
                outputContent.Text = $"{err.GetType().Name}{Environment.NewLine}{err.Message}";
            }


        }
        private async void ReadUsingFileIO(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = filePath.Text;
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }
                var file = await StorageFile.GetFileFromPathAsync(path);
                var content = await FileIO.ReadTextAsync(file);
                outputContent.Text = content;
            }
            catch (Exception err)
            {
                outputContent.Text = $"{err.GetType().Name}{Environment.NewLine}{err.Message}";
            }
        }

        private void ReadUsingDotnetAPI(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = filePath.Text;
                var content = File.ReadAllText(path);
                outputContent.Text = content;
            }
            catch (Exception err)
            {
                outputContent.Text = $"{err.GetType().Name}{Environment.NewLine}{err.Message}";
            }
        }

        private async void ClearContent(object sender, RoutedEventArgs e)
        {
            outputContent.Text = "";
        }

        private async void ReadUsingFileDialog(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = await Win32ApiUtilitiy.PickFileViaDialog();
                filePath.Text = path;
                var content = File.ReadAllText(path);
                outputContent.Text = content;
            }
            catch (Exception err)
            {
                outputContent.Text = err.Message;
            }

        }
    }
}
