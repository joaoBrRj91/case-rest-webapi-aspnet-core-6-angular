using System;
namespace DevIO.Api.Configuration
{
	public class AppSettings
	{
		public string? Secret { get; set; }

        public short ExpiracaoHoras { get; set; }

        public string? Emissor { get; set; }

        public string? ValidoEm { get; set; }

    }
}

