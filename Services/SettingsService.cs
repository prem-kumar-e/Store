using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.Data
{
    public class SettingsService
    {
        private readonly StoreDBContext _context;

        public SettingsService(StoreDBContext context)
        {
            _context = context;
        }

        public async Task<List<Setting>> GetSettings()
        {
            return await _context.Settings.ToListAsync();
        }
        public async Task<Setting> GetSetting(int id)
        {
            var setting = await _context.Settings.FindAsync(id);
            return setting;
        }
        public async Task<string> GetSettingValue(string Key)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key.Contains(Key));
            return setting.Value;
        }
        public async Task<Setting> GetSettingByKey(string Key)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key.Contains(Key));
            return setting;
        }
        public async Task<List<AdminSetting>> GetAdminSettings()
        {
            return await _context.AdminSettings.ToListAsync();
        }

        public async Task<AdminSetting> GetAdminSetting(int id)
        {
            var adminSetting = await _context.AdminSettings.FindAsync(id);
            return adminSetting;
        }

        public async Task AddAdminSettingAsync(AdminSetting setting)
        {
            _context.AdminSettings.Add(setting);
            await _context.SaveChangesAsync();
        }

        public async Task AddSettingAsync(Setting setting)
        {
            _context.Settings.Add(setting);
            await _context.SaveChangesAsync();
        }
    }
}
