using DataAnalysis.Core.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Core.Data.Entity
{
    public class ResponseMsg<T>
    {
        public ResponseMsg() { }

        public int StatusCode { get; set; }
        public string StatusMsg { get; set; }
        public T Data { get; set; }


        public ResponseMsg<T> Ok()
        {
            ResponseMsg<T> responseMsg = new ResponseMsg<T>()
            {
                StatusCode = (int)StatusCodeEnum.Success
            };
            return responseMsg;
        }

        public  ResponseMsg<T> Ok(T t) 
        {
            ResponseMsg<T> responseMsg = new ResponseMsg<T>()
            {
                StatusCode = (int)StatusCodeEnum.Success,
                Data = t
            };
            return responseMsg;
        }

        public  ResponseMsg<T> Error(Exception exception)
        {
            ResponseMsg<T> responseMsg = new ResponseMsg<T>()
            {
                StatusCode = (int)StatusCodeEnum.Error,
                StatusMsg=exception.Message
            };
            return responseMsg;
        }
    }
}

