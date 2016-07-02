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
using Sdict2db.Properties;
using Sdict2db.Helpers;

namespace Sdict2db {
	class BackgroundOperator : IDisposable {

		public delegate void DoBgWorkEventHandler(object sender,
			DoWorkEventArgs e);

		public delegate void BgProgressChangedEventHandler(object sender,
			ProgressChangedEventArgs e);

		public delegate void BgWorkerCompletedEventHandler(object sender,
			RunWorkerCompletedEventArgs e);

		#region Private members

		private BackgroundWorker _backgroundWorker;
		private OperationState _state;
		private RunWorkerCompletedEventArgs _resultArgs;

		private DoBgWorkEventHandler _backgroundWorkerRoutine;
		private BgProgressChangedEventHandler _progressChangedHandler;
		private BgWorkerCompletedEventHandler _backgroundCompletedHandler;

		#endregion

		#region Properties

		public RunWorkerCompletedEventArgs ResultArgs {
			get {
				return _resultArgs;
			}
		}

		#endregion

		#region Construction

		public BackgroundOperator() {
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.WorkerReportsProgress = true;
			_backgroundWorker.WorkerSupportsCancellation = true;
		}

		#endregion

		#region BackgoundWorker

		#region BackgoundWorker members wrapper

		#region Properties

		public bool IsBusy {
			get {
				return _backgroundWorker.IsBusy;
			}
		}

		public bool Cancelling {
			get {
				return _backgroundWorker.CancellationPending;
			}
		}

		#endregion

		public void StartWorker() {
			_state = OperationState.Started;
			_backgroundWorker.RunWorkerAsync();
		}

		public void PauseWorker() {
			if (_backgroundWorker.IsBusy &&
				!_backgroundWorker.CancellationPending) {
				_state = OperationState.Paused;
				_backgroundWorker.CancelAsync();
			}
		}

		public void Stop() {
			_state = OperationState.Stopped;
		}

		#endregion

		#region Event handlers

		private void OnPercentageChangedHandler(object sender,
				DoWorkEventArgs e) {
			BackgroundWorkerHelper.UpdatePercentageProgress(_backgroundWorker,
				e);
		}

		#endregion

		#region Event subscription

		private void UnsubscribeBackgoundWorker() {
			if (_backgroundWorkerRoutine != null) {
				_backgroundWorker.DoWork -=
				new DoWorkEventHandler(_backgroundWorkerRoutine);
			}
			if (_progressChangedHandler != null) {
			_backgroundWorker.ProgressChanged -=
				new ProgressChangedEventHandler(_progressChangedHandler);
			}
			_backgroundWorker.RunWorkerCompleted -=
				new RunWorkerCompletedEventHandler(OnBackgroundCompleted);
		}

		public void SubscribeBackgoundWorker(
				DoBgWorkEventHandler backgroundWorkerRoutine,
				BgProgressChangedEventHandler progressChangedHandler,
				BgWorkerCompletedEventHandler backgroundCompletedHandler) {
			UnsubscribeBackgoundWorker();

			_backgroundWorkerRoutine = backgroundWorkerRoutine;
			_progressChangedHandler = progressChangedHandler;
			_backgroundCompletedHandler = backgroundCompletedHandler;

			if (_backgroundWorkerRoutine != null) {
				_backgroundWorker.DoWork +=
					new DoWorkEventHandler(_backgroundWorkerRoutine);
			}
			if (_progressChangedHandler != null) {
				_backgroundWorker.ProgressChanged +=
					new ProgressChangedEventHandler(_progressChangedHandler);
			}
			_backgroundWorker.RunWorkerCompleted +=
				new RunWorkerCompletedEventHandler(OnBackgroundCompleted);
		}

		public void SubscribePercentageChanged(IProgressable invokator) {
			if (invokator != null) {
				invokator.PercentageChanged += OnPercentageChangedHandler;
			}
		}

		private void OnBackgroundCompleted(object sender,
				RunWorkerCompletedEventArgs e) {
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			if (e.Error != null) {
				_state = OperationState.Stopped;
				MessageHelper.ShowErrorMessage(e.Error.Message);
			}
			_resultArgs = e;
			if (_state != OperationState.Paused) {
				_backgroundCompletedHandler(sender, e);
			}
		}

		#endregion

		#endregion

		#region IDisposable Members

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (_backgroundWorker != null) {
					_backgroundWorker.Dispose();
					_backgroundWorker = null;
				}
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

	}
}
