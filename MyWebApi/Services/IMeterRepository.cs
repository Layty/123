using System.Collections.Generic;
using System.Threading.Tasks;
using MyWebApi.Entities;

namespace MyWebApi.Services
{
    public interface IMeterRepository
    {
        /// <summary>
        /// 增加表号档案
        /// </summary>
        /// <param name="meterId"></param>
        void AddMeter(string meterId);

        void RemoveMeter(string meterId);

        Task<IEnumerable<Meter>> GetMeters();

        Task<Meter> GetMeter(string meterId);

        /// <summary>
        /// 对表插入电能数据
        /// </summary>
        /// <param name="meterId"></param>
        /// <param name="energy"></param>
        void AddEnergyData(string meterId, List<Energy> energy);
        void AddPowerData(string meterId, List<Power> powers);
        /// <summary>
        /// 查询所有数据？？不好
        /// </summary>
        /// <param name="meterId"></param>
        /// <returns></returns>
        Task<Meter> GetDataAsync(string meterId);
        Task<IEnumerable<Energy>> GetEnergyDataAsync(string meterId);
        /// <summary>
        /// 验证输入的meterId是否存在
        /// </summary>
        /// <param name="meterId"></param>
        /// <returns></returns>
        Task<bool> MeterIdExistsAsync(string meterId);

        /// <summary>
        /// 保存操作
        /// </summary>
        /// <returns></returns>
        Task<bool> SaveAsync();
    }
}