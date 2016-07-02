// This file is part of Sdict2db.
//
// Sdict2db is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sdict2db is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sdict2db. if not, see <http://www.gnu.org/licenses/>.
//
// Copyright 2008 Alla Morgunova

using System;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Text;
using System.Resources;
using MSDASC;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using Sdict2db.Helpers;
using Sdict2db.Events;
using Sdict2db.Properties;
using Sdict2db.RecentFilesMenu;
using Sdict2db.PagedGridView;
using Sdict2db.Parsers;

namespace Sdict2db {

	/// <summary>
	/// GUI class implementing IMainView interface.
	/// Performs all interactions with user.
	/// </summary>
	public partial class MainForm : Form, IMainView {

		#region Constants

		#endregion

		#region Private members

		private bool _isBusy;
		private DictHeader _header;
		private const string PercentChar = "%";
		private RecentFilesMenuItem _recentFilesMenu;
		private PagedDataGridView _dictionaryView;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the MainForm class.
		/// </summary>
		public MainForm() {
			Resources.Culture = Thread.CurrentThread.CurrentUICulture;
			InitializeComponent();
			connectionStringTextBox.Text = Settings.Default.ConnectionString;
			tableNameTextBox.Text = Settings.Default.TableSuffix;
			saveToolStripMenuItem.Enabled = false;
			progressBar.Visible = false;
			percentageLabel.Visible = false;
			searchGroupBox.Visible = false;
			cancelButton.DropDownButtonWidth = 0;
			ruToolStripMenuItem.Checked =
				(Resources.Culture.IetfLanguageTag == "ru-RU");
			enUSToolStripMenuItem.Checked = !ruToolStripMenuItem.Checked;
			_recentFilesMenu = new RecentFilesMenuItem(
				recentFilesToolStripMenuItem, OpenFile, 10, 60);
			_recentFilesMenu.Load(StringHelper.ConcatPath(
				Application.LocalUserAppDataPath, Settings.Default.AppIniFilename));
			_dictionaryView = new PagedDataGridView(viewPanel);
		}

		#endregion

		#region IMainView interface realization

		#region Events

		/// <summary>
		/// The event is raised after retrieving SQL parameters from user
		/// in order to invoke further SQL operations.
		/// Event argument contains SQL connection string and table name.
		/// </summary>
		public event EventHandler<GetSqlParametersEventArgs> OnGetSqlParameters;

		private void OnGetSqlParametersHandler(object sender,
				GetSqlParametersEventArgs e) {
			if (OnGetSqlParameters != null) {
				OnGetSqlParameters(this, e);
			}
		}

		/// <summary>
		/// The event is raised after user chose saving dictionary data to file.
		/// Event argument contains file name to be saved.
		/// </summary>
		public event EventHandler<FileOpsEventArgs> OnSaveFile;

		private void OnSaveFileHandler(object sender, FileOpsEventArgs e) {
			if (OnSaveFile != null) {
				OnSaveFile(this, e);
			}
		}

		/// <summary>
		/// The event is raised after user chose loading
		/// dictionary data from file.
		/// Event argument should contain name of file to be parsed.
		/// </summary>
		public event EventHandler<FileOpsEventArgs> OnOpenFile;

		private void OnOpenFileHandler(object sender, FileOpsEventArgs e) {
			if (OnOpenFile != null) {
				OnOpenFile(this, e);
			}
		}

		/// <summary>
		/// The event is raised on cancelling operation.
		/// </summary>
		public event EventHandler<EventArgs> OnCancel;

		private void OnCancelHandler(object sender, EventArgs e) {
			if (OnCancel != null) {
				OnCancel(this, e);
			}
		}

		/// <summary>
		/// This event is raised on cancelling operation
		/// when it is required to remove all changes that were made.
		/// </summary>
		public event EventHandler<EventArgs> OnHardCancel;

		private void OnHardCancelHandler(object sender, EventArgs e) {
			if (OnHardCancel != null) {
				OnHardCancel(this, e);
			}
		}

