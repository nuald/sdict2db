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

using System.Text;
using System.IO;
using System;
using Sdict2db.Helpers;

namespace Sdict2db.Parsers {
	/// <summary>
	/// Represents header structure of .dct dictionary files.
	/// </summary>
	public class DictHeader {

		#region Private members

		string _signature;
		string _inputLanguage;
		string _outputLanguage;
		string _title;
		string _copyright;
		string _version;
		byte _compressAndIndex;
		int _compress;
		int _index;
		int _amount;
		int _lenghtShot;
		int _titleLength;
		int _copyrightLength;
		int _versionLength;
		int _offsetShotIndex;
		int _offsetWordsIndex;
		int _offsetArticles;

		#endregion

		#region Properties

		/// <summary>
		/// Signature string.
		/// </summary>
		public string Signature {
			get { return _signature; }
		}

		/// <summary>
		/// Dictionary's input language.
		/// </summary>
		public string InputLanguage {
			get { return _inputLanguage; }
		}

		/// <summary>
		/// Dictionary's output language.
		/// </summary>
		public string OutputLanguage {
			get { return _outputLanguage; }
		}

		/// <summary>
		/// Title of dictionary file.
		/// </summary>
		public string Title {
			get { return _title; }
		}

		/// <summary>
		/// Copyright.
		/// </summary>
		public string Copyright {
			get { return _copyright; }
		}

		/// <summary>
		/// Dictionary file version.
		/// </summary>
		public string Version {
			get { return _version; }
		}

		/// <summary>
		/// Combined byte value containing information
		/// about compression and index.
		/// Lower 4 bits are for compression, Next 4 are or index.
		/// </summary>
		public byte CompressAndIndex {
			get { return _compressAndIndex; }
		}

		/// <summary>
		/// Compression method (0 - uncompressed, 1 - deflate).
		/// </summary>
		public int Compress {
			get { return _compress; }
		}

		/// <summary>
		/// Index.
		/// </summary>
		public int Index {
			get { return _index; }
		}

		/// <summary>
		/// Amount of word-article pairs in the dictionary.
		/// </summary>
		public int Amount {
			get { return _amount; }
		}

		/// <summary>
		/// Shot length.
		/// </summary>
		public int LenghtShot {
			get { return _lenghtShot; }
		}

		/// <summary>
		/// Title string length.
		/// </summary>
		public int TitleLength {
			get { return _titleLength; }
		}

		/// <summary>
		/// Copyright string length.
		/// </summary>
		public int CopyrightLength {
			get { return _copyrightLength; }
		}

		/// <summary>
		/// Version string length.
		/// </summary>
		public int VersionLength {
			get { return _versionLength; }
		}

		/// <summary>
		/// Offset to shot index.
		/// </summary>
		public int OffsetShotIndex {
			get { return _offsetShotIndex; }
		}

		/// <summary>
		/// Offset to beginning of words index part.
		/// </summary>
		public int OffsetWordsIndex {
			get { return _offsetWordsIndex; }
		}

		/// <summary>
		/// Offset to beginning of articles part.
		/// </summary>
		public int OffsetArticles {
			get { return _offsetArticles; }
		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the DictHeader class
		/// with data parsed from a binary stream.
		/// </summary>
		/// <param name="binReader">
		/// Binary stream for file header parsing.
		/// </param>
		/// <param name="encoding">
		/// Encoding to be used.
		/// </param>
		public DictHeader(BinaryReader binReader, Encoding encoding) {
			if (binReader == null) {
				throw new ArgumentNullException("binReader");
			}
			if (encoding == null) {
				throw new ArgumentNullException("encoding");
			}
			binReader.BaseStream.Position = 0;
			_signature =
				encoding.GetString(binReader.ReadBytes(4)).TrimEnd('\0');
			_inputLanguage =
				encoding.GetString(binReader.ReadBytes(3)).TrimEnd('\0');
			_outputLanguage =
				encoding.GetString(binReader.ReadBytes(3)).TrimEnd('\0');
			_compressAndIndex = binReader.ReadByte();
			_compress = (_compressAndIndex & 0xF);
			_index = (byte)(_compressAndIndex >> 4);
			_amount = binReader.ReadInt32();
			_lenghtShot = binReader.ReadInt32();
			_titleLength = binReader.ReadInt32();
			_copyrightLength = binReader.ReadInt32();
			_versionLength = binReader.ReadInt32();
			_offsetShotIndex = binReader.ReadInt32();
			_offsetWordsIndex = binReader.ReadInt32();
			_offsetArticles = binReader.ReadInt32();
			_title = UnitReader.ReadUnitAsString(binReader, _titleLength,
				_compress, encoding).TrimEnd('\0');
			_copyright = UnitReader.ReadUnitAsString(binReader,
				_copyrightLength, _compress, encoding).TrimEnd('\0');
			_version = UnitReader.ReadUnitAsString(binReader, _versionLength,
				_compress, encoding).TrimEnd('\0');
		}

		#endregion

	}
}
