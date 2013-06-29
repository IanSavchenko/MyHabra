using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace UacApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (IsAdmin())
            {
                RestartPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ActionButton.IsEnabled = false;

                System.Drawing.Icon img = System.Drawing.SystemIcons.Shield;

                System.Drawing.Bitmap bitmap = img.ToBitmap();
                IntPtr hBitmap = bitmap.GetHbitmap();

                ImageSource wpfBitmap =
                     System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          hBitmap, IntPtr.Zero, Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());

                RestartButtonIcon.Source = wpfBitmap;
                RestartButtonIcon.Height = 16;
            }

            // Analyzing arguments
            string [] args = Environment.GetCommandLineArgs();
            if (args.Count() > 1)
            {
                this.Left = Int32.Parse(args[2]);
                this.Top = Int32.Parse(args[3]);
                this.Width = Int32.Parse(args[4]);
                this.Height = Int32.Parse(args[5]);
            }
        }

        public static bool IsAdmin()
        {
            System.Security.Principal.WindowsIdentity id = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal p = new System.Security.Principal.WindowsPrincipal(id);

            return p.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello, Habr!");
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            RestartAsAdmin();
        }

        private void RestartAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            startInfo.Arguments = "restart " + this.Left + " " 
                + this.Top + " " + this.Width + " " + this.Height;

            startInfo.Verb = "runas";
            try
            {
                Process p = Process.Start(startInfo);
                this.Close();
            }
            catch (Exception ex)
            {
                // UAC elevation failed
            }
        }

    }
}
