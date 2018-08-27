using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ONU_Manager
{
    public class TelnetClient : IDisposable, IServiceProvider
    {
        protected readonly Socket theSocket;
        private readonly byte[] m_buffer;
        private SocketError m_error;
        private readonly AsyncCallback receiveDataCallback;
        private readonly AsyncCallback initalizeCallback;
        private readonly BinaryReader m_reader;

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        protected void FireDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (DataReceived != null)
                DataReceived(sender, e);
        }

        protected void FireDataReceived(int count)
        {
            FireDataReceived(this, new DataReceivedEventArgs(count, m_buffer));
        }

        public TelnetClient()
        {
            theSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_buffer = new byte[32767];
            receiveDataCallback = new AsyncCallback(ReceiveData);
            initalizeCallback = new AsyncCallback(Initialize);
            m_reader = new BinaryReader(new MemoryStream(m_buffer), Encoding.ASCII);
        }

        private void Initialize(IAsyncResult ar)
        {
            theSocket.BeginReceive(m_buffer, 0, m_buffer.Length, SocketFlags.None, out m_error,
                receiveDataCallback, null);
        }

        private void ReceiveData(IAsyncResult ar)
        {
            FireDataReceived(theSocket.EndReceive(ar));
        }

        public void Send(byte[] buffer)
        {
            theSocket.EndSend(theSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, out m_error, initalizeCallback, null));
        }

        public void Send(string s)
        {
            Send(Encoding.ASCII.GetBytes(s));
        }

        public void Send(char c)
        {
            Send(c.ToString());
        }

        

        public void Connect(EndPoint remoteEP)
        {
            theSocket.Connect(remoteEP);
            Initialize(null);
        }

        public SocketError LastError
        {
            get { return m_error; }
        }

        public BinaryReader InputBuffer
        {
            get { return m_reader; }
        }

        public void Connect(string host, int port)
        {
            theSocket.Connect(host, port);
            Initialize(null);
        }

        public void Connect(IPAddress ip, int port)
        {
            theSocket.Connect(ip, port);
            Initialize(null);
        }

        public void Connect(byte[] ip, int port)
        {
            Connect(ConvertToIPv4(ip), port);
        }

        public void Connect(Uri url)
        {
            Connect(url.AbsoluteUri, url.Port);
            Initialize(null);
        }

        public void Disconnect()
        {
            theSocket.Disconnect(false);
        }

        

        #region IDisposable Members

        public void Dispose()
        {
            m_reader.Close();
            theSocket.Close();
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType.FullName == typeof(Socket).FullName)
                return theSocket;
            return null;
        }

        #endregion

        public static explicit operator Socket(TelnetClient client)
        {
            return client.theSocket;
        }

        public static IPAddress ConvertToIPv4(byte[] address)
        {
            return new IPAddress(address);
        }

        ~TelnetClient()
        {
            Dispose();
        }
    }
}
