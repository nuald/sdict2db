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

using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;

namespace Sdict2db.Parsers {
	static class UnitReader {

		public static string ReadUnitAsString(BinaryReader binReader,
				int offset, int compress, Encoding encoding) {
			return encoding.GetString(Decompress(ReadUnit(binReader, offset),
				compress));
		}

		public static byte[] ReadUnit(BinaryReader binReader, int offset) {
			binReader.BaseStream.Position = offset;
			int lenght = binReader.ReadInt32();
			return binReader.ReadBytes(lenght);
		}

		private static byte[] Decompress(byte[] buffer, int compress) {
			if (compress == 1) {
				using (MemoryStream ms = new MemoryStream(buffer)) {
					// Assume max length of decompressed buffer 
					// is not greater than double original length:
					int doubleBufferLength = buffer.Length * 2;
					ms.Position = 0;
					byte[] decompressed = new byte[doubleBufferLength];
					Stream inflater = new InflaterInputStream(ms);
					int count = inflater.Read(decompressed, 0, doubleBufferLength);
					buffer = new byte[count];
					Array.ConstrainedCopy(decompressed, 0, buffer, 0, count);
				}
			}
			return buffer;
		}

	}
}
