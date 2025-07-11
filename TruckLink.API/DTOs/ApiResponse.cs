using Microsoft.OpenApi.Any;

namespace TruckLink.API.DTOs
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int? Code { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T? data, string message = "Success", int code = 200) => new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            Code = code
        };

        public static ApiResponse<T> Error(string message = "Something went wrong", int code = 500) => new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Code = code
        };

    }
}
