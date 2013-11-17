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
    public class AntiForgeryToken
    {
        protected readonly string username;
        protected readonly string ipAddress;
        protected readonly DateTime expiresOn;
        protected readonly long random;

        public long Random
        {
            get { return random; }
        }

        public DateTime ExpiresOn
        {
            get { return expiresOn; }
        }

        public string IPAddress
        {
            get { return ipAddress; }
        }

        public string Username
        {
            get { return username; }
        }

        public AntiForgeryToken(string username, string ipAddress, DateTime expiresOn, long random)
        {
            this.username = username;
            this.ipAddress = ipAddress;
            this.expiresOn = expiresOn;
            this.random = random;
        }

        public static void Serialize(AntiForgeryToken token, Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(token.Username);
                writer.Write(token.IPAddress);
                writer.Write(token.ExpiresOn.Ticks);
                writer.Write(token.Random);
            }
        }

        public static AntiForgeryToken Deserialize(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                var username = reader.ReadString();
                var ipAddress = reader.ReadString();
                var expiresOn = reader.ReadInt64();
                var random = reader.ReadInt64();

                return new AntiForgeryToken(username, ipAddress, new DateTime(expiresOn), random);
            }
        }

        public override string ToString()
        {
            return string.Format("[Username='{0}', IPAddress='{1}', ExpiresOn={2}, Random={3}]", Username, IPAddress, ExpiresOn, Random);
        }

        public byte[] AsBytes()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serialize(this, memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static AntiForgeryToken FromBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                return Deserialize(memoryStream);
            }
        }
    }
}
