namespace Buyora.Helper
{
    public class PageResult<T> where T : class
    {
        public List<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int  PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public string? CategoryName { get; set; }
        public string? Search { get; set; }
    }
}
