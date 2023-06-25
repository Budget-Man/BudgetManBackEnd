namespace MayNghien.Models.Response.Base
{
    public class AppResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; } 
    }
}
