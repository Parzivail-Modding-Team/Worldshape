namespace MinecraftStructureLib.Core
{
    public abstract class Structure
    {
        /// <summary>
        /// Loads the structure from the given file
        /// </summary>
        /// <param name="filename">The file to load from</param>
	    public abstract void Load(string filename);
        
        /// <summary>
        /// Saves the structure to the given file
        /// </summary>
        /// <param name="filename">The file to save to</param>
	    public abstract void Save(string filename);

        /// <summary>
        /// Returns the block at the given position relative to the origin of the file
        /// </summary>
        /// <param name="x">The X coordinate of the block</param>
        /// <param name="y">The Y coordinate of the block</param>
        /// <param name="z">The Z coordinate of the block</param>
        /// <returns>The block at the specified coordinates, or null if one is not explicitly defined</returns>
	    public abstract Block this[int x, int y, int z] { get; set; }
    }
}
