namespace MinecraftStructureLib.Core
{
    public abstract class Structure
    {
	    public abstract void Load(string filename);
	    public abstract void Save(string filename);
	    public abstract Block this[int x, int y, int z] { get; set; }
    }
}
