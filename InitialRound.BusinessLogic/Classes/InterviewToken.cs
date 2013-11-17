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
    public class InterviewToken
    {
        public Guid InterviewID { get; set; }

        public long Random { get; set; }

        public InterviewToken(Guid interviewId, long random)
        {
            InterviewID = interviewId;
            Random = random;
        }

        public override string ToString()
        {
            return string.Format("{1},{2}", InterviewID, Random);
        }

        public static void Serialize(InterviewToken token, Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(token.InterviewID.ToByteArray(), 0, 16);
                writer.Write(token.Random);
            }
        }

        public static InterviewToken Deserialize(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                var interviewId = reader.ReadBytes(16);
                var random = reader.ReadInt64();

                return new InterviewToken(new Guid(interviewId), random);
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

        public static InterviewToken FromBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                return Deserialize(memoryStream);
            }
        }
    }
}
