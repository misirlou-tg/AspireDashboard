namespace GetTemps;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<GetTemps>();

        var host = builder.Build();
        host.Run();
    }
}
