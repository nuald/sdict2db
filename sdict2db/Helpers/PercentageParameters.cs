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

namespace Sdict2db.Helpers {
	/// <summary>
	/// Parameters, used when percentage progress
	/// of some continuous operation has changed.
	/// </summary>
	public class PercentageParameters {
		#region Private members

		int _percentCompleted;
		int _percentageReached;

		#endregion

		#region Properties

		/// <summary>
		/// Percentage that has been reached now, i.e. 'new value'.
		/// </summary>
		public int PercentCompleted {
			set { _percentCompleted = value; }
			get { return _percentCompleted; }
		}

		/// <summary>
		/// Percentage that was already reached early, i.e. 'old value'.
		/// </summary>
		public int PercentageReached {
			set { _percentageReached = value; }
			get { return _percentageReached; }
		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the PercentageChangedEventArgs class.
		/// </summary>
		public PercentageParameters() {
		}

		/// <summary>
		/// Initializes a new instance of the PercentageChangedEventArgs class
		/// with specified progress parameters.
		/// </summary>
		/// <param name="percentCompleted">
		/// Percentage that has been reached now, i.e. 'new value'.
		/// </param>
		/// <param name="percentageReached">
		/// Percentage that was already reached early, i.e. 'old value'.
		/// </param>
		public PercentageParameters(int percentCompleted,
			int percentageReached) {
			_percentCompleted = percentCompleted;
			PercentageReached = percentageReached;
		}

		#endregion
	}
}
