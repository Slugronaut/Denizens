using ProceduralPopulationDatabase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Denizens
{
    /// <summary>
    /// An extension of a <see cref="Database"/> specific to this a fantasy game setting.
    /// </summary>
    public class DenizenDatabase : Database
    {
        //Cache for a thread-safe collection of ids and tasks for finding them
        static readonly ConcurrentBag<int> TempIdsSafe = new();
        static readonly List<Task> Tasks = new();


        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="totalPopulation"></param>
        /// <param name="kingdomData"></param>
        public DenizenDatabase(int seed, PopulationTree totalPopulation, KingdomAsset kingdomData) : base(seed, totalPopulation)
        {
            SlicePopulationWithKingdomData(totalPopulation, kingdomData);
        }
        
        /// <summary>
        /// Remaps a uid from the Database to a DenizenRecord with corresponding values.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public DenizenRecord GenerateDenizen(int uid)
        {
            var remap = Tree.Remap(uid);
            return new DenizenRecord(
                uid,
                new WorldCoord((byte)remap[(int)TreeDepths.Regions-1], (byte)GetDenizenTownId(uid)),
                (Genders)remap[(int)TreeDepths.Genders-1],
                (Ages)remap[(int)TreeDepths.Ages-1],
                (Races)remap[(int)TreeDepths.Races-1],
                (Jobs)remap[(int)TreeDepths.Jobs-1]
            );
        }
        #endregion


        #region Database Query
        /// <summary>
        /// Searches the global population for denizen ids that have been assgined to the given town id.
        /// </summary>
        /// <param name="town"></param>
        /// <param name="ids"></param>
        public void FindAllIdsForTown(byte town, ref List<int> ids)
        {
            ids.Clear();
            var states = States.AsSpan();
            for (int i = 0; i < states.Length; i++)
            {
                if ((states[i] & Constants.TownIdMask) >> Constants.TownIdShift == town)
                    ids.Add(i);
            }
        }

        /// <summary>
        /// Searches the global population for denizen ids that have been assgined to the given town id.
        /// This version is multi-threaded using tasks and can see an average of up to 33% increased
        /// performance on some machines.
        /// </summary>
        /// <param name="town"></param>
        /// <param name="ids"></param>
        public void FindAllIdsForTownMT(byte town, ref List<int> ids)
        {
            ids.Clear();
            TempIdsSafe.Clear();
            var states = States.AsSpan();

            int chunkSize = states.Length / Environment.ProcessorCount;
            for (int i = 0; i < states.Length; i += chunkSize)
            {
                int startIndex = i;
                int endIndex = Math.Min(startIndex + chunkSize, states.Length);
                Task task = Task.Run(() =>
                {
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        var state = States[j];
                        if ((state & Constants.TownIdMask) >> Constants.TownIdShift == town)
                            TempIdsSafe.Add(j);
                    }
                });

                Tasks.Add(task);
            }

            Task.WaitAll(Tasks.ToArray());
            Tasks.Clear();
            ids.AddRange(TempIdsSafe);
        }

        /// <summary>
        /// Searches the given population for denizen ids that have been assgined to the given town id.
        /// </summary>
        /// <param name="town"></param>
        /// <param name="ids"></param>
        public void FindSampleIdsForTown(PopulationSample pop, byte town, ref List<int> ids)
        {
            ids.Clear();
            var states = States.AsSpan();
            var ranges = pop.Ranges;
            for (int rangeIndex = 0; rangeIndex < ranges.Count; rangeIndex++)
            {
                var range = ranges[rangeIndex];
                for (int id = range.StartIndex; id <= range.EndIndex; id++)
                {
                    var state = states[id];
                    if ((state & Constants.TownIdMask) >> Constants.TownIdShift == town)
                        ids.Add(id);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pop"></param>
        /// <param name="town"></param>
        /// <param name="ids"></param>
        [Obsolete("This version performs SIGNIFICANTLY slower than the single-threaded version. It is only for testing purposes.")]
        public void FindSampleIdsForTownMT(PopulationSample pop, byte town, ref List<int> ids)
        {
            ids.Clear();
            TempIdsSafe.Clear();
            var states = States.AsSpan();
            var ranges = pop.Ranges;

            foreach (var range in ranges)
            {
                var task = Task.Run(() =>
                {
                    for (int id = range.StartIndex; id <= range.EndIndex; id++)
                    {
                        var state = States[id];
                        if ((state & Constants.TownIdMask) >> Constants.TownIdShift == town)
                            TempIdsSafe.Add(id);
                    }
                });

                Tasks.Add(task);
            }

            Task.WaitAll(Tasks.ToArray());
            Tasks.Clear();
            ids.AddRange(TempIdsSafe);
        }

        /// <summary>
        /// Searches the global population for all denizen ids that have been assigned to the given town and residence ids.
        /// </summary>
        public void FindAllIdsForResidence(byte town, uint residence, ref List<int> ids)
    {
        ids.Clear();
        var states = States.AsSpan();
        for (int id = 0; id < states.Length; id++)
        {
            var state = states[id];
            if ((state & Constants.ResidenceIdMask) >> Constants.ResidenceIdShift == residence)
            {
                    if ((state & Constants.TownIdMask) >> Constants.TownIdShift == town)
                        ids.Add(id);
                }
            }
        }

        /// <summary>
        /// Searches the global population for denizen ids that have been assgined to the given town and residence ids.
        /// This version is multi-threaded using tasks and can see an average of up to 33% increased
        /// performance on some machines.
        /// </summary>
        /// <param name="town"></param>
        /// <param name="ids"></param>
        public void FindAllIdsForResidenceMT(byte town, uint residence, ref List<int> ids)
        {
            ids.Clear();
            TempIdsSafe.Clear();
            var states = States.AsSpan();

            int chunkSize = states.Length / Environment.ProcessorCount;
            for (int i = 0; i < states.Length; i += chunkSize)
            {
                int startIndex = i;
                int endIndex = Math.Min(startIndex + chunkSize, states.Length);
                Task task = Task.Run(() =>
                {
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        var state = States[j];
                        if ((state & Constants.ResidenceIdMask) >> Constants.ResidenceIdShift == residence)
                        {
                            if ((state & Constants.TownIdMask) >> Constants.TownIdShift == town)
                                TempIdsSafe.Add(j);
                        }
                    }
                });

                Tasks.Add(task);
            }

            Task.WaitAll(Tasks.ToArray());
            Tasks.Clear();
            ids.AddRange(TempIdsSafe);
        }

        /// <summary>
        /// Searches this population for all denizen ids that have been assigned to the given town and residence ids.
        /// </summary>
        public void FindSampleIdsForResidence(PopulationSample pop, byte town, uint residence, ref List<int> ids)
        {
            ids.Clear();
            var states = States.AsSpan();
            var ranges = pop.Ranges;
            for (int rangeIndex = 0; rangeIndex < ranges.Count; rangeIndex++)
            {
                var range = ranges[rangeIndex];
                for (int id = range.StartIndex; id <= range.EndIndex; id++)
                {
                    var state = states[id];
                    if ((state & Constants.ResidenceIdMask) >> Constants.ResidenceIdShift == residence)
                    {
                        if ((state & Constants.TownIdMask) >> Constants.TownIdShift == town)
                            ids.Add(id);
                    }
                }
            }
        }

        /// <summary>
        /// Searches the global population for all denizen ids that have been assigned to the given town and residence ids.
        /// </summary>
        public void FindAllIdsForWorkplace(byte town, byte workplace, ref List<int> ids)
        {
            ids.Clear();
            var states = States.AsSpan();
            for (int id = 0; id < states.Length; id++)
            {
                var state = states[id];
                if ((state & Constants.WorkplaceIdMask) >> Constants.WorkplaceIdShift == workplace)
                {
                    if ((state & Constants.TownIdMask) >> Constants.TownIdShift == town)
                        ids.Add(id);
                }
            }
        }

        /// <summary>
        /// Searches the global population for denizen ids that have been assgined to the given town and workplace ids.
        /// This version is multi-threaded using tasks and can see an average of up to 33% increased
        /// performance on some machines.
        /// </summary>
        /// <param name="town"></param>
        /// <param name="ids"></param>
        public void FindAllIdsForWorkplaceMT(byte town, byte workplace, ref List<int> ids)
        {
            ids.Clear();
            TempIdsSafe.Clear();
            var states = States.AsSpan();

            int chunkSize = states.Length / Environment.ProcessorCount;
            for (int i = 0; i < states.Length; i += chunkSize)
            {
                int startIndex = i;
                int endIndex = Math.Min(startIndex + chunkSize, states.Length);
                Task task = Task.Run(() =>
                {
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        var state = States[j];
                        if ((state & Constants.WorkplaceIdMask) >> Constants.WorkplaceIdShift == workplace)
                        {
                            if ((state & Constants.TownIdMask) >> Constants.TownIdShift == town)
                                TempIdsSafe.Add(j);
                        }
                    }
                });

                Tasks.Add(task);
            }

            Task.WaitAll(Tasks.ToArray());
            Tasks.Clear();
            ids.AddRange(TempIdsSafe);
        }

        /// <summary>
        /// Searches this population for all denizen ids that have been assigned to the given town and residence ids.
        /// </summary>
        public void FindSampleIdsForWorkplace(PopulationSample pop, byte town, byte workplace, ref List<int> ids)
        {
            ids.Clear();
            var states = States.AsSpan();
            var ranges = pop.Ranges;
            for (int rangeIndex = 0; rangeIndex < ranges.Count; rangeIndex++)
            {
                var range = ranges[rangeIndex];
                for (int id = range.StartIndex; id <= range.EndIndex; id++)
                {
                    var state = states[id];
                    if ((state & Constants.WorkplaceIdMask) >> Constants.WorkplaceIdShift == workplace)
                    {
                        if ((state & Constants.TownIdMask) >> Constants.TownIdShift == town)
                            ids.Add(id);
                    }
                }
            }
        }

        #endregion


        #region Denizen State Query
        /// <summary>
        /// 
        /// </summary>
        public bool IsDenizenDead(int uid) => (States[uid] & Constants.DeathStateMask) > 0;

        /// <summary>
        /// 
        /// </summary>
        public uint GetDenizenResidenceId(int uid) => (uint)(States[uid] & Constants.ResidenceIdMask) >> Constants.ResidenceIdShift;

        /// <summary>
        /// 
        /// </summary>
        public byte GetDenizenTownId(int uid) => (byte)((uint)(States[uid] & Constants.TownIdMask) >> Constants.TownIdShift);

        /// <summary>
        /// 
        /// </summary>
        public uint GetDenizenWorkplaceId(int uid) => (uint)(States[uid] & Constants.WorkplaceIdMask) >> Constants.WorkplaceIdShift;

        /// <summary>
        /// 
        /// </summary>
        public Factions GetDenizenFaction(int uid) => (Factions)((States[uid] & Constants.CivilFactionMask) >> Constants.CivilFactionShift);

        /// <summary>
        /// 
        /// </summary>
        public Personalities GetDenizenPersonality(int uid) => (Personalities)((States[uid] & Constants.PersonalityMask) >> Constants.PersonalityShift);

        /// <summary>
        /// 
        /// </summary>
        public byte GetDenizenScheduleTable(int uid) => (byte)((States[uid] & Constants.ScheduleMask) >> Constants.ScheduleShift);
        
        #endregion


        #region Denizen State Assign
        /// <summary>
        /// 
        /// </summary>
        public void SetDenizenDead(int uid, bool value)
        {
            if (value) States[uid] |= Constants.DeathStateMask;
            else States[uid] &= ~Constants.DeathStateMask;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDenizenResidenceId(int uid, uint value)
        {
            Assert.IsTrue(value < 1024, "Invalid ResidenceId. Cannot be greater than 1024.");
            States[uid] &= ~Constants.ResidenceIdMask;
            States[uid] |= ((ulong)value << Constants.ResidenceIdShift);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDenizenTownId(int uid, uint value)
        {
            Assert.IsTrue(value < 256, "Invalid TownId. Cannot be greater than 63.");
            States[uid] &= ~Constants.TownIdMask;
            States[uid] |= ((ulong)value << Constants.TownIdShift);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDenizenWorkplaceId(int uid, uint value)
        {
            Assert.IsTrue(value < 128, "Invalid WorkplaceId. Cannot be greater than 127.");
            States[uid] &= ~Constants.WorkplaceIdMask;
            States[uid] |= ((ulong)value << Constants.WorkplaceIdShift);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDenizenFaction(int uid, Factions value)
        {
            Assert.IsTrue((uint)value < 16, "Invalid CivilFaction. Cannot be greater than 15.");
            States[uid] &= ~Constants.CivilFactionMask;
            States[uid] |= ((ulong)value << Constants.CivilFactionShift);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDenizenPersonality(int uid, Personalities value)
        {
            Assert.IsTrue((uint)value < 16, "Invalid PersonalityTrait. Cannot be greater than 15.");
            States[uid] &= ~Constants.PersonalityMask;
            States[uid] |= ((ulong)value << Constants.PersonalityShift);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDenizenScheduleTable(int uid, byte value)
        {
            Assert.IsTrue((uint)value < 256, "Invalid ScheduleId. Cannot be greater than 255.");
            States[uid] &= ~Constants.ScheduleMask;
            States[uid] |= ((ulong)value << Constants.ScheduleShift);
        }

        #endregion


        #region Static Methods
        /// Helper method for turning a kingdom asset and its census data into a population tree.
        /// </summary>
        /// <param name="totalPopulation"></param>
        /// <param name="kingdomData"></param>
        public static void SlicePopulationWithKingdomData(PopulationTree totalPopulation, KingdomAsset kingdomData)
        {
            //the first slice will be normal, after that each region has their own percentages so we need to get fancy
            totalPopulation.Slice((int)TreeDepths.Regions, kingdomData.Regions.Select(region => region.PercentageOfTotalPopulation).ToArray());

            //first, build the list of censuses for all regions
            var censusesPerRegionAtCurrentDepth = new PopulationCensusHierarchy[kingdomData.Regions.Count];
            for (int regionIndex = 0; regionIndex < kingdomData.Regions.Count; regionIndex++)
                censusesPerRegionAtCurrentDepth[regionIndex] = kingdomData.Regions[regionIndex].Census.AsHierarchy;

            //now we need to sample each census at the desired level (adjusting for indexing difference between pop slices and
            //census slices) and then 'tile' that data for the entire width of the depth. Then we'll have exactly the list needed
            //to be sliced for the current depth in the population
            int startDepth = (int)TreeDepths.Genders;
            int endDepth = (int)TreeDepths.Jobs;
            for (int depth = startDepth; depth <= endDepth; depth++)
            {
                var r = censusesPerRegionAtCurrentDepth.SelectMany(x => x.GetTiledAtDepth(depth - startDepth)).ToArray();
                totalPopulation.JaggedSlice(depth, r);
            }
        }
        #endregion
    }



}
