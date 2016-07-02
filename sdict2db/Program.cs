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
using System.Collections.Generic;
using System.Resources;
using System.Security.Permissions;
using System.Windows.Forms;
using Sdict2db.Helpers;

[assembly: CLSCompliant(true)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]
[assembly: NeutralResourcesLanguage("en-US")]
namespace Sdict2db {
	static class Program {

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			using (MainWindowPresenter presenter = new MainWindowPresenter()) {
				IMainView view = new MainForm();
				presenter.View = view;
				MainForm mainView = view as MainForm;
				if (mainView == null) {
					return;
				}
				Application.Run(mainView);
			}
		}

	}
}