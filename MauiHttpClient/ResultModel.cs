namespace MauiHttpClient
{
    public class ResultModel<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }

    public class PageResultModel<T>
    {
        public int Total { get; set; }

        public List<T> List { get; set; }
    }
}
