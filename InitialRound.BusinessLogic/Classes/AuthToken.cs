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
    public class AuthToken : AntiForgeryToken
    {
        protected readonly bool isAdmin;

        public bool IsAdmin
        {
            get { return isAdmin; }
        }

        public AuthToken(string username, string ipAddress, DateTime expiresOn, bool isAdmin, long random)
            : base(username, ipAddress, expiresOn, random)
        {
            this.isAdmin = isAdmin;
        }

        public static void Serialize(AuthToken token, Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(token.Username);
                writer.Write(token.IPAddress);
                writer.Write(token.ExpiresOn.Ticks);
                writer.Write(token.IsAdmin);
                writer.Write(token.Random);
            }
        }

        public new static AuthToken Deserialize(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                var username = reader.ReadString();
                var ipAddress = reader.ReadString();
                var expiresOn = reader.ReadInt64();
                var isAdmin = reader.ReadBoolean();
                var random = reader.ReadInt64();

                return new AuthToken(username, ipAddress, new DateTime(expiresOn), isAdmin, random);
            }
        }

        public override string ToString()
        {
            return string.Format("[Username='{0}', IPAddress='{1}', ExpiresOn={2}, IsAdmin={3}, Random={4}]", Username, IPAddress, ExpiresOn, IsAdmin, Random);
        }

        public new byte[] AsBytes()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serialize(this, memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static new AuthToken FromBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                return Deserialize(memoryStream);
            }
        }
    }
}
