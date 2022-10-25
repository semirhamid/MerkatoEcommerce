namespace ECommerce.Pagination
{
    public abstract class QueryStringParameters
    {
        const int maxRecordPerPage = 50;
        public int Page { get; set; } = 1;
        private int recordsPerPage = 5;
        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                recordsPerPage = (value > maxRecordPerPage) ? maxRecordPerPage : value;
            }
        }
    }
}
