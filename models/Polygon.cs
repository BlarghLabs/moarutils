using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoarUtils.Model {
  public class Polygon {
    public List<Coordinate> loc = new List<Coordinate>();

    public List<Polygon> exclusionaryPolygons = new List<Polygon>();
  }
}
