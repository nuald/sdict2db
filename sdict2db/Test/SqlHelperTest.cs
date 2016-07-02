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

using NUnit.Framework;
using System.Data.SqlClient;
using System;
using Sdict2db.Helpers;
using System.Data;
using System.ComponentModel;
using System.Globalization;
using Sdict2db.Properties;

namespace Sdict2db.Test {
	/// <summary>
	/// Tests for SqlHelper class.
	/// </summary>
	[TestFixture]
	public class SqlHelperTest {

		#region Private members

		SqlDataFiller _sql;

		#endregion

		#region Constants

		const string SelectQueryPattern = @"select * from {0}";

		const int TestRowCount = 50;

		#endregion
		
		/// <summary>
		/// Sets up data for testing.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		[SetUp]
		public void Init() {
		}

		/// <summary>
		/// Finishes data after testing.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		[TearDown]
		public void Deinit() {
		}

		#region PrepareFillTableSqlQuery Test

		/// <summary>
		/// Tests PrepareFillTableSqlQuery method.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		[Test]
		public void PrepareFillTableSqlQueryTest() {
			string wrongConnectionString =
				"Data Source=.\\SQLEXPRESS;Initial Catalog=ASPNETDB;; " +
				"Integrated Security=True";
			string connectionString = "Data Source=.\\SQLEXPRESS;;; " +
				"Integrated Security=True";
			string tableName = String.Format(CultureInfo.CurrentCulture,
				Resources.TableNamePattern, "Test");
			string info = "";
			TestWrongConnection(wrongConnectionString, tableName, ref info);
			Console.Out.WriteLine("SqlHelper.PrepareFillTableSqlQuery: " +
				" Wrong connection test is passed.");
			_sql = TestConnection(connectionString, tableName, ref info);
			Console.Out.WriteLine("SqlHelper.PrepareFillTableSqlQuery: " +
				" Proper connection test is passed.");
			TestTableCreated(connectionString, tableName);
			Console.Out.WriteLine("SqlHelper.PrepareFillTableSqlQuery: " +
				" Table exists, empty.");
			_sql.DeleteTable();
			Console.Out.WriteLine("SqlHelper.PrepareFillTableSqlQuery: " +
				" Test table was removed.");
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		private SqlDataFiller TestConnection(string connectionString, string tableName,
				ref string info) {
			try {
				_sql = new SqlDataFiller(connectionString, tableName);
				info = _sql.PrepareFillTableSqlQuery();
				Assert.IsTrue(StringValidator.IsValidSqlInsertCommand(
					_sql.CommandString));
				return _sql;
			} catch (SqlException e) {
				Assert.IsEmpty(e.Message);
			} catch (InvalidOperationException e) {
				Assert.IsEmpty(e.Message);
			}
			return null;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		private void TestWrongConnection(string wrongConnectionString,
				string tableName, ref string info) {
			try {
				_sql = new SqlDataFiller(wrongConnectionString, tableName);
				info = _sql.PrepareFillTableSqlQuery();
				// Previous line must throw an exception,
				// so next line must be unreachable.
				Assert.IsTrue(false);
			} catch (SqlException e) {
				Assert.IsNotEmpty(e.Message);
			} catch (InvalidOperationException e) {
				Assert.IsNotEmpty(e.Message);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		private void TestTableCreated(string connectionString,
		string tableName) {
			try {
				using (SqlConnection connection =
						new SqlConnection(connectionString)) {
					using (SqlCommand command = new SqlCommand()) {
						command.Connection = connection;
						connection.Open();
						command.CommandText =
							String.Format(CultureInfo.CurrentCulture,
							SelectQueryPattern, tableName);
						SqlDataReader r = command.ExecuteReader();
						Assert.IsFalse(r.HasRows);
						r.Close();
					}
				}
			} catch (SqlException e) {
				Assert.IsEmpty(e.Message);
			} catch (InvalidOperationException e) {
				Assert.IsEmpty(e.Message);
			}
		}

		#endregion

		#region FillTable Test

		/// <summary>
		/// Tests FillTable method.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		[Test]
		public void FillTableTest() {
			// Create a table with data:
			DataTable d = new DataTable();
			d.Locale = CultureInfo.InvariantCulture;
			FillLocalTestDataTable(d);
			Console.Out.WriteLine("SqlHelper.FillTable: " +
				" Local data table is filled with test data.");
			// Create DB table:
			string connectionString = "Data Source=.\\SQLEXPRESS;;; " +
				"Integrated Security=True";
			string tableName = "Dictionary_Test1";
			string info = "";
			_sql = TestConnection(connectionString, tableName, ref info);
			_sql.PercentageChanged += OnPercentageChangedHandler;
			Console.Out.WriteLine("SqlHelper.FillTable: " +
				" DB table was created.");
			Console.Out.WriteLine("SqlHelper.FillTable: " +
				" Filling DB table...");
			// And fill it with our test data:
			DoWorkEventArgs args = new DoWorkEventArgs(this);
			_sql.FillTable(d, args);
			// Then check DB table content:
			CheckSqlDbContent(d);
			Console.Out.WriteLine("SqlHelper.FillTable: " +
				" Content of created table is successfully verified.");
			_sql.DeleteTable();
			Console.Out.WriteLine("SqlHelper.FillTable: " +
				" Test table was removed.");
			_sql.PercentageChanged -= OnPercentageChangedHandler;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		private void FillLocalTestDataTable(DataTable table) {
			table.Columns.Add(new DataColumn(Resources.WordField,
				typeof(string)));
			table.Columns.Add(new DataColumn(Resources.TranslateField,
				typeof(string)));
			for (int i = 0; i < TestRowCount; ++i) {
				table.Rows.Add(new object[] { "a word #" +
					i.ToString(CultureInfo.CurrentCulture),
					"some useful and long article about translation " +
					"of the word #" +
					i.ToString(CultureInfo.CurrentCulture) });
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		private void CheckSqlDbContent(DataTable d) {
			try {
				using (SqlConnection connection =
						new SqlConnection(_sql.ConnectionString)) {
					using (SqlCommand command = new SqlCommand()) {
						command.Connection = connection;
						connection.Open();
						command.CommandText =
							String.Format(CultureInfo.CurrentCulture,
							SelectQueryPattern, _sql.TableName);
						SqlDataReader r = command.ExecuteReader();
						int i = 0;
						while (r.Read()) {
							Assert.AreEqual(d.Rows[i][0], r.GetString(0));
							Assert.AreEqual(d.Rows[i][1], r.GetString(1));
							++i;
						}
						Assert.AreEqual(d.Rows.Count, i);
						r.Close();
					}
				}
			} catch (SqlException e) {
				Assert.IsEmpty(e.Message);
			} catch (InvalidOperationException e) {
				Assert.IsEmpty(e.Message);
			}
		}

		#endregion

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		private void OnPercentageChangedHandler(object sender,
				DoWorkEventArgs e) {
			PercentageParameters p = e.Result as PercentageParameters;
			if (p == null) {
				return;
			}
			Console.Out.WriteLine("PercentageChangedEvent raised: " +
				p.PercentCompleted + "%");
		}

	}
}
