using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Channels.Serial
{
    public interface ISerialMessageFinder<T>
    {

        void handleBytes(byte[] bytes);

        event Action<T> MessageFound;

    }
}
