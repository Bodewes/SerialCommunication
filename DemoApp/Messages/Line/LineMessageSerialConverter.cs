using Channels.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComCommunication.Messages.Line
{
    public class LineMessageSerialConverter : ISerialMessageConverter<LineMessage>
    {
        public LineMessage fromBytesArray(byte[] data)
        {
            var stringData = System.Text.Encoding.UTF8.GetString(data);

            var lm = new LineMessage()
            {
                Data = stringData.TrimEnd(new char[] { '\n', '\r' })
            };
            return lm;
        }

        public byte[] toByteArray(LineMessage msg)
        {
            var dataToSend = msg.Data;
            if (!dataToSend.EndsWith("\n"))
            {
                dataToSend += "\n"; // or \r\n ?
            }
            return System.Text.Encoding.UTF8.GetBytes(dataToSend);
        }
    }
}
