using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThnOlgLauncher.model;

namespace ThnOlgLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //The operation was canceled by the user.
        const int ERROR_CANCELLED = 1223;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Game cod4Game = new Game();
            cod4Game.name = "Call of Duty 4";
            cod4Game.tag = "cod4";
            cod4Game.executable = @"C:\Program Files (x86)\Call of Duty 4\iw3mp.exe";
            cod4Game.runAsAdmin = false;

            Game cod2Game = new Game();
            cod2Game.name = "Call of Duty 2";
            cod2Game.tag = "cod2";
            cod2Game.executable = @"C:\Program Files (x86)\Call of Duty 2\cod2mp_s.exe";
            cod2Game.runAsAdmin = true;

            Server hellnetCod4Server = new Server();
            hellnetCod4Server.name = "The HellNet.org CoD4 Server";
            hellnetCod4Server.game = cod4Game;
            hellnetCod4Server.address = "cod4.thehellnet.org";
            hellnetCod4Server.port = 28964;

            Server hellnetCod2Server = new Server();
            hellnetCod2Server.name = "The HellNet.org CoD2 Server";
            hellnetCod2Server.game = cod2Game;
            hellnetCod2Server.address = "cod2.thehellnet.org";
            hellnetCod2Server.port = 28960;

            mainWindow.listView.Items.Add(hellnetCod4Server);
            mainWindow.listView.Items.Add(hellnetCod2Server);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mainWindow.launchButton.IsEnabled =
                mainWindow.listView.SelectedItem != null;
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            Server server = (Server) mainWindow.listView.SelectedItem;
            Game game = server.game;

            String arguments = " +connect " + server.address + ":" + server.port;
            String workingDirectory = new FileInfo(game.executable).Directory.FullName;

            ProcessStartInfo process = new ProcessStartInfo(game.executable);
            process.WorkingDirectory = workingDirectory;
            process.Arguments = arguments;
            process.UseShellExecute = true;
            if (game.tag.Equals("cod2")) {
                process.Verb = "runas";
            }

            try {
                Process.Start(process);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
