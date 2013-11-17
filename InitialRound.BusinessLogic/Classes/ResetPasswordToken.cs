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
    public class ResetPasswordToken
    {
        public string Username { get; set; }

        public DateTime ExpiresOn { get; set; }

        public long Random { get; set; }

        public ResetPasswordToken(string username, DateTime expiresOn, long random)
        {
            Username = username;
            ExpiresOn = expiresOn;
            Random = random;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", Username, ExpiresOn, Random);
        }

        public static void Serialize(ResetPasswordToken token, Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(token.Username);
                writer.Write(token.ExpiresOn.Ticks);
                writer.Write(token.Random);
            }
        }

        public static ResetPasswordToken Deserialize(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                var username = reader.ReadString();
                var expiresOn = reader.ReadInt64();
                var random = reader.ReadInt64();

                return new ResetPasswordToken(username, new DateTime(expiresOn), random);
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

        public static ResetPasswordToken FromBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                return Deserialize(memoryStream);
            }
        }
    }
}
