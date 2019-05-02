namespace MinecraftStructureLib.Core
{
    public interface IStructureLoader
    {
        bool CanLoad(string filename);

        Structure Load(string filename);
    }
}