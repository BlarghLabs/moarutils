using System.IO;
using System.Text;

namespace MoarUtils.Utils {
  public class Streams {
    public class RemoveEmptyLineWriter : MemoryStream {
      private readonly Stream filter;

      public RemoveEmptyLineWriter(Stream filter) {
        this.filter = filter;
      }

      public override void Write(byte[] buffer, int offset, int count) {
        var content = Encoding.UTF8.GetString(buffer);
        content = RemoveFirstEmptyLine(content);
        filter.Write(Encoding.UTF8.GetBytes(content), offset, Encoding.UTF8.GetByteCount(content));
      }

      private static string RemoveFirstEmptyLine(string content) {
        var firstLineIsEmpty = content.Substring(0, 2) == "\r\n";
        return firstLineIsEmpty ? content.Substring(2, content.Length - 2) : content;
      }
    }

    public static Stream GenerateStreamFromString(string s) {
      MemoryStream stream = new MemoryStream();
      StreamWriter writer = new StreamWriter(stream);
      writer.Write(s);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }

  }
}
