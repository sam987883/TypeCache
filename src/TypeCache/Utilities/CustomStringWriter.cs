// Copyright (c) 2021 Samuel Abraham

using System.Text;

namespace TypeCache.Utilities;

public sealed class CustomStringWriter : StringWriter
{
	public override Encoding Encoding { get; }

	public CustomStringWriter(Encoding encoding)
		=> this.Encoding = encoding;

	internal CustomStringWriter(StringBuilder builder, Encoding encoding) : base(builder)
		=> this.Encoding = encoding;
}
