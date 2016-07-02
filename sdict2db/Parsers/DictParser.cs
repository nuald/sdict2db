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
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Data;
using Sdict2db.Helpers;
using Sdict2db.Properties;

namespace Sdict2db.Parsers {
	class DictParser : Parser {

		#region Construction

		public DictParser(string file, Encoding encoding) 
			: base (file, encoding) {
			_parseShot.Article = -1;
			_parseShot.Position = -1;
		}

		#endregion

		#region Pause/Resume parsing

		private struct ParseShot {
			public int Article;
			public int Position;
		}

		ParseShot _parseShot;

		private void SaveParsing(int article, int position) {
			_parseShot.Article = article;
			_parseShot.Position = position;
		}

		private void LoadParsing(ref int article, DictHeader header,
			ref int position, BinaryReader br) {
			if ((_parseShot.Article > 0) &&
				(_parseShot.Article <= header.Amount) &&
				(_parseShot.Position >= 0) &&
				(_parseShot.Position < br.BaseStream.Length)) {
				article = _parseShot.Article;
				position = _parseShot.Position;
			}
		}

		#endregion

		#region Private methods

		private DictHeader ReadHeader(BinaryReader binReader) {
			try {
				return new DictHeader(binReader, TheEncoding);
			} catch (EndOfStreamException) {
				MessageHelper.ShowErrorMessage(Resources.FileParsingErrorText);
				return null;
			} catch (IOException) {
				MessageHelper.ShowErrorMessage(Resources.FileParsingErrorText);
				return null;
			} catch (ObjectDisposedException) {
				MessageHelper.ShowErrorMessage(Resources.FileParsingErrorText);
				return null;
			}
		}

		private void ReadArticles(BinaryReader binReader, DataTable table,
				DictHeader header, DoWorkEventArgs e) {
			int position = 0;
			int i = 1;
			LoadParsing(ref i, header, ref position, binReader);
			int previousPercentage = 0;
			for (; i <= header.Amount; ++i) {
				string word, article;
				ReadWordArticlePair(binReader, ref position,
					out word, out article, header);
				table.Rows.Add(new object[] { word, article });
				if (e.Cancel) {
					SaveParsing(++i, position);
					break;
				}
				int percentComplete =
					(int)((float)i / (float)header.Amount * 100);
				ReportPercentage(ref previousPercentage, percentComplete, e);
			}
		}

		private void ReadWordArticlePair(BinaryReader binReader,
				ref int position, out string word, out string article,
				DictHeader header) {
			binReader.BaseStream.Position = header.OffsetWordsIndex + position;
			Int16 blockLength = binReader.ReadInt16();
			binReader.ReadInt16();
			int articlePointer = binReader.ReadInt32();
			int wordLength = blockLength - sizeof(Int16) -
				sizeof(Int16) - sizeof(Int32);
			word = TheEncoding.GetString(binReader.ReadBytes(wordLength));
			article = UnitReader.ReadUnitAsString(binReader,
				header.OffsetArticles + articlePointer,
				header.Compress, TheEncoding);
			position += blockLength;
		}

		#endregion

		#region Public methods

		public DictHeader ParseHeader() {
			using (FileStream inputStream = new FileStream(File,
				FileMode.Open, FileAccess.Read, FileShare.Read, 1024)) {
				using (BinaryReader binReader = new BinaryReader(inputStream)) {
					return ReadHeader(binReader);
				}
			}
		}

		public override void ParseFile(DataTable table, DoWorkEventArgs e) {
			if (table == null) {
				throw new ArgumentNullException("table");
			}
			if (table.Columns.Count != 2) {
				throw new ArgumentException(Resources.TableIsNotValidText);
			}
			try {
				using (FileStream inputStream = new FileStream(File,
					FileMode.Open, FileAccess.Read, FileShare.Read, 1024)) {
					using (BinaryReader binReader =
						new BinaryReader(inputStream)) {
						DictHeader header = ReadHeader(binReader);
						if (header != null) {
							binReader.BaseStream.Position =
								header.OffsetWordsIndex;
							ReadArticles(binReader, table, header, e);
						}
					}
				}
			} catch (EndOfStreamException) {
				MessageHelper.ShowErrorMessage(Resources.FileParsingErrorText);
			} catch (IOException) {
				MessageHelper.ShowErrorMessage(Resources.FileParsingErrorText);
			} catch (ObjectDisposedException) {
				MessageHelper.ShowErrorMessage(Resources.FileParsingErrorText);
			}
		}

		public override void WriteFile(DataTable table, DoWorkEventArgs e) {
			//throw new Exception("The method 'WriteFile' is not implemented.");
		}

		#endregion

	}
}
