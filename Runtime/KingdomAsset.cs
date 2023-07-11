using System;
using System.Collections.Generic;
using UnityEngine;

namespace Denizens
{
    /// <summary>
    /// Defines a kingdom and all sub regions within it including fiefs and townships.
    /// </summary>
    [CreateAssetMenu(fileName = "Kingdom Definitions", menuName = "Denizens/Kingdom Definitions")]
    public class KingdomAsset : ScriptableObject
    {
        [Serializable]
        public class MunicipalCounts
        {
            public byte Metros = 1;
            public byte Capitals = 3;
            public byte Cities = 10;
            public byte Towns = 25;
            public byte Villages = 30;
            public byte Farms = 50;
            public byte Manors = 10;
            public byte Houses = 40;
        }

        [Serializable]
        public class MunicipalPopulations
        {
            public ushort Metropolis    = 15_000;
            public ushort Captial       = 10_000;
            public ushort City          = 5_000;
            public ushort Town          = 1_000;
            public byte Village         = 130;
            public byte Farm            = 50;
            public byte Manor           = 100;
            public byte House           = 5;
        }

        public enum TownTypes
        {
            Metropolis  = 15_000,
            Captial     = 10_000,
            City        = 5_000,
            Town        = 1_000,
            Viallage    = 130,
            Farm        = 50,
            Manor       = 100,
            House       = 5,
        }


        #region Inner Classes
        [Serializable]
        public class Region
        {
            [Tooltip("The name of this region.")]
            public string Name;
            [Tooltip("The percentage of the entire kingdom population for this region.")]
            public float PercentageOfTotalPopulation;
            [Tooltip("The distribution of the population for this region.")]
            public CensusAsset Census;

            [Space(12)]
            [Header("The number of each type of municipality in this region.")]
            public MunicipalCounts MunicipalCounts;

            [Space(12)]
            [Header("The number of each type of municipality in this region.")]
            public MunicipalPopulations MunicipalPopulations;
        }

        #endregion
        [Space(16)]
        [Tooltip("The total count of the persistent population of the in-game world. This number will be distributed among all regions.")]
        public int TotalPopulation = 4_194_304;

        public List<Region> Regions;

    }


}
