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

using System.ComponentModel;
using System.Data;
using System;
using Sdict2db.Events;
using System.Text;
using Sdict2db.Helpers;

namespace Sdict2db.Parsers {
	abstract class Parser : IProgressable, IDisposable {

		#region Private members

		private string _file;
		private Encoding _encoding;

		#endregion

		#region Properties

		public string File {
			get { return _file; }
		}

		public Encoding TheEncoding {
			get { return _encoding; }
		}

		#endregion

		#region Construction

		public Parser(string file, Encoding encoding) {
			_file = file;
			_encoding = encoding;
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

		public abstract void ParseFile(DataTable table, DoWorkEventArgs e);

		public abstract void WriteFile(DataTable table, DoWorkEventArgs e);

		public void DeleteFile() {
			System.IO.File.Delete(_file);
		}

		#endregion

		#region Protected methods

		protected void ReportPercentage(ref int previousPercentage,
				int currentPercentage, DoWorkEventArgs e) {
			PercentageParameters args = new PercentageParameters(
				currentPercentage, previousPercentage);
			e.Result = args as object;
			if (e.Result == null) {
				return;
			}
			PercentageChangedHandler(this, e);
			previousPercentage = currentPercentage;
		}

		#endregion

		#region IDisposable Members

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}

		public virtual void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

	}
}
