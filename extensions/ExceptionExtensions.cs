using System;
using System.Collections.Generic;

namespace MoarUtils.extensions {
  //https://codeshare.co.uk/blog/get-a-flat-list-of-exception-and-inner-exception-error-messages/
  public static class ExceptionExtensions {
    public static IEnumerable<Exception> FlattenHierarchy(this Exception ex) {
      if (ex == null) {
        throw new ArgumentNullException("ex");
      }

      var innerException = ex;
      do {
        yield return innerException;
        innerException = innerException.InnerException;
      }
      while (innerException != null);
    }
  }
}