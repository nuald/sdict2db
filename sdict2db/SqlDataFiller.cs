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
using System.Data.SqlClient;
using Sdict2db.Properties;
using Sdict2db.Events;
using Sdict2db.Helpers;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Sdict2db {
	class SqlDataFiller : IProgressable {

		#region Private members

		string _connectionString;
		string _commandString;
		string _tableName;

		#endregion

		#region Properties

		public string ConnectionString {
			get {
				return _connectionString;
			}
		}

		public string CommandString {
			get {
				return _commandString;
			}
		}

		public string TableName {
			get {
				return _tableName;
			}
		}

		#endregion

		#region Constants

		const string WordField = "word";
		const string TranslateField = "translate";

		const string EmptyValue = "";

		const string InsertRecordQueryPattern = @"INSERT {0}" +
			"(" + WordField + "," + TranslateField + ")" +
			@" VALUES (@" + WordField + ", @" + TranslateField + ")";

		const string CreateTableQueryPattern = @"CREATE TABLE {0}" +
			@"(" + WordField + " nvarchar(100), " + 
			TranslateField + " ntext)";

		const string CreateIndexQueryPattern = @"CREATE INDEX idword ON {0}" +
				@" (" + WordField + ")";

		const string DeleteTableQueryPattern = @"IF OBJECT_ID(N'{0}" +
				@"') IS NOT NULL DROP TABLE {0}";

		#endregion

		#region Constructors

		public SqlDataFiller(string connectionString, string tableName) {
			_connectionString = connectionString;
			_tableName = tableName;
			_sqlFillShot.TableRow = -1;
		}

		#endregion

		#region Pause/Resume Sql filling

		private struct SqlFillShot {
			public int TableRow;
		}

		SqlFillShot _sqlFillShot;

		private void SaveSqlFilling(int row) {
			_sqlFillShot.TableRow = row;
		}

		private void LoadSqlFilling(ref int row, DataTable table,
				ref bool append) {
			if ((_sqlFillShot.TableRow >= 0) &&
				(_sqlFillShot.TableRow < table.Rows.Count)) {
				row = _sqlFillShot.TableRow;
				append = true;
			} else {
				append = false;
			}
		}

		#endregion

		#region Events

		public event EventHandler<DoWorkEventArgs> PercentageChanged;

		private void PercentageChangedHandler(object sender, DoWorkEventArgs e) {
			if (PercentageChanged != null) {
				PercentageChanged(this, e);
			}
		}

		#endregion

		#region Public methods

		public string PrepareFillTableSqlQuery() {
			if (String.IsNullOrEmpty(_connectionString) ||
				String.IsNullOrEmpty(_tableName)) {
				_commandString = string.Empty;
				return Resources.NoneText;
			}
			StringBuilder sb = new StringBuilder();
			using (SqlConnection conn = new SqlConnection(_connectionString)) {
				conn.Open();
				using (SqlCommand command = new SqlCommand()) {
					command.Connection = conn;
					sb.AppendLine(Resources.ConnectionOpenedText);
					DeleteTable(command);
					sb.AppendLine(Resources.DeleteTableText);
					CreateTable(command);
					sb.AppendLine(Resources.CreateTableText);
					CreateTableIndex(command);
					sb.AppendLine(Resources.CreateTableIndexText);
				}
			}
			sb.AppendLine(Resources.FillDataText);
			_commandString = String.Format(CultureInfo.CurrentCulture,
				InsertRecordQueryPattern, _tableName);
			return sb.ToString();
		}

		public void FillTable(DataTable source,
				DoWorkEventArgs e) {
			int i = 0;
			bool append = false;
			LoadSqlFilling(ref i, source, ref append);
			using (SqlConnection conn = new SqlConnection(_connectionString)) {
				conn.Open();
				using (SqlCommand command =
					new SqlCommand(_commandString, conn)) {
					command.Parameters.Add(new SqlParameter(WordField,
						EmptyValue));
					command.Parameters.Add(new SqlParameter(TranslateField,
						EmptyValue));
					FillTable(i,command, source, e);
				}
			}
		}

		public void DeleteTable() {
			using (SqlConnection conn = new SqlConnection(_connectionString)) {
				using (SqlCommand command = new SqlCommand()) {
					command.Connection = conn;
					conn.Open();
					DeleteTable(command);
				}
			}
		}

		#endregion

		#region Private methods

		private void CreateTable(SqlCommand command) {
			command.CommandText = String.Format(CultureInfo.CurrentCulture, 
				CreateTableQueryPattern, _tableName);
			command.ExecuteNonQuery();
		}

		private void CreateTableIndex(SqlCommand command) {
			command.CommandText = String.Format(CultureInfo.CurrentCulture,
				CreateIndexQueryPattern, _tableName);
			command.ExecuteNonQuery();
		}

		private void DeleteTable(SqlCommand command) {
			command.CommandText = String.Format(CultureInfo.CurrentCulture,
				DeleteTableQueryPattern, _tableName);
			command.ExecuteNonQuery();
		}

		private void FillTable(int i, SqlCommand command, DataTable source,
				DoWorkEventArgs e) {
			int previousPercentage = 0;
			for (; i < source.Rows.Count; i++) {
				DataRow row = source.Rows[i];
				command.Parameters[WordField].Value = row[0];
				command.Parameters[TranslateField].Value = row[1];
				command.ExecuteNonQuery();
				if (e.Cancel) {
					SaveSqlFilling(++i);
					break;
				}
				int percentComplete =
					(int)((float)i / (float)source.Rows.Count * 100);
				PercentageParameters args = new PercentageParameters(
					percentComplete, previousPercentage);
				e.Result = args as object;
				if (e.Result == null) {
					return;
				}
				PercentageChangedHandler(null, e);
				previousPercentage = percentComplete;
			}
		}

		#endregion

	}
}
