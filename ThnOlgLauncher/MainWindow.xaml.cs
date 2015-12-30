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

namespace ThnOlgLauncher {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //The operation was canceled by the user.
        private const int ERROR_CANCELLED = 1223;

        private DataStore data = new DataStore();
        private JsonStorage jsonStorage = new JsonStorage();

        public MainWindow() {
            InitializeComponent();
        }

        private void launchGame() {
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
                Process.Start(process);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e) {
            jsonStorage.loadJson(data);
            gameList.ItemsSource = data.games;
            serverList.ItemsSource = data.servers;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void launchButtonUpdate() {
            launchButton.IsEnabled =
                tabControl.SelectedItem.Equals(serverTab)
                && mainWindow.serverList.SelectedItem != null;
        }

        private void updateSaveButtonEnable() {
            saveButton.IsEnabled = jsonStorage.isDirty(data);
        }

        private void serverList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            launchButtonUpdate();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            launchButtonUpdate();
        }

        private void launchButton_Click(object sender, RoutedEventArgs e) {
            launchGame();
        }

        private void gameList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            updateSaveButtonEnable();
        }

        private void serverList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            updateSaveButtonEnable();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {
            try {
                jsonStorage.saveJson(data);
            } catch(System.UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message);
            }

            updateSaveButtonEnable();
        }

        private void serverList_KeyDown(object sender, KeyEventArgs e) {
            updateSaveButtonEnable();
        }

        private void gameList_KeyDown(object sender, KeyEventArgs e) {
            updateSaveButtonEnable();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            updateSaveButtonEnable();
        }

        private void serverList_CurrentCellChanged(object sender, EventArgs e) {
            updateSaveButtonEnable();
        }

        private void gameList_CurrentCellChanged(object sender, EventArgs e) {
            updateSaveButtonEnable();
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
            updateSaveButtonEnable();
        }

        private void linkText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process myProcess = new Process();

            try {
                // true is the default, but it is important not to set it to false
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = "http://www.thehellnet.org/";
                myProcess.Start();
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void ComboBox_Initialized(object sender, EventArgs e) {
            ComboBox comboBox = (ComboBox) sender;
            data.games.ForEach(game => comboBox.Items.Add(game.tag));
        }
    }
}
