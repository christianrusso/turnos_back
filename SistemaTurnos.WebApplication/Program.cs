using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SistemaTurnos.WebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        //public static IWebHost BuildWebHost(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseUrls("http://*:5000")
        //        .UseStartup<Startup>()
        //        .Build();

        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 5000, lstOpt => lstOpt.UseHttps("orbitsa.xyz.pfx", "1682951"));
                })
                .UseStartup<Startup>()
            .Build();

        //private static X509Certificate2 GetCertificateFromStore()
        //{
        //    var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        //    try
        //    {
        //        store.Open(OpenFlags.ReadOnly);
        //        var certCollection = store.Certificates;
        //        var currentCerts = certCollection.Find(X509FindType.FindBySubjectDistinguishedName, "CN=orbitsa.xyz", false);
        //        return currentCerts.Count == 0 ? null : currentCerts[0];
        //    }
        //    finally
        //    {
        //        store.Close();
        //    }
        //}
    }
}
