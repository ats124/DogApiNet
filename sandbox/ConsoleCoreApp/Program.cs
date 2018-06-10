using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Utf8Json;
using DogApiNet;
using DogApiNet.JsonFormatters;
using System.Threading.Tasks;

namespace ConsoleCoreApp
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("DATADOG_API_KEY");
            var appKey = Environment.GetEnvironmentVariable("DATADOG_APP_KEY");

            using (var client = new DogApiClient(apiKey, appKey))
            {
                var users = await client.User.GetAllAsync();
                var user = await client.User.GetAsync(users[0].Handle);

                var newUser = await client.User.CreateAsync("test@example.com", "test", "ro");
                newUser.Name = "update name";
                newUser.EMail = "update@example.com";
                newUser.AccessRole = "adm";
                newUser.Disabled = false;

                var update = await client.User.UpdateAsync(newUser);

                await client.User.DeleteAsync(update.Handle);
            }
        }

        private static DateTimeOffset Normalize(this DateTimeOffset @this)
        {
            @this = @this.UtcDateTime;
            return new DateTimeOffset(@this.Year, @this.Month, @this.Day, @this.Hour, @this.Minute, @this.Second, TimeSpan.Zero);
        }
    }
}