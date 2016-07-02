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
using System.Windows.Forms;
using Sdict2db.Events;
using System.Data;
using Sdict2db.Parsers;

namespace Sdict2db {
	/// <summary>
	/// Defines a interface for some window component
	/// in order to interact with MainWindowPresenter instance.
	/// </summary>
	public interface IMainView {

		#region Events

		/// <summary>
		/// This event should be raised after retrieving SQL parameters
		/// in order to invoke further SQL operations.
		/// Event argument should contain SQL connection string and table name.
		/// </summary>
		event EventHandler<GetSqlParametersEventArgs> OnGetSqlParameters;

		/// <summary>
		/// This event should be raised when required
		/// to perform saving dictionary data to file.
		/// Event argument should contain file name to be saved.
		/// </summary>
		event EventHandler<FileOpsEventArgs> OnSaveFile;

		/// <summary>
		/// This event should be raised when required
		/// to perform parsing dictionary data from file.
		/// Event argument should contain name of file to be parsed.
		/// </summary>
		event EventHandler<FileOpsEventArgs> OnOpenFile;

		/// <summary>
		/// This event should be raised when a background operation
		/// was calcelled.
		/// </summary>
		event EventHandler<EventArgs> OnCancel;

		/// <summary>
		/// This event should be raised when a background operation was 
		/// cancelled and it is required to remove all changes that were made.
		/// </summary>
		event EventHandler<EventArgs> OnHardCancel;

		/// <summary>
		/// This event should be raised when a background operation is 
		/// required to be paused.
		/// </summary>
		event EventHandler<EventArgs> PauseWorker;

		/// <summary>
		/// This event should be raised when a background operation is 
		/// required to be resumed.
		/// </summary>
		event EventHandler<EventArgs> ResumeWorker;

		#endregion

		#region Event handlers

		/// <summary>
		/// The handler is called when progress of some
		/// continuous operation has changed.
		/// </summary>
		/// <param name="sender">
		/// Object which has raised event.
		/// </param>
		/// <param name="e">
		/// Arguments, containing information about progress.
		/// </param>
		void BackgroundWorkChangedHandler(object sender,
			ProgressChangedEventArgs e);

		/// <summary>
		/// The handler is called when dictionary file parsing
		/// has been finished.
		/// </summary>
		/// <param name="sender">
		/// Object which has raised event.
		/// </param>
		/// <param name="e">
		/// Event arguments, describing operation completion.
		/// </param>
		/// <param name="table">
		/// Represents parsed dictionary data. If operation has been canceled,
		/// the table contains partial data.
		/// </param>
		void OnBackgroundParsingCompletedHandler(object sender,
			RunWorkerCompletedEventArgs e, DataTable table);

		/// <summary>
		/// The handler is called when dictionary file writing
		/// has been finished.
		/// </summary>
		/// <param name="sender">
		/// Object which has raised event.
		/// </param>
		/// <param name="e">
		/// Event arguments, describing operation completion.
		/// </param>
		void OnBackgroundWritingCompletedHandler(object sender,
			RunWorkerCompletedEventArgs e);

		/// <summary>
		/// The handler is called when filling of SQL data table with dictionary
		/// has been finished.
		/// </summary>
		/// <param name="sender">
		/// Object which has raised event.
		/// </param>
		/// <param name="e">
		/// Event arguments, containing results of completed operation.
		/// </param>
		void OnSqlBackgroundCompletedHandler(object sender,
			RunWorkerCompletedEventArgs e);

		#endregion

		#region Other stuff

		/// <summary>
		/// Routine for outputting dict-file header information.
		/// </summary>
		/// <param name="hdr">
		/// Parsed dict-file header.
		/// </param>
		void OutputHeaderInfo(DictHeader hdr);

		/// <summary>
		/// Routine for outputting some status information text.
		/// </summary>
		/// <param name="info">
		/// Text to be output.
		/// </param>
		void OutputStatusInfo(string info);

		/// <summary>
		/// The method is called after an error has raised
		/// while performing some operatoin, to make some interface changes.
		/// </summary>
		void ChangeInterfaceAfterError();

		#endregion

	}
}
