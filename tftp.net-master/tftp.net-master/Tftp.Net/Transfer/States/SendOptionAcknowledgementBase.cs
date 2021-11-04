using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer
{
    class SendOptionAcknowledgementBase : StateThatExpectsMessagesFromDefaultEndPoint
    {
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            SendAndRepeat(new OptionAcknowledgement(Context.NegotiatedOptions.ToOptionList()));
        }

        public override void OnError(Error command)
        {
            Context.SetState(new ReceivedError(command));
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(reason));
        }
    }
}
