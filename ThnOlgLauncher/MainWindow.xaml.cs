using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThnOlgLauncher.model;
using ThnOlgLauncher.pinger;

namespace ThnOlgLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DataStore _data = new DataStore();
        private readonly JsonStorage _jsonStorage = new JsonStorage();
        private Process _gameProcess;

        private Thread _integrityThread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LaunchServer()
        {
            var server = (Server) mainWindow.serverList.SelectedItem;
            var game = _data.Games.Find(item => item.Tag == server.GameTag);
            if (game == null)
            {
                MessageBox.Show("Game not found!\nPlease add a game in Games tab and assign it to this server",
                    "ERROR");
                return;
            }

            var arguments = " +connect " + server.Address + ":" + server.Port;
            var directoryInfo = new FileInfo(game.Executable).Directory;
            if (directoryInfo == null)
                return;

            var workingDirectory = directoryInfo.FullName;

            var process = new ProcessStartInfo(game.Executable)
            {
                WorkingDirectory = workingDirectory,
                Arguments = arguments,
                UseShellExecute = true
            };

            if (game.RunAsAdmin)
            {
                process.Verb = "runas";
            }

            try
            {
                _gameProcess = Process.Start(process);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LaunchGame()
        {
            var game = (Game) gameList.SelectedItem;
            if (game == null)
            {
                MessageBox.Show("Game not found!\nPlease add a game in Games tab and assign it to this server",
                    "ERROR");
                return;
            }

            var directoryInfo = new FileInfo(game.Executable).Directory;
            if (directoryInfo == null)
                return;

            var workingDirectory = directoryInfo.FullName;

            var process = new ProcessStartInfo(game.Executable)
            {
                WorkingDirectory = workingDirectory,
                UseShellExecute = true
            };
            if (game.RunAsAdmin)
            {
                process.Verb = "runas";
            }

            try
            {
                Process.Start(process);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void LinkOpen(string url)
        {
            var myProcess = new Process();

            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = url;
                myProcess.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void UpdateServerPingAndPlayers()
        {
            updatePingButton.IsEnabled = false;
            serverList.IsReadOnly = true;
            progressBar.Visibility = Visibility.Visible;
            SetServerMoveButtonStatus(false);

            progressBar.Maximum = _data.Servers.Count;
            progressBar.Value = 0;

            var progress = new Progress<int>((value) =>
            {
                serverList.Items.Refresh();
                progressBar.Value += 1;
            });

            await Task.Run(() => { _data.Servers.ForEach(s => UpdateServerInfos(s, progress)); });

            serverList.Items.Refresh();

            progressBar.Value = 0;
            progressBar.Visibility = Visibility.Hidden;

            serverList.IsReadOnly = false;
            SetServerMoveButtonStatus(true);
            updatePingButton.IsEnabled = true;
        }

        private static void UpdateServerInfos(Server server, IProgress<int> progress)
        {
            var pingResult = Pinger.PingServer(server);
            server.Ping = pingResult.Ping;
            server.Players = string.Format("{0}/{1}", new object[] {pingResult.Players, pingResult.MaxPlayers});
            server.Map = pingResult.Map.Length > 0 ? pingResult.Map + " (" + pingResult.Gametype + ")" : "";
            progress.Report(0);
        }

        private void SetDataBindings()
        {
            gameList.ItemsSource = _data.Games;
            serverList.ItemsSource = _data.Servers;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PrepareUi();

            _jsonStorage.LoadJson(_data);
            SetDataBindings();
            UpdateServerPingAndPlayers();
            SetPingElementsEnable(false);
            //setServerMoveButtonStatus(true);

            IntegrityButtonsEnable();
        }

        private void PrepareUi()
        {
            progressBar.Visibility = Visibility.Hidden;

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            versionText.Text = "Version " + fvi.FileVersion;
        }

        private void SetServerMoveButtonStatus(bool status)
        {
            if (status && serverList.SelectedItem != null)
            {
                serverMoveUpButton.IsEnabled = serverList.SelectedIndex > 0;
                serverMoveDownButton.IsEnabled = serverList.SelectedIndex < _data.Servers.Count - 1;
            }
            else
            {
                serverMoveUpButton.IsEnabled = false;
                serverMoveDownButton.IsEnabled = false;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ServerListUpdate(object sender, EventArgs e)
        {
            ServerSaveButtonUpdateEnable();
            ServerLaunchButtonUpdateEnable();
        }

        private void GameListUpdate(object sender, EventArgs e)
        {
            GameSaveButtonUpdateEnable();
            GameLaunchButtonUpdateEnable();
        }

        private void ServerLaunchButtonUpdateEnable()
        {
            serverLaunchButton.IsEnabled = serverList.SelectedItem != null;
        }

        private void ServerSaveButtonUpdateEnable()
        {
            serverSaveButton.IsEnabled = _jsonStorage.IsDirtyServer(_data);
        }

        private void GameLaunchButtonUpdateEnable()
        {
            gameLaunchButton.IsEnabled = gameList.SelectedItem != null;
        }

        private void GameSaveButtonUpdateEnable()
        {
            gameSaveButton.IsEnabled = _jsonStorage.IsDirtyGame(_data);
        }

        private void GameExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog =
                new System.Windows.Forms.OpenFileDialog
                {
                    Filter = @"Executables (*.exe)|*.exe",
                    FilterIndex = 1,
                    Multiselect = false
                };
            dialog.ShowDialog();
            var fileName = dialog.FileName;

            var game = (Game) gameList.SelectedItem;
            game.Executable = fileName;
            var button = (Button) sender;
            var grid = (Grid) button.Parent;
            var textBlock = grid.Children.OfType<TextBlock>().First();
            textBlock.Text = fileName;
            ServerSaveButtonUpdateEnable();
        }

        private void LinkText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LinkOpen("http://www.thehellnet.org/");
        }

        private void GameSelectComboBox_Initialized(object sender, EventArgs e)
        {
            var comboBox = (ComboBox) sender;
            _data.Games.ForEach(game => comboBox.Items.Add(game.Tag));
        }

        private void SetPingElementsEnable(bool status)
        {
            if (status)
            {
                pingAddressPortText.IsEnabled = false;
                pingStartButton.IsEnabled = false;
                pingStopButton.IsEnabled = true;
            }
            else
            {
                pingAddressPortText.IsEnabled = true;
                pingStartButton.IsEnabled = true;
                pingStopButton.IsEnabled = false;
            }
        }

        private void ServerLaunchButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchServer();
        }

        private void GameLaunchButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchGame();
        }

        private void ServerSaveButton_Click(object sender, RoutedEventArgs e)
        {
            _jsonStorage.SaveJsonServers(_data);
            ServerSaveButtonUpdateEnable();
        }

        private void GameSaveButton_Click(object sender, RoutedEventArgs e)
        {
            _jsonStorage.SaveJsonGames(_data);
            GameSaveButtonUpdateEnable();
        }

        private void UpdatePingButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateServerPingAndPlayers();
        }

        private void ServerListSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            ServerListUpdate(sender, e);
            SetServerMoveButtonStatus(true);
        }

        private void ServerMoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            var index = serverList.SelectedIndex;
            var tempServer = _data.Servers[index];
            _data.Servers[index] = _data.Servers[index + 1];
            _data.Servers[index + 1] = tempServer;
            serverList.Items.Refresh();
            SetServerMoveButtonStatus(true);
        }

        private void ServerMoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            var index = serverList.SelectedIndex;
            var tempServer = _data.Servers[index];
            _data.Servers[index] = _data.Servers[index - 1];
            _data.Servers[index - 1] = tempServer;
            serverList.Items.Refresh();
            SetServerMoveButtonStatus(true);
        }

        private void IntegrityFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = @"All hash file (*.md5, *.sha1, *.sha256, *.sha512)|*.md5;*.sha1;*.sha256;*.sha512;
                |MD5 hash file (*.md5)|*.md5
                |SHA 1 hash file (*.sha1)|*.sha1
                |SHA 256 hash file (*.sha256)|*.sha256
                |SHA 512 hash file (*.sha512)|*.sha512",
                FilterIndex = 1,
                Multiselect = false
            };
            dialog.ShowDialog();
            var fileName = dialog.FileName;

            integrityFileText.Text = fileName;
        }

        private void IntegrityCheckButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void IntegrityCalculateButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void IntegrityStopButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void IntegrityButtonsEnable()
        {
            var threadRunning = _integrityThread != null && _integrityThread.IsAlive;

            if (threadRunning)
            {
                integrityStopButton.IsEnabled = true;
                integrityCheckButton.IsEnabled = false;
                integrityCalculateButton.IsEnabled = false;
                return;
            }

            var algorithmSelected = integrityAlgorithmCombo.SelectedIndex != -1;
            var gameSelected = integrityGameCombo.SelectedIndex != -1;

            integrityStopButton.IsEnabled = false;
            integrityCheckButton.IsEnabled = algorithmSelected && gameSelected;
            integrityCalculateButton.IsEnabled = algorithmSelected && gameSelected;
        }
    }
}