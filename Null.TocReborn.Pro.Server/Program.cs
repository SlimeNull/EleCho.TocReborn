using Null.TocReborn.Pro.Server.Model;
using NullLib.TocReborn.Pro;
using System.Net;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Null.TocReborn.Pro.Server
{
    class Program
    {
        const string AppConfigFileName = "./config.yaml";
        static AppConfig? CurAppConfig { get; set; }
        static void Main(string[] args)
        {
            if (AppInit(args))
                MainLogic();
            else
                Environment.ExitCode = -1;
        }
        static void MainLogic()
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add($"ws://{CurAppConfig!.HttpListening?.Address}:{CurAppConfig!.HttpListening?.Port}/");
            TocProServer tocProServer = new TocProServer(
                new TocProListener(new IPEndPoint(IPAddress.Parse(CurAppConfig!.TcpListening.Address), CurAppConfig.TcpListening.Port)),

        }

        static bool AppInit(string[] args)
        {
            try
            {
                if (File.Exists(AppConfigFileName))
                {
                    IDeserializer deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                    using (StreamReader reader = new StreamReader(AppConfigFileName))
                    {
                        CurAppConfig = deserializer.Deserialize<AppConfig>(reader);
                    }
                }
                else
                {
                    ISerializer serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                    CurAppConfig = AppConfig.Default;
                    using (StreamWriter writer = new StreamWriter(AppConfigFileName))
                    {
                        serializer.Serialize(writer, CurAppConfig);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}