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
	/// Event arguments, used when SQL parameters have been retrieved.
	/// </summary>
	public class GetSqlParametersEventArgs : EventArgs {

		#region Private members
		
		string _connectionString;
		string _tableName;
		DataTable _data;

		#endregion

		#region Properties

		/// <summary>
		/// Connestion string for SQL operations.
		/// </summary>
		public string ConnectionString {
			set { _connectionString = value; }
			get { return _connectionString; }
		}

		/// <summary>
		/// Name of data table.
		/// </summary>
		public string TableName {
			set { _tableName = value; }
			get { return _tableName; }
		}

		/// <summary>
		/// Data for exporting to SQL database table.
		/// </summary>
		public DataTable Data {
			set { _data = value; }
			get { return _data; }
		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the GetSqlParametersEventArgs class.
		/// </summary>
		public GetSqlParametersEventArgs() {
		}

		/// <summary>
		/// Initializes a new instance of the GetSqlParametersEventArgs class
		/// with spacified SQL parameters.
		/// </summary>
		/// <param name="connectionString">
		/// SQL connection string.
		/// </param>
		/// <param name="tableName">
		/// Data table name.
		/// </param>
		/// <param name="data">
		/// Data for exporting to SQL database table.
		/// </param>
		public GetSqlParametersEventArgs(string connectionString,
			string tableName, DataTable data) {
			_connectionString = connectionString;
			_tableName = tableName;
			_data = data;
		}

		#endregion

	}
}
