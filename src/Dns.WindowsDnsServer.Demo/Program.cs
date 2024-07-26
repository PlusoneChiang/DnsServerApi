using System;
using System.Linq;

namespace Dns.WindowsDnsServer.Demo
{
    using static System.Console;

    class Program
    {
        static void Main(string[] args)
        {

            // connect to DNS server
            Write($"Connecting to {Environment.GetEnvironmentVariable("DnsServerAddress")}...");
            //using (var server = WindowsDnsServer.Connect(
            //    Environment.GetEnvironmentVariable("DnsServerAddress"),
            //    Environment.GetEnvironmentVariable("DnsServerUsername"),
            //    Environment.GetEnvironmentVariable("DnsServerPassword")))
            //{
            //    WriteLine(" [Done]");

            //    DumpDnsServer(server);

            //}
            using var server = WindowsDnsServer.Connect("192.168.68.170", "ntlmdomain:DOMAIN", "administrator", "1qaz@WSX");
            // using var server = WindowsDnsServer.Connect("192.168.68.170");
            // server.CreateZone("appx.com", DnsZoneType.Primary, false, "plusnone");
            // server.CreateZone("appx-dev.com", DnsZoneType.Primary, false, "plusnone");
            // server.DeleteZone("appx.sample.com");
            // server.GetZone("appx.com").CreateARecord("www.appx.com", TimeSpan.FromDays(365), "192.168.68.170");
            // server.GetZone("appx.com").CreateCNAMERecord("sub.appx.com", TimeSpan.FromDays(365), "www.appx.com");
            // server.GetZone("appx-dev.com").CreateCNAMERecord("sub.appx-dev.com", TimeSpan.FromDays(365), "www.appx-dev.com");
            DumpDnsServer(server);
        }

        #region Console Helpers
        static void DumpDnsServer(WindowsDnsServer server)
        {
            WriteLine(server);

            WriteLine("Zones:");
            foreach (var zone in server.GetZones().Cast<WindowsDnsZone>())
            {
                DumpDnsZone(zone);
                WriteLine("----");
            }
        }

        static void DumpDnsZone(WindowsDnsZone zone)
        {
            WriteLine(zone);
            WriteLine($"          Data File: {zone.DataFile}");
            WriteLine($"      AD Integrated: {zone.DsIntegrated}");

            if (zone.Type != DnsZoneType.Forwarder)
            {
                var soa = zone.StartOfAuthority;
                WriteLine("Start Of Authority:");
                WriteLine($"      Primary Server: {soa.PrimaryServer}");
                WriteLine($"  Responsible Person: {soa.ResponsiblePerson}");
                WriteLine($"              Serial: {soa.Serial}");
                WriteLine($"    Refresh Interval: {soa.RefreshInterval}");
                WriteLine($"         Retry Delay: {soa.RetryDelay}");
                WriteLine($"        Expire Limit: {soa.ExpireLimit}");
                WriteLine($"         Minimum TTL: {soa.MinimumTimeToLive}");
                WriteLine();
            }

            WriteLine("Name Servers:");
            foreach (var nameServer in zone.NameServers)
            {
                WriteLine($"   {nameServer}");
            }

            WriteLine("Records:");
            foreach (var record in zone.GetRecords().Where(r => r.Type != DnsRecordTypes.SOA && r.Type != DnsRecordTypes.NS))
            {
                WriteLine(record);
            }
        }
        #endregion
    }
}
