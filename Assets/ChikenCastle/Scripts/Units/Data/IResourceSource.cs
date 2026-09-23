public interface IResourceSource
{
    bool HasResource { get; }
    int Gather(int amount);
}
