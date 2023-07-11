
namespace Denizens
{
    public enum Personalities : byte
    {
        Neutral,

        Brave,
        Charismatic,
        Friendly,
        Loyal,
        Wise,
        Optimistic,
        Curious,

        Stoic,

        Aggressive,
        Cautious,
        Timid,
        Greedy,
        Reckless,
        Devious,
        Foolish
    }

    /*
    [Flags]
    public enum Personalities : byte
    {
        Devious     = 1 << 0,
        Cruel       = 1 << 1,
        Cowardly    = 1 << 2,
        Greedy      = 1 << 3,

        Honest      = 1 << 4,
        Merciful    = 1 << 5,
        Valiant     = 1 << 6,
        Generous    = 1 << 7,
    }
    */
}
