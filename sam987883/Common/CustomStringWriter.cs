// Copyright (c) 2020 Samuel Abraham

using System.IO;
using System.Text;

namespace sam987883.Common
{
	internal class CustomStringWriter : StringWriter
	{
		public override Encoding Encoding { get; }

		public CustomStringWriter(Encoding encoding) => this.Encoding = encoding;

		public CustomStringWriter(StringBuilder builder, Encoding encoding) : base(builder) => this.Encoding = encoding;
	}
}