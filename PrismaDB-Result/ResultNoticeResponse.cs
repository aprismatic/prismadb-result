namespace PrismaDB.Result
{
    public class ResultNoticeResponse : ResultNonQueryResponse
    {
        public string NoticeMessage { get; set; }

        public ResultNoticeResponse(string noticeMessage = "", string sqlState = "00000") : base()
        {
            NoticeMessage = noticeMessage;
            SqlState = sqlState;
        }
    }
}
