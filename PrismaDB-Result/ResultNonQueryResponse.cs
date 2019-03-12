namespace PrismaDB.Result
{
    public class ResultNonQueryResponse : ResultResponse
    {
        public long LastInsertId { get; set; }
        public int Warnings { get; set; }
        public string InfoMessage { get; set; }

        public ResultNonQueryResponse()
        {
            RowsAffected = 0;
            LastInsertId = 0;
            Warnings = 0;
            InfoMessage = "";
        }
    }
}
