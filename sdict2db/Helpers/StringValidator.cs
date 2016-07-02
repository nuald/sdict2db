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

using System.Text.RegularExpressions;
using System.Globalization;

namespace Sdict2db.Helpers {
	static class StringValidator {

		#region Constants

		// We need not exclude SQL reserved words,
		// numbers at the beginning in table name 
		// since our table name has a prefix.
		const string TableNamePattern = @"^[\w]+$";

		// We need not exclude SQL reserved words in table name 
		// since our table name has a prefix and cannot be
		// like a reserved word.
		const string InsertQueryPattern = @"^\s*insert\s+" +
			@"(into\s+)?[a-zA-Z_]+[\w]*\s*" +
			@"(\(\s*word\s*,\s*translate\s*\))?\s+" +
			@"values\s*" +
			@"\(\s*@word\s*,\s*@translate\s*\)\s*$";

		#region Not used

		const string ConnectionStringPattern = @"^(\s*((Application\s+Name)|" +
			@"((AttachDBFilename)|(extended\s+properties)|" +
			@"(Initial\s+File\s+Name))|(Connect(ion?)\s+Timeout)|" +
			@"(Context\s+Connection)|(Current\s+Language)|" +
			@"((Data\s+Source)|(Server)|(Addr(ess)?)|(Network\s+Address))|" +
			@"(Encrypt)|(Enlist)|(Failover\s+Partner)|" +
			@"((Initial\s+Catalog)|(Database))|" +
			@"((Integrated\s+Security)|(Trusted_Connection))|" +
			@"(MultipleActiveResultSets)|(Net(work\s+Library)?)|" +
			@"(Packet\s+Size)|((Password)|(Pwd))|(Persist\s+Security\s+Info)|" +
			@"(Replication)|(Transaction\s+Binding)|(TrustServerCertificate)|" +
			@"(Type\s+System\s+Version)|(UserID)|(User\s+Instance)|" +
			@"(Workstation\s+ID))\s*=\s*\S+\s*;\s*)+$";

		const string AttributePattern = @"(?<found>" +
			@"({0}\s*=\s*\S+\s*(;|(?:$))\s*))";

		const string AttributeFormat = @"{0}={1};";

		#endregion

		#endregion

		public static bool IsValidTableName(string tableName) {
			return Regex.IsMatch(tableName, TableNamePattern);
		}

		public static bool IsValidSqlInsertCommand(string command) {
			RegexOptions options = new RegexOptions();
			options = RegexOptions.IgnoreCase;
			return Regex.IsMatch(command, InsertQueryPattern, options);
		}

		#region Not used

		public static bool IsValidConnectionString(string connString) {
			connString = connString.Trim();
			if (!connString.EndsWith(";")) {
				connString += ";";
			}
			RegexOptions options = new RegexOptions();
			options = RegexOptions.IgnoreCase;
			return Regex.IsMatch(connString, ConnectionStringPattern, options);
		}

		/*public static string RemoveAttribute(string text, string attribute) {
			RegexOptions options = new RegexOptions();
			options = RegexOptions.IgnoreCase;
			string pattern = string.Format(CultureInfo.CurrentCulture,
				AttributePattern, attribute);
			return Regex.Replace(text, pattern, string.Empty, options);
		}*/

		/*public static string AddAttribute(string text, string attribute,
				string value) {
			return text.Insert(0, string.Format(CultureInfo.CurrentCulture,
				AttributeFormat, attribute, value));
		}*/

		#endregion

	}
}
