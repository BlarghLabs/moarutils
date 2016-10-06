using System.Collections.Generic;
using System.Linq;

namespace MoarUtils.Utils {
  public class ModelStateUtils {
    public static List<string> GetErrorListFromModelStateMvc(
      System.Web.Mvc.ModelStateDictionary modelState
    ) {
      var query = from state in modelState.Values
                  from error in state.Errors
                  select error.ErrorMessage;

      var errorList = query.ToList();
      return errorList;
    }

    public static List<string> GetErrorListFromModelStateWebApi(
      System.Web.Http.ModelBinding.ModelStateDictionary modelState
    ) {
      var query = from state in modelState.Values
                  from error in state.Errors
                  select error.ErrorMessage;

      var errorList = query.ToList();
      return errorList;
    }
  }
}
