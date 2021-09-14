using RestSharp;

namespace 三相智慧能源网关调试软件.Model.Jobs
{
    /// <summary>
    /// 使用WebApi对数据进行提交至后台
    /// </summary>
    public interface IJobWebApi
    {
        string BaseUriString { get; set; }
        string MeterId { get; set; }
        RestClient RestClient { get; set; }
        RestRequest RestRequest { get; set; }
        void InsertData();
    }
}