		/// <summary>
		/// This event is raised when a background operation is 
		/// required to be paused.
		/// </summary>
		public event EventHandler<EventArgs> PauseWorker;

		private void PauseWorkerHandler(object sender, EventArgs e) {
			if (PauseWorker != null) {
				PauseWorker(this, e);
			}
		}

		/// <summary>
		/// This event is raised when a background operation is 
		/// required to be resumed.
		/// </summary>
		public event EventHandler<EventArgs> ResumeWorker;

		private void ResumeWorkerHandler(object sender, EventArgs e) {
			if (ResumeWorker != null) {
				ResumeWorker(this, e);
			}
		}

		#endregion

		#region Event handlers

		/// <summary>
		/// Outputs changing of progress to status bar.
		/// </summary>
		/// <param name="sender">
		/// Object which has raised event.
		/// </param>
		/// <param name="e">
		/// Arguments, containing information about progress.
		/// </param>
		public void BackgroundWorkChangedHandler(object sender,
				ProgressChangedEventArgs e) {
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			progressBar.Value = e.ProgressPercentage;
			percentageLabel.Text = e.ProgressPercentage.ToString(
				CultureInfo.CurrentCulture) + PercentChar;
		}

		/// <summary>
		/// Outputs the result of file parsing to data grid,
		/// unblocks intarface.
		/// </summary>
		/// <param name="sender">
		/// Object which has raised event.
		/// </param>
		/// <param name="e">
		/// Event arguments, describing operation completion.
		/// </param>
		/// <param name="table">
		/// Represents parsed dictionary data. If operation has been canceled,
		/// the table contains partial data.
		/// </param>
		public void OnBackgroundParsingCompletedHandler(object sender,
				RunWorkerCompletedEventArgs e, DataTable table) {
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			if (e.Cancelled) {
				MessageHelper.ShowInfoMessage(Resources.OperationCanceledText,
					Resources.CancelText, RightToLeft);
			}
			if ((table != null) && (table.Rows.Count > 0)) {
				searchGroupBox.Show();
				saveToolStripMenuItem.Enabled = true;
				_dictionaryView.SourceData = table;
			} else if ((table != null) && (!e.Cancelled)) {
				MessageHelper.ShowInfoMessage(Resources.EmptyDictionaryText,
					Resources.InfoText, RightToLeft);
			}
			progressBar.Value = 0;
			percentageLabel.Text = Resources.ZeroPercentageText;
			UnblockInterface();
		}

		/// <summary>
		/// Unblocks intarface. after writing of dictionary file
		/// has been completed.
		/// </summary>
		/// <param name="sender">
		/// Object which has raised event.
		/// </param>
		/// <param name="e">
		/// Event arguments, describing operation completion.
		/// </param>
		public void OnBackgroundWritingCompletedHandler(object sender,
				RunWorkerCompletedEventArgs e) {
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			if (e.Cancelled) {
				MessageHelper.ShowInfoMessage(Resources.OperationCanceledText,
					Resources.CancelText, RightToLeft);
			}
			progressBar.Value = 0;
			percentageLabel.Text = Resources.ZeroPercentageText;
			UnblockInterface();
		}

		/// <summary>
		/// Unblocks intarface. after filling of SQL data table with dictionary.
		/// </summary>
		/// <param name="sender">
		/// Object which has raised event.
		/// </param>
		/// <param name="e">
		/// Event arguments, containing results of completed operation.
		/// </param>
		public void OnSqlBackgroundCompletedHandler(object sender,
				RunWorkerCompletedEventArgs e) {
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			if (e.Cancelled) {
				MessageHelper.ShowInfoMessage(Resources.OperationCanceledText,
					Resources.CancelText, RightToLeft);
			}
			progressBar.Value = 0;
			percentageLabel.Text = Resources.ZeroPercentageText;
			UnblockInterface();
		}

		#endregion

		#region Other stuff

		/// <summary>
		/// Outputs dict-file header information to info text box.
		/// </summary>
		/// <param name="hdr">
		/// Parsed header.
		/// </param>
		public void OutputHeaderInfo(DictHeader hdr) {
			_header = hdr;
			if (hdr == null) {
				return;
			}
			OutputHeaderMainInfo();
			OutputHeaderAdditionalInfo();

		}

