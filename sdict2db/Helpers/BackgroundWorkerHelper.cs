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
using Sdict2db.Helpers;

namespace Sdict2db.Helpers {
	static class BackgroundWorkerHelper {

		public static void UpdatePercentageProgress(BackgroundWorker worker,
				DoWorkEventArgs e) {
			if (worker.CancellationPending) {
				e.Cancel = true;
			}
			PercentageParameters parameters = e.Result as PercentageParameters;
			if ((parameters != null) &&
				(parameters.PercentCompleted > parameters.PercentageReached)) {
				worker.ReportProgress(parameters.PercentCompleted);
			}
		}

	}
}
