using System.Text;

namespace ProtocolStream.Test;

public class ProtocolWriterTestCase
{
    [Fact]
    public void TestWritePrimitive()
    {
        var buffer = new byte[1024];
        var writer = new ProtocolWriter(buffer);

        int value1 = 0xff;
        Int128 value2 = 0xffeecc;

        writer.Write(value1);
        Assert.Equal(4, writer.AsSpan().Length);

        writer.Seek(5);
        writer.Write(value2);

        Assert.Equal(0xff, buffer[0]);
        Assert.Equal(0, buffer[1]);
        Assert.Equal(0, buffer[2]);
        Assert.Equal(0, buffer[3]);
        Assert.Equal(0xcc, buffer[5]);
        Assert.Equal(0xee, buffer[6]);
        Assert.Equal(0xff, buffer[7]);
        Assert.Equal(0, buffer[8]);
        Assert.Equal(0, buffer[9]);
        Assert.Equal(0, buffer[10]);
        Assert.Equal(0, buffer[11]);
        Assert.Equal(0, buffer[12]);
        Assert.Equal(21, writer.AsSpan().Length);
    }

    [Fact]
    public void TestWritePrimitiveWithBigEndian()
    {
        var buffer = new byte[1024];
        var writer = new ProtocolWriter(buffer);

        int value1 = 0xff;
        Int128 value2 = 0xffeecc;

        writer.WriteWithBigEndian(value1);
        Assert.Equal(4, writer.AsSpan().Length);

        writer.Seek(5);
        writer.WriteWithBigEndian(value2);

        Assert.Equal(0xff, buffer[3]);
        Assert.Equal(0, buffer[2]);
        Assert.Equal(0, buffer[1]);
        Assert.Equal(0, buffer[0]);
        Assert.Equal(0xcc, buffer[20]);
        Assert.Equal(0xee, buffer[19]);
        Assert.Equal(0xff, buffer[18]);
        Assert.Equal(0, buffer[17]);
        Assert.Equal(0, buffer[16]);
        Assert.Equal(0, buffer[15]);
        Assert.Equal(0, buffer[14]);
        Assert.Equal(0, buffer[13]);
        Assert.Equal(21, writer.AsSpan().Length);
    }

    [Fact]
    public void TestWriteBoolean()
    {
        var buffer = new byte[1024];
        var writer = new ProtocolWriter(buffer);

        writer.Write(true);
        writer.Seek(0, 2);
        writer.Write(true);
        writer.Seek(0, 4);
        writer.Write(true);
        Assert.Equal(0b10101, buffer[0]);

        writer.Seek(0, 2);
        writer.Write(false);
        Assert.Equal(0b10001, buffer[0]);

        Assert.Equal(1, writer.AsSpan().Length);
    }

    [Fact]
    public void TestWriteAligned()
    {
        var buffer = new byte[1024];
        var writer = new ProtocolWriter(buffer);

        int value1 = 0xff;
        Int128 value2 = 0xffeecc;

        writer.WriteAligned(value1, 100);
        Assert.Equal(13, writer.AsSpan().Length);

        writer.Seek(16, 2);
        writer.WriteAligned(value2, 17);

        Assert.Equal(0xff, buffer[0]);
        Assert.Equal(0, buffer[1]);
        Assert.Equal(0, buffer[2]);
        Assert.Equal(0, buffer[3]);
        Assert.Equal(0b110000, buffer[16]);
        Assert.Equal(0b10111011, buffer[17]);
        Assert.Equal(0b111, buffer[18]);
        Assert.Equal(0, buffer[19]);
    }

    [Fact]
    public void TestWriteAlignedInRange()
    {
        var buffer = new byte[1024];
        var writer = new ProtocolWriter(buffer);

        int value1 = 0xff;
        Int128 value2 = 0xffeecc;

        writer.WriteAligned(value1, 100);
        Assert.Equal(13, writer.AsSpan().Length);

        writer.Seek(16, 2);
        writer.WriteAligned(0xffffffff_ffffffff);

        writer.Seek(16, 2);
        writer.WriteAligned(value2, 17);

        Assert.Equal(0xff, buffer[0]);
        Assert.Equal(0, buffer[1]);
        Assert.Equal(0, buffer[2]);
        Assert.Equal(0, buffer[3]);
        Assert.Equal(0b110000, buffer[16]);
        Assert.Equal(0b10111011, buffer[17]);
        Assert.Equal(0b11111111, buffer[18]);
        Assert.Equal(0b11111111, buffer[19]);
        Assert.Equal(25, writer.AsSpan().Length);
    }

    [Fact]
    public void TestWriteAlignedOutRange()
    {
        var buffer = new byte[1024];
        var writer = new ProtocolWriter(buffer);

        int value1 = 0xff;
        Int128 value2 = 0xffeecc;

        writer.WriteAligned(value1, 100);
        Assert.Equal(13, writer.AsSpan().Length);

        writer.Seek(16, 2);
        writer.WriteAligned(0xffffffff_ffffffff);

        writer.Seek(32, 4);
        writer.WriteAligned(0xffffffff_ffffffff);

        writer.Seek(16, 2);
        writer.WriteAligned(value2, 130);

        Assert.Equal(0xff, buffer[0]);
        Assert.Equal(0, buffer[1]);
        Assert.Equal(0, buffer[2]);
        Assert.Equal(0, buffer[3]);
        Assert.Equal(0b110000, buffer[16]);
        Assert.Equal(0b10111011, buffer[17]);
        Assert.Equal(0b11111111, buffer[18]);
        Assert.Equal(0b11, buffer[19]);
        Assert.Equal(41, writer.AsSpan().Length);
        Assert.Equal(0b11110000, buffer[32]);
        Assert.Equal(0b11111111, buffer[33]);
    }

    [Fact]
    public void TestWriteString()
    {
        var str = "12345";
        var buffer = new byte[1024];
        var writer = new ProtocolWriter(buffer);

        writer.Write(str);
        Assert.Equal("12345", Encoding.ASCII.GetString(buffer, 0, 5));

        writer.Seek(0);
        str = "123";
        writer.Write(str, 5);
        Assert.Equal("123", Encoding.ASCII.GetString(buffer, 0, 5).TrimEnd(default(char)));
    }
}