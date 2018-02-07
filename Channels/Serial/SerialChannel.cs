using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Logger;
using System.IO;
using System.Threading;
using Interfaces;

namespace Channels.Serial
{
    public class SerialChannel<T> : IChannel<T>, IDisposable 
    {
        private SerialPort _port;
        private CancellationTokenSource cancellationTokenSource;

        private ILogger _logger;
        private ISerialMessageConverter<T> _converter;
        private ISerialMessageFinder<T> _finder;

        public SerialChannel(string portName, ILogger logger, ISerialMessageConverter<T> converter, ISerialMessageFinder<T> finder)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));

            _finder.MessageFound += (msg) => MessageReceived(this, msg);

            _port = new SerialPort(portName);
        }

        /// <summary>
        /// Opens a channel
        /// </summary>
        public void Open()
        {
            if (_port.IsOpen)
                return;

            _port.Open();
            _logger.Log("Serial Port open");
            cancellationTokenSource = new CancellationTokenSource();
            Read(cancellationTokenSource.Token);
        }

        public void Close()
        {
            if (!_port.IsOpen)
                return;
            cancellationTokenSource.Cancel();
            _port.Close();
            _logger.Log("Serial Port closed");
        }

        public void Send(T msg)
        {
            if (!_port.IsOpen)
                Open();

            byte[] dataToSend =  _converter.toByteArray(msg);

            _logger.Log($"Sending: {String.Join(",", msg)}");
            _port.Write(dataToSend, 0, dataToSend.Length);

        }

        public event EventHandler<T> MessageReceived;

        /// <summary>
        /// https://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport
        /// </summary>
        private async void Read(CancellationToken token)
        {
            _logger.Log("Serial Port Reading");
            byte[] buffer = new byte[255];
            while (_port.IsOpen)
            {
                try
                {
                    int actualLength = await _port.BaseStream.ReadAsync(buffer, 0, 255, token);
                    byte[] received = new byte[actualLength];
                    Buffer.BlockCopy(buffer, 0, received, 0, actualLength);
                    handleIncomingBytes(received);
                }
                catch (IOException exc)
                {
                    _logger.Log(exc.Message);
                }
            }
        }
        

        private void handleIncomingBytes(byte[] receivedBytes)
        {
            _logger.Log($"Receiving  {String.Join(",", receivedBytes)}");
            // Use the finder to find messages in the just received bytes.
            _finder.handleBytes(receivedBytes);
        }

        public void Dispose()
        {
            if (_port.IsOpen)
                Close();
        }
    }

}
