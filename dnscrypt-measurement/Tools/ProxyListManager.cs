using System.Collections.Generic;
using dnscrypt_measurement.Model;
using Microsoft.VisualBasic.FileIO;

namespace dnscrypt_measurement.Tools
{
	public static class ProxyListManager
	{
		internal static string ClearString(string s)
		{
			return s.Replace("\"", "").Trim();
		}

		public static List<DnsCryptProxyEntry> ReadProxyList(string proxyListFile, bool filterIpv6 = true, bool onlyDnsSec = true, bool onlyNoLogs = true)
		{
			var dnsCryptProxyList = new List<DnsCryptProxyEntry>();
			using (var parser = new TextFieldParser(proxyListFile) { HasFieldsEnclosedInQuotes = true })
			{
				parser.SetDelimiters(",");
				while (!parser.EndOfData)
				{
					var s = parser.ReadFields();
					var tmp = new DnsCryptProxyEntry
					{
						Name = ClearString(s[0]),
						FullName = ClearString(s[1]),
						Description = ClearString(s[2]),
						Location = ClearString(s[3]),
						Coordinates = ClearString(s[4]),
						Url = ClearString(s[5]),
						Version = s[6],
						DnssecValidation = (s[7].Equals("yes")),
						NoLogs = (s[8].Equals("yes")),
						Namecoin = (s[9].Equals("yes")),
						ResolverAddress = ClearString(s[10]),
						ProviderName = ClearString(s[11]),
						ProviderPublicKey = ClearString(s[12]),
						ProviderPublicKeyTextRecord = ClearString(s[13]),
						LocalPort = 53 //set the default port 
					};
					if (!tmp.Description.Equals("Description"))
					{
						if (filterIpv6)
						{
							if (!tmp.ResolverAddress.StartsWith("["))
							{
								var add = true;
								add = !onlyDnsSec || tmp.DnssecValidation;

								if (add)
								{
									add = !onlyNoLogs || tmp.NoLogs;
								}

								if (add)
								{
									dnsCryptProxyList.Add(tmp);
								}
						
							}
						}
						else
						{
							var add = true;
							add = !onlyDnsSec || tmp.DnssecValidation;

							if (add)
							{
								add = !onlyNoLogs || tmp.NoLogs;
							}

							if (add)
							{
								dnsCryptProxyList.Add(tmp);
							}
						}
					}
				}
			}

			return dnsCryptProxyList;
		}
	}
}