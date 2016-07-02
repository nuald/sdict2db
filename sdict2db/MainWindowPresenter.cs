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
using System.ComponentModel;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;
using Sdict2db.Events;
using Sdict2db.Helpers;
using Sdict2db.Properties;
using Sdict2db.Parsers;

namespace Sdict2db {

	class MainWindowPresenter : IDisposable {

		#region Private members

		Encoding _encoding = Encoding.UTF8;

		OperationType _operation;
		IMainView _view;
		BackgroundOperator _bg;
		Parser _parser;
		SqlDataFiller _sql;
		DataTable _dictionary;

		#endregion

		#region Properties

		public IMainView View {
			set {
				if (_view != null) {
					UnsubscribeFromView();
				}
				_view = value;
				if (_view != null) {
					SubscribeOnView();
				}
			}
		}

		#endregion

		#region Constructors

		public MainWindowPresenter() {
			_bg = new BackgroundOperator();
		}

		#endregion

		#region View events subscribtion

		private void SubscribeOnView() {
			_view.OnGetSqlParameters += OnGetSqlParametersHandler;
			_view.OnSaveFile += OnSaveFileHandler;
			_view.OnOpenFile += OnOpenFileHandler;
			_view.OnCancel += OnCancelHandler;
			_view.OnHardCancel += OnHardCancelHandler;
			_view.PauseWorker += PauseWorkerHandler;
			_view.ResumeWorker += ResumeWorkerHandler;
		}

		private void UnsubscribeFromView() {
			_view.OnGetSqlParameters -= OnGetSqlParametersHandler;
			_view.OnSaveFile -= OnSaveFileHandler;
			_view.OnOpenFile -= OnOpenFileHandler;
			_view.OnCancel -= OnCancelHandler;
			_view.OnHardCancel -= OnHardCancelHandler;
			_view.PauseWorker -= PauseWorkerHandler;
			_view.ResumeWorker -= ResumeWorkerHandler;
		}

		#endregion

		#region View event handlers

		#region Starting operations

		private void OnOpenFileHandler(object sender, FileOpsEventArgs e) {
			DisposeParser();
			InitDictionary();
			_dictionary.Locale = CultureInfo.InvariantCulture;
			if (e.Filename.EndsWith(Resources.TxtExtensionText)) {
				_parser = new TxtParser(e.Filename, _encoding);
			}
			if (e.Filename.EndsWith(Resources.DctExtensionText)) {
				_parser = new DictParser(e.Filename, _encoding);
				DictHeader header = ((DictParser)_parser).ParseHeader();
				if (header == null) {
					_view.OnBackgroundParsingCompletedHandler(this,
						new RunWorkerCompletedEventArgs(null, null, false),
						null);
					return;
				}
				_view.OutputHeaderInfo(header);
			}
			StartParsing();
		}

		private void StartParsing() {
			if (_parser != null) {
				_operation = OperationType.Parsing;
				_bg.SubscribeBackgoundWorker(ParseBackground,
					_view.BackgroundWorkChangedHandler,
					OnBackgroundParsingCompleted);
				_bg.SubscribePercentageChanged(_parser);
				_bg.StartWorker();
			}
		}

		private void InitDictionary() {
			DisposeDictionary();
			_dictionary = new DataTable();
			_dictionary.Locale = Thread.CurrentThread.CurrentUICulture;
			_dictionary.Columns.Add(new DataColumn(Resources.WordField,
				typeof(string)));
			_dictionary.Columns.Add(new DataColumn(Resources.TranslateField,
				typeof(string)));
		}

		private void OnSaveFileHandler(object sender, FileOpsEventArgs e) {
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			_dictionary = e.Data;
			if (_dictionary.Rows.Count == 0) {
				return;
			}
			DisposeParser();
			if (e.Filename.EndsWith(Resources.TxtExtensionText)) {
				_parser = new TxtParser(e.Filename, _encoding);
			}
			StartWriting();
		}

		private void StartWriting() {
			if (_parser != null) {
				_operation = OperationType.Writing;
				_bg.SubscribeBackgoundWorker(WriteBackground,
					_view.BackgroundWorkChangedHandler,
					OnBackgroundWritingCompleted);
				_bg.SubscribePercentageChanged(_parser);
				_bg.StartWorker();
			}
		}