		private void OutputHeaderMainInfo() {
			StringBuilder sb = new StringBuilder();
			//ResourceManager rm = new ResourceManager("Resources",
			//	typeof(MainForm).Assembly);
			//string btnHello = rm.GetString("HeaderInfoText", CultureInfo.CurrentUICulture);
			//sb.AppendLine(btnHello);
			sb.AppendLine(Resources.HeaderInfoText);
			sb.AppendLine(Resources.TitleText);
			sb.AppendLine(Resources.CopyrightText);
			sb.AppendLine(Resources.VersionText);
			infoLabel.Text = sb.ToString();
			sb.Remove(0, sb.Length);
			sb.AppendLine(Resources.HeaderInfoAdditionalText);
			sb.AppendLine(StringHelper.ToCString(_header.Title));
			sb.AppendLine(StringHelper.ToCString(_header.Copyright));
			sb.AppendLine(StringHelper.ToCString(_header.Version));
			infoValuesLabel.Text = sb.ToString();
		}

		private void OutputHeaderAdditionalInfo() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(Tab() + Resources.AdditionalInfoText);
			sb.AppendLine(StringHelper.ToTabString(Resources.SignatureText) +
				StringHelper.ToCString(_header.Signature));
			sb.AppendLine(StringHelper.ToTabString(Resources.InputLanguageText) +
				StringHelper.ToCString(_header.InputLanguage));
			sb.AppendLine(StringHelper.ToTabString(Resources.OutputLanguageText) +
				StringHelper.ToCString(_header.OutputLanguage));
			sb.AppendLine(StringHelper.ToTabString(Resources.CompressText) +
				StringHelper.ToCString(_header.Compress));
			sb.AppendLine(StringHelper.ToTabString(Resources.IndexText) +
				StringHelper.ToCString(_header.Index));
			sb.AppendLine(StringHelper.ToTabString(Resources.AmountText) +
				StringHelper.ToCString(_header.Amount));
			sb.AppendLine(StringHelper.ToTabString(Resources.LenghtShotText) +
				StringHelper.ToCString(_header.LenghtShot));
			sb.AppendLine(StringHelper.ToTabString(Resources.TitleLengthText) +
				StringHelper.ToCString(_header.TitleLength));
			sb.AppendLine(StringHelper.ToTabString(Resources.CopyrightLengthText) +
				StringHelper.ToCString(_header.CopyrightLength));
			sb.AppendLine(StringHelper.ToTabString(Resources.VersionLengthText) +
				StringHelper.ToCString(_header.VersionLength));
			sb.AppendLine(StringHelper.ToTabString(Resources.OffsetShotIndexText) +
				StringHelper.ToCString(_header.OffsetShotIndex));
			sb.AppendLine(StringHelper.ToTabString(Resources.OffsetWordsIndexText) +
				StringHelper.ToCString(_header.OffsetWordsIndex));
			sb.AppendLine(StringHelper.ToTabString(Resources.OffsetArticlesText) +
				StringHelper.ToCString(_header.OffsetArticles));
			infoToolTip.ToolTipTitle = sb.ToString();
			infoToolTip.SetToolTip(infoLabel, sb.ToString());
			infoToolTip.SetToolTip(infoValuesLabel, sb.ToString());
		}

		private static string Tab() {
			return Tab(1);
		}

		private static string Tab(int n) {
			if (n < 1) {
				n = 1;
			}
			char tabChar = (char)Keys.Tab;
			StringBuilder result = new StringBuilder();
			for(int i = 0; i < n; ++i) {
				result.Append(tabChar.ToString());
			}
			return result.ToString();
		}

		/// <summary>
		/// Outputs given status info text to status label.
		/// </summary>
		/// <param name="info">
		/// Some information text to be shown.
		/// </param>
		public void OutputStatusInfo(string info) {
			statusLabel.ToolTipText = info;
			if (info != null) {
				char[] lineSpacers = { '\n', '\r' };
				info = info.Substring(info.LastIndexOfAny(lineSpacers) + 1);
			}
			statusLabel.Text = info;
		}

