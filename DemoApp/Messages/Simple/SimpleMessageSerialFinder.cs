using Channels.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComCommunication.Messages.Simple
{
    public class SimpleMessageSerialFinder : ISerialMessageFinder<SimpleMessage>
    {
        private readonly ISerialMessageConverter<SimpleMessage> _converter;
        private byte[] buffer = new byte[0];

        private byte StartByte => 0xAA;
        private byte StopByte => 0xCC;

        public SimpleMessageSerialFinder(ISerialMessageConverter<SimpleMessage> converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public event Action<SimpleMessage> MessageFound;

        public void handleBytes(byte[] receivedBytes)
        {
            // Append new bytes to current buffer
            buffer = buffer.Append(receivedBytes);

            // Find as many messages as possible.
            while (FindMessageInBuffer()) ;
        }


        private bool FindMessageInBuffer()
        {
            //find start and stop byte
            int startBytePosition = Array.IndexOf(buffer, this.StartByte);
            int stopBytePosition = Array.IndexOf(buffer, this.StopByte, startBytePosition + 1);

            if (startBytePosition >= 0 && stopBytePosition >= 0)
            {
                byte[] msgBytes = new byte[stopBytePosition - startBytePosition + 1];
                Buffer.BlockCopy(buffer, startBytePosition, msgBytes, 0, stopBytePosition - startBytePosition + 1);

                try
                {
                    var result = _converter.fromBytesArray(msgBytes);

                    // Invoke calleback with found msg.
                    MessageFound?.Invoke(result);

                    // strip used/parsed bytes from buffer.
                    buffer = buffer.Slice(stopBytePosition + 1, buffer.Length - (stopBytePosition + 1));
                    return true;
                }
                catch (Exception e)
                {
                    //_logger.Log($"Could not parse incomming msg: {e.Message}");
                    // todo: how to recover?
                    // - delete up to startbyte+1?
                    // - find an other stopbyte?
                }
            }
            return false;
        }
    }
}
