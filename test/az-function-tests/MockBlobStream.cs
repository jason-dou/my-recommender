using System.Text;

namespace My.Function;
public class MockBlobStream : MemoryStream
{
    public override void Flush()
    {
        // Do nothing
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        // Do nothing
        return 0;
    }

    public override void SetLength(long value)
    {
        // Do nothing
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        // Return some test data
        byte[] testData = Encoding.UTF8.GetBytes("Hello, world!");
        Buffer.BlockCopy(testData, 0, buffer, offset, testData.Length);
        return testData.Length;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        // Do nothing
    }
}
