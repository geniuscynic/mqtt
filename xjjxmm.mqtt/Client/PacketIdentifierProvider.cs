namespace xjjxmm.mqtt;

public class PacketIdentifierProvider
{
    private readonly object _syncRoot = new object();

    private ushort _value =0;

    public void Reset()
    {
        lock (_syncRoot)
        {
            _value = 0;
        }
    }

    public ushort Current => _value;
    public ushort Next()
    {
        lock (_syncRoot)
        {
            _value++;

            if (_value == 0)
            {
                _value = 1;
            }

            return _value;
        }
    }
}