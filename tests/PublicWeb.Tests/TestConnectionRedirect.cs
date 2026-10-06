using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace PublicWeb.Tests;

// Test-process only: the published legacy Context hardcodes its connection and repositories
// construct it directly. Redirect EF connections before opening without changing shared code.
internal sealed class TestConnectionRedirect : IObserver<DiagnosticListener>,
    IObserver<KeyValuePair<string, object?>>, IDisposable
{
    private readonly string connection;
    private readonly List<IDisposable> subscriptions = new();
    private readonly IDisposable listeners;

    public TestConnectionRedirect(string connection)
    {
        this.connection = connection;
        listeners = DiagnosticListener.AllListeners.Subscribe(this);
    }

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == DbLoggerCategory.Name)
            subscriptions.Add(listener.Subscribe(this, name =>
                name == RelationalEventId.ConnectionOpening.Name));
    }

    public void OnNext(KeyValuePair<string, object?> value)
    {
        if (value.Value is ConnectionEventData data && data.Context is DataAccessLayer.Concrete.Context)
            data.Connection.ConnectionString = connection;
    }

    public void OnCompleted() { }
    public void OnError(Exception error) => throw error;
    public void Dispose()
    {
        listeners.Dispose();
        foreach (var subscription in subscriptions) subscription.Dispose();
    }
}
