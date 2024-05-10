using BenchmarkDotNet.Attributes;
using System.IO;
using System.Text;

namespace ProtocolStream.Test.Benchmark
{
    [Config(typeof(BenchmarkConfig))]
    public class Benchmarks
    {
        [Benchmark]
        public void ProtocolWriterPrimativeScenario()
        {
            using var writer = new ProtocolWriter(32);

            writer.Write(1);
            writer.Write(2D);
            writer.Write(3F);
            writer.Write(4L);
            writer.Write(5U);

            var buffer = writer.AsSpan();
        }

        [Benchmark]
        public void BinaryWriterPrimativeScenario()
        {
            using var stream = new MemoryStream(32);
            using var writer = new BinaryWriter(stream);

            writer.Write(1);
            writer.Write(2D);
            writer.Write(3F);
            writer.Write(4L);
            writer.Write(5U);

            var buffer = stream.GetBuffer();
        }

        [Benchmark]
        public void ProtocolWriterStringScenario()
        {
            using var writer = new ProtocolWriter(8);

            writer.Write("12345678");

            var buffer = writer.AsSpan();
        }

        [Benchmark]
        public void BinaryWriterStringScenario()
        {
            using var stream = new MemoryStream(8);
            using var writer = new BinaryWriter(stream);

            writer.Write("12345678");

            var buffer = stream.GetBuffer();
        }

        [Benchmark]
        public void ProtocolReaderPrimativeScenario()
        {
            var buffer = new byte[8] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8 };
            using var reader = new ProtocolReader(buffer);

            _ = reader.ReadInt();
            _ = reader.ReadFloat();
        }

        [Benchmark]
        public void BinaryReaderPrimativeScenario()
        {
            var stream = new MemoryStream([0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8]);
            var reader = new BinaryReader(stream);

            _ = reader.ReadInt32();
            _ = reader.ReadSingle();
        }

        [Benchmark]
        public void ProtocolReaderStringScenario()
        {
            var buffer = new byte[8] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38 };
            using var reader = new ProtocolReader(buffer);

            _ = reader.ReadString(8, Encoding.ASCII);
        }

        [Benchmark]
        public void BinaryReaderStringScenario()
        {
            var buffer = new byte[8] { 0x07, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38 };
            using var stream = new MemoryStream(buffer);
            using var reader = new BinaryReader(stream, Encoding.ASCII);

            _ = reader.ReadString();
        }
    }
}
