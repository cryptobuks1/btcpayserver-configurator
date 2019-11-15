using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using BTCPayServerDockerConfigurator.Models;
using Microsoft.AspNetCore.Mvc;

namespace BTCPayServerDockerConfigurator.Controllers
{
    public partial class ConfiguratorController
    {
        [HttpGet("additional")]
        public IActionResult AdditionalServices()
        {
            var model = GetConfiguratorSettings();

            return View(new UpdateSettings<AdditionalServices, AdditionalDataStub>()
            {
                Json = model.ToString(),
                Settings = model.AdditionalServices
            });
        }

        [HttpPost("additional")]
        public async Task<IActionResult> AdditionalServices(
            UpdateSettings<AdditionalServices, AdditionalDataStub> updateSettings)
        {
            var configuratorSettings = string.IsNullOrEmpty(updateSettings.Json)
                ? new ConfiguratorSettings()
                : JsonSerializer.Deserialize<ConfiguratorSettings>(updateSettings.Json);

            if (ModelState.IsValid)
            {
                if (updateSettings.Settings.BTCTransmuterSettings.Enabled)
                {
                    var error = await CheckHost(updateSettings.Settings.BTCTransmuterSettings.Host,
                        configuratorSettings);
                    if (!string.IsNullOrEmpty(error))
                    {
                        ModelState.AddModelError(
                            nameof(updateSettings.Settings) + "." +
                            nameof(updateSettings.Settings.BTCTransmuterSettings) + "." +
                            nameof(updateSettings.Settings.BTCTransmuterSettings.Host),
                            error);
                    }
                }

                if (updateSettings.Settings.LibrePatronSettings.Enabled)
                {
                    var error = await CheckHost(updateSettings.Settings.LibrePatronSettings.Host,
                        configuratorSettings);
                    if (!string.IsNullOrEmpty(error))
                    {
                        ModelState.AddModelError(
                            nameof(updateSettings.Settings) + "." +
                            nameof(updateSettings.Settings.LibrePatronSettings) + "." +
                            nameof(updateSettings.Settings.LibrePatronSettings.Host),
                            error);
                    }
                }

                if (updateSettings.Settings.WooCommerceSettings.Enabled)
                {
                    var error = await CheckHost(updateSettings.Settings.WooCommerceSettings.Host,
                        configuratorSettings);
                    if (!string.IsNullOrEmpty(error))
                    {
                        ModelState.AddModelError(
                            nameof(updateSettings.Settings) + "." +
                            nameof(updateSettings.Settings.WooCommerceSettings) + "." +
                            nameof(updateSettings.Settings.WooCommerceSettings.Host),
                            error);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return View(updateSettings);
            }

            configuratorSettings.AdditionalServices = updateSettings.Settings;
            SetConfiguratorSettings(configuratorSettings);
            return RedirectToAction(nameof(AdvancedSettings));
        }
    }
}