using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Interfaces
{
    public interface IChannel<T>
    {
        void Send(T msg);

        void Open();

        void Close();

        event EventHandler<T> MessageReceived;
    }
}
