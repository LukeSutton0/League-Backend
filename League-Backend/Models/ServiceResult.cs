using System.Net;

namespace League_Backend.Models
{
    public class ServiceResult<T>
    {
        public bool IsSuccessful => Errors.Count == 0;
        public List<string> Errors { get; } = [];
        public HttpStatusCode StatusCode { get; private set; }
        public T? Data { get; private set; }

        private ServiceResult() { }

        // Success
        public static ServiceResult<T> Success(T data, HttpStatusCode status = HttpStatusCode.OK) => new()
        {
            Data = data,
            StatusCode = status
        };

        // Failure Factories
        public static ServiceResult<T> Failure(string error, HttpStatusCode status) => new()
        {
            Errors = { error },
            StatusCode = status
        };

        public static ServiceResult<T> NotFound(string message = "Not Found") => Failure(message, HttpStatusCode.NotFound);

        public static ServiceResult<T> BadRequest(string message = "Bad Request") => Failure(message, HttpStatusCode.BadRequest);

        public static ServiceResult<T> Unauthorized(string message = "Unauthorized") => Failure(message, HttpStatusCode.Unauthorized);

        public static ServiceResult<T> ServerError(string message = "Server Error") => Failure(message, HttpStatusCode.InternalServerError);
    }
}
