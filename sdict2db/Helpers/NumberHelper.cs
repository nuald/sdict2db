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
	static class NumberHelper {

		public static int GetValueInRange(int value, int minimum, int maximum) {
			if (value < minimum) {
				return minimum;
			}
			if (value > maximum) {
				return maximum;
			}
			return value;
		}

		public static bool IsInRange(int value, int minimum, int maximum) {
			if ((value < minimum) || (value > maximum)) {
				return false;
			}
			return true;
		}

	}
}
