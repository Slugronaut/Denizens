
namespace Denizens
{
    /// <summary>
    /// An exact time within the world simulation down to the second.
    /// </summary>
    public readonly struct WorldTime
    {
        public readonly float TimeOfDayInSeconds;
        public readonly uint Year;
        public readonly ushort DayOfYear;
        public readonly byte DayOfWeek;
        public readonly byte DayOfMonth;
    }
}
