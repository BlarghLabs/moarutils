using System;
using System.Collections.Generic;
using System.Linq;

namespace MoarUtils.Utils.Geography {
  public static class States {
    public static List<State> los = new List<State> {
      //us
      new State("US","AL", "Alabama"),
      new State("US","AK", "Alaska"),
      new State("US","AZ", "Arizona"),
      new State("US","AR", "Arkansas"),
      new State("US","CA", "California"),
      new State("US","CO", "Colorado"),
      new State("US","CT", "Connecticut"),
      new State("US","DE", "Delaware"),
      new State("US","DC", "District Of Columbia"),
      new State("US","FL", "Florida"),
      new State("US","GA", "Georgia"),
      new State("US","HI", "Hawaii"),
      new State("US","ID", "Idaho"),
      new State("US","IL", "Illinois"),
      new State("US","IN", "Indiana"),
      new State("US","IA", "Iowa"),
      new State("US","KS", "Kansas"),
      new State("US","KY", "Kentucky"),
      new State("US","LA", "Louisiana"),
      new State("US","ME", "Maine"),
      new State("US","MD", "Maryland"),
      new State("US","MA", "Massachusetts"),
      new State("US","MI", "Michigan"),
      new State("US","MN", "Minnesota"),
      new State("US","MS", "Mississippi"),
      new State("US","MO", "Missouri"),
      new State("US","MT", "Montana"),
      new State("US","NE", "Nebraska"),
      new State("US","NV", "Nevada"),
      new State("US","NH", "New Hampshire"),
      new State("US","NJ", "New Jersey"),
      new State("US","NM", "New Mexico"),
      new State("US","NY", "New York"),
      new State("US","NC", "North Carolina"),
      new State("US","ND", "North Dakota"),
      new State("US","OH", "Ohio"),
      new State("US","OK", "Oklahoma"),
      new State("US","OR", "Oregon"),
      new State("US","PA", "Pennsylvania"),
      new State("US","RI", "Rhode Island"),
      new State("US","SC", "South Carolina"),
      new State("US","SD", "South Dakota"),
      new State("US","TN", "Tennessee"),
      new State("US","TX", "Texas"),
      new State("US","UT", "Utah"),
      new State("US","VT", "Vermont"),
      new State("US","VA", "Virginia"),
      new State("US","WA", "Washington"),
      new State("US","WV", "West Virginia"),
      new State("US","WI", "Wisconsin"),
      new State("US","WY", "Wyoming"),
      //canada
      new State("CA","AB", "Alberta"),
      new State("CA","BC", "British Columbia"),
      new State("CA","MB", "Manitoba"),
      new State("CA","NB", "New Brunswick"),
      new State("CA","NL", "Newfoundland and Labrador"),
      new State("CA","NS", "Nova Scotia"),
      new State("CA","NT", "Northwest Territories"),
      new State("CA","NU", "Nunavut"),
      new State("CA","ON", "Ontario"),
      new State("CA","PE", "Prince Edward Island"),
      new State("CA","QC", "Quebec"),
      new State("CA","SK", "Saskatchewan"),
      new State("CA","YT", "Yukon"),
      };

    public static List<string> StateCodes() {
      return los.Select(s => s.stateCode).ToList();
    }

    public static List<string> Names() {
      return los.Select(s => s.name).ToList();
    }

    public static string GetStateName(string stateCode) {
      return los.Where(s => s.stateCode.Equals(stateCode, StringComparison.CurrentCultureIgnoreCase)).Select(s => s.name).FirstOrDefault();
    }

    public static string GetStateCode(string name) {
      return los.Where(s => s.name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Select(s => s.stateCode).FirstOrDefault();
    }

    public static List<State> All {
      get {
        return los;
      }
    }

    public static List<State> US {
      get {
        return los.Where(s => los.IndexOf(s) <= 50).ToList();
      }
    }

    public static List<State> CA {
      get {
        return los.Where(s => los.IndexOf(s) > 50).ToList();
      }
    }
  }

  public class State {
    public State(string countryCode, string stateCode, string name) {
      this.countryCode = countryCode;
      this.stateCode = stateCode;
      this.name = name;
    }

    public string countryCode { get; set; }
    public string name { get; set; }
    public string stateCode { get; set; }

    public override string ToString() {
      return string.Format("{0} {1} {2}", countryCode, stateCode, name);
    }

  }
}