		/// <summary>
		/// Unblocks interface.
		/// </summary>
		public void ChangeInterfaceAfterError() {
			UnblockInterface();
		}

		#endregion

		#endregion

		#region Menu

		private void OpenMenuClick(object sender, EventArgs e) {
			if (openFileDialog.ShowDialog() == DialogResult.OK &&
				!string.IsNullOrEmpty(openFileDialog.FileName)) {
				OpenFile(openFileDialog.FileName);
			}
		}

		private void OpenFile(string filename) {
			if (!System.IO.File.Exists(filename)) {
				MessageHelper.ShowErrorMessage(Resources.FileDoesNotExistText,
					RightToLeft);
				return;
			}
			_recentFilesMenu.AddFile(filename);
			_dictionaryView.Clear();
			searchWordTextBox.Text = string.Empty;
			searchTranslateTextBox.Text = string.Empty;
			filterCheckBox.Checked = false;
			searchGroupBox.Hide();
			statusLabel.Text = Resources.OperationParsingText;
			statusLabel.ToolTipText = string.Empty;
			saveToolStripMenuItem.Enabled = false;
			infoLabel.Text = Resources.DefaultHeaderInfoText;
			infoValuesLabel.Text = Resources.NoneText;
			infoToolTip.SetToolTip(infoLabel, Resources.NoneText);
			infoToolTip.SetToolTip(infoValuesLabel, Resources.NoneText);
			BlockInterface();
			FileOpsEventArgs args =
				new FileOpsEventArgs(filename, null);
			OnOpenFileHandler(this, args);
		}

		private void SaveMenuClick(object sender, EventArgs e) {
			DataTable dictionary = AskForFilteredData(Resources.SaveText);
			if (dictionary == null) {
				return;
			}
			DialogResult result = saveFileDialog.ShowDialog();
			string filename = saveFileDialog.FileName;
			if (result == DialogResult.OK &&
				!string.IsNullOrEmpty(filename)) {
				FileOpsEventArgs args =
					new FileOpsEventArgs(saveFileDialog.FileName,
					dictionary);
				BlockInterface();
				statusLabel.Text = Resources.OperationWritingText;
				statusLabel.ToolTipText = string.Empty;
				OnSaveFileHandler(this, args);
			}
		}

		private DataTable AskForFilteredData(string caption) {
			if (_dictionaryView.Count == 0) {
				MessageHelper.ShowInfoMessage(Resources.EmptyDictionaryText,
					Resources.InfoText, RightToLeft);
				return null;
			}
			if (_dictionaryView.Count == _dictionaryView.CurrentCount) {
				return _dictionaryView.SourceData;
			}
			DialogResult result = MessageHelper.ShowQuestionMessage(
				Resources.FilteredDataMessageQuestion, caption, RightToLeft);
			if (result == DialogResult.Yes) {
				if (_dictionaryView.CurrentCount > 0) {
					// Use partial data:
					return _dictionaryView.CurrentData;
				} else {
					MessageHelper.ShowInfoMessage(Resources.EmptyDictionaryText,
						Resources.InfoText, RightToLeft);
					return null;
				}
			} else if (result == DialogResult.No) {
				// Use all data:
				return _dictionaryView.SourceData;
			}
			return null;
		}

		private void CancelButtonClick(object sender, EventArgs e) {
			CancelOperation(sender, e);
		}

		private void CancelOperation(object sender, EventArgs e) {
			PauseWorkerHandler(sender, e);
			DialogResult result = MessageHelper.ShowQuestionMessage(
				Resources.CancelMessageQuestion, Resources.CancelText,
				RightToLeft);
			if (result == DialogResult.Yes) {
				// Keep partial data:
				OnCancelHandler(sender, e);
			} else if (result == DialogResult.No) {
				// Remove partial data:
				OnHardCancelHandler(sender, e);
			} else if (result == DialogResult.Cancel) {
				ResumeWorkerHandler(sender, e);
			}
		}

