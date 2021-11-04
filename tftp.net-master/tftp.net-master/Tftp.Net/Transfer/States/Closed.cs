namespace Tftp.Net.Transfer.States
{
    class Closed : BaseState
    {
        public override void OnStateEnter()
        {
            Context.Dispose();
        }
    }
}
