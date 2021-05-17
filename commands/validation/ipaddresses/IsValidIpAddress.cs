using MoarUtils.commands.logging;
using System;
using System.Net;

namespace MoarUtils.commands.validation.ipaddresses {
  public class IsValidIpAddress {
    public bool Execute(string input) {
      try {
        IPAddress address;
        if (IPAddress.TryParse(input, out address)) {
          switch (address.AddressFamily) {
            case System.Net.Sockets.AddressFamily.InterNetwork:
              // we have IPv4
              return true;
            case System.Net.Sockets.AddressFamily.InterNetworkV6:
              // we have IPv6
              return true;
            default:
              // umm... yeah... I'm going to need to take your red packet and...
              return false;
          }
        }
        return false;
      } catch (Exception ex) {
        LogIt.E(ex);
        return false;
      }

    }
  }
}