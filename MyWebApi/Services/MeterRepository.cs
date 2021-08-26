using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities;

namespace MyWebApi.Services
{
    public class MeterRepository : IMeterRepository
    {
        private readonly MeterDbContext _dbContext;

        public MeterRepository(MeterDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        public void AddMeter(string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentNullException(nameof(meterId));
            }

            _dbContext.Meters.Add(new Meter() {MeterId = meterId});
        }

        public void DeleteMeter(string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentNullException(nameof(meterId));
            }

            var t1 = _dbContext.Meters.FirstOrDefault(t => t.MeterId == meterId);
            var t2 = _dbContext.Energies.Where(t => t.MeterId == meterId);
            _dbContext.Energies.RemoveRange(t2);
            _dbContext.Meters.Remove(t1);
        }

        public async Task<IEnumerable<Meter>> GetMeters()
        {
            return await _dbContext.Meters.ToListAsync();
        }

        public async Task<Meter> GetMeter(string meterId)
        {
            if (meterId == string.Empty)
            {
                throw new ArgumentNullException(nameof(meterId));
            }

            return await _dbContext.Meters.FirstOrDefaultAsync(x => x.MeterId == meterId);
        }

        public void AddEnergyData(string meterId, List<Energy> energy)
        {
            if (energy == null)
            {
                throw new ArgumentNullException(nameof(energy));
            }


            foreach (var energy1 in energy)
            {
                energy1.MeterId = meterId;
            }


            _dbContext.Energies.AddRange(energy);
        }

        public void AddPowerData(string meterId, List<Power> powers)
        {
            if (powers == null)
            {
                throw new ArgumentNullException(nameof(powers));
            }

            foreach (var power in powers)
            {
                power.MeterId = meterId;
            }

            _dbContext.Powers.AddRange(powers);
        }

        public async Task<Meter> GetDataAsync(string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentNullException(nameof(meterId));
            }

            return await _dbContext.Meters.FirstOrDefaultAsync(i => i.MeterId == meterId);
        }

        public async Task<IEnumerable<Energy>> GetEnergyDataAsync(string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentNullException(nameof(meterId));
            }

            return await _dbContext.Energies.Where(t => meterId == t.MeterId).ToListAsync();
        }

        public async Task<IEnumerable<Power>> GetPowerDataAsync(string meterId)
        {
            if (meterId == "")
            {
                throw new ArgumentNullException(nameof(meterId));
            }

            return await _dbContext.Powers.Where(t => meterId == t.MeterId).ToListAsync();
        }


        public async Task<bool> MeterIdExistsAsync(string meterId)
        {
            if (meterId == string.Empty)
            {
                throw new ArgumentNullException(nameof(meterId));
            }

            return await _dbContext.Meters.AnyAsync(x => x.MeterId == meterId);
        }


        public async Task<bool> SaveAsync()
        {
            return (await _dbContext.SaveChangesAsync()) >= 0;
        }
    }
}