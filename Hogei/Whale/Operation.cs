namespace Hogei;
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
    public TimeSpan Wait { init; get; }
    public Operation(ICollection<KeySpecifier> keys, TimeSpan wait)
    {
        _Keys = keys.ToArray();
        Wait = wait;
    }
}
