using CommunityToolkit.HighPerformance.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProtocolStream
{
    public partial class ProtocolReader : IDisposable
    {
        private readonly ReadOnlyMemory<byte> _buffer;

        private readonly MemoryOwner<byte>? _memoryOwner;

        private readonly int _length;

        private int _bytePointer = 0;

        private int _bitPointer = 0;

        public ProtocolReader(byte[] buffer)
        {
            _length = buffer.Length;
            _buffer = buffer.AsMemory();
        }

        public ProtocolReader(ReadOnlySpan<byte> buffer)
        {
            _length = buffer.Length;
            _memoryOwner = MemoryOwner<byte>.Allocate(_length);
            _buffer = _memoryOwner.Memory;

            Unsafe.CopyBlock(ref _memoryOwner.Span[0], in buffer[0], (uint)_length);
        }

        public ProtocolReader(ReadOnlyMemory<byte> buffer)
        {
            _buffer = buffer;
            _length = buffer.Length;
        }

        public void Seek(int byteOffset)
        {
            _bytePointer = byteOffset;
            _bitPointer = 0;
        }

        public byte ReadByte()
        {
            var p = _bytePointer;
            _bytePointer += 1;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<byte>(in _buffer.Span[p]);
        }

        public sbyte ReadSByte()
        {
            var p = _bytePointer;
            _bytePointer += 1;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<sbyte>(in _buffer.Span[p]);
        }

        public ushort ReadUShort()
        {
            var p = _bytePointer;
            _bytePointer += 2;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<ushort>(in _buffer.Span[p]);
        }

        public short ReadShort()
        {
            var p = _bytePointer;
            _bytePointer += 2;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<short>(in _buffer.Span[p]);
        }

        public uint ReadUInt()
        {
            var p = _bytePointer;
            _bytePointer += 4;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<uint>(in _buffer.Span[p]);
        }

        public int ReadInt()
        {
            var p = _bytePointer;
            _bytePointer += 4;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<int>(in _buffer.Span[p]);
        }

        public ulong ReadULong()
        {
            var p = _bytePointer;
            _bytePointer += 8;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<ulong>(in _buffer.Span[p]);
        }

        public long ReadLong()
        {
            var p = _bytePointer;
            _bytePointer += 8;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<long>(in _buffer.Span[p]);
        }

        public UInt128 ReadUInt128()
        {
            var p = _bytePointer;
            _bytePointer += 16;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<UInt128>(in _buffer.Span[p]);
        }

        public Int128 ReadInt128()
        {
            var p = _bytePointer;
            _bytePointer += 16;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<Int128>(in _buffer.Span[p]);
        }

        public Half ReadHalf()
        {
            var p = _bytePointer;
            _bytePointer += 2;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<Half>(in _buffer.Span[p]);
        }

        public float ReadFloat()
        {
            var p = _bytePointer;
            _bytePointer += 4;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<float>(in _buffer.Span[p]);
        }

        public double ReadDouble()
        {
            var p = _bytePointer;
            _bytePointer += 8;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<double>(in _buffer.Span[p]);
        }

        public string ReadString(int length)
        {
            return this.ReadString(length, Encoding.ASCII);
        }

        public string ReadString(int length, Encoding encoding)
        {
            var p = _bytePointer;
            _bytePointer += length;
            this.ValidatePointer();
            return encoding.GetString(_buffer.Slice(p, length).Span)
                .TrimEnd(default(char));
        }

        public byte[] ReadBytes(int length)
        {
            var p = _bytePointer;
            _bytePointer += length;
            this.ValidatePointer();
            return _buffer.Slice(p, length).ToArray();
        }

        public bool ReadBoolean()
        {
            var p = _bytePointer;
            var q = _bitPointer;
            _bitPointer += 1;
            _bytePointer += _bitPointer >> 3;
            _bitPointer &= 7;
            this.ValidatePointer();
            return (_buffer.Span[p] >> q & 1) == 1;
        }

        private void ValidatePointer()
        {
            if (_bytePointer > _length || _bytePointer == _length && _bitPointer > 0)
            {
                throw new IndexOutOfRangeException(nameof(_bytePointer));
            }
        }

        public void Dispose()
        {
            _memoryOwner?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
