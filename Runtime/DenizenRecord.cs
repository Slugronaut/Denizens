
namespace Denizens
{
    /// <summary>
    /// Defines a unique denizen in the world.
    /// </summary>
    public readonly struct DenizenRecord
    {
        readonly public int Guid;                       //4 bytes
        readonly public WorldCoord HomeLand;            //4 bytes
        readonly public Genders Gender;                 //1 byte
        readonly public Ages Age;                       //1 byte
        readonly public Races Race;                     //1 byte
        readonly public Jobs Job;                       //1 byte


        public DenizenRecord(int uid, WorldCoord homeland, Genders gender, Ages age, Races race, Jobs job)
        {
            Guid = uid;
            HomeLand = homeland;
            Gender = gender;
            Age = age;
            Race = race;
            Job = job;
        }

        public override string ToString()
        {
            return $"Denizen Record: {Guid}  " +
                    $"{System.Enum.GetName(typeof(Ages), Age)} " +
                    $"{System.Enum.GetName(typeof(Genders), Gender)} " +
                    $"{System.Enum.GetName(typeof(Races), Race)} " +
                    $"{System.Enum.GetName(typeof(Jobs), Job)} ";
        }

    }

}
