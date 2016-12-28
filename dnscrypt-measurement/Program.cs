using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using dnscrypt_measurement.Model;
using dnscrypt_measurement.Tools;
using DNS.Client;
using DNS.Protocol;
using NDesk.Options;

namespace dnscrypt_measurement
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var showHelp = false;
			var list = "dnscrypt-proxy.csv";
			var noLogs = false;
			var onlyDnssec = false;

			var p = new OptionSet
			{
				{
					"l|list=", "the path to the dnscrypt-proxy.csv file",
					v => { if (v != null) list = v; }
				},
				{
					"n|nologs", "only test resolvers with NoLogs support enabled.",
					v => { if (v != null) noLogs = true; }
				},
				{
					"d|dnssec", "only test resolvers with DNSSEC support enabled.",
					v => { if (v != null) onlyDnssec = true; }
				},
				{
					"h|help", "show this message and exit",
					v => showHelp = v != null
				}
			};

			List<string> extra;
			try
			{
				extra = p.Parse(args);
			}
			catch (OptionException e)
			{
				Console.Write("dnscrypt-measurement: ");
				Console.WriteLine(e.Message);
				Console.WriteLine("Try `dnscrypt-measurement --help' for more information.");
				return;
			}

			if (showHelp)
			{
				ShowHelp(p);
				return;
			}

			if (File.Exists(list))
			{
				var proxyList = ProxyListManager.ReadProxyList(list, true, onlyDnssec, noLogs);
				var measurements = Measure(proxyList);
				measurements.Sort((a, b) => a.Time.CompareTo(b.Time));
				Console.WriteLine("=====================================");
				Console.WriteLine($"{measurements.Count} Resolvers (fastest first)");
				Console.WriteLine($"Only DNSSEC: {onlyDnssec}");
				Console.WriteLine($"Only NoLogs: {noLogs}");
				Console.WriteLine("Only IPv4: true");
				Console.WriteLine("=====================================");
				foreach (var measurement in measurements)
				{
					if (!measurement.Failed)
					{
						Console.WriteLine(
							$"{measurement.Time} ms, {measurement.Name}, NoLogs: {measurement.NoLogs}, DNSSEC: {measurement.DnsSec}");
					}
				}
			}
			else
			{
				Console.WriteLine("Missing dnscrypt-proxy.csv");
			}
		}

		private static List<Measurement> Measure(List<DnsCryptProxyEntry> proxyList)
		{
			var measurements = new List<Measurement>();
			foreach (var proxy in proxyList)
			{
				var measurement = new Measurement
				{
					Name = proxy.FullName,
					DnsSec = proxy.DnssecValidation,
					NoLogs = proxy.NoLogs
				};
				try
				{
					var request = new ClientRequest(proxy.ResolverAddress, 443);
					request.Questions.Add(new Question(Domain.FromString(proxy.ProviderName), RecordType.TXT));
					request.RecursionDesired = true;
					var sw = Stopwatch.StartNew();
					var response = request.Resolve();
					sw.Stop();
					measurement.Time = sw.ElapsedMilliseconds;
					var answers = response.AnswerRecords;
					measurement.Failed = answers.Count <= 0;
				}
				catch (Exception)
				{
					measurement.Failed = true;
				}
				measurements.Add(measurement);
			}
			return measurements;
		}

		private static void ShowHelp(OptionSet p)
		{
			Console.WriteLine("Usage: dnscrypt-measurement [OPTIONS]+");
			Console.WriteLine();
			Console.WriteLine("Options:");
			p.WriteOptionDescriptions(Console.Out);
		}
	}
}