		private void CreateDbTableButtonClick(object sender, EventArgs e) {
			string tableName = String.Format(CultureInfo.CurrentCulture,
				Resources.TableNamePattern, tableNameTextBox.Text);
			if (CheckConnectionString(connectionStringTextBox.Text) == null) {
				return;
			}
			if (!StringValidator.IsValidTableName(tableName)) {
				tableNameErrorProvider.SetError(tableNameTextBox,
					Resources.MessageValidationQueryStringTableErrorText);
				return;
			}
			DataTable dictionary =
				AskForFilteredData(Resources.CreateDataTableText);
			if (dictionary != null) {
				BlockInterface();
				GetSqlParametersEventArgs args = new GetSqlParametersEventArgs(
					connectionStringTextBox.Text, tableName, dictionary);
				statusLabel.ToolTipText = string.Empty;
				OnGetSqlParametersHandler(this, args);
				statusLabel.Text = Resources.OperationSqlText;
			}
		}

		private void ConnectionStringToolStripMenuItemClick(object sender,
				EventArgs e) {
			DataLinks connectionDlg = new DataLinks();
			if (connectionDlg == null) {
				return;
			}
			ADODB._Connection connection = null;
			try {
				connection = EditConnectionString(connectionDlg);
			} catch (NullReferenceException) {
				connection = NewConnectionString(connectionDlg);
			} catch (COMException) {
				connection = NewConnectionString(connectionDlg);
			}
			if (connection != null) {
				OutputConnectionString(connection);
			}
		}

		private ADODB._Connection NewConnectionString(
				DataLinks connectionDlg) {
			try {
				return (ADODB._Connection)connectionDlg.PromptNew();
			} catch (COMException ex) {
				MessageHelper.ShowErrorMessage(ex.Message, RightToLeft);
				return null;
			}
		}

		private ADODB._Connection EditConnectionString(DataLinks
				connectionDlg) {
			ADODB._Connection connection = new ADODB.ConnectionClass();
			try {
				OleDbConnectionStringBuilder builderOLE =
					new OleDbConnectionStringBuilder(
					connectionStringTextBox.Text);
				if (string.IsNullOrEmpty(builderOLE.Provider)) {
					builderOLE.Provider = Settings.Default.DefaultSqlProvider;
				}
				connection.ConnectionString = builderOLE.ConnectionString;
			} catch (ArgumentException ex) {
				MessageHelper.ShowErrorMessage(ex.Message, RightToLeft);
			}
			object oConnection = connection;
			if (!connectionDlg.PromptEdit(ref oConnection)) {
				connection = null;
			}
			return connection;
		}

		private void OutputConnectionString(ADODB._Connection connection) {
			OleDbConnectionStringBuilder builderOLE;
			try {
				builderOLE = new OleDbConnectionStringBuilder(
					connection.ConnectionString);
				builderOLE.Remove("Provider");
				builderOLE.Remove("Extended Properties");
				connectionStringTextBox.Text = builderOLE.ToString();
				CheckConnectionString(connectionStringTextBox.Text);
			} catch (ArgumentNullException ex) {
				MessageHelper.ShowErrorMessage(ex.Message, RightToLeft);
			} catch (ArgumentException ex) {
				MessageHelper.ShowErrorMessage(ex.Message, RightToLeft);
			}
		}

		SqlConnectionStringBuilder CheckConnectionString(
				string connString) {
			try {
				SqlConnectionStringBuilder sb =
					new SqlConnectionStringBuilder(connString);
				return sb;
			} catch (KeyNotFoundException ex) {
				connectionStringErrorProvider.SetError(connectionStringTextBox,
				ex.Message);
			} catch (FormatException ex) {
				connectionStringErrorProvider.SetError(connectionStringTextBox,
				ex.Message);
			} catch (ArgumentException ex) {
				connectionStringErrorProvider.SetError(connectionStringTextBox,
				ex.Message);
			}
			return null;
		}

