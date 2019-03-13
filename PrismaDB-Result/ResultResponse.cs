namespace PrismaDB.Result
{
    public abstract class ResultResponse
    {
        public int RowsAffected { get; set; }
        public string SqlState { get; set; }

        public ResultResponse()
        {
            RowsAffected = 0;
            SqlState = "00000";
        }
    }
}
