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
using Sdict2db.Helpers;
using System;

namespace Sdict2db.Test {
	/// <summary>
	/// Tests for StringValidator class.
	/// </summary>
	[TestFixture]
	public class StringValidatorTest {

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

		/// <summary>
		/// Tests IsValidTableName method.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		[Test]
		public void IsValidTableNameTest() {
			// That can be:
			Assert.IsTrue(StringValidator.IsValidTableName("atable"));
			Assert.IsTrue(StringValidator.IsValidTableName("atable"));
			Assert.IsTrue(StringValidator.IsValidTableName("atable123"));
			// Actually we need not to check on SQL reserved words matching,
			// because in the project table name always has a prefix,
			// e.g. "Dictionary_":
			Assert.IsTrue(StringValidator.IsValidTableName("insert"));
			// So, name can also start with _1-9:
			Assert.IsTrue(StringValidator.IsValidTableName("_table"));
			Assert.IsTrue(StringValidator.IsValidTableName("123table"));
			Assert.IsTrue(StringValidator.IsValidTableName("123"));

			// That must not be:
			Assert.IsFalse(StringValidator.IsValidTableName("a table"));
			Assert.IsFalse(StringValidator.IsValidTableName(" atable"));
			Assert.IsFalse(StringValidator.IsValidTableName(""));

			Console.Out.WriteLine("StringValidator.IsValidTableName " +
				" test is passed.");
		}

		/// <summary>
		/// Tests IsValidSqlInsertCommand method.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		[Test]
		public void IsValidSqlInsertCommandTest() {
			// That can be:
			Assert.IsTrue(StringValidator.IsValidSqlInsertCommand(
				"insert into Sometable values( @word, @translate )"));
			Assert.IsTrue(StringValidator.IsValidSqlInsertCommand(
				"INSERT Sometable(word, translate) values(@word, @translate)"));
			Assert.IsTrue(StringValidator.IsValidSqlInsertCommand(
				"insert into _atable values( @word, @translate )"));

			// That must not be:
			Assert.IsFalse(StringValidator.IsValidSqlInsertCommand(
				"INSERT 123(word, translate) values(@word, @translate)"));
			Assert.IsFalse(StringValidator.IsValidSqlInsertCommand(
				"insert ta ble(word, smth) values('aa', 'bbb')"));
			Assert.IsFalse(StringValidator.IsValidSqlInsertCommand(
				"insert atable(word) values(@word, @translate)"));
			Assert.IsFalse(StringValidator.IsValidSqlInsertCommand(
				"insert atable values(@translate)"));

			Console.Out.WriteLine("StringValidator.IsValidSqlInsertCommand " +
				" test is passed.");
		}

		/// <summary>
		/// Tests IsValidConnectionString method.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		[Test]
		public void IsValidConnectionStringTest() {
			// That can be:
			Assert.IsTrue(StringValidator.IsValidConnectionString(
				"Data Source=.\\SQLEXPRESS;Initial Catalog=ASPNETDB;; " +
				"Integrated Security=True"));
			Assert.IsTrue(StringValidator.IsValidConnectionString(
				"Data Source=.\\SQLEXPRESS;;; " +
				"Integrated Security=True"));

			// That must not be:
			Assert.IsFalse(StringValidator.IsValidConnectionString(
				"DataSource=.\\SQLEXPRSS;;; " +
				"Integrated Security=True"));
			Assert.IsFalse(StringValidator.IsValidConnectionString(
				"Data Source=.\\SQLEXPRESS;Initial Catalog=ASPNETDB;; " +
				"Integrated Security="));
			Assert.IsFalse(StringValidator.IsValidConnectionString(
				"Data Source= ;Initial Catalog=ASPNETDB;; " +
				"Integrated Security=True"));

			Console.Out.WriteLine("StringValidator.IsValidConnectionString " +
				" test is passed.");
		}

	}
}
