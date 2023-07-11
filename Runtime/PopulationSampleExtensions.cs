using ProceduralPopulationDatabase;
using System.Collections.Generic;

namespace Denizens
{
    /// <summary>
    /// Extends the base <see cref="PopulationSample"/> class to something more domain-specific.
    /// </summary>
    public static class PopulationSampleExtensions
    {
        static List<int> TempList = new(16);

        public static PopulationSample Region(this PopulationSample sample, int regionIndex) => sample.Query((int)TreeDepths.Regions, regionIndex);
        public static PopulationSample Gender(this PopulationSample sample, Genders gender) => sample.Query((int)TreeDepths.Genders, (int)gender);
        public static PopulationSample Age(this PopulationSample sample, Ages age) => sample.Query((int)TreeDepths.Ages, (int)age);
        public static PopulationSample Race(this PopulationSample sample, Races race) => sample.Query((int)TreeDepths.Races, (int)race);
        public static PopulationSample Job(this PopulationSample sample, Jobs job) => sample.Query((int)TreeDepths.Jobs, (int)job);

        public static PopulationSample TownId(this PopulationSample sample, DenizenDatabase db, byte townId)
        {
            db.FindSampleIdsForTown(sample, townId, ref TempList);
            return new PopulationSample(sample.SourceTree, IndexRange.Condense(TempList));
        }

        public static PopulationSample Workplace(this PopulationSample sample, DenizenDatabase db, byte townId, byte workplace)
        {
            db.FindSampleIdsForWorkplace(sample, townId, workplace, ref TempList);
            return new PopulationSample(sample.SourceTree, IndexRange.Condense(TempList));
        }

        public static PopulationSample Residence(this PopulationSample sample, DenizenDatabase db, byte townId, uint residence)
        {
            db.FindSampleIdsForResidence(sample, townId, residence, ref TempList);
            return new PopulationSample(sample.SourceTree, IndexRange.Condense(TempList));
        }
    }
}