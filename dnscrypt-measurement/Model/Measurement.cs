namespace dnscrypt_measurement.Model
{
	public class Measurement
	{
		public string Name { get; set; }
		public long Time { get; set; }
		public bool DnsSec { get; set; }
		public bool NoLogs { get; set; }
		public bool Failed { get; set; }
		public Certificate Certificate { get; set; }
	}
}
