using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnscrypt_measurement.Model;
using dnscrypt_measurement.Tools;
using DNS.Client;
using DNS.Protocol;
using Helper;
using NDesk.Options;
using Sodium;

namespace dnscrypt_measurement
{
	internal class Program
	{

		private static void Main(string[] args)
		{
			MainAsync(args).Wait();
		}

		private static async Task MainAsync(IEnumerable<string> args)
		{
			var showHelp = false;
			var list = "dnscrypt-proxy.csv";
			var noLogs = false;
			var onlyDnssec = false;

			var p = new OptionSet
			{
				{
					"l|list=", "the path to the dnscrypt-proxy.csv file",
					v =>
					{
						if (v != null) list = v;
					}
				},
				{
					"n|nologs", "only test resolvers with NoLogs support enabled.",
					v =>
					{
						if (v != null) noLogs = true;
					}
				},
				{
					"d|dnssec", "only test resolvers with DNSSEC support enabled.",
					v =>
					{
						if (v != null) onlyDnssec = true;
					}
				},
				{
					"h|help", "show this message and exit",
					v => showHelp = v != null
				}
			};

			try
			{
				p.Parse(args);
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
				var measurements = await Measure(proxyList).ConfigureAwait(false);
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
							$"{measurement.Time} ms, {measurement.Name}, NoLogs: {measurement.NoLogs}, DNSSEC: {measurement.DnsSec},  Certificate Valid: {measurement.Certificate.Valid}");
					}
				}
			}
			else
			{
				Console.WriteLine("Missing dnscrypt-proxy.csv");
			}
			Console.ReadLine();
		}

		private static Certificate ExtractCertificate(byte[] data, byte[] providerKey)
		{
			var certificate = new Certificate();
			if (data.Length != 116) return null;
			certificate.MagicQuery = ArrayHelper.SubArray(data, 96, 8);
			var serial = ArrayHelper.SubArray(data, 104, 4);
			var tsBegin = ArrayHelper.SubArray(data, 108, 4);
			var tsEnd = ArrayHelper.SubArray(data, 112, 4);

			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(serial);
				Array.Reverse(tsBegin);
				Array.Reverse(tsEnd);
			}
			certificate.Serial = BitConverter.ToInt32(serial, 0);
			certificate.TsBegin = UnixTimeStampToDateTime(BitConverter.ToInt32(tsBegin, 0));
			certificate.TsEnd = UnixTimeStampToDateTime(BitConverter.ToInt32(tsEnd, 0));

			try
			{
				var m = PublicKeyAuth.Verify(data, providerKey);
				certificate.Valid = true;
				return certificate;
			}
			catch (Exception)
			{
			}
			return null;
		}

		private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dateTime;
		}

		private static async Task<List<Measurement>> Measure(List<DnsCryptProxyEntry> proxyList)
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

					string address;
					var port = 443;
					if (proxy.ResolverAddress.Contains(":"))
					{
						if (proxy.ResolverAddress.StartsWith("["))
						{
							//IPv6
							var id = proxy.ResolverAddress.LastIndexOf(':');
							address = proxy.ResolverAddress.Substring(0, id).Replace("[", "").Replace("]", "");
							port = Convert.ToInt32(proxy.ResolverAddress.Substring(id + 1));
						}
						else
						{
							//IPv4
							var t = proxy.ResolverAddress.Split(':');
							address = t[0];
							port = Convert.ToInt32(t[1]);
						}
					}
					else
					{
						address = proxy.ResolverAddress;
					}

					var providerKey = Utilities.HexToBinary(proxy.ProviderPublicKey);
					var request = new ClientRequest(address, port);
					request.Questions.Add(new Question(Domain.FromString(proxy.ProviderName), RecordType.TXT));
					request.RecursionDesired = true;
					var sw = Stopwatch.StartNew();
					var response = await request.Resolve().ConfigureAwait(false);
					sw.Stop();

					foreach (var answerRecord in response.AnswerRecords)
					{
						var certificates = new List<Certificate>();
						var tr = Encoding.ASCII.GetString(ArrayHelper.SubArray(answerRecord.Data, 0, 9));
						if (tr.Equals("|DNSC\0\u0001\0\0") || tr.Equals("|DNSC\0\u0002\0\0"))
						{
							var certificate = ExtractCertificate(ArrayHelper.SubArray(answerRecord.Data, 9), providerKey);
							if (certificate != null)
							{
								if (certificate.Valid)
								{
									certificates.Add(certificate);
								}
							}
						}
						if (certificates.Count > 0)
						{
							var newestCertificate = certificates.OrderByDescending(item => item.Serial).FirstOrDefault();
							if (newestCertificate != null)
							{
								measurement.Certificate = newestCertificate;
								measurement.Failed = false;
							}
							else
							{
								measurement.Failed = true;
							}
						}
						else
						{
							measurement.Failed = true;
						}
					}
					measurement.Time = sw.ElapsedMilliseconds;
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