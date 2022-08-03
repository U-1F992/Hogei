namespace HogeiJunkyard;
public record Operation
{
    KeySpecifier[] _Keys;
    public IReadOnlyCollection<KeySpecifier> Keys
    {
        get
        {
            return _Keys;
        }
    }
    public readonly TimeSpan Wait;
    public Operation(ICollection<KeySpecifier> keys, TimeSpan wait)
    {
        _Keys = keys.ToArray();
        Wait = wait;
    }
}
