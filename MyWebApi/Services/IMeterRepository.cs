using MyWebApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWebApi.Services
{
    public interface IMeterRepository
    {
        /// <summary>
        /// 增加表档案
        /// </summary>
        /// <param name="meterId"></param>
        void AddMeter(string meterId);
        /// <summary>
        /// 删除表档案
        /// </summary>
        /// <param name="meterId"></param>
        void DeleteMeter(string meterId);
        /// <summary>
        /// 获取所有的表档案
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Meter>> GetMeters();
        /// <summary>
        /// 获取指定表号的档案信息
        /// </summary>
        /// <param name="meterId"></param>
        /// <returns></returns>
        Task<Meter> GetMeter(string meterId);

        /// <summary>
        /// 对表插入电能曲线数据
        /// </summary>
        /// <param name="meterId"></param>
        /// <param name="energy"></param>
        void AddEnergyData(string meterId, List<Energy> energy);
        /// <summary>
        ///  对表插入功率曲线数据
        /// </summary>
        /// <param name="meterId"></param>
        /// <param name="powers"></param>
        void AddPowerData(string meterId, List<Power> powers);

        /// <summary>
        /// 对表插入日电能曲线数据
        /// </summary>
        /// <param name="meterId"></param>
        /// <param name="energy"></param>
        void AddDayData(string meterId, List<Day> energy);
        /// <summary>
        /// 查询所有数据？？不好
        /// </summary>
        /// <param name="meterId"></param>
        /// <returns></returns>
        Task<Meter> GetDataAsync(string meterId);

        /// <summary>
        /// 获取电能曲线数据
        /// </summary>
        /// <param name="meterId"></param>
        /// <returns></returns>
        Task<IEnumerable<Energy>> GetEnergyDataAsync(string meterId);
        /// <summary>
        /// 获取功率曲线数据
        /// </summary>
        /// <param name="meterId"></param>
        /// <returns></returns>
        Task<IEnumerable<Power>> GetPowerDataAsync(string meterId);


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