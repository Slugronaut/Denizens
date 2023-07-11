namespace Denizens
{

    /// <summary>
    /// We can define up to 7 factions plus 'None' here due to the 3-bits used to define factions.
    /// </summary>
    public enum Factions : byte
    {
        None        = 0,
        Commoner    = 1 << 0,
        Nobility    = 1 << 1,
        Scholar     = 1 << 2,
        Underworld  = 1 << 3,
        Law         = 1 << 4,
        Merchant    = 1 << 5,
        Clergy      = 1 << 6,
    }
}
