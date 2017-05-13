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
using System.Windows.Forms;
using System.Data;
using Sdict2db.Helpers;

namespace Sdict2db.PagedGridView {
	class DataPager : IDisposable {

		#region Private members

		private DataTable _sourceData;
		private BindingSource _currentData;
		private DataTable _page;

		private int _pageSize;
		private int _maxSelectionOffset;
		private int _selection;

		private SortOrder _sortOrder;
		private string _sortField;

		#endregion

		#region Properties

		public DataTable SourceData {
			get {
				return _sourceData;
			}
			set {
				Clear();
				_sourceData = value;
				_currentData =
					new BindingSource(_sourceData, _sourceData.TableName);
			}
		}

		public DataTable CurrentData {
			get {
				if (_currentData == null) {
					return SourceData;
				}
				DataView view = _currentData.List as DataView;
				if (view == null) {
					return null;
				} else {
					return view.ToTable();
				}
			}
		}

		public bool Empty {
			get {
				return (_sourceData == null) || (_currentData == null);
			}
		}

		public DataTable Page {
			get {
				return _page;
			}
		}

		public int Count {
			get {
				if ((_sourceData == null) ||
					(_sourceData.Rows == null)) {
					return 0;
				}
				return _sourceData.Rows.Count;
			}
		}

		public int CurrentCount {
			get {
				if (_currentData == null) {
					return 0;
				}
				return _currentData.CurrencyManager.Count;
			}
		}

		public int Selection {
			get {
				return _selection;
			}
			set {
				_selection = RangePosition(value);
				_currentData.Position = _selection;
			}
		}

		public int PageSize {
			get {
				return _pageSize;
			}
			set {
				_pageSize = NumberHelper.GetValueInRange(value, 1, value);
			}
		}

		public int MaxOffset {
			get {
				return _maxSelectionOffset;
			}
		}

		public SortOrder SortOrder {
			get {
				return _sortOrder;
			}
			set {
				_sortOrder = value;
			}
		}

		public string SortField {
			get {
				return _sortField;
			}
		}

		#endregion

		#region Construction

		public DataPager() {
		}

		public void Clear() {
			if (_sourceData != null) {
				_sourceData.Dispose();
				_sourceData = null;
			}
			if (_currentData != null) {
				_currentData.Dispose();
				_currentData = null;
			}
		}

		#endregion

		#region Events

		public event EventHandler<EventArgs> PageLoaded;

		private void PageLoadedHandler(object sender, EventArgs e) {
			if (PageLoaded != null) {
				PageLoaded(this, e);
			}
		}

		#endregion

		#region Paging

		public void SetPageSize(float pageSize) {
			PageSize = (int)Math.Ceiling(pageSize);
			_maxSelectionOffset = ((float)PageSize - pageSize) > 0.3 ?
				PageSize - 2 : PageSize - 1;
		}

		public void LoadDataPage(int currentScrollPosition) {
			if (Empty) {
				return;
			}
			_page = _sourceData.Clone();
			_page.Rows.Clear();
			DataView view = _currentData.List as DataView;
			if (view == null) {
				return;
			}
			for (int i = currentScrollPosition;
				i < currentScrollPosition + _pageSize; ++i) {
				if ((i >= 0) && (i < view.Count)) {
					_page.ImportRow(view[i].Row);
				}
			}
			PageLoadedHandler(this, new EventArgs());
		}

		#endregion

		#region Sorting/Filtering/Searching

		#region Sorting

		public void Sort(string field) {
			if (Empty || (!_sourceData.Columns.Contains(field))) {
				return;
			}
			ChooseSortMode();
			if (_sortOrder == SortOrder.None) {
				_currentData.RemoveSort();
				_sortField = string.Empty;
			} else {
				_currentData.Sort =
					StringHelper.CreateSortExpression(field, _sortOrder);
				_sortField = field;
			}
			Selection = 0;
		}

		private void ChooseSortMode() {
			if (_sortOrder == SortOrder.Ascending) {
				_sortOrder = SortOrder.Descending;
			} else if ((_sortOrder == SortOrder.Descending) &&
				(CurrentCount == Count)) {
				_sortOrder = SortOrder.None;
			} else {
				_sortOrder = SortOrder.Ascending;
			}
		}

		#endregion

		#region Filtering

		public void Filter(string[] fields, string[] values) {
			if (Empty || (!ContainsFields(_sourceData, fields))) {
				return;
			}
			if (StringHelper.ConsistsOfEmptyStrings(values)) {
				_currentData.RemoveFilter();
			} else {
				_currentData.Filter =
					StringHelper.CreateFilterExpression(fields, values);
			}
			Selection = 0;
		}

		#endregion

		#region Searching

		public bool Find(string field, string value) {
			if (Empty || (_sourceData.Rows.Count <= 0) ||
				(!_sourceData.Columns.Contains(field)) ||
				(string.IsNullOrEmpty(value))) {
				return false;
			}
			if (!string.IsNullOrEmpty(_currentData.Sort)) {
				_currentData.RemoveFilter();
			}
			int position = FindRecord(_currentData, field, value, false);
			if (position >= 0) {
				Selection = position;
				return true;
			}
			return false;
		}

		private static int FindRecord(BindingSource data, string field,
				string value, bool caseSensitive) {
			if (data == null) {
				return -1;
			}
			try {
				DataView view = data.List as DataView;
				if (view == null) {
					return -1;
				}
				for (int i = 0; i < view.Count; ++i) {
					if (StringHelper.BeginsWith(view[i].Row[field].ToString(),
						value, caseSensitive)) {
						return i;
					}
				}
			} catch (ArgumentException ex) {
				MessageHelper.ShowErrorMessage(ex.Message);
			}
			return -1;
		}

		#endregion

		#endregion

		#region Editing

		public void SubmitEditCurrentRow(DataGridViewCellEventArgs e) {
			DataRowView rowToBeEdited = _currentData.Current as DataRowView;
			if ((rowToBeEdited == null) ||
				(rowToBeEdited.Row.ItemArray.Length != _page.Columns.Count)) {
				return;
			}
			for (int i = 0; i < _page.Columns.Count; ++i) {
				rowToBeEdited.Row[i] = _page.Rows[e.RowIndex][i];
			}
		}

		#endregion

		#region Helpers

		private int RangePosition(int value) {
			return NumberHelper.GetValueInRange(value, 0, CurrentCount - 1);
		}

		private static bool ContainsFields(DataTable data, string[] fields) {
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (fields == null) {
				throw new ArgumentNullException("fields");
			}
			for (int i = 0; i < fields.Length; ++i) {
				if (!data.Columns.Contains(fields[i])) {
					return false;
				}
			}
			return true;
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				// dispose managed resources
				_currentData.Dispose();
			}
			// free native resources
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

	}
}
