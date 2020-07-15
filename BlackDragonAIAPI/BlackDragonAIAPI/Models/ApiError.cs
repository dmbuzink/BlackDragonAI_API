using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlackDragonAIAPI.Models
{
    public class ApiError
    {
        public int StatusCode { get; private set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; private set; }

        public ApiError(HttpStatusCode statusCode)
        {
            this.StatusCode = (int)statusCode;
        }

        public ApiError(HttpStatusCode statusCode, string message)
            : this(statusCode)
        {
            this.Message = message;
        }
    }

    public class BadRequestError : ApiError
    {
        public BadRequestError(string message = null)
            : base(HttpStatusCode.BadRequest, message ?? "400 Bad Request. See: https://tools.ietf.org/html/rfc7231#section-6.5.1")
        { }
    }

    public class NotFoundError : ApiError
    {
        public NotFoundError(string message = null)
            : base(HttpStatusCode.NotFound, message ?? "404 Not Found. See: https://tools.ietf.org/html/rfc7231#section-6.5.4")
        { }
    }

    public class InternalServerError : ApiError
    {
        public InternalServerError(string message = null)
            : base(HttpStatusCode.InternalServerError, message ?? "500 Internal Server Error. See: https://tools.ietf.org/html/rfc7231#section-6.6.1")
        { }
    }

    public class UnauthorizedError : ApiError
    {
        public UnauthorizedError(string message = null) : base(HttpStatusCode.Unauthorized, message ?? "401 Unauthorized. See: https://tools.ietf.org/html/rfc7235#section-3.1")
        { }
    }
}
