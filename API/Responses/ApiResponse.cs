namespace API.Responses
{
    public class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public bool Status { get; set; }
        public T? Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(T? data, bool status = true, string message = "Success")
        {
            Data = data;
            Status = status;
            Message = message;
        }
    }
}
