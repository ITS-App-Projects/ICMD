using System.Net;
using System.Numerics;

namespace ICMD.Core.Account
{
    public class BaseResponse
    {
        public string Message { get; set; }
        public bool IsSucceeded { get; set; } = true;
        public bool IsWarning { get; set; } = false;
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public object? Data { get; set; }
        public bool IsModelValidation { get; set; }

        public BaseResponse() { }
        public BaseResponse(bool isSucceeded)
        {
            IsSucceeded = isSucceeded;
        }
        public BaseResponse(string message)
        {
            Message = message;
        }

        public BaseResponse(object data)
        {
            Data = data;
        }

        public BaseResponse(bool isSucceeded, string message, HttpStatusCode statusCode, object? data = null, bool isModelValidation = false)
        {
            IsSucceeded = isSucceeded;
            Message = message;
            StatusCode = statusCode;
            IsModelValidation = isModelValidation;
            Data = data;
        }
    }
}
