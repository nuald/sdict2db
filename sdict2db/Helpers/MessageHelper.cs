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
using Sdict2db.Properties;

namespace Sdict2db.Helpers {
	static class MessageHelper {

		public static void ShowErrorMessage(string text,
				RightToLeft rightToLeft) {
			MessageBoxOptions options = (MessageBoxOptions)0;
			if (rightToLeft == RightToLeft.Yes) {
				options =
					MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign;
			}
			MessageBox.Show(text, Resources.ErrorText, MessageBoxButtons.OK,
				MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
				options);
		}

		public static void ShowErrorMessage(string text) {
			ShowErrorMessage(text, RightToLeft.Inherit);
		}

		public static void ShowInfoMessage(string text, string caption,
				RightToLeft rightToLeft) {
			MessageBoxOptions options = (MessageBoxOptions)0;
			if (rightToLeft == RightToLeft.Yes) {
				options =
					MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign;
			}
			MessageBox.Show(text, caption, MessageBoxButtons.OK,
				MessageBoxIcon.Information, MessageBoxDefaultButton.Button1,
				options);
		}

		public static void ShowInfoMessage(string text, string caption) {
			ShowInfoMessage(text, caption, RightToLeft.Inherit);
		}

		public static DialogResult ShowQuestionMessage(string text,
				string caption, RightToLeft rightToLeft) {
			MessageBoxOptions options = (MessageBoxOptions)0;
			if (rightToLeft == RightToLeft.Yes) {
				options = MessageBoxOptions.RtlReading |
					MessageBoxOptions.RightAlign;
			}
			return MessageBox.Show(text, caption, MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
				options);
		}

		public static DialogResult ShowQuestionMessage(string text,
			string caption) {
			return ShowQuestionMessage(text, caption, RightToLeft.Inherit);
		}


	}
}
