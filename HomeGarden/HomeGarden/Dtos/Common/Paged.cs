namespace HomeGarden.Dtos.Common
{

    public sealed class PagedRequest
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 20;
        public string? SortBy { get; set; }
        public bool Desc { get; set; } = true;
    }

    public sealed class PagedResult<T>
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public long Total { get; set; }
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

        public static PagedResult<T> Create(IEnumerable<T> items, int page, int size, long total)
        {
            return new PagedResult<T>
            {
                Page = page,
                Size = size,
                Total = total,
                Items = items.ToList()
            };
        }
    }

    public sealed class ApiResponse<T>
    {
        public bool Ok { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public int? StatusCode { get; set; }

        public static ApiResponse<T> Success(T data, string? message = null)
            => new ApiResponse<T> { Ok = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message)
            => new ApiResponse<T> { Ok = false, Message = message };

        public static ApiResponse<T> Fail(string msg, int statusCode)
           => new() { Ok = false, Message = msg, StatusCode = statusCode };
    }

    public static class ApiResponse
    {
        public static ApiResponse<T> Success<T>(T data, string? message = null)
            => ApiResponse<T>.Success(data, message);

        public static ApiResponse<T> Fail<T>(string message)
            => ApiResponse<T>.Fail(message);

        public static ApiResponse<T> Fail<T>(string message, int statusCode)
       => ApiResponse<T>.Fail(message, statusCode);
    }
}
