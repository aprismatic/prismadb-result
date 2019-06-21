using System.Runtime.Serialization;

namespace PrismaDB.Result
{
    [DataContract]
    public class ResultNonQueryResponse : ResultResponse
    {
        [DataMember]
        public long LastInsertId { get; set; }
        [DataMember]
        public int Warnings { get; set; }
        public CommandTag Tag { get; set; }

        public ResultNonQueryResponse(int rowsAffected = 0, long lastInsertId = 0, int warnings = 0, CommandTag tag = CommandTag.INSERT) : base()
        {
            RowsAffected = rowsAffected;
            LastInsertId = lastInsertId;
            Warnings = warnings;
            Tag = tag;
        }
    }

    public enum CommandTag
    {
        INSERT,
        DELETE,
        UPDATE,
        SELECT,
        MOVE,
        FETCH,
        COPY
    }
}
