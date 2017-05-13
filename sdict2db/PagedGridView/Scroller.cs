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
using System.ComponentModel;
using Sdict2db.Helpers;

namespace Sdict2db.PagedGridView {
	class Scroller: IDisposable {

		public delegate bool IsScrollNecessaryChecker(
			ScrollEventType scrollEventType);

		#region Private members

		private VScrollBar _vScrollBar;
		private int _currentScrollPosition;

		private IsScrollNecessaryChecker _isScrollNecessary;

		#endregion

		#region Properties

		public VScrollBar ScrollBar {
			get {
				return _vScrollBar;
			}
		}

		public int Position {
			get {
				return _currentScrollPosition;
			}
			set {
				_currentScrollPosition = RangePosition(value);
				ScrollPositionChangedHandler(this, new EventArgs());
				if (_vScrollBar.Value != _currentScrollPosition) {
					_vScrollBar.Value = _currentScrollPosition;
				}
			}
		}

		public int Minimum {
			get {
				return _vScrollBar.Minimum;
			}
			set {
				_vScrollBar.Minimum =
					NumberHelper.GetValueInRange(value, 0, value);
			}
		}

		public int Maximum {
			get {
				return _vScrollBar.Maximum;
			}
			set {
				_vScrollBar.Maximum =
					NumberHelper.GetValueInRange(value, 1, value);
			}
		}

		public int SmallChange {
			get {
				return _vScrollBar.SmallChange;
			}
			set {
				_vScrollBar.SmallChange = value;
			}
		}

		public int LargeChange {
			get {
				return _vScrollBar.LargeChange;
			}
			set {
				_vScrollBar.LargeChange = value;
			}
		}

		public bool Visible {
			set {
				_vScrollBar.Visible = value;
			}
		}

		#endregion

		#region Construction

		public Scroller(IsScrollNecessaryChecker isScrollNecessaryChecker) {
			if (isScrollNecessaryChecker == null) {
				throw new ArgumentNullException("isScrollNecessaryChecker");
			}
			_isScrollNecessary = isScrollNecessaryChecker;
			_vScrollBar = new System.Windows.Forms.VScrollBar();
			_vScrollBar.Name = "_vScrollBar";
			_vScrollBar.Dock = DockStyle.Right;
			_vScrollBar.Hide();

			_vScrollBar.Scroll += new ScrollEventHandler(
				ScrollBarScrollHandler);
			_vScrollBar.KeyDown += new KeyEventHandler(
				KeyDownScrollHandler);
			_vScrollBar.MouseWheel += new MouseEventHandler(
				MouseWheelScrollHandler);
		}

		#endregion

		#region Events

		public event EventHandler<EventArgs> ScrollPositionChanged;

		private void ScrollPositionChangedHandler(object sender, EventArgs e) {
			if (ScrollPositionChanged != null) {
				ScrollPositionChanged(this, e);
			}
		}

		public event EventHandler<EventArgs> BeginScrolling;

		private void BeginScrollingHandler(object sender, EventArgs e) {
			if (BeginScrolling != null) {
				BeginScrolling(this, e);
			}
		}

		public event EventHandler<EventArgs> EndScrolling;

		private void EndScrollingHandler(object sender, EventArgs e) {
			if (EndScrolling != null) {
				EndScrolling(this, e);
			}
		}
		
		#endregion

		#region Scrolling

		private void ScrollBarScrollHandler(object sender, ScrollEventArgs e) {
			if (e.ScrollOrientation != ScrollOrientation.VerticalScroll) {
				return;
			}
			BeginScrollingHandler(this, new EventArgs());
			if (e.Type == ScrollEventType.Last) {
				Position = Maximum;
			} else if (e.Type == ScrollEventType.First) {
				Position = Minimum;
			} else if (e.Type == ScrollEventType.SmallIncrement) {
				Position += SmallChange;
			} else if (e.Type == ScrollEventType.SmallDecrement) {
				Position -= SmallChange;
			} else if (e.Type == ScrollEventType.LargeIncrement) {
				Position += LargeChange;
			} else if (e.Type == ScrollEventType.LargeDecrement) {
				Position -= LargeChange;
			} else if (e.Type == ScrollEventType.ThumbTrack) {
				Position = e.NewValue;
			} else if (e.Type == ScrollEventType.ThumbPosition) {
				Position = e.NewValue;
			}
			EndScrollingHandler(this, new EventArgs());
		}

		public void MouseWheelScrollHandler(object sender,
				MouseEventArgs e) {
			BeginScrollingHandler(this, new EventArgs());
			if (e.Delta < 0) {
				if (Position < Maximum) {
					Position += SmallChange;
				}
			} else if (e.Delta > 0) {
				if (Position > Minimum) {
					Position -= SmallChange;
				}
			}
			EndScrollingHandler(this, new EventArgs());
		}

		public void KeyDownScrollHandler(object sender, KeyEventArgs e) {
			if ((e.KeyCode == Keys.Up) &&
				(_isScrollNecessary(ScrollEventType.SmallDecrement))) {
				_vScrollBar.Value = RangePosition(_vScrollBar.Value -
					_vScrollBar.SmallChange);
				InvokeScroll(ScrollEventType.SmallDecrement);
			} else if ((e.KeyCode == Keys.Down) &&
				(_isScrollNecessary(ScrollEventType.SmallIncrement))) {
				_vScrollBar.Value = RangePosition(_vScrollBar.Value +
					_vScrollBar.SmallChange);
				InvokeScroll(ScrollEventType.SmallIncrement);
			} else if ((e.KeyCode == Keys.PageUp) &&
				(_isScrollNecessary(ScrollEventType.LargeDecrement))) {
				_vScrollBar.Value = RangePosition(_vScrollBar.Value -
					_vScrollBar.LargeChange);
				InvokeScroll(ScrollEventType.LargeDecrement);
			} else if ((e.KeyCode == Keys.PageDown) &&
				(_isScrollNecessary(ScrollEventType.LargeIncrement))) {
				_vScrollBar.Value = RangePosition(_vScrollBar.Value +
					_vScrollBar.LargeChange);
				InvokeScroll(ScrollEventType.LargeIncrement);
			} else if (e.KeyCode == Keys.Home) {
				_isScrollNecessary(ScrollEventType.First);
				_vScrollBar.Value = _vScrollBar.Minimum;
				InvokeScroll(ScrollEventType.First);
			} else if (e.KeyCode == Keys.End) {
				_isScrollNecessary(ScrollEventType.Last);
				_vScrollBar.Value = _vScrollBar.Maximum;
				InvokeScroll(ScrollEventType.Last);
			}
			e.SuppressKeyPress = true;
		}

		private void InvokeScroll(ScrollEventType scrollEventType) {
			ScrollEventArgs args =
				new ScrollEventArgs(scrollEventType, _vScrollBar.Value,
				ScrollOrientation.VerticalScroll);
			ScrollBarScrollHandler(this, args);
		}

		public void ResizeScrolling(int minimun, int maximum,
				int smallChange, int largeChange) {
			Minimum = minimun;
			Maximum = maximum - LastPageChunk(largeChange);
			SmallChange = smallChange;
			LargeChange = largeChange;
			_vScrollBar.Visible = (maximum > largeChange) ? true : false;
		}

		#endregion

		#region Wrappers/Helpers

		private int RangePosition(int value) {
			return NumberHelper.GetValueInRange(value, Minimum, Maximum);
		}

		private static int LastPageChunk(int pageSize) {
			return 4 * pageSize / 5;
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				// dispose managed resources
				_vScrollBar.Dispose();
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
