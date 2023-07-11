using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Denizens
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName= "Census Asset", menuName= "Denizens/Census")]
    public class CensusAsset : ScriptableObject
    {
        [Space(12)]
        [CustomValueDrawer("DrawGenders")]
        [ListDrawerSettings(OnBeginListElementGUI = "StartGenders", OnEndListElementGUI = "EndGenders", HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        public double[] Genders;
#if UNITY_EDITOR
        [CustomValueDrawer("DrawGendersTotal")]
        [SerializeField]
        [ReadOnly]
        float GendersTotal;
#endif


        [Space(12)]
        [CustomValueDrawer("DrawAges")]
        [ListDrawerSettings(OnBeginListElementGUI = "StartAges", OnEndListElementGUI = "EndAges", HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        public double[] Ages;
#if UNITY_EDITOR
        [CustomValueDrawer("DrawAgesTotal")]
        [SerializeField]
        [ReadOnly]
        float AgesTotal;
#endif

        [Space(12)]
        [CustomValueDrawer("DrawRaces")]
        [ListDrawerSettings(OnBeginListElementGUI = "StartRaces", OnEndListElementGUI = "EndRaces", HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        public double[] Races;
#if UNITY_EDITOR
        [CustomValueDrawer("DrawRacesTotal")]
        [SerializeField]
        [ReadOnly]
        float RacesTotal;
#endif

        [Space(12)]
        [CustomValueDrawer("DrawJobs")]
        [ListDrawerSettings(OnBeginListElementGUI = "StartJobs", OnEndListElementGUI = "EndJobs", HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        public double[] Jobs;
#if UNITY_EDITOR
        [CustomValueDrawer("DrawJobsTotal")]
        [SerializeField]
        [ReadOnly]
        float JobsTotal;
#endif


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PopulationCensusHierarchy AsHierarchy
        {
            get
            {
                var jobs = new PopulationCensusHierarchy(Jobs, null);
                var races = new PopulationCensusHierarchy(Races, jobs);
                var ages = new PopulationCensusHierarchy(Ages, races);
                var genders = new PopulationCensusHierarchy(Genders, ages);
                return genders;
            }
        }

        public CensusAsset()
        {
            int ageLen = (int)Enum.GetValues(typeof(Ages)).Cast<Ages>().Max() + 1;
            if (Ages == null || Ages.Length != ageLen) Ages = new double[ageLen];

            int raceLen = (int)Enum.GetValues(typeof(Races)).Cast<Races>().Max() + 1;
            if (Races == null || Races.Length != raceLen) Races = new double[raceLen];

            int genderLen = (int)Enum.GetValues(typeof(Genders)).Cast<Genders>().Max() + 1;
            if (Genders == null || Genders.Length != genderLen) Genders = new double[genderLen];

            int occLen = (int)Enum.GetValues(typeof(Jobs)).Cast<Jobs>().Max() + 1;
            if (Jobs == null || Jobs.Length != occLen) Jobs = new double[occLen];
        }

        #region Inspector Drawers
#if UNITY_EDITOR
        public float DrawAgesTotal(float value, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            var result = EditorGUILayout.FloatField(new GUIContent("Ages Total"), (float)Ages.Sum());
            EditorGUI.EndDisabledGroup();
            return result;
        }

        public float DrawRacesTotal(float value, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            var result = EditorGUILayout.FloatField(new GUIContent("Races Total"), (float)Races.Sum());
            EditorGUI.EndDisabledGroup();
            return result;
        }

        public float DrawGendersTotal(float value, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            var result = EditorGUILayout.FloatField(new GUIContent("Genders Total"), (float)Genders.Sum());
            EditorGUI.EndDisabledGroup();
            return result;
        }

        public float DrawJobsTotal(float value, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            var result = EditorGUILayout.FloatField(new GUIContent("Occupations Total"), (float)Jobs.Sum());
            EditorGUI.EndDisabledGroup();
            return result;
        }

        private void StartAges(int index)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Enum.GetNames(typeof(Ages))[index]);
        }

        private float DrawAges(float value, GUIContent label)
        {
            var total = (float)Ages.Sum();
            if (total > 1.0f)
                value = value - (total - 1.0f);
            return EditorGUILayout.Slider(value, 0, 1);
        }

        private void EndAges(int index)
        {
            EditorGUILayout.EndHorizontal();
        }

        private void StartRaces(int index)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Enum.GetNames(typeof(Races))[index]);
        }

        private float DrawRaces(float value, GUIContent label)
        {
            var total = (float)Races.Sum();
            if (total > 1.0f)
                value = value - (total - 1.0f);
            return EditorGUILayout.Slider(value, 0, 1);
        }

        private void EndRaces(int index)
        {
            EditorGUILayout.EndHorizontal();
        }

        private void StartGenders(int index)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Enum.GetNames(typeof(Genders))[index]);
        }

        private float DrawGenders(float value, GUIContent label)
        {
            var total = (float)Genders.Sum();
            if (total > 1.0f)
                value = value - (total - 1.0f);
            return EditorGUILayout.Slider(value, 0, 1);
        }

        private void EndGenders(int index)
        {
            EditorGUILayout.EndHorizontal();
        }

        private void StartJobs(int index)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Enum.GetNames(typeof(Jobs))[index]);
        }

        private float DrawJobs(float value, GUIContent label)
        {
            var total = (float)Jobs.Sum();
            if (total > 1.0f)
                value = value - (total - 1.0f);
            return EditorGUILayout.Slider(value, 0, 1);
        }

        private void EndJobs(int index)
        {
            EditorGUILayout.EndHorizontal();
        }
#endif
        #endregion
    }


    /// <summary>
    /// Transforms a PopulationCensusAsset into a hierarchical structure
    /// to allow for an easier time recusrively parsing data.
    /// </summary>
    public class PopulationCensusHierarchy
    {
        public double[] CurrentLevel { get; }
        public PopulationCensusHierarchy NextLevel { get; }

        public PopulationCensusHierarchy(double[] values, PopulationCensusHierarchy child)
        {
            CurrentLevel = values;
            NextLevel = child;
        }

        /// <summary>
        /// Gets a sample of the census at the given depth.
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public double[] GetAtDepth(int depth)
        {
            int currDepth = 0;
            var node = this;
            while(node != null)
            {
                if(currDepth == depth)
                    return node.CurrentLevel;
                
                node = node.NextLevel;
                currDepth++;
            }

            throw new Exception("Invalid level specified");
        }

        /// <summary>
        /// Gets the full range of the census at the given depth, repeated for each split above it in the hierarchy.
        /// </summary>
        /// <returns></returns>
        public double[] GetFlattenedTiledAtDepth(int depth)
        {
            int tileFactor = 1;
            int currDepth = 0;
            var node = this;
            while (node != null)
            {
                if (currDepth == depth)
                    return Enumerable.Repeat(node.CurrentLevel, tileFactor)
                        .SelectMany(x => x)
                        .ToArray();

                tileFactor *= node.CurrentLevel.Length;
                node = node.NextLevel;
                currDepth++;
            }

            throw new Exception("Invalid level specified");
        }

        /// <summary>
        /// Gets the full range of the census at the given depth, repeated for each split above it in the hierarchy.
        /// </summary>
        /// <returns></returns>
        public double[][] GetTiledAtDepth(int depth)
        {
            int tileFactor = 1;
            int currDepth = 0;
            var node = this;
            while (node != null)
            {
                if (currDepth == depth)
                    return Enumerable.Repeat(node.CurrentLevel, tileFactor)
                        .Select(x => x)
                        .ToArray();

                tileFactor *= node.CurrentLevel.Length;
                node = node.NextLevel;
                currDepth++;
            }

            throw new Exception("Invalid level specified");
        }
    }
}
