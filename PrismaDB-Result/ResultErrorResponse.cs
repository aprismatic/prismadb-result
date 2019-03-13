namespace PrismaDB.Result
{
    public class ResultErrorResponse : ResultNonQueryResponse
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public ResultErrorResponse(string errorMessage = "", string sqlState = "HY000", int errorCode = 0) : base()
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            SqlState = sqlState;
        }
    }
}
