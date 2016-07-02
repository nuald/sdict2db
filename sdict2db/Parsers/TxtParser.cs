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
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Text;
using Sdict2db.Helpers;
using System.Globalization;

namespace Sdict2db.Parsers {
	class TxtParser : Parser, IDisposable {

		#region Private members

		StreamReader _sr;

		#endregion

		#region Constants

		const string Splitter = "___";
		const string RecordPattern = "{0}___{1}";

		#endregion

		#region Construction

		public TxtParser(string file, Encoding encoding)
			: base (file, encoding) {
			_writeShot.TableRow = -1;
		}

		#endregion

		#region Pause/Resume parsing

		private static void SaveParsing() {
		}

		private void LoadParsing() {
			if (_sr == null) {
				_sr = new StreamReader(File);
			}
		}

		private struct WriteShot {
			public int TableRow;
		}

		WriteShot _writeShot;

		private void SaveWriting(int row) {
			_writeShot.TableRow = row;
		}

		private void LoadWriting(ref int row, DataTable table,
				ref bool append) {
			if ((_writeShot.TableRow >= 0) &&
				(_writeShot.TableRow < table.Rows.Count)) {
				row = _writeShot.TableRow;
				append = true;
			} else {
				append = false;
			}
		}

		#endregion

		#region Private methods

		private void ReadArticles(DataTable table, DoWorkEventArgs e) {
			string line;
			int previousPercentage = 0;
			while ((line = _sr.ReadLine()) != null) {
				ProcessLine(table, line);
				if (e.Cancel) {
					SaveParsing();
					break;
				}
				int percentComplete = (int)((float)_sr.BaseStream.Position /
					(float)_sr.BaseStream.Length * 100);
				ReportPercentage(ref previousPercentage, percentComplete, e);
			}
		}

		private static void ProcessLine(DataTable table, string line) {
			string[] SplitArray;
			try {
				SplitArray = Regex.Split(line, Splitter);
				if (SplitArray.Length == 2) {
					table.Rows.Add(new object[] { SplitArray[0],
						SplitArray[1] });
				}
			} catch (ArgumentException) { }
		}

		private void WriteArticles(StreamWriter sw, int i, DataTable table,
				DoWorkEventArgs e) {
			int previousPercentage = 0;
			for (; i < table.Rows.Count; ++i) {
				string outputFormat = String.Format(
					CultureInfo.CurrentCulture, RecordPattern,
					table.Rows[i][0], table.Rows[i][1]);
				sw.WriteLine(outputFormat);
				if (e.Cancel) {
					SaveWriting(++i);
					break;
				}
				int percentComplete =
					(int)((float)i / (float)table.Rows.Count * 100);
				ReportPercentage(ref previousPercentage, percentComplete, e);
			}
		}

		#endregion

		#region Public methods

		public override void ParseFile(DataTable table, DoWorkEventArgs e) {
			try {
				if (_sr == null) {
					_sr = new StreamReader(File);
				}
				LoadParsing();
				ReadArticles(table, e);
			} catch (IOException ex) {
				MessageHelper.ShowErrorMessage(ex.Message);
			} catch (OutOfMemoryException ex) {
				MessageHelper.ShowErrorMessage(ex.Message);
			}
		}

		public override void WriteFile(DataTable table, DoWorkEventArgs e) {
			int i = 0;
			bool append = false;
			LoadWriting(ref i, table, ref append);
			using (StreamWriter sw = new StreamWriter(File, append)) {
				WriteArticles(sw, i, table, e);
			}
		}

		#endregion

		#region IDisposable Members

		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (_sr != null) {
						_sr.Close();
						_sr = null;
					}
				}
			} finally {
			base.Dispose(disposing);
			}
		}

		#endregion
	
	}
}
