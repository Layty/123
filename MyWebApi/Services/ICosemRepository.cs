using System.Collections.Generic;
using System.Threading.Tasks;
using MyWebApi.Entities;

namespace MyWebApi.Services
{
    public  interface ICosemRepository
    {
        /// <summary>
        /// 根据输入的OBIS对象集合进行查找
        /// </summary>
        /// <param name="obisEnumerable"></param>
        /// <returns></returns>
        Task<IEnumerable<CosemObject>> GetCosemObjectsAsync(IEnumerable<string> obisEnumerable);
        /// <summary>
        /// 查询所有的对象
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CosemObject>> GetCosemObjectsAsync();
        /// <summary>
        /// 根据输入的单个OBIS查找
        /// </summary>
        /// <param name="obis"></param>
        /// <returns></returns>
        Task<CosemObject> GetCosemObjectAsync(string obis);
        /// <summary>
        /// 根据输入的名称查找，返回集合
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IEnumerable<CosemObject>> GetCosemObjectsByNameAsync(string name);
        /// <summary>
        /// 根据输入的ClassId查找,返回集合
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        Task<IEnumerable<CosemObject>> GetCosemObjectsByClassIdAsync(int classId);
        /// <summary>
        /// 增
        /// </summary>
        /// <param name="cosemObject"></param>
        void AddCosemObject(CosemObject cosemObject);
        /// <summary>
        /// 改
        /// </summary>
        /// <param name="cosemObject"></param>
        void UpdateCosemObject(CosemObject cosemObject);
        /// <summary>
        /// 删
        /// </summary>
        /// <param name="cosemObject"></param>
        void DeleteCosemObject(CosemObject cosemObject);
        /// <summary>
        /// 验证输入的OBIS是否存在
        /// </summary>
        /// <param name="obis"></param>
        /// <returns></returns>
        Task<bool> CosemObjectExistsAsync(string obis);
        /// <summary>
        /// 保存操作
        /// </summary>
        /// <returns></returns>
        Task<bool> SaveAsync();
    }
}