		private void ChangeLanguageRadioClick(object sender, EventArgs e) {
			SuspendLayout();
			enUSToolStripMenuItem.Checked = (sender ==
				enUSToolStripMenuItem);
			ruToolStripMenuItem.Checked = (sender ==
				ruToolStripMenuItem);
			if (ruToolStripMenuItem.Checked) {
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
				Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
			} else {
				// English language is default:
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("");
				Thread.CurrentThread.CurrentCulture = new CultureInfo("");
			}
			bool searchIsVisible = searchGroupBox.Visible;
			ApplyResources();
			searchGroupBox.Visible = searchIsVisible;
			ResumeLayout();
		}

		void ExitMenuClick(object sender, EventArgs e) {
			Close();
		}

		#endregion

		#region Interface event handling

		void MainFormClosing(object sender, FormClosingEventArgs e) {
			if (_isBusy) {
				e.Cancel = true;
			} else {
				_recentFilesMenu.Save(StringHelper.ConcatPath(
				Application.LocalUserAppDataPath,
				Settings.Default.AppIniFilename));
			}
		}

		void QueryStringTableChanged(object sender, EventArgs e) {
			tableNameErrorProvider.Clear();
		}

		void QueryStringChanged(object sender, EventArgs e) {
			connectionStringErrorProvider.Clear();
		}

		private void SearchWordTextBoxTextChanged(object sender, EventArgs e) {
			if (filterCheckBox.Checked) {
				string[] fields = new string[2];
				fields[0] = Resources.WordField;
				fields[1] = Resources.TranslateField;
				string[] values = new string[2];
				values[0] = searchWordTextBox.Text;
				values[1] = searchTranslateTextBox.Text;
				_dictionaryView.Filter(fields, values);
			} else {
				_dictionaryView.Search(Resources.WordField,
					searchWordTextBox.Text);
			}
		}

		private void SearchTranslateTextBoxTextChanged(object sender,
				EventArgs e) {
			if (filterCheckBox.Checked) {
				string[] fields = new string[2];
				fields[0] = Resources.WordField;
				fields[1] = Resources.TranslateField;
				string[] values = new string[2];
				values[0] = searchWordTextBox.Text;
				values[1] = searchTranslateTextBox.Text;
				_dictionaryView.Filter(fields, values);
			} else {
				_dictionaryView.Search(Resources.TranslateField,
					searchTranslateTextBox.Text);
			}
		}

		private void FilterCheckBoxCheckedChanged(object sender, EventArgs e) {
			string[] fields = new string[2];
			fields[0] = Resources.WordField;
			fields[1] = Resources.TranslateField;
			string[] values = new string[2];
			if (!filterCheckBox.Checked) {
				values[0] = string.Empty;
				values[1] = string.Empty;
				_dictionaryView.Filter(fields, values);
				if (!string.IsNullOrEmpty(searchWordTextBox.Text)) {
					_dictionaryView.Search(Resources.WordField,
						searchWordTextBox.Text);
				} else if (!string.IsNullOrEmpty(searchTranslateTextBox.Text)) {
					_dictionaryView.Search(Resources.TranslateField,
						searchTranslateTextBox.Text);
				}
			} else {
				values[0] = searchWordTextBox.Text;
				values[1] = searchTranslateTextBox.Text;
				_dictionaryView.Filter(fields, values);
			}
			searchWordTextBox.Focus();
		}

		private void MainFormKeyDown(object sender, KeyEventArgs e) {
			if (_isBusy && e.KeyCode == Keys.Escape) {
				CancelOperation(sender, e);
			}
		}

		#endregion

		#region Interface stuff

		void BlockInterface() {
			_isBusy = true;

			progressBar.Visible = true;
			percentageLabel.Visible = true;
			cancelButton.Visible = true;

			fileToolStripMenuItem.Enabled = false;
			optionsToolStripMenuItem.Enabled = false;

			createDbTableButton.Enabled = false;
			tableNameTextBox.Enabled = false;
			connectionStringButton.Enabled = false;
			connectionStringTextBox.Enabled = false;
			searchGroupBox .Enabled = false;
			_dictionaryView.Enabled = false;
		}

