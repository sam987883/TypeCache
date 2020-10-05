﻿// Copyright (c) 2020 Samuel Abraham

using System.IO;
using System.Text;

namespace Sam987883.Common
{
	public class CustomStringWriter : StringWriter
	{
		public override Encoding Encoding { get; }

		public CustomStringWriter(Encoding encoding)
			=> this.Encoding = encoding;

		internal CustomStringWriter(StringBuilder builder, Encoding encoding) : base(builder)
			=> this.Encoding = encoding;
	}
}