using System.Text.RegularExpressions;

namespace MoarUtils.Utils {
  public class Network {
    public const string privIpRegex = @"(^127\.)|(^10\.)|(^172\.1[6-9]\.)|(^172\.2[0-9]\.)|(^172\.3[0-1]\.)|(^192\.168\.)";

    public static bool IsPrivateIpAddress(string ip) {
      bool result = Regex.IsMatch(ip, privIpRegex);
      return result;
    }
  }
}
