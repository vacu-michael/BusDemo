using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL;

public sealed class AdminBusinessLogic(DemoDbContext _db, ILogger<AdminBusinessLogic> _logger)
{
    public async Task<bool> GetSetting(string name) =>
        (await _db.Settings.FirstOrDefaultAsync(s => s.Name == name))?.Value ?? false;

    public async Task ToggleSetting(string settingName) =>
        await UpdateSetting(settingName, !await GetSetting(settingName));

    public async Task UpdateSetting(string name, bool value)
    {
        var setting = await _db.Settings.FirstOrDefaultAsync(s => s.Name == name);
        if (setting is null) return;

        _logger.LogInformation("Updating setting {Setting} to {Value}", name, value);

        setting.Value = value;
        await _db.SaveChangesAsync();
    }
}