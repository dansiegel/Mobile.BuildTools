namespace Mobile.BuildTools.Configuration;

internal sealed class EmptyDisposable : IDisposable
{
    public static EmptyDisposable Instance { get; } = new EmptyDisposable();

    private EmptyDisposable()
    {
    }

    public void Dispose()
    {
    }
}
