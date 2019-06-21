using System.Runtime.Serialization;

namespace PrismaDB.Result
{
    [DataContract]
    public abstract class ResultResponse
    {
        [DataMember]
        public int RowsAffected { get; set; }
        [DataMember]
        public string SqlState { get; set; }

        public ResultResponse()
        {
            RowsAffected = 0;
            SqlState = "00000";
        }
    }
}
