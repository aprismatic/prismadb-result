using System.Runtime.Serialization;

namespace PrismaDB.Result
{
    [DataContract]
    public class ResultErrorResponse : ResultNonQueryResponse
    {
        [DataMember]
        public int ErrorCode { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }

        public ResultErrorResponse(string errorMessage = "", string sqlState = "HY000", int errorCode = 0) : base()
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            SqlState = sqlState;
        }
    }
}
