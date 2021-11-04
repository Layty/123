using System.Collections.Generic;

namespace Tftp.Net.Transfer.States
{
    class StartIncomingWrite : BaseState
    {
        private readonly IEnumerable<TransferOption> optionsRequestedByClient;
        public StartIncomingWrite(IEnumerable<TransferOption> optionsRequestedByClient)
        {
            this.optionsRequestedByClient = optionsRequestedByClient;
        }

        public override void OnStateEnter()
        {
            Context.ProposedOptions = new TransferOptionSet(optionsRequestedByClient);
        }

        public override void OnStart()
        {
            //Do we have any acknowledged options?
            Context.FinishOptionNegotiation(Context.ProposedOptions);
            List<TransferOption> options = Context.NegotiatedOptions.ToOptionList();
            if (options.Count > 0)
            {
                Context.SetState(new SendOptionAcknowledgementForWriteRequest());
            }
            else
            {
                //Start receiving
                Context.SetState(new AcknowledgeWriteRequest());
            }
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(reason));
        }
    }
}
