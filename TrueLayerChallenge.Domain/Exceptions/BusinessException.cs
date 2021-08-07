using System;
using System.Collections.Generic;
using System.Text;

namespace TrueLayerChallenge.Domain.Exceptions
{
    public abstract class BusinessException: Exception
    {
        public BusinessException(string msg) : base(msg)
        { }

        public ErrorCode Code { get; set; }
        public Guid InstanceId { get; set; }
    }
}
