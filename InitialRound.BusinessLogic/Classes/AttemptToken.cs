///
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Exceptions;

namespace InitialRound.BusinessLogic.Classes
{
    public class AttemptToken
    {
        public long Random { get; set; }

        public Guid InterviewQuestionID { get; set; }

        public Guid MostRecentAttemptID { get; set; }

        public AttemptToken(Guid interviewQuestionId, long random, Guid mostRecentAttemptId)
        {
            InterviewQuestionID = interviewQuestionId;
            Random = random;
            MostRecentAttemptID = mostRecentAttemptId;
        }

        public override string ToString()
        {
            return string.Format("{1},{2},{3}", InterviewQuestionID, Random, MostRecentAttemptID);
        }

        public static void Serialize(AttemptToken token, Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(token.InterviewQuestionID.ToByteArray(), 0, 16);
                writer.Write(token.Random);
                writer.Write(token.MostRecentAttemptID.ToByteArray(), 0, 16);
            }
        }

        public static AttemptToken Deserialize(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                var interviewQuestionId = reader.ReadBytes(16);
                var random = reader.ReadInt64();
                var mostRecentAttemptId = reader.ReadBytes(16);

                return new AttemptToken(new Guid(interviewQuestionId), random, new Guid(mostRecentAttemptId));
            }
        }

        public byte[] AsBytes()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serialize(this, memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static AttemptToken FromBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                return Deserialize(memoryStream);
            }
        }
    }
}
