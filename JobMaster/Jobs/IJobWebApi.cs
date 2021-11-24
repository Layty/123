using RestSharp;
using System.Collections.Generic;

namespace JobMaster.Jobs
{
    /// <summary>
    /// 使用WebApi对数据进行提交至后台
    /// </summary>
    public interface IJobWebApi<T>
    {
        string BaseUriString { get; set; }

        RestClient RestClient { get; set; }
        RestRequest RestRequest { get; set; }
        void InsertData(string meterId, List<T> data);
    }
}