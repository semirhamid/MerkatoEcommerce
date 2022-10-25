namespace ECommerce.DTO
{
    public class Pagination
    {
        public int Page { get; set; } = 1;
        private int recordPerPage = 10;
        private readonly int maxRecordPerPage = 50;
        public int RecordsPerPage {
            get
            {
                return recordPerPage;
            }
            set {
                recordPerPage = value >  maxRecordPerPage ? maxRecordPerPage : value;
            }
        }
    }
}
