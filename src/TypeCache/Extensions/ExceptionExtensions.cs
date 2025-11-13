namespace TypeCache.Extensions;

public static class ExceptionExtensions
{
	extension(Exception @this)
	{
		public Exception InnerMostException
		{
			get
			{
				var exception = @this;
				while (exception.InnerException is not null)
					exception = exception.InnerException;

				return exception;
			}
		}
	}
}
