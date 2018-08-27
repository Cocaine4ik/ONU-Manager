using System;
using System.Collections.Generic;
using System.Text;

namespace ONU_Manager
{
    public class DataReceivedEventArgs:EventArgs
    {
        public readonly int BytesReceived;
        public readonly byte[] Data;

        internal DataReceivedEventArgs(int count, byte[] buffer)
        {
            BytesReceived = count;
            Data = buffer;
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(Data);
        }
    }
}
