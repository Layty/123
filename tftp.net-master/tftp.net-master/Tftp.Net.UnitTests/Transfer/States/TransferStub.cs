using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;
using System.Net;
using System.IO;

namespace Tftp.Net.UnitTests
{
    class TransferStub : TftpTransfer
    {
        class Uninitialized : BaseState
        {
        }

        private ChannelStub Channel { get { return (ChannelStub)connection; } }
        public List<ITftpCommand> SentCommands { get { return Channel.SentCommands; } }
        public bool HadNetworkTimeout { get; set; }

        public TransferStub(MemoryStream stream)
            : base(new ChannelStub(), "dummy.txt", new Uninitialized()) 
        {
            base.InputOutputStream = stream;
            HadNetworkTimeout = false;
            this.OnError += new TftpErrorHandler(TransferStub_OnError);
        }

        void TransferStub_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            if (error is TimeoutError)
                HadNetworkTimeout = true;
        }

        public TransferStub()
            : this(null) { }

        public ITransferState State
        {
            get { return state; }
        }

        public void OnCommand(ITftpCommand command)
        {
            State.OnCommand(command, GetConnection().RemoteEndpoint);
        }

        public bool CommandWasSent(Type commandType)
        {
            return SentCommands.Any(x => x.GetType().IsAssignableFrom(commandType));
        }

        protected override ITransferState DecorateForLogging(ITransferState state)
        {
            return state;
        }

        public void OnTimer()
        {
            state.OnTimer();
        }

        public override void Dispose()
        {
            //Dont dispose the input/output stream during unit tests
            this.InputOutputStream = null;

            base.Dispose();
        }
    }

    class ChannelStub : ITransferChannel
    {
        public event TftpCommandHandler OnCommandReceived;
        public event TftpChannelErrorHandler OnError;
        public EndPoint RemoteEndpoint { get; set; }
        public readonly List<ITftpCommand> SentCommands = new List<ITftpCommand>();

        public ChannelStub()
        {
            RemoteEndpoint = new IPEndPoint(IPAddress.Loopback, 69);
        }

        public void Open()
        {
        }

        public void RaiseCommandReceived(ITftpCommand command, EndPoint endpoint)
        {
            if (OnCommandReceived != null)
                OnCommandReceived(command, endpoint);
        }

        public void RaiseOnError(TftpTransferError error)
        {
            if (OnError != null)
                OnError(error);
        }

        public void Send(ITftpCommand command)
        {
            SentCommands.Add(command);
        }

        public void Dispose()
        {
        }
    }
}
