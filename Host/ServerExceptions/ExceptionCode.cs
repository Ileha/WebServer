using System;

namespace MyWebServer.ServerExceptions
{
    public class ExceptionCode : Exception
    {
        private string Code;
        public readonly bool IsFatal;

        public ExceptionCode(bool is_fatal) {
            IsFatal = is_fatal;
        }

        public override string ToString() {
            return Code;
        }

        public static ExceptionCode BadRequest() {
            ExceptionCode res = new ExceptionCode(true);
            res.Code = "400 Bad Request";
            return res;
        }
        public static ExceptionCode OK() {
            ExceptionCode res = new ExceptionCode(false);
            res.Code = "200 OK";
            return res;
        }
        public static ExceptionCode NotFound() {
            ExceptionCode res = new ExceptionCode(true);
            res.Code = "404 Not Found";
            return res;
        }
        public static ExceptionCode InternalServerError() {
            ExceptionCode res = new ExceptionCode(true);
            res.Code = "500 Internal Server Error";
            return res;
        }
        public static ExceptionCode MovedPermanently(string new_url, string host) {
            ExceptionCode res = new ExceptionCode(true);
            res.Code = @"HTTP/1.1 301 Moved Permanently
Location: "+host+new_url;
            return res;
        }

    }
}
