
namespace Denizens
{
    public enum TreeDepths
    {
        Root = 0,
        Regions,
        Genders,
        Ages,
        Races,
        Jobs,
    }

    public enum Genders : byte
    {
        Male,
        Female,
    }
    
    public enum Ages : byte
    {
        Infant,
        Child,
        Teen,
        YoungAdult,
        MiddleAged,
        Elderly,
    }

    public enum Races : byte
    {
        Human,
        Elf,
        DarkElf,
        Shagga,
        Mark,
        Ocarim,
    }

    public enum Jobs : byte
    {
        //specialty
        Adventurer,

        //civil jobs
        Laborer,
        Farmer,
        Carpenter,
        Smithy,
        Clothier,
        Armorer,
        Alchemist,
        Merchant,
        Librarian,
        Scholar,
        Magician,

        //enforcement
        Militia,
        Officer,

        //religious jobs
        Clergy,

        //military jobs
        Soldier,
        MilitaryOfficer,

        //noble jobs
        Noble,
        Royalty,

        //underworld
        Thief,
        Thug,
        Assassin,
        CriminalOverlord,
    }
}
