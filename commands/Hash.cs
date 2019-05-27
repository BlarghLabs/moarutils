using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MoarUtils.Utils {
  public class Hash {
    public static string GetSha1(string path) {
      try {
        using (FileStream fs = new FileStream(path, FileMode.Open)) {
          return GetSha1(fs);
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        throw ex;
      }
    }
    public static string GetSha1(Stream s) {
      try {
        using (var bs = new BufferedStream(s)) {
          using (var sha1 = new SHA1Managed()) {
            byte[] hash = sha1.ComputeHash(bs);
            var formatted = new StringBuilder(2 * hash.Length);
            foreach (var b in hash) {
              formatted.AppendFormat("{0:X2}", b);
            }
            var sha1Hash = formatted.ToString();
            return sha1Hash;
          }
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        throw ex;
      }
    }

    public static string GetSha1(byte[] ba) {
      try {
        using (var sha1 = new SHA1Managed()) {
          byte[] hash = sha1.ComputeHash(ba);
          var formatted = new StringBuilder(2 * hash.Length);
          foreach (var b in hash) {
            formatted.AppendFormat("{0:X2}", b);
          }
          var sha1Hash = formatted.ToString();
          return sha1Hash;
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        throw ex;
      }
    }
  }
}
