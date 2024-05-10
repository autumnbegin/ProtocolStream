using System.Text;

namespace ProtocolStream.Test
{
    public class ProtocolReaderTestCase
    {
        [Fact]
        public void TestReadPrimitive()
        {
            byte[] buffer = [0b10110110, 0b11100001, 0b10010001];
            var reader = new ProtocolReader(buffer);

            Assert.Equal(0b10110110, reader.ReadByte());
            Assert.Equal(0b10010001_11100001, reader.ReadUShort());

            reader.Seek(0);
            Assert.Throws<IndexOutOfRangeException>(() => reader.ReadInt());
        }

        [Fact]
        public void TestReadPrimitiveWithBigEndian()
        {
            byte[] buffer = [0b10110110, 0b11100001, 0b10010001];
            var reader = new ProtocolReader(buffer);

            Assert.Equal(0b10110110_11100001, reader.ReadUShortWithBigEndian());

            reader.Seek(0);
            Assert.Throws<IndexOutOfRangeException>(()=> reader.ReadUIntWithBigEndian());
        }

        [Fact]
        public void TestReadBoolean()
        {
            byte[] buffer = [0b10110110, 0b11100001, 0b10010001];
            var reader = new ProtocolReader(buffer);

            Assert.False(reader.ReadBoolean());
            Assert.True(reader.ReadBoolean());
            Assert.True(reader.ReadBoolean());
            Assert.False(reader.ReadBoolean());
            reader.Seek(1, 3);
            Assert.False(reader.ReadBoolean());
            Assert.False(reader.ReadBoolean());
            Assert.True(reader.ReadBoolean());
            Assert.True(reader.ReadBoolean());
        }

        [Fact]
        public void TestReadAligned()
        {
            byte[] buffer = [0b10110110, 0b11100001, 0b10010001, 0b11010101];
            var reader = new ProtocolReader(buffer);

            reader.Seek(0, 2);
            Assert.Equal(0b01101101, reader.ReadAligned<byte>());
            Assert.Equal(0b0110010001111000, reader.ReadAligned<ushort>());

            reader.Seek(1, 3);
            Assert.Equal(0b00111100, reader.ReadAligned<byte>());
            reader.Seek(1, 3);
            Assert.Equal(0b1011001000111100, reader.ReadAligned<ushort>());
        }

        [Fact]
        public void TestReadAlignedInRange()
        {
            byte[] buffer = [0b10110110, 0b11100001, 0b10010001, 0b11010101];
            var reader = new ProtocolReader(buffer);

            Assert.Equal(0b01_11100001_10110110, reader.ReadAligned<int>(18));
            Assert.Equal(0b1_01100100, reader.ReadAligned<int>(9));
        }

        [Fact]
        public void TestReadAlignedOutRange()
        {
            byte[] buffer = [0b10110110, 0b11100001, 0b10010001, 0b11010101];
            var reader = new ProtocolReader(buffer);

            reader.Seek(0, 1);
            Assert.Equal(0b11011011, reader.ReadAligned<byte>(12));
            Assert.Equal(0b10101100_10001111, reader.ReadAligned<ushort>(19));
        }

        [Fact]
        public void TestReadString()
        {
            var str = "12345";
            var buffer = new byte[1024];
            var reader = new ProtocolReader(buffer);

            Encoding.ASCII.GetBytes(str.AsSpan(), buffer.AsSpan());
            Assert.Equal("12345", reader.ReadString(10));

            reader.Seek(0);
            Assert.Equal("123", reader.ReadString(3));
        }
    }
}
