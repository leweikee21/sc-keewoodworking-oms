using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data,string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }
        public Response(string message)
        {
            Succeeded = false;
            Message = message;
        }
        public static Response<T> WithException(string id, string message, string exceptionType)
        {
            return new Response<T>
            {
                Succeeded = false,
                Id = id,
                Message = message,
                ExceptionType = exceptionType
            };
        }

        public static Response<T> WithException(int id, string message, string exceptionType)
        {
            return new Response<T>
            {
                Succeeded = false,
                Id2 = id,
                Message = message,
                ExceptionType = exceptionType
            };
        }
        public string Id { get; set; }
        public int Id2 {  get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public string ExceptionType { get; set; }
        public T Data { get; set; }
    }
}
