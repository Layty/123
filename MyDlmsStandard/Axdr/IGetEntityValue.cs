namespace MyDlmsStandard.Axdr
{
    public interface IGetEntityValue<out T>
    {
        T GetEntityValue();
    }
}