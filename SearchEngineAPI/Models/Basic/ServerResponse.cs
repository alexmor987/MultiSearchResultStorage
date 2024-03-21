using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Basic
{
    public class BaseResult
    {
        public bool IsError { get; set; }
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }

        public void SetError(int errorCode, string errorMessage)
        {
            IsError = true;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public void SetError(int errorCode)
        {
            IsError = true;
            ErrorCode = errorCode;

        }

        public BaseResult() { }
       

        public BaseResult(int errorCode)
        {
                SetError(errorCode);
        }
    }

    public class ServerResponse : BaseResult
    {

        public ServerResponse() { }
        public ServerResponse(int errorCode) : base(errorCode) { }

        public object? Data { get; set; }
    }
}
