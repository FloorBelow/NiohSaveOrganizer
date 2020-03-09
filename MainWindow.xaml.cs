using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace NiohSaveOrganizer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Save[] saves;
		List<Save> storedSaves { get; set; }
		HashSet<string> storedSaveNames;
		string[] disallowedCharacters = new string[] { "/", "\\", "|", "*", "?", ":", "<", ">" };
		string path { get; set; }
		bool committing;

		public MainWindow() {
			InitializeComponent();
			this.DataContext = this;
			path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KoeiTecmo\\NIOH\\Savedata\\";
			if (Directory.GetDirectories(path) != null) {
				UpdatePath(Directory.GetDirectories(path)[0]);
			}
		}

		void UpdatePath(string newPath) {
				path = newPath;
				SaveLocationTextBox.Text = path;
				saves = new Save[100];
				storedSaves = new List<Save>();
				storedSaveNames = new HashSet<string>();
				UpdateSaves();
				UpdateStoredSaves();
				StoredDataGrid.ItemsSource = storedSaves;
				SavesDataGrid.ItemsSource = saves;
		}

		int UpdateSaves() {
			ImportRecentSaveButton.IsEnabled = true;
			DateTime mostRecent = DateTime.MinValue; int mostRecentIndex = -1;
			for (int i = 0; i < 100; i++) {
				string savePath = path + String.Format("\\SAVEDATA{0:00}", i);
				if (Directory.Exists(savePath) && File.Exists(savePath + "\\SAVEDATA.BIN")) {
					saves[i] = new Save(savePath);
					if (DateTime.Compare(saves[i].lastModified, mostRecent) > 0) { mostRecent = saves[i].lastModified; mostRecentIndex = i; }
				} else saves[i] = new Save(i);
			}
			return mostRecentIndex;
		}

		void UpdateStoredSaves() {
			storedSaves.Clear();
			storedSaveNames.Clear();
			if (!Directory.Exists(path + "\\Save Organizer")) return;
			foreach(string path in Directory.GetDirectories(path + "\\Save Organizer")) {
				if(File.Exists(path + "\\SAVEDATA.BIN")) {
					storedSaves.Add(new Save(Path.GetFileName(path), path + "\\SAVEDATA.BIN"));
					storedSaveNames.Add(Path.GetFileName(path));
				}
			}
		}

		Save ImportSave(Save save) {
			Save newSave = save.Copy();
			string name = newSave.name;
			int i = 0;
			while(storedSaveNames.Contains(name)) {
				i++;
				name = newSave.name + " " + i.ToString();
			}
			if (!Directory.Exists(path + "\\Save Organizer")) Directory.CreateDirectory(path + "\\Save Organizer");
			newSave.path = path + "\\Save Organizer\\" + name;
			Directory.CreateDirectory(newSave.path);
			File.Copy(save.GetFilePath(), newSave.path + "\\SAVEDATA.BIN");
			newSave.name = name;
			storedSaves.Add(newSave);
			storedSaveNames.Add(name);
			return newSave;
		}

		void SortStoredSaves(Save selectedSave) {
			storedSaves.Sort();
			StoredDataGrid.ItemsSource = null;
			StoredDataGrid.ItemsSource = storedSaves;
			StoredDataGrid.SelectedItem = selectedSave;
		}

		void SortStoredSaves() {
			SortStoredSaves((Save)StoredDataGrid.SelectedItem);
		}

		void StoredDataGridCellEdit(object sender, DataGridCellEditEndingEventArgs e) {
			if (committing) return;
			Save save = (Save)e.Row.Item;
			if (save.name == save.backupName) return;
			foreach(string d in disallowedCharacters) {
				if(save.name.Contains(d)) {
					e.Cancel = true;
					System.Windows.MessageBox.Show("Save names cannot contain / \\ | : * ? < >", "Error");
					return;
				}
			}
			if(storedSaveNames.Contains(save.name)) {
				e.Cancel = true;
				System.Windows.MessageBox.Show("Save names must be unique.", "Error");
				return;
			}
			committing = true;
			storedSaveNames.Remove(save.backupName);
			storedSaveNames.Add(save.name);
			string newpath = Path.GetDirectoryName(save.path) + "\\" + save.name;
			Directory.Move(save.path, newpath);
			save.path = newpath;
			StoredDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
			committing = false;
			SortStoredSaves(save);
		}

		private void StoredDataGridSelectionChanged(object sender, SelectedCellsChangedEventArgs e) {
			if (SavesDataGrid.SelectedItem != null) OverwriteSaveButton.IsEnabled = true;
			DeleteStoredSaveButton.IsEnabled = true;
		}

		private void SavesDataGridSelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (SavesDataGrid.SelectedItem == null) return;
			if (StoredDataGrid.SelectedItem != null) OverwriteSaveButton.IsEnabled = true;
			Save save = (Save)SavesDataGrid.SelectedItem;
			if (save.missionName != "") {
				ImportSelectedSaveButton.IsEnabled = true;
				DeleteSelectedSaveButton.IsEnabled = true;
			} else {
				ImportSelectedSaveButton.IsEnabled = false;
				DeleteSelectedSaveButton.IsEnabled = false;
			}
		}

		private void ImportMostRecentSaveButtonClick(object sender, RoutedEventArgs e) {
			int i = UpdateSaves();
			if (i >= 0) {
				Save newSave = ImportSave(saves[i]);
				SavesDataGrid.SelectedItem = saves[i];
				StoredDataGrid.SelectedItem = newSave;
				SortStoredSaves();
			}
		}

		private void ImportSelectedSaveButtonClick(object sender, RoutedEventArgs e) {
			Save save = (Save)SavesDataGrid.SelectedItem;
			Save newSave = ImportSave(save);
			StoredDataGrid.SelectedItem = newSave;
			SortStoredSaves();
		}

		private void DeleteSelectedSaveButtonClick(object sender, RoutedEventArgs e) {
			int i = SavesDataGrid.SelectedIndex;
			Save save = saves[i];
			Directory.Delete(save.path, true);
			saves[i] = new Save(i);
			SavesDataGrid.SelectedItem = saves[i];
			SavesDataGrid.Items.Refresh();
		}

		private void DeleteStoredSaveButtonClick(object sender, RoutedEventArgs e) {
			Save save = (Save)StoredDataGrid.SelectedItem;
			int i = StoredDataGrid.SelectedIndex;
			Directory.Delete(save.path, true);
			storedSaves.Remove(save);
			storedSaveNames.Remove(save.name);
			StoredDataGrid.SelectedItem = storedSaves[Math.Min(i, storedSaves.Count - 1)];
			StoredDataGrid.Items.Refresh();
		}

		private void OverwriteSaveButtonClick(object sender, RoutedEventArgs e) {
			Save storedsave = (Save)StoredDataGrid.SelectedItem;
			int saveIndex = SavesDataGrid.SelectedIndex;
			Save save = saves[saveIndex];
			saves[saveIndex] = storedsave.Copy();
			saves[saveIndex].indexName = String.Format("{0:00}", saveIndex);
			string savePath = path + String.Format("\\SAVEDATA{0:00}", saveIndex);
			if (save.missionName == "") {
				Directory.CreateDirectory(savePath);
			} else {
				File.Delete(savePath + "\\SAVEDATA.BIN");
			}
			File.Copy(storedsave.path + "\\SAVEDATA.BIN", savePath + "\\SAVEDATA.BIN");
			saves[saveIndex].path = savePath;
			SavesDataGrid.SelectedItem = saves[saveIndex];
			SavesDataGrid.Items.Refresh();
		}

		private void BrowsePathButtonClick(object sender, RoutedEventArgs e) {
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if(folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				UpdatePath(folderBrowserDialog.SelectedPath);
			}
		}

		private void WindowActivated(object sender, EventArgs e) {
			if (Directory.GetDirectories(path) != null) {
				int i = SavesDataGrid.SelectedIndex;
				UpdateSaves();
				SavesDataGrid.Items.Refresh();
				if(i > -1) SavesDataGrid.SelectedItem = saves[i];
			}
		}

	}
}
