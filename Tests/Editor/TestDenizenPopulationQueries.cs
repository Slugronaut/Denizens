using NUnit.Framework;
using ProceduralPopulationDatabase;
using UnityEngine;

namespace Denizens.Tests.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public class TestDenizenPopulationQueries : ScriptableObject
    {
        public KingdomAsset K1;

        const int Seed = 69420;
        const int RegionCount = 2;
        const int GenderCount = 2;
        const int AgeCount = 6;
        const int RaceCount = 6;
        const int JobCount = 22;

        DenizenDatabase Db;
        PopulationTree PopTree;


        [OneTimeSetUp]
        public void SetupDb()
        {
            PopTree = new PopulationTree(K1.TotalPopulation);
            Db = new DenizenDatabase(Seed, PopTree, K1);
        }

        [Test]
        public void KingdomAssetParsesToPopTree()
        {
            Assert.AreEqual((int)TreeDepths.Jobs + 1, Db.Tree.MaxDepth); //root, region, gender, age, race, job = 6

            Assert.AreEqual(RegionCount, Db.Tree.GetSliceGroupCount((int)TreeDepths.Regions));
            Assert.AreEqual(GenderCount, Db.Tree.GetSliceGroupCount((int)TreeDepths.Genders));
            Assert.AreEqual(AgeCount, Db.Tree.GetSliceGroupCount((int)TreeDepths.Ages));
            Assert.AreEqual(RaceCount, Db.Tree.GetSliceGroupCount((int)TreeDepths.Races));
            Assert.AreEqual(JobCount, Db.Tree.GetSliceGroupCount((int)TreeDepths.Jobs));
        }

        [Test]
        public void QueryRegion()
        {
            var pop0 = PopTree.Query.Region(0);
            var pop1 = PopTree.Query.Region(1);

            Assert.AreNotSame(pop0, pop1);

            Assert.AreEqual(4000, pop0.Count);
            Assert.AreEqual(1, pop1.Ranges.Count);
            Assert.AreEqual(0, pop0.Ranges[0].StartIndex);
            Assert.AreEqual(3999, pop0.Ranges[0].EndIndex);
            Assert.AreEqual(4000, pop0.Ranges[0].Length);

            Assert.AreEqual(6000, pop1.Count);
            Assert.AreEqual(1, pop1.Ranges.Count);
            Assert.AreEqual(4000, pop1.Ranges[0].StartIndex);
            Assert.AreEqual(9999, pop1.Ranges[0].EndIndex);
            Assert.AreEqual(6000, pop1.Ranges[0].Length);
        }

        [Test]
        public void ReversedQueryGivesSameRanges()
        {
            var pop1 = PopTree.Query
                .Gender(Genders.Male)
                .Age(Ages.Teen)
                .Race(Races.DarkElf)
                .Job(Jobs.Adventurer);

            var pop2 = PopTree.Query
               .Job(Jobs.Adventurer)
               .Race(Races.DarkElf)
               .Age(Ages.Teen)
               .Gender(Genders.Male);

            Assert.AreNotSame(pop1, pop2);
            Assert.AreEqual(pop1.Count, pop2.Count);

            for(int i = 0; i < pop1.Ranges.Count; i++)
            {
                var range1 = pop1.Ranges[i];
                var range2 = pop2.Ranges[i];
                Assert.AreEqual(range1.Length, range2.Length);
                Assert.AreEqual(range1.StartIndex, range2.StartIndex);
                Assert.AreEqual(range1.EndIndex, range2.EndIndex);
            }
        }

        [Test]
        public void ScrambledQueryGivesSameRanges()
        {
            var pop1 = PopTree.Query
                .Gender(Genders.Male)
                .Age(Ages.Teen)
                .Race(Races.DarkElf)
                .Job(Jobs.Adventurer);

            var pop2 = PopTree.Query
               .Age(Ages.Teen)
               .Race(Races.DarkElf)
               .Job(Jobs.Adventurer)
               .Gender(Genders.Male);

            Assert.AreNotSame(pop1, pop2);
            Assert.AreEqual(pop1.Count, pop2.Count);

            for (int i = 0; i < pop1.Ranges.Count; i++)
            {
                var range1 = pop1.Ranges[i];
                var range2 = pop2.Ranges[i];
                Assert.AreEqual(range1.Length, range2.Length);
                Assert.AreEqual(range1.StartIndex, range2.StartIndex);
                Assert.AreEqual(range1.EndIndex, range2.EndIndex);
            }
        }

        [Test]
        public void QueryCorrectRanges_Region0()
        {
            var pop = PopTree.Query
                .Region(0);

            Assert.AreEqual(4000, pop.Count);
            Assert.AreEqual(1, pop.Ranges.Count);
            Assert.AreEqual(4000, pop.Ranges[0].Length);
            Assert.AreEqual(0, pop.Ranges[0].StartIndex);
            Assert.AreEqual(3999, pop.Ranges[0].EndIndex);
        }

        [Test]
        public void QueryCorrectRanges_Region0Female()
        {
            var pop = PopTree.Query
                .Region(0)
                .Gender(Genders.Female);

            Assert.AreEqual(2000, pop.Count);
            Assert.AreEqual(1, pop.Ranges.Count);
            Assert.AreEqual(2000, pop.Ranges[0].Length);
            Assert.AreEqual(2000, pop.Ranges[0].StartIndex);
            Assert.AreEqual(3999, pop.Ranges[0].EndIndex);
        }

        [Test]
        public void QueryCorrectRanges_Region0FemaleTeen()
        {
            var pop = PopTree.Query
                .Region(0)
                .Gender(Genders.Female)
                .Age(Ages.Teen);

            Assert.AreEqual(200, pop.Count);
            Assert.AreEqual(1, pop.Ranges.Count);
            Assert.AreEqual(200, pop.Ranges[0].Length);
            Assert.AreEqual(3700, pop.Ranges[0].StartIndex);
            Assert.AreEqual(3899, pop.Ranges[0].EndIndex);
        }

        [Test]
        public void QueryCorrectRanges_Region0FemaleTeenDarkElf()
        {
            var pop = PopTree.Query
                .Region(0)
                .Gender(Genders.Female)
                .Age(Ages.Teen)
                .Race(Races.DarkElf);

            Assert.AreEqual(40, pop.Count);
            Assert.AreEqual(1, pop.Ranges.Count);
            Assert.AreEqual(40, pop.Ranges[0].Length);
            Assert.AreEqual(3840, pop.Ranges[0].StartIndex);
            Assert.AreEqual(3879, pop.Ranges[0].EndIndex);
        }

        [Test]
        public void QueryGivesCorrectRanges_Region0FemaleTeenDarkElfAdventurerers()
        {
            var pop = PopTree.Query
                .Region(0)
                .Gender(Genders.Female)
                .Age(Ages.Teen)
                .Race(Races.DarkElf)
                .Job(Jobs.Adventurer);

            Assert.AreEqual(20, pop.Count);
            Assert.AreEqual(1, pop.Ranges.Count);
            Assert.AreEqual(20, pop.Ranges[0].Length);
            Assert.AreEqual(3840, pop.Ranges[0].StartIndex);
            Assert.AreEqual(3859, pop.Ranges[0].EndIndex);
        }

        [Test]
        public void RecreateDenizenRecord()
        {
            var pop = PopTree.Query
                .Region(1)
                .Gender(Genders.Female)
                .Age(Ages.Teen)
                .Race(Races.DarkElf)
                .Job(Jobs.Adventurer);

            var id = Db.RandomId(pop, InUseStates.Either);
            Db.SetDenizenTownId(id, 69);
            var den = Db.GenerateDenizen(id);

            Assert.AreEqual(1, den.HomeLand.Region);
            Assert.AreEqual(69, den.HomeLand.Town);
            Assert.AreEqual(Genders.Female, den.Gender);
            Assert.AreEqual(Ages.Teen, den.Age);
            Assert.AreEqual(Races.DarkElf, den.Race);
            Assert.AreEqual(Jobs.Adventurer, den.Job);
        }
    }
}
