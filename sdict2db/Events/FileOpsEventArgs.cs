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

namespace Sdict2db.Events {
	/// <summary>
	/// Event arguments, used while performing file operations.
	/// </summary>
	public class FileOpsEventArgs : EventArgs {

		#region Private members
		
		string _filename;
		DataTable _data;

		#endregion

		#region Properties

		/// <summary>
		/// A name of a file to be operated.
		/// </summary>
		public string Filename {
			set { _filename = value; }
			get { return _filename; }
		}

		/// <summary>
		/// Data for saving to file.
		/// </summary>
		public DataTable Data {
			set { _data = value; }
			get { return _data; }
		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the FileOpsEventArgs class.
		/// </summary>
		public FileOpsEventArgs() {
		}

		/// <summary>
		/// Initializes a new instance of the FileOpsEventArgs class
		/// with specified file name.
		/// </summary>
		/// <param name="filename">
		/// Name of a file to be operated.
		/// </param>
		/// <param name="data">
		/// Data for saving to file.
		/// </param>
		public FileOpsEventArgs(string filename, DataTable data) {
			_filename = filename;
			_data = data;
		}

		#endregion
	}
}
