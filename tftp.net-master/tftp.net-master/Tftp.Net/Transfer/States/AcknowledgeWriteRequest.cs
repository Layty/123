namespace Tftp.Net.Transfer.States
{
    class AcknowledgeWriteRequest : StateThatExpectsMessagesFromDefaultEndPoint
    {
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            SendAndRepeat(new Acknowledgement(0));
        }

        public override void OnData(Data command)
        {
            var nextState = new Receiving();
            Context.SetState(nextState);
            nextState.OnCommand(command, Context.GetConnection().RemoteEndpoint);
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(reason));
        }

        public override void OnError(Error command)
        {
            Context.SetState(new ReceivedError(command));
        }
    }
}
