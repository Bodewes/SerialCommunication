using Channels.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComCommunication.Messages.Line
{
    class LineMessageSerialFinder : ISerialMessageFinder<LineMessage>
    {
        private readonly Channels.Serial.ISerialMessageConverter<LineMessage> _converter;

        public LineMessageSerialFinder(LineMessageSerialConverter converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public event Action<LineMessage> MessageFound;

        private byte[] buffer = new byte[0];

        public void handleBytes(byte[] bytes)
        {
            // Append new byte to current buffer.
            buffer = buffer.Append(bytes);

            while (FindMessageInBuffer()) ;

        }

        private bool FindMessageInBuffer()
        {
            int stopBytePosition = Array.IndexOf(buffer, (byte)'\n');
            if (stopBytePosition >= 0)
            {
                byte[] msgBytes = new byte[stopBytePosition];
                Buffer.BlockCopy(buffer, 0, msgBytes, 0, stopBytePosition);

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
