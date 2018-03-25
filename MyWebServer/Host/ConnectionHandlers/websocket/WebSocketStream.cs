using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Assets;

namespace Host.ConnectionHandlers
{
	public class WebSocketFrameReader {
		private NetworkStream _stream;
		private readonly static byte fin_mask = 0x80;
		private readonly static byte opcode_mask = 0x0F;
		private readonly static byte small_lenght = 0x7F;
		private uint total_lenght = 0;
		private byte[] mask_array = null;
		private byte fin;
		private byte mask;
		private byte opcode;
		private uint curr_read = 0;

		public bool isEndOfFrame { get { return total_lenght == curr_read; } }
		public uint Lenght { get { return total_lenght; } }
		public bool HasNextFrame { get { return fin == 0x00; } }

		public WebSocketFrameReader(NetworkStream stream) {
			_stream = stream;
			byte[] fin_lenght = new byte[2];
			_stream.Read(fin_lenght, 0, 2);
			fin = (byte)(fin_lenght[0] & fin_mask);
			opcode = (byte)(fin_lenght[0] & opcode_mask);
			if (opcode == 0x08 || opcode == 0x00) { throw new BreakConnection(); }
			mask = (byte)(fin_lenght[1] & fin_mask);
			byte lenght = (byte)(fin_lenght[1] & small_lenght);
			if (lenght == 126) {
				byte[] lenght_byte = new byte[4];
				for (int i = 1; i >= 0; i--) {
					_stream.Read(lenght_byte, i, 1);
				}
				total_lenght = BitConverter.ToUInt32(lenght_byte, 0);
			}
			else if (lenght == 127) {
				byte[] lenght_byte = new byte[8];
				_stream.Read(lenght_byte, 0, 8);
				for (int i = 7; i >= 0; i--) {
					_stream.Read(lenght_byte, i, 1);
				}
				total_lenght = BitConverter.ToUInt32(lenght_byte, 0);
			}
			else {
				total_lenght = lenght;
			}
			if (mask == 0x80) {
				mask_array = new byte[4];
				_stream.Read(mask_array, 0, 4);
			}
		}

		public int Read(byte[] buffer, int offset, int count) {
			if (count > total_lenght) {
				count = (int)total_lenght;
			}
			int res = _stream.Read(buffer, offset, count);
			if (mask == 0x80) {
				for (int i = offset; i < res; i++) {
					buffer[i] = (byte)(buffer[i] ^ mask_array[curr_read % 4]);
					curr_read++;
				}
			}
			return res;
		}
	}

	public class WebSocketStream : Stream {
		private NetworkStream _stream;
		private WebSocketFrameReader _reader;

		public WebSocketStream(NetworkStream stream) {
			_stream = stream;
		}

		public override bool CanRead { get { return _stream.DataAvailable; } }

		public override bool CanSeek { get { return false; } }

		public override bool CanWrite { get { return true; } }

		public override long Length { get { throw new NotImplementedException(); } }

		public override long Position {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public override void Flush() { throw new NotImplementedException(); }

		public override int Read(byte[] buffer, int offset, int count) {
			int read_count = 0;
			if (_reader == null) {
				_reader = new WebSocketFrameReader(_stream);
			}
			while (true) {
				read_count += _reader.Read(buffer, offset + read_count, count - read_count);

				if (_reader.isEndOfFrame && _reader.HasNextFrame && read_count < count) {
					_reader = new WebSocketFrameReader(_stream);
				}
				else {
					if (_reader.isEndOfFrame && _reader.HasNextFrame) {
						_reader = new WebSocketFrameReader(_stream);
					}
					else if (_reader.isEndOfFrame && !_reader.HasNextFrame) {
						_reader = null;
					}
					break;
				}
			}
			return read_count;
		}

		public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }

		public override void SetLength(long value) { throw new NotImplementedException(); }

		public override void Write(byte[] buffer, int offset, int count) {
			List<byte> two_byte = new List<byte>();
			//fin_opcode 
			two_byte.Add(0x81);
			//mask flag
			two_byte.Add(0x00);
			if (count < 126) {
				two_byte[1] = (byte)(two_byte[1] | Convert.ToByte(count));
			}
			else if (count >= 126 && count <= UInt16.MaxValue) {
				two_byte[1] = (byte)(two_byte[1] | (byte)126);
				UInt16 res = Convert.ToUInt16(count);
				byte[] count_res = BitConverter.GetBytes(res);
				for (int i = 1; i >= 0; i--) {
					two_byte.Add(count_res[i]);
				}
			}
			else {
				two_byte[1] = (byte)(two_byte[1] | (byte)127);
				UInt64 res = Convert.ToUInt64(count);
				byte[] count_res = BitConverter.GetBytes(res);
				for (int i = 7; i >= 0; i--) {
					two_byte.Add(count_res[i]);
				}
			}
			_stream.Write(two_byte.ToArray(), 0, two_byte.Count);
			//byte[] mask = new byte[8];
			//MyRandom.rnd.NextBytes(mask);
			//_stream.Write(mask, 0, 8);
			//int rnd_count = 0;
			//for (int i = offset; i < count; i++) {
			//	buffer[i] = (byte)(buffer[i] ^ mask[rnd_count % 8]);
			//	rnd_count++;
			//}
			_stream.Write(buffer, offset, count);
		}
	}
}
