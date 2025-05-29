using System;
using System.Text;
using VeadoTube.BleatCan;


public class Test : IInstancesReceiver, IConnectionReceiver
{
    private IConnectionReceiver _connectionReceiverImplementation;

    static void Main(string[] args)
    {
        var test = new Test();
        Console.WriteLine("< active instances (will connect to last one, otherwise will quit):");
        Instance curr = default;
        foreach (var i in Instances.Enumerate())
        {
            curr = i;
            Console.WriteLine($"< {i.id} ({i.server})");
        }
        if (!curr.id.isValid) return;
        using var instances = new Instances(test);
        using var connection = new Connection(curr.server, "Test", test);
        bool stop = false;
        Console.CancelKeyPress += (o, e) => stop = true;
        while (!stop)
        {
            var s = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(s)) break;
            int i = s.IndexOf(':');
            if (i < 0) continue;
            if (connection.Send(s.Substring(0, i).TrimEnd(), Encoding.UTF8.GetBytes(s.Substring(i + 1).TrimStart())))
            {
                Console.WriteLine($"< sent");
            }
            else
            {
                Console.WriteLine($"< failed to send");
            }
        }
    }

    public void OnStart(Instance instance)
    {
        Console.WriteLine($"< started instance: {instance.id} ({instance.server})");
    }
    public void OnChange(Instance instance)
    {
        Console.WriteLine($"< changed instance: {instance.id} ({instance.server})");
    }
    public void OnEnd(InstanceID id)
    {
        Console.WriteLine($"< ended instance: {id}");
    }

    public void OnConnect(Connection connection, bool active)
    {
        Console.WriteLine($"connected: {active}");
    }

    public void OnReceive(Connection connection, string channel, byte[] data)
    {
        _connectionReceiverImplementation.OnReceive(connection, channel, data);
    }

    public void OnError(Connection connection, ConnectionError error)
    {
        Console.WriteLine($"connection error: {error}");
    }
    public void OnReceive(Connection connection, string channel, ReadOnlySpan<byte> data)
    {
        Console.WriteLine($"received: \"{channel}\"");
        try
        {
            Console.WriteLine(Encoding.UTF8.GetString(data));
        }
        catch
        {
            Console.WriteLine($"failed to parse");
        }
    }
}