
using System.Collections;

namespace MoarUtils.Model {

  /// <summary>
  /// GeoAddress is an address object which includes latitude and longitude (and the precison of that geoCoding).
  /// </summary>
  public class YahooGeoAddress {
    public string street { get; set; }
    public string city { get; set; }
    public string stateCode { get; set; }
    public string zipCode { get; set; }
    public string latitude { get; set; }
    public string longitude { get; set; }
    public string precision { get; set; }
    public string warning { get; set; }
    public string errorMessage { get; set; }

    public YahooGeoAddress(string street, string city, string stateCode, string zipCode, string latitude, string longitude, string precision, string warning, string errorMessage) {
      this.street = street;
      this.city = city;
      this.stateCode = stateCode;
      this.zipCode = zipCode;
      this.latitude = latitude;
      this.longitude = longitude;
      this.precision = precision;
      this.warning = warning;
      this.errorMessage = errorMessage;
    }

    /*
    StringBuilder sbResult = new StringBuilder();
    sbResult.Append("<br/>Street:" + address.Street);
    sbResult.Append("<br/>City: " + address.City);
    sbResult.Append("<br/>State: " + address.StateCode);
    sbResult.Append("<br/>ZipCode: " + address.ZipCode);
    sbResult.Append("<br/>Latitude: " + address.Latitude);
    sbResult.Append("<br/>Longitude: " + address.Longitude);
    sbResult.Append("<br/>Precision: " + address.Precision);
    sbResult.Append("<br/>Warning: " + address.Warning);
    sbResult.Append("<br/>Error Message: " + address.ErrorMessage);
    */
  }
}