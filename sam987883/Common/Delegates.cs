namespace sam987883.Common
{
	public delegate void ActionRef<T>(ref T item);
	public delegate void ActionRef<T, I>(ref T item, I index);
}
