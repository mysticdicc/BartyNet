using BartyLib.Classes.Images;
using BartyLib.Classes.Posts;

namespace BartyNet.Client.Services
{
    public class SharedServices()
    {
        public static void Register(IServiceCollection services, Uri baseAddress)
        {
            services.AddTransient<ImageAPI>();
            services.AddHttpClient<ImageAPI>(client =>
            {
                client.BaseAddress = baseAddress;
            });
            services.AddTransient<WebsitePostAPI>();
            services.AddHttpClient<WebsitePostAPI>(client =>
            {
                client.BaseAddress = baseAddress;
            });
        }
    }
}
