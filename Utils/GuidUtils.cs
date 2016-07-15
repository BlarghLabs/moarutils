using System;

namespace MoarUtils.Utils {
  public class GuidUtils {
    //GUIDs stored as BINARY(16) in MySQL will require this to string compare

    public static Guid FlipEndian(Guid guid) {
      var newBytes = new byte[16];
      var oldBytes = guid.ToByteArray();

      for (var i = 8; i < 16; i++)
        newBytes[i] = oldBytes[i];

      newBytes[3] = oldBytes[0];
      newBytes[2] = oldBytes[1];
      newBytes[1] = oldBytes[2];
      newBytes[0] = oldBytes[3];
      newBytes[5] = oldBytes[4];
      newBytes[4] = oldBytes[5];
      newBytes[6] = oldBytes[7];
      newBytes[7] = oldBytes[6];

      return new Guid(newBytes);
    }
  }
}

////test inserting
////samepl insert
//var windowsGuid =  new Guid("ef954055-0a85-460c-8d6b-615564a2b0f5");
//var newSl = new sessions_log {
//guid_bin = GuidUtils.FlipEndian(windowsGuid),
//username = "_fake",
//action_timestamp = DateTime.Now,
//deployment_location = "",
//};
//if (swit.lib.Db.SessionsLog.Add(ref newSl)) {
////then retrieve
//var newSl1 = swit.lib.Db.SessionsLog.Get(guid: windowsGuid);

//LogIt.D(newSl1);
////then delete

//}


////samepl insert
//var windowsGuid = new Guid("91d832f6-bcdb-4ca2-aeff-b6d5522136ff");
//var newSl = new sessions_log {
//  //ensure stored as is seen above in mysql
//  guid_bin = GuidUtils.FlipEndian(windowsGuid),
//  username = "_fake",
//  action_timestamp = DateTime.Now,
//  deployment_location = "",
//};
//if (swit.lib.Db.SessionsLog.Add(ref newSl)) {
//  //then retrieve
//  var newSl1 = swit.lib.Db.SessionsLog.Get(guid: windowsGuid);

//  LogIt.D("mysql format:" + newSl1);
//  LogIt.D("wndws format:" + GuidUtils.FlipEndian(newSl1.guid_bin));
//  //then delete

//}