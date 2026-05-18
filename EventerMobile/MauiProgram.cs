using EventerMobile.Database;
using Microsoft.Extensions.Logging;

namespace EventerMobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // --- База даних (Singleton = один екземпляр) ---
        builder.Services.AddSingleton<DatabaseContext>(serviceProvider =>
        {
            var db = new DatabaseContext();
            db.InitAsync().GetAwaiter().GetResult();
            return db;
        });

        // --- Репозиторії ---
        builder.Services.AddSingleton<LocationRepository>();
        builder.Services.AddSingleton<OrganizerRepository>();
        builder.Services.AddSingleton<EventRepository>();
        builder.Services.AddSingleton<BookingRepository>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}