		void UnblockInterface() {
			_isBusy = false;

			progressBar.Visible = false;
			percentageLabel.Visible = false;
			cancelButton.Visible = false;

			fileToolStripMenuItem.Enabled = true;
			optionsToolStripMenuItem.Enabled = true;

			createDbTableButton.Enabled = true;
			tableNameTextBox.Enabled = true;
			connectionStringButton.Enabled = true;
			connectionStringTextBox.Enabled = true;
			searchGroupBox.Enabled = true;
			_dictionaryView.Enabled = true;

			statusLabel.Text = Resources.NoneText;
			statusLabel.ToolTipText = string.Empty;
		}

		#endregion

		#region Resources applying

		private void ApplyResources() {
			System.Drawing.Size size = Size;
			System.Drawing.Point location = Location;
			Resources.Culture = Thread.CurrentThread.CurrentUICulture;
			System.ComponentModel.ComponentResourceManager resources =
				new System.ComponentModel.ComponentResourceManager(
				typeof(MainForm));
			resources.ApplyResources(this, "$this");
			Location = location;
			Size = size;
			ApplyResources(this);
			ApplyResourcesOnTooltips();
			if (_header != null) {
				OutputHeaderInfo(_header);
			}
			string [] columnHeaders = new string [2];
			columnHeaders[0] = Resources.WordField;
			columnHeaders[1] = Resources.TranslateField;
			_dictionaryView.ApplyResources(columnHeaders);
		}

		private void ApplyResourcesOnTooltips() {
			System.ComponentModel.ComponentResourceManager resources =
				new System.ComponentModel.ComponentResourceManager(
				typeof(MainForm));
			infoToolTip.SetToolTip(connectionStringButton,
				resources.GetString("connectionStringButton.ToolTip"));
			infoToolTip.SetToolTip(createDbTableButton,
				resources.GetString("createDbTableButton.ToolTip"));
			infoToolTip.SetToolTip(filterCheckBox,
				resources.GetString("filterCheckBox.ToolTip"));
			infoToolTip.SetToolTip(searchWordTextBox,
				resources.GetString("searchWordTextBox.ToolTip"));
			infoToolTip.SetToolTip(searchTranslateTextBox,
				resources.GetString("searchTranslateTextBox.ToolTip"));
		}

		private static void ApplyResources(Control control) {
			MenuStrip menuControl = control as MenuStrip;
			if (menuControl != null) {
				ApplyResources(menuControl);
				return;
			}
			StatusStrip statusControl = control as StatusStrip;
			if (statusControl != null) {
				ApplyResources(statusControl);
				return;
			}
			System.ComponentModel.ComponentResourceManager resources =
				new System.ComponentModel.ComponentResourceManager(
				typeof(MainForm));
			resources.ApplyResources(control, control.Name);
			foreach (Control child in control.Controls) {
				ApplyResources(child);
			}
		}

		private static void ApplyResources(MenuStrip menu) {
			System.ComponentModel.ComponentResourceManager resources =
				new System.ComponentModel.ComponentResourceManager(
				typeof(MainForm));
			resources.ApplyResources(menu, menu.Name);
			foreach (ToolStripMenuItem child in menu.Items) {
				ApplyResources(child);
			}
		}

		private static void ApplyResources(StatusStrip status) {
			System.ComponentModel.ComponentResourceManager resources =
				new System.ComponentModel.ComponentResourceManager(
				typeof(MainForm));
			resources.ApplyResources(status, status.Name);
			foreach (ToolStripItem child in status.Items) {
				ApplyResources(child);
			}
		}

		private static void ApplyResources(ToolStripMenuItem item) {
			System.ComponentModel.ComponentResourceManager resources =
				new System.ComponentModel.ComponentResourceManager(
				typeof(MainForm));
			resources.ApplyResources(item, item.Name);
			foreach (ToolStripItem child in item.DropDownItems) {
				ApplyResources(child);
			}
		}

		private static void ApplyResources(ToolStripItem item) {
			System.ComponentModel.ComponentResourceManager resources =
				new System.ComponentModel.ComponentResourceManager(
				typeof(MainForm));
			resources.ApplyResources(item, item.Name);
		}

		#endregion

	}
}
