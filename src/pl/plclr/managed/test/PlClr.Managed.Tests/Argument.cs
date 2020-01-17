namespace PlClr.Managed.Tests
{
    public enum ArgumentMode : byte
    {
        In = (byte)'i',
        Out = (byte)'o',
        InOut = (byte)'b',
        Variadic = (byte)'v',
        Table = (byte)'t',
    }

    public class Argument
    {
        public Argument(TypeOid oid, string? name = null, ArgumentMode mode = ArgumentMode.In)
        {
            Oid = oid;
            Name = name;
            Mode = mode;
        }

        public TypeOid Oid { get; }
        public string? Name { get; }
        public ArgumentMode Mode { get; }
    }
}
