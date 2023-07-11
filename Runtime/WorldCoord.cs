

namespace Denizens
{
    /// <summary>
    /// A global map location within the world system.
    /// This is used for coarse map locations and is not meant to be
    /// for preciely locating a denizen in 3D space.
    /// </summary>
    public readonly struct WorldCoord
    {
        public readonly byte Region;
        public readonly byte Town;

        public WorldCoord(byte regionIndex, byte townshipIndex)
        {
            Region = regionIndex;
            Town = townshipIndex;
        }
    }

    
}
