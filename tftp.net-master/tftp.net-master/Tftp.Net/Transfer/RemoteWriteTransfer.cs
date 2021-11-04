using System;
using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer
{
    class RemoteWriteTransfer : TftpTransfer
    {
        public RemoteWriteTransfer(ITransferChannel connection, String filename)
            : base(connection, filename, new StartOutgoingWrite())
        {
        }
    }
}
