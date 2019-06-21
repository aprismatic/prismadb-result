using System.Runtime.Serialization;

namespace PrismaDB.Result
{
    [DataContract]
    public class ResultNoticeResponse : ResultNonQueryResponse
    {
        [DataMember]
        public string NoticeMessage { get; set; }

        public ResultNoticeResponse(string noticeMessage = "", string sqlState = "00000") : base()
        {
            NoticeMessage = noticeMessage;
            SqlState = sqlState;
        }
    }
}
