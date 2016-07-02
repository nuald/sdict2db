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
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Security;
using System.Text;
using Sdict2db.Helpers;
using Sdict2db.Properties;

namespace Sdict2db.RecentFilesMenu {
	class RecentFilesMenuItem : ToolStripMenuItem {

		public delegate void OnClickHandler(string filename);

		#region Constants

		private const int DefaultMaxCount = 5;
		private const int MaximalMaxCount = 20;
		private const int MinimalMaxCount = 3;
		private const int MaxShortPathLength = 80;
		private const string MenuTextPattern = "{0} {1}";
		private const int BaseNumber = 1;
		private const string XmlSectionName = "RecentFiles";
		private const string XmlElementName = "Filename";

		#endregion

		#region Private members

		private ToolStripMenuItem _recentFilesMenuItem;
		private OnClickHandler _onClickHandler;
		private int _maxCount;
		private int _maxShortPathLength;
		private Encoding _encoding = Encoding.UTF8;

		#endregion

		#region Properties

		public int MaxCount {
			set {
				if (value > MinimalMaxCount) {
					_maxCount =
						(value < MaximalMaxCount) ? value : MaximalMaxCount;
				} else {
					_maxCount = MinimalMaxCount;
				}
				while (MenuItems.Count >= _maxCount) {
					MenuItems.RemoveAt(MenuItems.Count - 1);
				}
			}
		}

		public ToolStripItemCollection MenuItems {
			get {
				return _recentFilesMenuItem.DropDownItems;
			}
		}

		public override bool Enabled {
			get {
				return _recentFilesMenuItem.Enabled;
			}
			set {
				_recentFilesMenuItem.Enabled = value;
			}
		}

		#endregion

		#region Event handling

		protected void OnClick(object sender, System.EventArgs e) {
			RecentFileMenuItem menuItem = sender as RecentFileMenuItem;
			if (menuItem == null) {
				return;
			}
			int index = MenuItems.IndexOf(menuItem);
			MenuItems.RemoveAt(index);
			MenuItems.Insert(0, menuItem);
			_onClickHandler(menuItem.Filename);
		}

		#endregion

		#region Construction

		public RecentFilesMenuItem(ToolStripMenuItem recentFilesMenuItem,
				OnClickHandler onClickHandler)
			: this(recentFilesMenuItem, onClickHandler, DefaultMaxCount,
				MaxShortPathLength) {
		}

		public RecentFilesMenuItem(ToolStripMenuItem recentFilesMenuItem,
				OnClickHandler onClickHandler, int maxShortPathLength)
			: this(recentFilesMenuItem, onClickHandler, DefaultMaxCount,
				maxShortPathLength) {
		}

		public RecentFilesMenuItem(ToolStripMenuItem recentFilesMenuItem,
				OnClickHandler onClickedHandler, int maxItemsAmount,
				int maxShortPathLength) {
			InitComponent(recentFilesMenuItem, onClickedHandler, maxItemsAmount,
				maxShortPathLength);
		}

		protected void InitComponent(ToolStripMenuItem recentFilesMenuItem,
				OnClickHandler onClickHandler, int maxItemsAmount,
			int maxShortPathLength) {
			if (recentFilesMenuItem == null) {
				throw new ArgumentNullException("recentFilesMenuItem");
			}
			_recentFilesMenuItem = recentFilesMenuItem;
			_recentFilesMenuItem.Enabled = false;
			_recentFilesMenuItem.Checked = false;

			MaxCount = maxItemsAmount;
			_maxShortPathLength = maxShortPathLength;
			_onClickHandler = onClickHandler;
		}

		#endregion

		#region Menu items adding

		public void AddFile(string filename) {
			string path = Path.GetFullPath(filename);
			AddFile(path, StringHelper.ToShortPath(path, _maxShortPathLength));
		}

		private void AddFile(string filename, string shortFilename) {
			if (filename == null) {
				throw new ArgumentNullException("filename");
			}
			if (filename.Length == 0) {
				throw new ArgumentException(Resources.EmptyStringText);
			}
			if (MenuItems.Count > 0) {
				int index = IndexOf(filename);
				if (index >= 0) {
					SetFirst(index);
					return;
				}
			}
			while (MenuItems.Count >= _maxCount) {
				MenuItems.RemoveAt(MenuItems.Count - 1);
			}
			if (MenuItems.Count < _maxCount) {
				RecentFileMenuItem menuItem =
					new RecentFileMenuItem(filename, shortFilename,
					new System.EventHandler(OnClick));
				MenuItems.Insert(0, menuItem);
				ArrangeNumbers();
			}
			Enabled = true;
		}

