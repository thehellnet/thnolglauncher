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
using ThnOlgLauncher.controller;
using ThnOlgLauncher.model;

using System.Windows.Forms.DataVisualization.Charting;

using static System.Environment;
using ThnOlgLauncher.pinger;
using System.Windows.Controls.Primitives;

namespace ThnOlgLauncher {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //The operation was canceled by the user.
        private const int ERROR_CANCELLED = 1223;

        private DataStore data = new DataStore();
        private JsonStorage jsonStorage = new JsonStorage();
        private Process gameProcess;

        public MainWindow() {
            InitializeComponent();
        }

        private void launchServer() {
            Server server = (Server) mainWindow.serverList.SelectedItem;
            Game game = data.games.Find(item => item.tag == server.gameTag);
            if(game == null) {
                MessageBox.Show("Game not found!\nPlease add a game in Games tab and assign it to this server", "ERROR");
                return;
            }

            String arguments = " +connect " + server.address + ":" + server.port;
            String workingDirectory = new FileInfo(game.executable).Directory.FullName;

            ProcessStartInfo process = new ProcessStartInfo(game.executable);
            process.WorkingDirectory = workingDirectory;
            process.Arguments = arguments;
            process.UseShellExecute = true;
            if(game.runAsAdmin == true) {
                process.Verb = "runas";
            }

            try {
                gameProcess = Process.Start(process);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void launchGame() {
            Server server = (Server) mainWindow.serverList.SelectedItem;
            Game game = (Game) gameList.SelectedItem;
            if(game == null) {
                MessageBox.Show("Game not found!\nPlease add a game in Games tab and assign it to this server", "ERROR");
                return;
            }

            String workingDirectory = new FileInfo(game.executable).Directory.FullName;

            ProcessStartInfo process = new ProcessStartInfo(game.executable);
            process.WorkingDirectory = workingDirectory;
            process.UseShellExecute = true;
            if(game.runAsAdmin == true) {
                process.Verb = "runas";
            }

            try {
                Process.Start(process);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void linkOpen(String url) {
            Process myProcess = new Process();

            try {
                // true is the default, but it is important not to set it to false
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = url;
                myProcess.Start();
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private async void updateServerPingAndPlayers() {
            updatePingButton.IsEnabled = false;
            serverList.IsReadOnly = true;
            progressBar.Visibility = Visibility.Visible;
            setServerMoveButtonStatus(false);

            progressBar.Maximum = data.servers.Count;
            progressBar.Value = 0;

            var progress = new Progress<int>((value) => {
                serverList.Items.Refresh();
                progressBar.Value += 1;
            });

            await Task.Run(() => {
                data.servers.ForEach(s => updateServerInfos(s, progress));
            });

            serverList.Items.Refresh();

            progressBar.Value = 0;
            progressBar.Visibility = Visibility.Hidden;

            serverList.IsReadOnly = false;
            setServerMoveButtonStatus(true);
            updatePingButton.IsEnabled = true;
        }

        private void updateServerInfos(Server server, IProgress<int> progress) {
            PingResult pingResult = Pinger.pingServer(server);
            server.ping = pingResult.ping;
            server.players = String.Format("{0}/{1}", new object[] { pingResult.players, pingResult.maxPlayers });
            server.map = pingResult.map.Length > 0 ? pingResult.map + " (" + pingResult.gametype + ")" : "";
            progress.Report(0);
        }

        private void setDataBindings() {
            gameList.ItemsSource = data.games;
            serverList.ItemsSource = data.servers;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e) {
            progressBar.Visibility = Visibility.Hidden;

            jsonStorage.loadJson(data);
            setDataBindings();
            updateServerPingAndPlayers();
            setPingElementsEnable(false);
            //setServerMoveButtonStatus(true);
        }

        private void setServerMoveButtonStatus(bool status) {
            if(status && serverList.SelectedItem != null) {
                serverMoveUpButton.IsEnabled = serverList.SelectedIndex > 0;
                serverMoveDownButton.IsEnabled = serverList.SelectedIndex < data.servers.Count - 1;
            } else {
                serverMoveUpButton.IsEnabled = false;
                serverMoveDownButton.IsEnabled = false;
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void serverListUpdate(object sender, EventArgs e) {
            serverSaveButtonUpdateEnable();
            serverLaunchButtonUpdateEnable();
        }

        private void gameListUpdate(object sender, EventArgs e) {
            gameSaveButtonUpdateEnable();
            gameLaunchButtonUpdateEnable();
        }

        private void serverLaunchButtonUpdateEnable() {
            serverLaunchButton.IsEnabled = serverList.SelectedItem != null;
        }

        private void serverSaveButtonUpdateEnable() {
            serverSaveButton.IsEnabled = jsonStorage.isDirtyServer(data);
        }

        private void gameLaunchButtonUpdateEnable() {
            gameLaunchButton.IsEnabled = gameList.SelectedItem != null;
        }

        private void gameSaveButtonUpdateEnable() {
            gameSaveButton.IsEnabled = jsonStorage.isDirtyGame(data);
        }

        private void gameExecutableButton_Click(object sender, RoutedEventArgs e) {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Executables (*.exe)|*.exe";
            dialog.FilterIndex = 1;
            dialog.Multiselect = false;
            dialog.ShowDialog();
            String fileName = dialog.FileName;

            Game game = (Game) gameList.SelectedItem;
            game.executable = fileName;
            Button button = (Button) sender;
            System.Windows.Controls.Grid grid = (System.Windows.Controls.Grid) button.Parent;
            TextBlock textBlock = grid.Children.OfType<TextBlock>().First();
            textBlock.Text = fileName;
            serverSaveButtonUpdateEnable();
        }

        private void linkText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            linkOpen("http://www.thehellnet.org/");
        }

        private void GameSelectComboBox_Initialized(object sender, EventArgs e) {
            ComboBox comboBox = (ComboBox) sender;
            data.games.ForEach(game => comboBox.Items.Add(game.tag));
        }

        private void setPingElementsEnable(bool status) {
            if(status) {
                pingAddressPortText.IsEnabled = false;
                pingStartButton.IsEnabled = false;
                pingStopButton.IsEnabled = true;
            } else {
                pingAddressPortText.IsEnabled = true;
                pingStartButton.IsEnabled = true;
                pingStopButton.IsEnabled = false;
            }
        }

        private void serverLaunchButton_Click(object sender, RoutedEventArgs e) {
            launchServer();
        }

        private void gameLaunchButton_Click(object sender, RoutedEventArgs e) {
            launchGame();
        }

        private void serverSaveButton_Click(object sender, RoutedEventArgs e) {
            jsonStorage.saveJsonServers(data);
            serverSaveButtonUpdateEnable();
        }

        private void gameSaveButton_Click(object sender, RoutedEventArgs e) {
            jsonStorage.saveJsonGames(data);
            gameSaveButtonUpdateEnable();
        }

        private void updatePingButton_Click(object sender, RoutedEventArgs e) {
            updateServerPingAndPlayers();
        }

        private void serverListSelectionChange(object sender, SelectionChangedEventArgs e) {
            serverListUpdate(sender, e);
            setServerMoveButtonStatus(true);
        }

        private void serverMoveDownButton_Click(object sender, RoutedEventArgs e) {
            int index = serverList.SelectedIndex;
            Server tempServer = data.servers[index];
            data.servers[index] = data.servers[index + 1];
            data.servers[index + 1] = tempServer;
            serverList.Items.Refresh();
            setServerMoveButtonStatus(true);
        }

        private void serverMoveUpButton_Click(object sender, RoutedEventArgs e) {
            int index = serverList.SelectedIndex;
            Server tempServer = data.servers[index];
            data.servers[index] = data.servers[index - 1];
            data.servers[index - 1] = tempServer;
            serverList.Items.Refresh();
            setServerMoveButtonStatus(true);
        }
    }
}
