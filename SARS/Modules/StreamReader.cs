using System.IO;
using System.Text;

internal class StreamReaderOver : StreamReader
{
    public StreamReaderOver(Stream stream)
    : base(stream)
    {
    }

    public StreamReaderOver(string path, Encoding encoding) : base(path, encoding)
    {
    }

    public override string ReadLine()
    {
        StringBuilder sb = new StringBuilder();
        while (true)
        {
            int ch = Read();
            switch (ch)
            {
                case -1:
                    goto exitloop;
                case 10: // \n
                    sb.Append('\n');
                    goto exitloop;
                case 13: // \r
                    sb.Append('\r');
                    goto exitloop;
                default:
                    sb.Append((char)ch);
                    break;
            }
        }
    exitloop:
        return sb.Length > 0 ? sb.ToString() : null;
    }
}