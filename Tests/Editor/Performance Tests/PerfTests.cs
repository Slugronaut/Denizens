using Denizens;
using NUnit.Framework;
using ProceduralPopulationDatabase;
using System.Collections.Generic;
using System.Linq;
using Unity.PerformanceTesting;
using UnityEngine;

namespace Denizens.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class PerfTests : ScriptableObject
    {
        #region Data
        [SerializeField]
        KingdomAsset Defs;

        DenizenDatabase Db;
        PopulationTree Tree;
        PopulationSample Sample;

        const int Seed = 69420;
        const uint TownPopulation = 1_000;
        const int RegionIndex = 1;
        const int TownId = 2;
        const int ResidenceId = 27;

        static List<int> Ids = new((int)TownPopulation);

        const int Warmup = 1;
        const int Measures = 3;
        const int Iterations = 1_00;
        #endregion


        #region Utility

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            Tree = new PopulationTree(Defs.TotalPopulation);
            Db = new DenizenDatabase(Seed, Tree, Defs);
            Db.ResetPopulationStates();
            Sample = Tree.Query.Region(RegionIndex);
            List<int> ids = new((int)TownPopulation);

            int townCount = 0;
            int resCount = 0;
            for(uint i = 0; i < TownPopulation; i++)
            {
                var id = Db.RandomId(Sample, InUseStates.NotInUse);
                Db.SetIdUse(id, true);
                ids.Add(id);
                Db.SetDenizenTownId(id, TownId);

                if (i % 2 == 0) Db.SetDenizenResidenceId(id, ResidenceId);
                else if (i % 3 == 0) Db.SetDenizenResidenceId(id, 3);
                else if (i % 5 == 0) Db.SetDenizenResidenceId(id, 4);
                else Db.SetDenizenResidenceId(id, 1);

                if (Db.GetDenizenTownId(id) == TownId) townCount++;
                if (Db.GetDenizenResidenceId(id) == ResidenceId) resCount++;
            }

            //make sure we have no dupelicates, we need absolute data integrity here
            bool hasDuplicates = ids.GroupBy(x => x).Any(g => g.Count() > 1);
            if (hasDuplicates)
                Debug.LogWarning("There are duplicate ids in the generated list!");

            foreach (var id in ids)
            {
                var town = Db.GetDenizenTownId(id);
                if (town != TownId)
                    Debug.LogWarning($"Uid {id} doesn't have a town id of {TownId}. Instead it is {town}.");

            }
            if (ids.Count != TownPopulation)
                Debug.LogWarning($"Generated Id count is {ids.Count} instead of {TownPopulation}");

            Db.FindAllIdsForTown(TownId, ref ids);
            if (ids.Count != TownPopulation)
                Debug.LogWarning($"Query Id count is {ids.Count} instead of {TownPopulation}");

            Debug.Log($"Total Pop Count: {Sample.Count}\n Population count assigned to Town ID {TownId}: {townCount}\n Population count assigned to Residence Id {ResidenceId}: {resCount}");
        }
        #endregion


        #region Creation
        //[Test, Performance]
        public void GenerateRandomIdsFromPop_OneThousand()
        {
            var tree = new PopulationTree(Defs.TotalPopulation);
            var db = new DenizenDatabase(Seed, tree, Defs);
            var pop = tree.Query.Region(1).Age(Ages.Elderly).Race(Races.Mark);
            const int total = 1_000;
            //Debug.Log($"Population total used for generation of {total} denizens was: {pop.Count}.");

            using (Measure.Scope())
            {
                for (int i = 0; i < total; i++)
                {
                    var id = db.RandomId(pop, InUseStates.NotInUse);
                    db.SetIdUse(id, true);
                    var denizen = db.GenerateDenizen(id);
                }
            }
        }

        [Test, Performance]
        public void GenerateSetOfRandomIdsFromPop_OneThousand()
        {
            var tree = new PopulationTree(Defs.TotalPopulation);
            var db = new DenizenDatabase(Seed, tree, Defs);
            var pop = tree.Query.Region(1).Age(Ages.Elderly).Race(Races.Mark);
            const int total = 1_000;
            //Debug.Log($"Population total used for generation of {total} denizens was: {pop.Count}.");
            List<int> ids = new (total);

            using (Measure.Scope())
            {
                db.RandomIds(pop, InUseStates.NotInUse, ref ids, total);
                var uids = ids;
                for (int i = 0; i < total; i++)
                {
                    int id = uids[i];
                    db.SetIdUse(id, true);
                    var denizen = db.GenerateDenizen(id);
                }
            }
        }

        //[Test, Performance]
        public void GenerateRandomIdsFromPop_TenThousand()
        {
            var tree = new PopulationTree(Defs.TotalPopulation);
            var db = new DenizenDatabase(Seed, tree, Defs);
            var pop = tree.Query.Region(1).Age(Ages.Elderly).Race(Races.Mark);
            const int total = 10_000;
            //Debug.Log($"Population total used for generation of {total} denizens was: {pop.Count}.");

            using (Measure.Scope())
            {
                for (int i = 0; i < total; i++)
                {
                    var id = db.RandomId(pop, InUseStates.NotInUse);
                    db.SetIdUse(id, true);
                    var denizen = db.GenerateDenizen(id);
                }
            }
        }

        [Test, Performance]
        public void GenerateSetOfRandomIdsFromPop_TenThousand()
        {
            var tree = new PopulationTree(Defs.TotalPopulation);
            var db = new DenizenDatabase(Seed, tree, Defs);
            var pop = tree.Query.Region(1).Age(Ages.Elderly).Race(Races.Mark);
            const int total = 10_000;
            //Debug.Log($"Population total used for generation of {total} denizens was: {pop.Count}.");
            List<int> ids = new(total);

            using (Measure.Scope())
            {
                db.RandomIds(pop, InUseStates.NotInUse, ref ids, total);
                var uids = ids;
                for (int i = 0; i < total; i++)
                {
                    int id = uids[i];
                    db.SetIdUse(id, true);
                    var denizen = db.GenerateDenizen(id);
                }
            }
        }
        #endregion


        #region Town Search
        [Test, Performance]
        public void FindAllDenizensOfTown_GlobalSearch()
        {
            Measure.Method(() =>
            {
                Db.FindAllIdsForTown(TownId, ref Ids);
            })
                .WarmupCount(Warmup)
                .MeasurementCount(Measures)
                .IterationsPerMeasurement(Iterations)
                .Run();

            Assert.AreEqual(TownPopulation, Ids.Count);
        }

        [Test, Performance]
        public void FindAllDenizensOfTown_GlobalSearch_Multithreaded()
        {
            Measure.Method(() =>
            {
                Db.FindAllIdsForTownMT(TownId, ref Ids);
            })
                .WarmupCount(Warmup)
                .MeasurementCount(Measures)
                .IterationsPerMeasurement(Iterations)
                .Run();

            Assert.AreEqual(TownPopulation, Ids.Count);
        }

        [Test, Performance]
        public void FindAllDenizensOfTown_PopulationSearch()
        {
            Measure.Method(() =>
            {
                Db.FindSampleIdsForTown(Sample, TownId, ref Ids);
            })
                .WarmupCount(Warmup)
                .MeasurementCount(Measures)
                .IterationsPerMeasurement(Iterations)
                .Run();

            Assert.AreEqual(TownPopulation, Ids.Count);
        }

        [Test, Performance]
        public void FindAllDenizensOfTown_PopulationSearch_Multithreaded()
        {
            Measure.Method(() =>
            {
                Db.FindSampleIdsForTownMT(Sample, TownId, ref Ids);
            })
                .WarmupCount(Warmup)
                .MeasurementCount(Measures)
                .IterationsPerMeasurement(Iterations)
                .Run();

            Assert.AreEqual(TownPopulation, Ids.Count);
        }
        #endregion


        #region Town & Residence Search
        [Test, Performance]
        public void FindAllDenizensOfResidence_GlobalSearch()
        {
            Measure.Method(() =>
            {
               Db.FindAllIdsForResidence(TownId, ResidenceId, ref Ids);
            })
                .WarmupCount(Warmup)
                .MeasurementCount(Measures)
                .IterationsPerMeasurement(Iterations)
                .Run();

            Assert.AreEqual(TownPopulation/2, Ids.Count);
        }

        [Test, Performance]
        public void FindAllDenizensOfResidence_GlobalSearch_Multithreaded()
        {
            Measure.Method(() =>
            {
                Db.FindAllIdsForResidenceMT(TownId, ResidenceId, ref Ids);
            })
                .WarmupCount(Warmup)
                .MeasurementCount(Measures)
                .IterationsPerMeasurement(Iterations)
                .Run();

            Assert.AreEqual(TownPopulation / 2, Ids.Count);
        }

        [Test, Performance]
        public void FindAllDenizensOfResidence_PopulationSearch()
        {
            Measure.Method(() =>
            {
                Db.FindSampleIdsForResidence(Sample, TownId, ResidenceId, ref Ids);
            })
                .WarmupCount(Warmup)
                .MeasurementCount(Measures)
                .IterationsPerMeasurement(Iterations)
                .Run();
            Assert.AreEqual(TownPopulation/2, Ids.Count);
        }

        #endregion
    }
}
