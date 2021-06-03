namespace 三相智慧能源网关调试软件.Model.Jobs
{
    public interface IDataJob : IBaseJob
    {
        CustomCosemDataModel DataModel { get; set; }
    }
}