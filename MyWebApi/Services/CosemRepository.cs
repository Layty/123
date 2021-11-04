using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApi.Services
{
    public class CosemRepository : ICosemRepository
    {
        private readonly CosemObjectDbContext _dbContext;

        public CosemRepository(CosemObjectDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<CosemObject>> GetCosemObjectsAsync(IEnumerable<string> obisEnumerable)
        {
            if (obisEnumerable == null)
            {
                throw new ArgumentNullException(nameof(obisEnumerable));
            }

            return await _dbContext.CosemObjects
                .Where(x => obisEnumerable.Contains(x.Obis))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<CosemObject>> GetCosemObjectsAsync()
        {
            return await _dbContext.CosemObjects.ToArrayAsync();
        }

        public async Task<CosemObject> GetCosemObjectAsync(string obis)
        {
            if (obis == "")
            {
                throw new ArgumentNullException(nameof(obis));
            }

            return await _dbContext.CosemObjects.FirstOrDefaultAsync(x => x.Obis == obis);
        }

        public async Task<IEnumerable<CosemObject>> GetCosemObjectsByNameAsync(string name)
        {
            if (name == "")
            {
                throw new ArgumentNullException(nameof(name));
            }

            return await _dbContext.CosemObjects.Where(i => i.Name.Contains(name)).OrderBy(x => x.ClassId)
                .ToListAsync();
            ;
        }

        public async Task<IEnumerable<CosemObject>> GetCosemObjectsByClassIdAsync(int classId)
        {
            if (classId == 0)
            {
                throw new ArgumentNullException(nameof(classId));
            }

            return await _dbContext.CosemObjects.Where(i => i.ClassId.Equals(classId)).OrderBy(x => x.ClassId)
                .ToListAsync();
        }


        public void AddCosemObject(CosemObject cosemObject)
        {
            if (cosemObject == null)
            {
                throw new ArgumentNullException(nameof(cosemObject));
            }

            cosemObject.Id = Guid.NewGuid();
            _dbContext.CosemObjects.Add(cosemObject);
        }

        public void UpdateCosemObject(CosemObject cosemObject)
        {
            if (cosemObject == null)
            {
                throw new ArgumentNullException(nameof(cosemObject));
            }

            _dbContext.CosemObjects.Update(cosemObject);
        }

        public void DeleteCosemObject(CosemObject cosemObject)
        {
            if (cosemObject == null)
            {
                throw new ArgumentNullException(nameof(cosemObject));
            }

            _dbContext.CosemObjects.Remove(cosemObject);
        }

        public async Task<bool> CosemObjectExistsAsync(string obis)
        {
            if (obis == string.Empty)
            {
                throw new ArgumentNullException(nameof(obis));
            }

            return await _dbContext.CosemObjects.AnyAsync(x => x.Obis == obis);
        }

        public async Task<bool> SaveAsync()
        {
            return (await _dbContext.SaveChangesAsync()) >= 0;
        }
    }
}