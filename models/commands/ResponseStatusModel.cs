using System.Net;

namespace MoarUtils.models.commands {
  public class ResponseStatusModel {

    public HttpStatusCode httpStatusCode { get; set; } = HttpStatusCode.BadRequest;
    public string status { get; set; }
  }
}
