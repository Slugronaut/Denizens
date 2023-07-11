using NUnit.Framework;
using ProceduralPopulationDatabase;
using System.Collections.Generic;
using UnityEngine;

namespace Denizens.Tests.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public class TestDenizenStateQueries : ScriptableObject
    {
        public KingdomAsset K1;

        static PopulationTree PopTree;
        static DenizenDatabase Db;
        static PopulationSample Pop;

        const int MaxPopulation = 10_000;
        const int Seed = 69420;
        const int ResidenceId = 27;
        const int TownId = 2;
        const int WorkplaceId = 69;
        const Factions Faction = Factions.Scholar;
        const Personalities Personality = Personalities.Aggressive;
        const int ScheduleId = 42;


        #region Setup
        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        public void DbSetup()
        {
            PopulationTree.ResetCache();
            PopTree = new PopulationTree(K1.TotalPopulation);
            Db = new DenizenDatabase(Seed, PopTree, K1);
            Pop = PopTree.Query;
        }
        #endregion


        /// <summary>
        /// Confirms that we can at least create a population, db, and pop sample.
        /// </summary>
        [Test]
        public void BasicSetupIsValid()
        {
            Assert.IsNotNull(Db);
            Assert.AreEqual(MaxPopulation, K1.TotalPopulation);
            Assert.AreEqual(K1.TotalPopulation, Db.Tree.PopulationSize);
            Assert.AreEqual(K1.TotalPopulation, Pop.Count);
        }

        //TO TEST
        //
        // Residence
        // Town
        // Workplace
        // CivilFaction
        // Death
        // Personality
        // Schedule Id

        [Test]
        public void ReadWriteResidence()
        {
            foreach (var uid in Pop)
                Assert.AreEqual(0, Db.GetDenizenResidenceId(uid));

            foreach (var uid in Pop)
                Db.SetDenizenResidenceId(uid, ResidenceId);

            foreach (var uid in Pop)
                Assert.AreEqual(ResidenceId, Db.GetDenizenResidenceId(uid));
        }

        [Test]
        public void FindAllUidsForResidence()
        {
            var baseList = new List<int>();
            foreach (var uid in Pop)
            {
                Assert.AreEqual(0, Db.GetDenizenResidenceId(uid));
                baseList.Add(uid);
            }

            foreach (var uid in Pop)
                Db.SetDenizenResidenceId(uid, ResidenceId);

            var idList = new List<int>();
            Db.FindAllIdsForResidence(0, ResidenceId, ref idList);
            Assert.AreEqual(Pop.Count, idList.Count);
            foreach (var uid in idList)
            {
                Assert.IsTrue(baseList.Contains(uid));
                Assert.AreEqual(ResidenceId, Db.GetDenizenResidenceId(uid));
            }

            Db.FindAllIdsForWorkplace(1, ResidenceId, ref idList); //different town id this time
            Assert.AreEqual(0, idList.Count);
        }

        [Test]
        public void ReadWriteTown()
        {
            foreach (var uid in Pop)
                Assert.AreEqual(0, Db.GetDenizenTownId(uid));

            foreach (var uid in Pop)
                Db.SetDenizenTownId(uid, TownId);

            foreach (var uid in Pop)
                Assert.AreEqual(TownId, Db.GetDenizenTownId(uid));
        }

        [Test]
        public void FindAllUidsForTown()
        {
            var baseList = new List<int>();
            foreach (var uid in Pop)
            {
                Assert.AreEqual(0, Db.GetDenizenTownId(uid));
                baseList.Add(uid);
            }

            foreach (var uid in Pop)
                Db.SetDenizenTownId(uid, TownId);

            var idList = new List<int>();
            Db.FindAllIdsForTown(TownId, ref idList);
            Assert.AreEqual(Pop.Count, idList.Count);
            foreach (var uid in idList)
            {
                Assert.IsTrue(baseList.Contains(uid));
                Assert.AreEqual(TownId, Db.GetDenizenTownId(uid));
            }
        }

        [Test]
        public void ReadWriteWorkplace()
        {
            foreach (var uid in Pop)
                Assert.AreEqual(0, Db.GetDenizenWorkplaceId(uid));

            foreach (var uid in Pop)
                Db.SetDenizenWorkplaceId(uid, WorkplaceId);

            foreach (var uid in Pop)
                Assert.AreEqual(WorkplaceId, Db.GetDenizenWorkplaceId(uid));
        }

        [Test]
        public void FindAllUidsForWorkplace()
        {
            var baseList = new List<int>();
            foreach (var uid in Pop)
            {
                Assert.AreEqual(0, Db.GetDenizenWorkplaceId(uid));
                baseList.Add(uid);
            }

            foreach (var uid in Pop)
                Db.SetDenizenWorkplaceId(uid, WorkplaceId);

            var idList = new List<int>();
            Db.FindAllIdsForWorkplace(0, WorkplaceId, ref idList);
            Assert.AreEqual(Pop.Count, idList.Count);
            foreach (var uid in idList)
            {
                Assert.IsTrue(baseList.Contains(uid));
                Assert.AreEqual(WorkplaceId, Db.GetDenizenWorkplaceId(uid));
            }

            Db.FindAllIdsForWorkplace(1, WorkplaceId, ref idList); //different town id this time
            Assert.AreEqual(0, idList.Count);
        }

        [Test]
        public void ReadWriteCivilFaction()
        {
            foreach (var uid in Pop)
                Assert.AreEqual(Factions.None, Db.GetDenizenFaction(uid));

            foreach (var uid in Pop)
                Db.SetDenizenFaction(uid, Faction);

            foreach (var uid in Pop)
                Assert.AreEqual(Faction, Db.GetDenizenFaction(uid));
        }

        [Test]
        public void ReadWriteDeathState()
        {
            foreach (var uid in Pop)
                Assert.AreEqual(false, Db.IsDenizenDead(uid));

            foreach (var uid in Pop)
                Db.SetDenizenDead(uid, true);

            foreach (var uid in Pop)
                Assert.AreEqual(true, Db.IsDenizenDead(uid));
        }

        [Test]
        public void ReadWritePersonality()
        {
            foreach (var uid in Pop)
                Assert.AreEqual(Personalities.Neutral, Db.GetDenizenPersonality(uid));

            foreach (var uid in Pop)
                Db.SetDenizenPersonality(uid, Personality);

            foreach (var uid in Pop)
                Assert.AreEqual(Personality, Db.GetDenizenPersonality(uid));
        }

        [Test]
        public void ReadWriteScheduleId()
        {
            foreach (var uid in Pop)
                Assert.AreEqual(0, Db.GetDenizenScheduleTable(uid));

            foreach (var uid in Pop)
                Db.SetDenizenScheduleTable(uid, ScheduleId);

            foreach (var uid in Pop)
                Assert.AreEqual(ScheduleId, Db.GetDenizenScheduleTable(uid));
        }
    }
}