		#endregion

		#region Menu items removing

		public void RemoveAllFiles() {
			foreach (ToolStripMenuItem item in MenuItems) {
				MenuItems.Remove(item);
			}
			Enabled = false;
		}

		public void RemoveFile(string filename) {
			if (filename == null) {
				throw new ArgumentNullException("filename");
			}
			if (filename.Length == 0) {
				throw new ArgumentException(Resources.EmptyStringText);
			}
			if (MenuItems.Count > 0) {
				int index = IndexOf(filename);
				if (index >= 0) {
					MenuItems.RemoveAt(index);
				}
			}
			Enabled = (MenuItems.Count > 0) ? true : false;
			ArrangeNumbers();
		}

		#endregion

		#region Menu items locating

		private int IndexOf(string filename) {
			foreach (ToolStripMenuItem item in MenuItems) {
				RecentFileMenuItem menuItem = item as RecentFileMenuItem;
				if ((menuItem != null) && (menuItem.Filename == filename)) {
					return MenuItems.IndexOf(item);
				}
			}
			return -1;
		}

		private void SetFirst(int index) {
			if (index < 0 ||
				index >= MenuItems.Count ||
				MenuItems.Count < 2) {
				return;
			}
			RecentFileMenuItem menuItem =
				MenuItems[index] as RecentFileMenuItem;
			if (menuItem == null) {
				return;
			}
			MenuItems.RemoveAt(index);
			MenuItems.Insert(0, menuItem);
			ArrangeNumbers();
		}

		private void ArrangeNumbers() {
			ArrangeNumbers(BaseNumber);
		}

		private void ArrangeNumbers(int baseNumber) {
			for (int i = 0; i < MenuItems.Count; ++i) {
				AssignNumber(MenuItems[i], baseNumber + i);
			}
		}

		private static void AssignNumber(ToolStripItem item, int number) {
			RecentFileMenuItem menuItem = item as RecentFileMenuItem;
			if (menuItem == null) {
				return;
			}
			menuItem.Text = string.Format(CultureInfo.CurrentCulture,
				MenuTextPattern, number, menuItem.ShortFilename);
		}

		#endregion

		#region Menu items saving/loading

		public void Save(string savePath) {
			if (MenuItems.Count < 1) {
				return;
			}
			try {
				XmlTextWriter writer = new XmlTextWriter(savePath,
					_encoding);
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument();
				writer.WriteStartElement(XmlSectionName);
				for (int i = MenuItems.Count - 1; i >= 0; --i) {
					RecentFileMenuItem item =
						MenuItems[i] as RecentFileMenuItem;
					if (item != null) {
						writer.WriteElementString(XmlElementName, item.Filename);
					}
				}
				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Close();
			} catch (ArgumentNullException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			} catch (DirectoryNotFoundException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			} catch (ArgumentException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			} catch (SecurityException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			} catch (UnauthorizedAccessException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			} catch (IOException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			} catch (InvalidOperationException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			}
		}

		public void Load(string loadPath) {
			RemoveAllFiles();
			if (!System.IO.File.Exists(loadPath)) {
				return;
			}
			try {
				XmlTextReader reader = new XmlTextReader(loadPath);
				bool inSection = false;
				while (reader.Read()) {
					TreatNode(reader, ref inSection);
				}
				reader.Close();
			} catch (FileNotFoundException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			} catch (InvalidOperationException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			} catch (DirectoryNotFoundException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			} catch (UriFormatException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			} catch (XmlException e) {
				MessageHelper.ShowErrorMessage(e.Message);
			}
		}

		private void TreatNode(XmlTextReader reader, ref bool inSection) {
			switch (reader.NodeType) {
				case XmlNodeType.Element: {
						if (reader.Name == XmlSectionName) {
							inSection = true;
						} else if (inSection &&
							(reader.Name == XmlElementName)) {
							string filename =
								reader.ReadElementContentAsString();
							AddFile(filename);
						}
					} break;
				case XmlNodeType.EndElement: {
						if (reader.Name == XmlSectionName) {
							inSection = false;
						}
					} break;
			}
		}

		#endregion

	}
}
