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

using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System;

namespace Sdict2db.Helpers {
	static class StringHelper {

		#region Constants

		private static string GapString = "...";
		private static int MinimalMaxLength = 15;
		private static string FilterOneFieldPattern = "{0} like '{1}%'";
		private static string FilterFieldAdditionPattern = " and {0} like '{1}%'";
		private static string DifferentLengthsText = "Arrays' lengths are different.";

		#endregion

		#region Public methods 

		public static string ToCString(string item) {
			return item.ToString(CultureInfo.CurrentCulture);
		}

		public static string ToCString(int item) {
			return item.ToString(CultureInfo.CurrentCulture);
		}

		public static string ToTabString(string text) {
			return text.Replace("\\t", "\t");
		}

		public static string ToShortPath(string path, int maxLength) {
			if (maxLength < MinimalMaxLength) {
				maxLength = MinimalMaxLength;
			}
			if (path.Length <= maxLength) {
				return path;
			}
			List<string> elems = SplitPath(path);
			if (elems.Count < 1) {
				return string.Empty;
			}
			if (elems.Count == 1) {
				return BuildCutPath("", elems[elems.Count - 1], maxLength);
			}
			string root = GetRoot(path, elems);
			if (elems.Count == 2) {
				return BuildCutPath(root, elems[elems.Count - 1], maxLength);
			}
			root += GapString;
			if (root.Length + Path.DirectorySeparatorChar.ToString().Length +
				elems[elems.Count - 1].Length > maxLength) {
				return BuildCutPath(root + Path.DirectorySeparatorChar,
					elems[elems.Count - 1], maxLength);
			}
			return FindShortPath(root, elems, maxLength);
		}

		public static string ConcatPath(string dir, string file) {
			return dir + Path.DirectorySeparatorChar + file;
		}

		public static bool BeginsWith(string text, string prefix,
				bool caseSensitive) {
			if (caseSensitive && (text.IndexOf(prefix, 0) == 0)) {
				return true;
			}
			if (text.ToLower(CultureInfo.CurrentCulture).IndexOf(
				prefix.ToLower(CultureInfo.CurrentCulture), 0) == 0) {
				return true;
			}
			return false;
		}

		public static string CreateSortExpression(string field,
				SortOrder sortOrder) {
			if (string.IsNullOrEmpty(field)) {
				throw new ArgumentNullException("field");
			}
			if (sortOrder == SortOrder.Ascending) {
				return field + " ASC";
			} else if (sortOrder == SortOrder.Descending) {
				return field + " DESC";
			}
			return string.Empty;
		}

		public static string CreateFilterExpression(string[] fields,
				string[] values) {
			if (fields == null) {
				throw new ArgumentNullException("fields");
			}
			if (values == null) {
				throw new ArgumentNullException("values");
			}
			if (fields.Length != values.Length) {
				throw new ArgumentException(DifferentLengthsText);
			}
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < fields.Length; ++i) {
				if (i == 0) {
					sb.AppendFormat(CultureInfo.CurrentCulture,
						FilterOneFieldPattern, fields[i], values[i]);
				} else {
					sb.AppendFormat(CultureInfo.CurrentCulture,
						FilterFieldAdditionPattern, fields[i], values[i]);
				}
			}
			return sb.ToString();
		}

		public static bool ConsistsOfEmptyStrings(string[] strings) {
			if (strings == null) {
				throw new ArgumentNullException("strings");
			}
			for (int i = 0; i < strings.Length; ++i) {
				if (!string.IsNullOrEmpty(strings[i])) {
					return false;
				}
			}
			return true;
		}

		#endregion

		#region Private methods

		private static string GetRoot(string path, List<string> elems) {
			string root = Path.GetPathRoot(path);
			if (root.Length > 3) {
				root += Path.DirectorySeparatorChar;
			}
			if (root.Length == 0) {
				root = elems[0];
				root += Path.DirectorySeparatorChar;
			}
			return root;
		}

		private static List<string> SplitPath(string path) {
			string[] splittered = path.Split(Path.DirectorySeparatorChar,
				Path.AltDirectorySeparatorChar);
			List<string> result = new List<string>();
			for (int i = 0; i < splittered.GetLength(0); ++i) {
				result.Add(splittered[i]);
			}
			return result;
		}

		private static string BuildCutPath(string root, string filename,
				int maxLength) {
			if (root.Length >= maxLength) {
				root = GapString + Path.DirectorySeparatorChar.ToString();
			}
			int restLength = maxLength - root.Length;
			if (restLength < filename.Length) {
				root += filename[0] + GapString;
				restLength = maxLength - root.Length;
			}
			int tailIndex = filename.Length - restLength - 1;
			if (tailIndex < 0) {
				tailIndex = 0;
			}
			if (tailIndex >= filename.Length) {
				tailIndex = filename.Length - 1;
			}
			return root + filename.Substring(tailIndex);
		}

		private static string FindShortPath(string root, List<string> elems,
				int maxLength) {
			for (int i = 2; i < elems.Count; ++i) {
				string pathTry = BuildShortPath(root, elems, i);
				if (pathTry.Length <= maxLength) {
					return pathTry;
				}
			}
			return BuildCutPath(root, elems[elems.Count - 1], maxLength);
		}

		private static string BuildShortPath(string root, List<string> strings,
				int index) {
			for (int i = index; i < strings.Count; ++i) {
				root += Path.DirectorySeparatorChar + strings[i];
			}
			return root;
		}

		#endregion

	}
}
