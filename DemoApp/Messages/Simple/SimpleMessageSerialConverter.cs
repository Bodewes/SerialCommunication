using Channels.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComCommunication.Messages.Simple
{
    public class SimpleMessageSerialConverter : ISerialMessageConverter<SimpleMessage>
    {
        private byte StartByte => 0xAA;
        private byte StopByte => 0xCC;
        
        public SimpleMessage fromBytesArray(byte[] bytes)
        {
            var result = new SimpleMessage()
            {
                Data = new byte[3]
            };

            if (bytes.Length != 5)
                throw new Exception("Byte count off");
            if (bytes[0] != StartByte)
                throw new Exception("First byte wrong");
            if (bytes[4] != StopByte)
                throw new Exception("Last byte wrong");

            Buffer.BlockCopy(bytes, 1, result.Data, 0, 3);

            return result;
        }

        public byte[] toByteArray(SimpleMessage msg )
        {
            if (msg.Data == null)
                throw new NullReferenceException("Data is null");

            var buffer = new byte[5];

            buffer[0] = StartByte;
            Buffer.BlockCopy(msg.Data, 0, buffer, 1, 3);
            buffer[4] = StopByte;

            return buffer;
        }

    }
}
