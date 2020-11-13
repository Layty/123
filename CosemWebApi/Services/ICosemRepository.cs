using System.Collections.Generic;
using System.Threading.Tasks;
using CosemWebApi.Entities;

namespace CosemWebApi.Services
{
    public  interface ICosemRepository
    {
        Task<IEnumerable<CosemObject>> GetCosemObjectsAsync(IEnumerable<string> obisEnumerable);

        Task<IEnumerable<CosemObject>> GetCosemObjectsAsync();
        Task<CosemObject> GetCosemObjectAsync(string obis);
        Task<IEnumerable<CosemObject>> GetCosemObjectsByNameAsync(string name);
        Task<IEnumerable<CosemObject>> GetCosemObjectsByClassIdAsync(int classId);
        void AddCosemObject(CosemObject cosemObject);
        void UpdateCosemObject(CosemObject cosemObject);
        void DeleteCosemObject(CosemObject cosemObject);
        Task<bool> CosemObjectExistsAsync(string obis);
        Task<bool> SaveAsync();
    }
}