		private void OnGetSqlParametersHandler(object sender,
				GetSqlParametersEventArgs e) {
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			_dictionary = e.Data;
			try {
				if (_dictionary != null && _dictionary.Rows.Count != 0) {
					_sql = new SqlDataFiller(e.ConnectionString, e.TableName);
					String info = _sql.PrepareFillTableSqlQuery();
					_view.OutputStatusInfo(info);
					StartSqlDataFilling();
				} else {
					_view.OnSqlBackgroundCompletedHandler(null, null);
				}
			} catch (SqlException e1) {
				MessageHelper.ShowErrorMessage(e1.Message);
				_view.ChangeInterfaceAfterError();
			} catch (InvalidOperationException e2) {
				MessageHelper.ShowErrorMessage(e2.Message);
				_view.ChangeInterfaceAfterError();
			}
		}

		private void StartSqlDataFilling() {
			_operation = OperationType.SqlDataFilling;
			_bg.SubscribeBackgoundWorker(FillTableBackground,
				_view.BackgroundWorkChangedHandler,
				OnSqlBackgroundCompleted);
			_bg.SubscribePercentageChanged(_sql);
			_bg.StartWorker();
		}

		#endregion

		#region Pausing/Resuming

		private void OnCancelHandler(object sender, EventArgs e) {
			_bg.Stop();
			switch (_operation) {
				case OperationType.Parsing: {
						_view.OnBackgroundParsingCompletedHandler(this,
							_bg.ResultArgs, _dictionary);
					} break;
				case OperationType.Writing: {
						_view.OnBackgroundWritingCompletedHandler(this,
							_bg.ResultArgs);
					} break;
				case OperationType.SqlDataFilling: {
						_view.OutputStatusInfo(
							Resources.ConnectionClosedText);
						_view.OnSqlBackgroundCompletedHandler(this,
							_bg.ResultArgs);
					} break;
			}
		}

		private void OnHardCancelHandler(object sender, EventArgs e) {
			RemoveResults();
			OnCancelHandler(sender, e);
		}

		private void PauseWorkerHandler(object sender, EventArgs e) {
			_bg.PauseWorker();
		}

		private void ResumeWorkerHandler(object sender, EventArgs e) {
			if (!_bg.IsBusy && !_bg.Cancelling) {
				switch (_operation) {
					case OperationType.Parsing: {
							StartParsing();
						} break;
					case OperationType.Writing: {
							StartWriting();
						} break;
					case OperationType.SqlDataFilling: {
							StartSqlDataFilling();
						} break;
				}
			}
		}

		private void RemoveResults() {
			switch (_operation) {
				case OperationType.Parsing: {
						RemoveParsingResults();
					} break;
				case OperationType.Writing: {
						RemoveWritingResults();
					} break;
				case OperationType.SqlDataFilling: {
						RemoveSqlDataFillingResults();
					} break;
			}
		}

		private void RemoveParsingResults() {
			_dictionary.Clear();
			_dictionary.Columns.Clear();
		}

		private void RemoveWritingResults() {
			_parser.DeleteFile();
		}

		private void RemoveSqlDataFillingResults() {
			_sql.DeleteTable();
		}

		#endregion

		#endregion

		#region Background operations

		private void ParseBackground(object sender, DoWorkEventArgs e) {
			_parser.ParseFile(_dictionary, e);
		}

		private void WriteBackground(object sender, DoWorkEventArgs e) {
			_parser.WriteFile(_dictionary, e);
		}

		private void FillTableBackground(object sender, DoWorkEventArgs e) {
			_sql.FillTable(_dictionary, e);
		}

		private void OnBackgroundParsingCompleted(object sender,
				RunWorkerCompletedEventArgs e) {
			_view.OnBackgroundParsingCompletedHandler(sender, e, _dictionary);
		}

		private void OnBackgroundWritingCompleted(object sender,
				RunWorkerCompletedEventArgs e) {
			_view.OnBackgroundWritingCompletedHandler(sender, e);
		}

		private void OnSqlBackgroundCompleted(object sender,
				RunWorkerCompletedEventArgs e) {
			_view.OutputStatusInfo(Resources.ConnectionClosedText);
			_view.OnSqlBackgroundCompletedHandler(sender, e);
		}

		#endregion

		#region IDisposable Members

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (_bg != null) {
					_bg.Dispose();
					_bg = null;
				}
				DisposeDictionary();
				DisposeParser();
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void DisposeParser() {
			if (_parser != null) {
				_parser.Dispose();
				_parser = null;
			}
		}

		private void DisposeDictionary() {
			if (_dictionary != null) {
				_dictionary.Dispose();
				_dictionary = null;
			}
		}

		#endregion
	}
}
