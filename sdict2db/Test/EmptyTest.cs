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
using NUnit.Framework;
using Sdict2db.Helpers;

namespace Sdict2db.Test {
	/// <summary>
	/// Empty test, just for checking.
	/// </summary>
	[TestFixture]
	public class EmptyTest {

		/// <summary>
		/// Trivial test, always must be passed successfully.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic")]
		[Test]
		public void Trivial() {
			Assert.IsTrue(true);
			Console.Out.WriteLine("Trivial test has run successfully.");
		}

	}
}
