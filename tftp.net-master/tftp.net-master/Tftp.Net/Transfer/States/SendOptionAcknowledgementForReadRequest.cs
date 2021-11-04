namespace Tftp.Net.Transfer.States
{
    class SendOptionAcknowledgementForReadRequest : SendOptionAcknowledgementBase
    {
        public override void OnAcknowledgement(Acknowledgement command)
        {
            if (command.BlockNumber == 0)
            {
                //We received an OACK, so let's get going ;)
                Context.SetState(new Sending());
            }
        }
    }
}
