using System;
using System.Collections.Generic;
using System.Text;

namespace TrueLayerChallenge.Domain.Services
{
    public class HttpResultResponse<T> where T : class
    {
        public static HttpResultResponse<T> OK(T data)
        {
            return new HttpResultResponse<T>()
            {
                Data = data,
                IsSucceed = true,
            };
        }

        public static HttpResultResponse<T> Error(string message)
        {
            return new HttpResultResponse<T>()
            {
                Data = null,
                IsSucceed = false,
                ErrorMessage = message
            };
        }

        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSucceed { get; set; }
    }
}
