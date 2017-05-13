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

using System.Windows.Forms;
using Sdict2db.Properties;

namespace Sdict2db {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">
		/// true if managed resources should be disposed; otherwise, false.
		/// </param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				_dictionaryView.Dispose();
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code Sdict2db.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createDBTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.connectionStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.enUSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ruToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.cancelButton = new System.Windows.Forms.ToolStripSplitButton();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.percentageLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.connectionStringErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.infoLabel = new System.Windows.Forms.Label();
			this.infoValuesLabel = new System.Windows.Forms.Label();
			this.connectionStringLabel = new System.Windows.Forms.Label();
			this.tableNameLabel = new System.Windows.Forms.Label();
			this.connectionStringButton = new System.Windows.Forms.Button();
			this.createDbTableButton = new System.Windows.Forms.Button();
			this.tableNameTextBox = new System.Windows.Forms.TextBox();
			this.connectionStringTextBox = new System.Windows.Forms.TextBox();
			this.tablePrefixLabel = new System.Windows.Forms.Label();
			this.infoPanel = new System.Windows.Forms.Panel();
			this.infoTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.searchGroupBox = new System.Windows.Forms.GroupBox();
			this.searchTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.filterCheckBox = new System.Windows.Forms.CheckBox();
			this.searchTranslateTextBox = new System.Windows.Forms.TextBox();
			this.searchWordTextBox = new System.Windows.Forms.TextBox();
			this.viewPanel = new System.Windows.Forms.Panel();
			this.tableNameErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.infoToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.menuStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.connectionStringErrorProvider)).BeginInit();
			this.infoPanel.SuspendLayout();
			this.infoTableLayoutPanel.SuspendLayout();
			this.mainTableLayoutPanel.SuspendLayout();
			this.searchGroupBox.SuspendLayout();
			this.searchTableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tableNameErrorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
			resources.ApplyResources(this.menuStrip, "menuStrip");
			this.menuStrip.Name = "menuStrip";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.createDBTableToolStripMenuItem,
            this.toolStripSeparator1,
            this.recentFilesToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
			// 
			// openToolStripMenuItem
			// 
			resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenMenuClick);
			// 
			// saveToolStripMenuItem
			// 
			resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveMenuClick);
			// 
			// createDBTableToolStripMenuItem
			// 
			this.createDBTableToolStripMenuItem.Image = global::Sdict2db.Properties.Resources.AddTableHS;
			resources.ApplyResources(this.createDBTableToolStripMenuItem, "createDBTableToolStripMenuItem");
			this.createDBTableToolStripMenuItem.Name = "createDBTableToolStripMenuItem";
			this.createDBTableToolStripMenuItem.Click += new System.EventHandler(this.CreateDbTableButtonClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// recentFilesToolStripMenuItem
			// 
			this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
			resources.ApplyResources(this.recentFilesToolStripMenuItem, "recentFilesToolStripMenuItem");
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitMenuClick);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionStringToolStripMenuItem,
            this.languageToolStripMenuItem});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
			// 
			// connectionStringToolStripMenuItem
			// 
			resources.ApplyResources(this.connectionStringToolStripMenuItem, "connectionStringToolStripMenuItem");
			this.connectionStringToolStripMenuItem.Name = "connectionStringToolStripMenuItem";
			this.connectionStringToolStripMenuItem.Click += new System.EventHandler(this.ConnectionStringToolStripMenuItemClick);
			// 
			// languageToolStripMenuItem
			// 
			this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enUSToolStripMenuItem,
            this.ruToolStripMenuItem});
			resources.ApplyResources(this.languageToolStripMenuItem, "languageToolStripMenuItem");
			this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
			// 
			// enUSToolStripMenuItem
			// 
			this.enUSToolStripMenuItem.Checked = true;
			this.enUSToolStripMenuItem.CheckOnClick = true;
			this.enUSToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enUSToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.enUSToolStripMenuItem.Name = "enUSToolStripMenuItem";
			resources.ApplyResources(this.enUSToolStripMenuItem, "enUSToolStripMenuItem");
			this.enUSToolStripMenuItem.Click += new System.EventHandler(this.ChangeLanguageRadioClick);
			// 
			// ruToolStripMenuItem
			// 
			this.ruToolStripMenuItem.CheckOnClick = true;
			this.ruToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.ruToolStripMenuItem.Name = "ruToolStripMenuItem";
			resources.ApplyResources(this.ruToolStripMenuItem, "ruToolStripMenuItem");
			this.ruToolStripMenuItem.Click += new System.EventHandler(this.ChangeLanguageRadioClick);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "txt";
			this.saveFileDialog.DereferenceLinks = false;
			resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
			// 
			// openFileDialog
			// 
			resources.ApplyResources(this.openFileDialog, "openFileDialog");
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cancelButton,
            this.statusLabel,
            this.progressBar,
            this.percentageLabel});
			resources.ApplyResources(this.statusStrip, "statusStrip");
			this.statusStrip.Name = "statusStrip";
			// 
			// cancelButton
			// 
			this.cancelButton.Image = global::Sdict2db.Properties.Resources.DeleteHS;
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.MergeIndex = 0;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.ButtonClick += new System.EventHandler(this.CancelButtonClick);
			// 
			// statusLabel
			// 
			this.statusLabel.MergeIndex = 1;
			this.statusLabel.Name = "statusLabel";
			resources.ApplyResources(this.statusLabel, "statusLabel");
			// 
			// progressBar
			// 
			this.progressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.progressBar.MergeIndex = 2;
			this.progressBar.Name = "progressBar";
			resources.ApplyResources(this.progressBar, "progressBar");
			// 
			// percentageLabel
			// 
			this.percentageLabel.MergeIndex = 3;
			this.percentageLabel.Name = "percentageLabel";
			resources.ApplyResources(this.percentageLabel, "percentageLabel");
			// 
			// connectionStringErrorProvider
			// 
			this.connectionStringErrorProvider.ContainerControl = this;
			// 
			// infoLabel
			// 
			resources.ApplyResources(this.infoLabel, "infoLabel");
			this.infoLabel.Name = "infoLabel";
			// 
			// infoValuesLabel
			// 
			resources.ApplyResources(this.infoValuesLabel, "infoValuesLabel");
			this.infoValuesLabel.Name = "infoValuesLabel";
			// 
			// connectionStringLabel
			// 
			resources.ApplyResources(this.connectionStringLabel, "connectionStringLabel");
			this.connectionStringLabel.Name = "connectionStringLabel";
			// 
			// tableNameLabel
			// 
			resources.ApplyResources(this.tableNameLabel, "tableNameLabel");
			this.tableNameLabel.Name = "tableNameLabel";
			// 
			// connectionStringButton
			// 
			resources.ApplyResources(this.connectionStringButton, "connectionStringButton");
			this.connectionStringButton.Image = global::Sdict2db.Properties.Resources.EditInformationHS;
			this.connectionStringButton.Name = "connectionStringButton";
			this.infoToolTip.SetToolTip(this.connectionStringButton, resources.GetString("connectionStringButton.ToolTip"));
			this.connectionStringButton.Click += new System.EventHandler(this.ConnectionStringToolStripMenuItemClick);
			// 
			// createDbTableButton
			// 
			resources.ApplyResources(this.createDbTableButton, "createDbTableButton");
			this.createDbTableButton.Image = global::Sdict2db.Properties.Resources.AddTableHS;
			this.createDbTableButton.Name = "createDbTableButton";
			this.infoToolTip.SetToolTip(this.createDbTableButton, resources.GetString("createDbTableButton.ToolTip"));
			this.createDbTableButton.Click += new System.EventHandler(this.CreateDbTableButtonClick);
			// 
			// tableNameTextBox
			// 
			resources.ApplyResources(this.tableNameTextBox, "tableNameTextBox");
			this.tableNameTextBox.Name = "tableNameTextBox";
			this.tableNameTextBox.TextChanged += new System.EventHandler(this.QueryStringTableChanged);
			// 
			// connectionStringTextBox
			// 
			this.mainTableLayoutPanel.SetColumnSpan(this.connectionStringTextBox, 2);
			resources.ApplyResources(this.connectionStringTextBox, "connectionStringTextBox");
			this.connectionStringTextBox.Name = "connectionStringTextBox";
			this.connectionStringTextBox.TextChanged += new System.EventHandler(this.QueryStringChanged);
			// 
			// tablePrefixLabel
			// 
			resources.ApplyResources(this.tablePrefixLabel, "tablePrefixLabel");
			this.tablePrefixLabel.Name = "tablePrefixLabel";
			// 
			// infoPanel
			// 
			resources.ApplyResources(this.infoPanel, "infoPanel");
			this.mainTableLayoutPanel.SetColumnSpan(this.infoPanel, 4);
			this.infoPanel.Controls.Add(this.infoTableLayoutPanel);
			this.infoPanel.Name = "infoPanel";
			// 
			// infoTableLayoutPanel
			// 
			resources.ApplyResources(this.infoTableLayoutPanel, "infoTableLayoutPanel");
			this.infoTableLayoutPanel.Controls.Add(this.infoValuesLabel, 1, 0);
			this.infoTableLayoutPanel.Controls.Add(this.infoLabel, 0, 0);
			this.infoTableLayoutPanel.Name = "infoTableLayoutPanel";
			// 
			// mainTableLayoutPanel
			// 
			resources.ApplyResources(this.mainTableLayoutPanel, "mainTableLayoutPanel");
			this.mainTableLayoutPanel.Controls.Add(this.infoPanel, 0, 0);
			this.mainTableLayoutPanel.Controls.Add(this.createDbTableButton, 3, 4);
			this.mainTableLayoutPanel.Controls.Add(this.tableNameTextBox, 2, 4);
			this.mainTableLayoutPanel.Controls.Add(this.tablePrefixLabel, 1, 4);
			this.mainTableLayoutPanel.Controls.Add(this.connectionStringButton, 3, 3);
			this.mainTableLayoutPanel.Controls.Add(this.connectionStringLabel, 0, 3);
			this.mainTableLayoutPanel.Controls.Add(this.tableNameLabel, 0, 4);
			this.mainTableLayoutPanel.Controls.Add(this.connectionStringTextBox, 1, 3);
			this.mainTableLayoutPanel.Controls.Add(this.searchGroupBox, 0, 1);
			this.mainTableLayoutPanel.Controls.Add(this.viewPanel, 0, 2);
			this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
			// 
			// searchGroupBox
			// 
			resources.ApplyResources(this.searchGroupBox, "searchGroupBox");
			this.mainTableLayoutPanel.SetColumnSpan(this.searchGroupBox, 4);
			this.searchGroupBox.Controls.Add(this.searchTableLayoutPanel);
			this.searchGroupBox.Name = "searchGroupBox";
			this.searchGroupBox.TabStop = false;
			// 
			// searchTableLayoutPanel
			// 
			resources.ApplyResources(this.searchTableLayoutPanel, "searchTableLayoutPanel");
			this.searchTableLayoutPanel.Controls.Add(this.filterCheckBox, 0, 0);
			this.searchTableLayoutPanel.Controls.Add(this.searchTranslateTextBox, 2, 0);
			this.searchTableLayoutPanel.Controls.Add(this.searchWordTextBox, 1, 0);
			this.searchTableLayoutPanel.Name = "searchTableLayoutPanel";
			// 
			// filterCheckBox
			// 
			resources.ApplyResources(this.filterCheckBox, "filterCheckBox");
			this.filterCheckBox.Image = global::Sdict2db.Properties.Resources.Filter2HS;
			this.filterCheckBox.Name = "filterCheckBox";
			this.infoToolTip.SetToolTip(this.filterCheckBox, resources.GetString("filterCheckBox.ToolTip"));
			this.filterCheckBox.CheckedChanged += new System.EventHandler(this.FilterCheckBoxCheckedChanged);
			// 
			// searchTranslateTextBox
			// 
			resources.ApplyResources(this.searchTranslateTextBox, "searchTranslateTextBox");
			this.searchTranslateTextBox.Name = "searchTranslateTextBox";
			this.infoToolTip.SetToolTip(this.searchTranslateTextBox, resources.GetString("searchTranslateTextBox.ToolTip"));
			this.searchTranslateTextBox.TextChanged += new System.EventHandler(this.SearchTranslateTextBoxTextChanged);
			// 
			// searchWordTextBox
			// 
			resources.ApplyResources(this.searchWordTextBox, "searchWordTextBox");
			this.searchWordTextBox.Name = "searchWordTextBox";
			this.infoToolTip.SetToolTip(this.searchWordTextBox, resources.GetString("searchWordTextBox.ToolTip"));
			this.searchWordTextBox.TextChanged += new System.EventHandler(this.SearchWordTextBoxTextChanged);
			// 
			// viewPanel
			// 
			resources.ApplyResources(this.viewPanel, "viewPanel");
			this.mainTableLayoutPanel.SetColumnSpan(this.viewPanel, 4);
			this.viewPanel.Name = "viewPanel";
			// 
			// tableNameErrorProvider
			// 
			this.tableNameErrorProvider.ContainerControl = this;
			// 
			// infoToolTip
			// 
			resources.ApplyResources(this.infoToolTip, "infoToolTip");
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mainTableLayoutPanel);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainFormKeyDown);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.connectionStringErrorProvider)).EndInit();
			this.infoPanel.ResumeLayout(false);
			this.infoPanel.PerformLayout();
			this.infoTableLayoutPanel.ResumeLayout(false);
			this.infoTableLayoutPanel.PerformLayout();
			this.mainTableLayoutPanel.ResumeLayout(false);
			this.mainTableLayoutPanel.PerformLayout();
			this.searchGroupBox.ResumeLayout(false);
			this.searchGroupBox.PerformLayout();
			this.searchTableLayoutPanel.ResumeLayout(false);
			this.searchTableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tableNameErrorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MenuStrip menuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem openToolStripMenuItem;
		private ToolStripMenuItem saveToolStripMenuItem;
		private ToolStripMenuItem exitToolStripMenuItem;
		private SaveFileDialog saveFileDialog;
		private OpenFileDialog openFileDialog;
		private StatusStrip statusStrip;
		private ToolStripProgressBar progressBar;
		private ToolStripStatusLabel percentageLabel;
		private ErrorProvider connectionStringErrorProvider;
		private ErrorProvider tableNameErrorProvider;
		private ToolStripStatusLabel statusLabel;
		private ToolStripMenuItem optionsToolStripMenuItem;
		private ToolStripMenuItem connectionStringToolStripMenuItem;
		private Label infoLabel;
		private ToolTip infoToolTip;
		private ToolStripMenuItem languageToolStripMenuItem;
		private ToolStripMenuItem enUSToolStripMenuItem;
		private ToolStripMenuItem ruToolStripMenuItem;
		private ToolStripMenuItem createDBTableToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripSplitButton cancelButton;
		private Label infoValuesLabel;
		private TextBox connectionStringTextBox;
		private Button connectionStringButton;
		private Label tableNameLabel;
		private Button createDbTableButton;
		private Label tablePrefixLabel;
		private TextBox tableNameTextBox;
		private Label connectionStringLabel;
		private ToolStripMenuItem recentFilesToolStripMenuItem;
		private Panel infoPanel;
		private TableLayoutPanel infoTableLayoutPanel;
		private TableLayoutPanel mainTableLayoutPanel;
		private GroupBox searchGroupBox;
		private TableLayoutPanel searchTableLayoutPanel;
		private TextBox searchWordTextBox;
		private TextBox searchTranslateTextBox;
		private ToolStripSeparator toolStripSeparator2;
		private CheckBox filterCheckBox;
		private Panel viewPanel;

	}
}

