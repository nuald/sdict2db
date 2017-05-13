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
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Sdict2db.Properties;

namespace Sdict2db.PagedGridView {
	class PagedDataGridView: IDisposable {

		#region Constants

		const int MinColumnHeaderHeight = 5;

		#endregion

		#region Private members

		private DataPager _dataPager;
		private Scroller _scroller;
		private DataGridView _gridView;
		private Panel _panel;
		private bool _columnResizingInProgress;

		#endregion

		#region Properties

		public DataTable SourceData {
			get {
				return _dataPager.SourceData;
			}
			set {
				_gridView.DataSource = null;
				_dataPager.SourceData = value;
				InitializeView();
			}
		}

		public DataTable CurrentData {
			get {
				return _dataPager.CurrentData;
			}
		}

		public int Count {
			get {
				return _dataPager.Count;
			}
		}

		public int CurrentCount {
			get {
				return _dataPager.CurrentCount;
			}
		}

		public bool Enabled {
			set {
				_panel.Enabled = value;
			}
		}

		#endregion

		#region Construction

		public PagedDataGridView(Panel panel) {
			if (panel == null) {
				throw new ArgumentNullException("panel");
			}
			_panel = panel;
			InitializeDataPager();
			InitializeScroller();
			_panel.SuspendLayout();
			InitializeGridView();
			_panel.Controls.Add(this._gridView);
			_panel.Controls.Add(this._scroller.ScrollBar);
			_panel.AutoSize = true;
			_panel.Dock = DockStyle.Fill;
			_panel.ResumeLayout();
		}

		private void InitializeDataPager() {
			_dataPager = new DataPager();
			_dataPager.PageLoaded += new EventHandler<EventArgs>(
				PageLoadedHandler);
		}

		private void InitializeScroller() {
			_scroller = new Scroller(IsScrollNecessary);
			_scroller.ScrollPositionChanged += new EventHandler<EventArgs>(
				ScrollPositionChangedHandler);
			_scroller.BeginScrolling += new EventHandler<EventArgs>(
				ScrollerBeginScrollingHandler);
			_scroller.EndScrolling += new EventHandler<EventArgs>(
				ScrollerEndScrollingHandler);
		}

