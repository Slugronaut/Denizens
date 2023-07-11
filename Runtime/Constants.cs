
namespace Denizens
{
    /// <summary>
    /// Stores constants used at both runtime and edit time.
    /// </summary>
    public static class Constants
    {
        //For a 32-bit state the breakdown is as follows:
        // BITS     |   PURPOSE    |  RANGE OF VALUES
        //--------------------------------------------
        // 1  Bit       In-Use           (2)
        // 10 Bits      Residence Id     (1024)
        // 8  Bits      Town Id          (256)
        // 7  Bits      Workplace Id     (128)
        // 4  Bits      Civil Faction    (16)
        // 1  Bit       Death State      (2)
        // 1  Bit       In Use           (2)
        // 4  Bits      Personality      (4)
        // 8  Bits      Schedule Table   (256)
        public static readonly ulong InUseStateMask         = 0x0000_0001; //                     1 (2)
        public static readonly ulong ResidenceIdMask        = 0x0000_07FE; //                 2_046 (1024)
        public static readonly ulong TownIdMask             = 0x0007_F800; //               522_240 (256)
        public static readonly ulong WorkplaceIdMask        = 0x03F8_0000; //            66_584_575 (128)
        public static readonly ulong CivilFactionMask       = 0x3C00_0000; //         1_006_632_960 (16)
        public static readonly ulong DeathStateMask         = 0x4000_0000; //         1_073_741_824 (2)
        public static readonly ulong PersonalityMask   = 0x0007_8000_0000; //        32,212,254,720 (16)
        public static readonly ulong ScheduleMask      = 0x07F8_0000_0000; //     8_761_733_283_840 (256)


        public static readonly int InUseStateShift      =  0;
        public static readonly int ResidenceIdShift     =  1;
        public static readonly int TownIdShift          = 11;
        public static readonly int WorkplaceIdShift     = 19;
        public static readonly int CivilFactionShift    = 26;
        public static readonly int DeathStateShift      = 30;
        public static readonly int PersonalityShift     = 31;
        public static readonly int ScheduleShift        = 35;
        public static readonly int LastShift            = 43; //currently shifts to the next unused bit in state

    }

}
