using Uno.Collections;

namespace Uno.Net.Http
{
    public static class HttpStatusReasonPhrase
    {
        //1xx: Informational - Request received, continuing process
        public static IDictionary<int, string> Informational = new Dictionary<int, string>
        {
            { 100, "Continue" },
            { 101, "Switching Protocols" }
        };

        //2xx: Success - The action was successfully received, understood, and accepted
        public static IDictionary<int, string> Success = new Dictionary<int, string>
        {
            { 200, "OK" },
            { 201, "Created" },
            { 202, "Accepted" },
            { 203, "Non-Authoritative Information" },
            { 204, "No Content" },
            { 205, "Reset Content" },
            { 206, "Partial Content" }
        };

        //3xx: Redirection - Further action must be taken in order to complete the request
        public static IDictionary<int, string> Redirection = new Dictionary<int, string>
        {
            { 300, "Multiple Choices" },
            { 301, "Moved Permanently" },
            { 302, "Found" },
            { 303, "See Other" },
            { 304, "Not Modified" },
            { 305, "Use Proxy" },
            { 307, "Temporary Redirect" }
        };

        //4xx: Client Error - The request contains bad syntax or cannot be fulfilled
        public static IDictionary<int, string> ClientErrors = new Dictionary<int, string>
        {
            { 400, "Bad Request" },
            { 401, "Unauthorized" },
            { 402, "Payment Required" },
            { 403, "Forbidden" },
            { 404, "Not Found" },
            { 405, "Method Not Allowed" },
            { 406, "Not Acceptable" },
            { 407, "Proxy Authentication Required" },
            { 408, "Request Time-out" },
            { 409, "Conflict" },
            { 410, "Gone" },
            { 411, "Length Required" },
            { 412, "Precondition Failed" },
            { 413, "Request Entity Too Large" },
            { 414, "Request-URI Too Large" },
            { 415, "Unsupported Media Type" },
            { 416, "Requested range not satisfiable" },
            { 417, "Expectation Failed" }
        };

        //5xx: Server Error - The server failed to fulfill an apparently valid request
        public static IDictionary<int, string> ServerErrors = new Dictionary<int, string>
        {
            { 500, "Internal Server Error" },
            { 501, "Not Implemented" },
            { 502, "Bad Gateway" },
            { 503, "Service Unavailable" },
            { 504, "Gateway Time-out" },
            { 505, "HTTP Version not supported" }
        };

        public static string GetFromStatusCode(int statusCode)
        {
            string description;
            if (Informational.TryGetValue(statusCode, out description))
                return description;

            if (Success.TryGetValue(statusCode, out description))
                return description;

            if (Redirection.TryGetValue(statusCode, out description))
                return description;

            if (ClientErrors.TryGetValue(statusCode, out description))
                return description;

            if (ServerErrors.TryGetValue(statusCode, out description))
                return description;

            return null;
        }
    }
}