		private void InitializeGridView() {
			_gridView = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(_gridView)).BeginInit();
			_gridView.Name = "_gridView";
			InitializeGridCellStyle();
			_gridView.AllowUserToAddRows = false;
			_gridView.AllowUserToDeleteRows = false;
			_gridView.AllowUserToResizeRows = false;
			_gridView.AllowUserToOrderColumns = false;
			_gridView.ColumnHeadersHeightSizeMode =
				DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			_gridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			_gridView.TabStop = true;
			_gridView.RowHeadersVisible = false;
			_gridView.AutoGenerateColumns = true;
			_gridView.ScrollBars = ScrollBars.None;
			_gridView.Dock = DockStyle.Fill;
			_gridView.ReadOnly = false;
			_gridView.EditMode = DataGridViewEditMode.EditOnF2;
			SubscribeGridEvents();
			((System.ComponentModel.ISupportInitialize)(_gridView)).EndInit();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
		private void InitializeGridCellStyle() {
			DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
			cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			cellStyle.BackColor = SystemColors.Window;
			Font font = new Font("Arial Unicode MS", 8.25F,
				FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
			cellStyle.Font = font;
			cellStyle.ForeColor = SystemColors.ControlText;
			cellStyle.SelectionBackColor = SystemColors.Highlight;
			cellStyle.SelectionForeColor = SystemColors.HighlightText;
			cellStyle.WrapMode = DataGridViewTriState.False;
			_gridView.DefaultCellStyle = cellStyle;
		}

		private void SubscribeGridEvents() {
			_gridView.ColumnWidthChanged += new DataGridViewColumnEventHandler(
				GridViewColumnWidthChanged);
			_gridView.Resize += new System.EventHandler(GridViewResize);
			_gridView.KeyDown += new KeyEventHandler(
				_scroller.KeyDownScrollHandler);
			_gridView.MouseWheel += new MouseEventHandler(
				_scroller.MouseWheelScrollHandler);
			_gridView.ColumnHeaderMouseClick +=
				new DataGridViewCellMouseEventHandler(
				GridViewColumnHeaderMouseClick);
			_gridView.CellClick += new DataGridViewCellEventHandler(
				GridViewCellClick);
			_gridView.CellEndEdit += new DataGridViewCellEventHandler(
				GridViewCellEndEdit);
		}

		public void Clear() {
			_gridView.DataSource = null;
			_dataPager.Clear();
			_scroller.Visible = false;
		}

		#endregion

		#region Event handlers

		private void ScrollPositionChangedHandler(object sender, EventArgs e) {
			_dataPager.LoadDataPage(_scroller.Position);
		}

		private void PageLoadedHandler(object sender, EventArgs e) {
			_gridView.DataSource = _dataPager.Page;
			SetCurrentGridSelection();
			SetSortGlyphs();
		}

		private void ScrollerBeginScrollingHandler(object sender, EventArgs e) {
			_panel.SuspendLayout();
		}

		private void ScrollerEndScrollingHandler(object sender, EventArgs e) {
			_panel.ResumeLayout();
		}

		private void GridViewColumnHeaderMouseClick(object sender,
				DataGridViewCellMouseEventArgs e) {
			DataGridView grid = sender as DataGridView;
			Sort(grid.Columns[e.ColumnIndex].Name);
		}

		#endregion
		
		#region Scrolling/paging

		private void InitializeView() {
			if (_dataPager.Empty) {
				return;
			}
			_dataPager.Selection = 0;
			InitializeSorting();
			ResizeView();
			_scroller.Position = 0;
			_gridView.Focus();
		}

		private void ResizeView() {
			if (_dataPager.Empty) {
				return;
			}
			_dataPager.SetPageSize(
				(float)(_gridView.Height - GetColumnHeaderHeight() -
				SystemInformation.Border3DSize.Height) /
				(float)_gridView.RowTemplate.Height);
			_scroller.ResizeScrolling(0, _dataPager.CurrentCount,
				1, _dataPager.MaxOffset);
			_dataPager.LoadDataPage(_scroller.Position);
			if (_gridView.Columns.Count > 0) {
				ResizeColumns(_gridView.Columns[_gridView.Columns.Count - 1]);
			}
		}

		private int GetColumnHeaderHeight() {
			if (_gridView.ColumnHeadersHeight > MinColumnHeaderHeight) {
				return _gridView.ColumnHeadersHeight;
			}
			return _gridView.RowTemplate.Height;
		}

		#endregion

		#region Grid selection

		private void GridViewCellClick(object sender,
				DataGridViewCellEventArgs e) {
			if ((_gridView.CurrentCell != null) && 
				(NumberHelper.IsInRange(_gridView.CurrentCell.RowIndex, 0,
				_dataPager.MaxOffset))) {
				_dataPager.Selection = _scroller.Position +
					_gridView.CurrentCell.RowIndex;
			}
		}

		private void SetCurrentGridSelection() {
			int offset = _dataPager.Selection - _scroller.Position;
			DataGridViewRowCollection rows = _gridView.Rows;
			for (int i = 0; i < _gridView.RowCount; ++i) {
				DataGridViewRow row = rows[i];
				if (i == offset) {
					row.Selected = true;
				} else {
					row.Selected = false;
				}
			}
		}

		private bool IsScrollNecessary(ScrollEventType scrollEventType) {
			CorrectScrollPosition();
			_dataPager.Selection = PreviewSelection(scrollEventType);
			if (!NumberHelper.IsInRange(
				_dataPager.Selection - _scroller.Position,
				0, _dataPager.MaxOffset)) {
				return true;
			}
			SetCurrentGridSelection();
			return false;
		}

		private int PreviewSelection(ScrollEventType scrollEventType) {
			if (scrollEventType == ScrollEventType.SmallDecrement) {
				return _dataPager.Selection - _scroller.SmallChange;
			} else if (scrollEventType == ScrollEventType.SmallIncrement) {
				return _dataPager.Selection + _scroller.SmallChange;
			} else if (scrollEventType == ScrollEventType.LargeDecrement) {
				if (_dataPager.Selection - _scroller.Position == 0) {
					return _dataPager.Selection - _scroller.LargeChange;
				} else {
					return _scroller.Position;
				}
			} else if (scrollEventType == ScrollEventType.LargeIncrement) {
				if (_dataPager.Selection - _scroller.Position ==
					_dataPager.MaxOffset) {
					return _dataPager.Selection + _scroller.LargeChange;
				} else {
					return _scroller.Position + _dataPager.MaxOffset;
				}
			} else if (scrollEventType == ScrollEventType.First) {
				return 0;
			} else if (scrollEventType == ScrollEventType.Last) {
				return _dataPager.CurrentCount - 1;
			}
			return 0;
		}

		private void CorrectScrollPosition() {
			if (!NumberHelper.IsInRange(
				_dataPager.Selection - _scroller.Position,
				0, _dataPager.MaxOffset)) {
				_scroller.Position = _dataPager.Selection;
			}
		}

		#endregion

		#region Editing

		private void GridViewCellEndEdit(object sender,
				DataGridViewCellEventArgs e) {
			_dataPager.SubmitEditCurrentRow(e);
		}

		#endregion

		#region Sorting/Filtering/Searching wrappers

		public void Sort(string field) {
			_dataPager.Sort(field);
			_dataPager.LoadDataPage(_scroller.Position);
		}

		public void Filter(string[] fields, string[] values) {
			_dataPager.Filter(fields, values);
			ResizeView();
			_scroller.Position = 0;
		}

		public void Search(string field, string value) {
			if (_dataPager.Find(field, value)) {
				_scroller.Position = _dataPager.Selection;
			}
		}

		private void InitializeSorting() {
			if ((_gridView == null) || (_gridView.Columns == null)) {
				return;
			}
			for (int i = 0; i < _gridView.ColumnCount; ++i) {
				_gridView.Columns[i].SortMode =
					DataGridViewColumnSortMode.Programmatic;
				_gridView.Columns[i].HeaderCell.SortGlyphDirection =
					SortOrder.None;
			}
			_dataPager.SortOrder = SortOrder.None;
		}

		private static SortOrder InvertSortOrder(SortOrder sortOrder) {
			SortOrder result = SortOrder.None;
			if (sortOrder == SortOrder.Ascending) {
				result = SortOrder.Descending;
			} else if (sortOrder == SortOrder.Descending) {
				result = SortOrder.Ascending;
			}
			return result;
		}

		private void SetSortGlyphs() {
			for (int i = 0; i < _gridView.ColumnCount; ++i) {
				_gridView.Columns[i].HeaderCell.SortGlyphDirection =
					SortOrder.None;
			}
			if (!string.IsNullOrEmpty(_dataPager.SortField)) {
				string field = _dataPager.SortField;
				_gridView.Columns[field].HeaderCell.SortGlyphDirection =
					InvertSortOrder(_dataPager.SortOrder);
			}
		}

		#endregion

		#region Column resizing

		private void GridViewColumnWidthChanged(object sender,
				DataGridViewColumnEventArgs e) {
			ResizeColumns(e.Column);
		}

		private void GridViewResize(object sender, EventArgs e) {
			ResizeView();
		}

		private void ResizeColumns(DataGridViewColumn c) {
			if (!_columnResizingInProgress) {
				_columnResizingInProgress = true;
				_panel.SuspendLayout();
				BalanceWidths(c);
				_panel.ResumeLayout();
				_columnResizingInProgress = false;
			}
		}

		private void BalanceWidths(DataGridViewColumn c) {
			int sum = GetAvailableColumnsWidthSum();
			if (c.Index == _gridView.Columns.Count - 1) {
				c.Width = sum -
					GetColumnsWidthWithoutIndex(_gridView.Columns.Count - 1);
			} else {
				_gridView.Columns[c.Index + 1].Width = sum -
					GetColumnsWidthWithoutIndices(c.Index, c.Index + 1) -
					_gridView.Columns[c.Index].Width;
			}
			AdjustWidths(c, sum);
		}

		private void AdjustWidths(DataGridViewColumn c, int availWidth) {
			if (GetColumnsWidthWithoutIndex(-1) == availWidth) {
				return;
			}
			for (int i = c.Index + 1; i < _gridView.Columns.Count; ++i) {
				_gridView.Columns[i].Width =
					availWidth - GetColumnsWidthWithoutIndex(i);
				if (GetColumnsWidthWithoutIndex(-1) == availWidth) {
					return;
				}
			}
			for (int i = c.Index; i >= 0; --i) {
				_gridView.Columns[i].Width =
					availWidth - GetColumnsWidthWithoutIndex(i);
				if (GetColumnsWidthWithoutIndex(-1) == availWidth) {
					return;
				}
			}
		}

		private int GetColumnsWidthWithoutIndex(int index) {
			int result = 0;
			for (int i = 0; i < _gridView.Columns.Count; ++i) {
				if (i != index) {
					result += _gridView.Columns[i].Width;
				}
			}
			return result;
		}

		private int GetColumnsWidthWithoutIndices(int index1, int index2) {
			int result = 0;
			for (int i = 0; i < _gridView.Columns.Count; ++i) {
				if ((i != index1) && (i != index2)) {
					result += _gridView.Columns[i].Width;
				}
			}
			return result;
		}

		private int GetAvailableColumnsWidthSum() {
			return _gridView.Width - SystemInformation.Border3DSize.Width;
		}

		#endregion

		#region Resources applying

		public void ApplyResources(string[] columnHeaders) {
			if ((_gridView.DataSource != null) &&
				(_gridView.ColumnCount == columnHeaders.Length)) {
				for (int i = 0; i < _gridView.ColumnCount; ++i ) {
					_gridView.Columns[i].HeaderText = columnHeaders[i];
				}
			}
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				// dispose managed resources
				_scroller.Dispose();
				_dataPager.Dispose();
				_gridView.Dispose();
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
