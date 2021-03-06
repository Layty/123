using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

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

            _dbContext.Meters.Add(new Meter() { MeterId = meterId });
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
                if (_dbContext.Energies.Any(t => t.DateTime == energy1.DateTime))
                {
                    var t = _dbContext.Energies.FirstOrDefault(t => t.DateTime == energy1.DateTime);
                    t.EnergyData = energy1.EnergyData;
                    //更新追踪的，必须改属性，不然调用update会报错
                    _dbContext.Energies.Update(t);
                }
                else
                {
                    //要判断是否已存在不然add时会报错
                    _dbContext.Energies.Add(energy1);
                }
            }




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
                if (_dbContext.Powers.Any(t => t.DateTime == power.DateTime))
                {
                    var t = _dbContext.Powers.FirstOrDefault(t => t.DateTime == power.DateTime);
                    t.PowerData = power.PowerData;
                    //更新追踪的，必须改属性，不然调用update会报错
                    _dbContext.Powers.Update(t);
                }
                else
                {
                    //要判断是否已存在不然add时会报错
                    _dbContext.Powers.Add(power);
                }
            }
        }
        public void AddDayData(string meterId, List<Day> days)
        {
            if (days == null)
            {
                throw new ArgumentNullException(nameof(days));
            }


            foreach (var day in days)
            {
                day.MeterId = meterId;
                if (_dbContext.Days.Any(t => t.DateTime == day.DateTime))
                {
                    var t = _dbContext.Days.FirstOrDefault(t => t.DateTime == day.DateTime);
                    t.DayData = day.DayData;
                    //更新追踪的，必须改属性，不然调用update会报错
                    _dbContext.Days.Update(t);
                }
                else
                {
                    //要判断是否已存在不然add时会报错
                    _dbContext.Days.Add(day);
                }
            }
        }

        public void AddNotificationData(string meterId, Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }
            var jsonstr = JsonConvert.SerializeObject(notification, Formatting.Indented);
            Console.WriteLine(jsonstr);
            //foreach (var notification in notifications)
            //{
            notification.MeterId = meterId;
            if (_dbContext.Notifications.Any(t => t.DateTime == notification.DateTime))
            {
                var t = _dbContext.Notifications.FirstOrDefault(t => t.DateTime == notification.DateTime);
                t.NotifyData = notification.NotifyData;
                //更新追踪的，必须改属性，不然调用update会报错
                Console.WriteLine("Update-Notifications");
                _dbContext.Notifications.Update(t);
            }
            else
            {
                Console.WriteLine("add-Notifications");
                //要判断是否已存在不然add时会报错
                _dbContext.Notifications.Add(notification);
            }
            //}
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