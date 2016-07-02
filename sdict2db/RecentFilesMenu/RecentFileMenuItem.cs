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
using System.Windows.Forms;

namespace Sdict2db.RecentFilesMenu {
	/// <summary>
	/// Represents a menu item object that is used for 'Recent files'
	/// menus.
	/// </summary>
	public class RecentFileMenuItem : ToolStripMenuItem {

		#region Private members

		private string _filename;
		private string _shortFilename;

		#endregion

		#region Properties

		/// <summary>
		/// Stores real filename for a recent file.
		/// </summary>
		public string Filename {
			get {
				return _filename;
			}
			set {
				_filename = value;
			}
		}

		/// <summary>
		/// Stores shorten filename that is displayed in the menu.
		/// </summary>
		public string ShortFilename {
			get {
				return _shortFilename;
			}
			set {
				_shortFilename = value;
			}
		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the RecentFileMenuItem class.
		/// </summary>
		public RecentFileMenuItem() : base() {
			Tag = "";
		}

		/// <summary>
		/// Initializes a new instance of the RecentFileMenuItem class
		/// with specified full filename, shorten filename
		/// and click handler.
		/// </summary>
		/// <param name="filename">Full filename.</param>
		/// <param name="shortFilename">Shorten filename.</param>
		/// <param name="eventHandler">Menu item click handler.</param>
		public RecentFileMenuItem(string filename, string shortFilename,
				EventHandler eventHandler)
			: base(shortFilename) {
			Filename = filename;
			ShortFilename = shortFilename;
			Click += eventHandler;
		}

		#endregion

	}
}
