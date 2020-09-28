using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace NDSRenameUI
{
    public partial class MainWindow : Window
    {
        private NDSMetadataProcessor _ndsMetaDataProcessor;
        private NDSMetaData _ndsMetaData;

        public MainWindow()
        {
            this._ndsMetaData = new NDSMetaData();
            try
            {
                this._ndsMetaDataProcessor = new NDSMetadataProcessor();
            } catch (NDSDatabaseLoadException e)
            {
                this.DisplayError(e.Message);
            }
            this.InitializeComponent();
        }

        private void ResetView()
        {
            this.PartialNameTextBox.Text = "";
            this.NewFileNameTextBox.Text = "";
            this.GameIdTextBox.Text = "";
            this.GameTDBMappingTextBox.Text = "";
            this.CurrentFileNameTextBox.Text = "";
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            var result = openFileDlg.ShowDialog();
            if (result == true)
            {
                this.ResetView();
                try
                {
                    var ndsMetaData = this._ndsMetaDataProcessor.ProcessNDSFile(openFileDlg);
                    this.PartialNameTextBox.Text = ndsMetaData.PartialName;
                    this.NewFileNameTextBox.Text = ndsMetaData.NewFileName;
                    this.GameIdTextBox.Text = ndsMetaData.GameId;
                    this.GameTDBMappingTextBox.Text = ndsMetaData.GameTDBMapping;
                    this.CurrentFileNameTextBox.Text = ndsMetaData.CurrentFileName;
                    this.FileNameTextBox.Text = ndsMetaData.FilePath;
                    this._ndsMetaData = ndsMetaData;
                }
                catch (NDSFileStreamException ex)
                {
                    this.DisplayError(ex.Message);
                }
                catch (NDSFileException ex)
                {
                    this.DisplayError(ex.Message);
                }
            }
        }

        private void DisplayError(string error) => MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        private void DisplayRenameSuccess() => MessageBox.Show("Rename Successful", "Complete", MessageBoxButton.OK, MessageBoxImage.Information);

        private void RenameFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ndsDirectory = Path.GetDirectoryName(this._ndsMetaData.FilePath);
                if (string.IsNullOrEmpty(ndsDirectory))
                {
                    this.DisplayError("nds directory not found");
                }
                else if (string.IsNullOrEmpty(this._ndsMetaData.CurrentFileName))
                {
                    this.DisplayError("nds file not found");
                }
                else
                {
                    File.Move(this._ndsMetaData.FilePath, ndsDirectory + "\\" + this._ndsMetaData.NewFileName + ".nds");
                    this.DisplayRenameSuccess();
                    return;
                }
            }
            catch(IOException ex)
            {
                this.DisplayError(ex.Message);
            }
            finally
            {
                this.ResetView();
            }
            this.DisplayError("Unknown Error");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
