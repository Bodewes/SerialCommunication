using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Channels.Serial
{

    public interface ISerialMessageConverter<T>
    {
        byte[] toByteArray(T msg);

        T fromBytesArray(byte[] data);
    }


}
