namespace PrismaDB.Result
{
    public class ResultErrorResponse : ResultResponse
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public ResultErrorResponse()
        {
            ErrorCode = 0;
            ErrorMessage = "";
        }
    }
}
