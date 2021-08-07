using System;
using System.Collections.Generic;
using System.Text;

namespace TrueLayerChallenge.Domain.Exceptions
{
    public class NoDataFoundException : BusinessException
    {
        public NoDataFoundException(string msg, Guid id): base(msg)
        {
            Code = ErrorCode.NoDataFound;
            InstanceId = id;
        }
    }
}
