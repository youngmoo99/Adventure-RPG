using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    // 각 캐릭터 클래스의 스탯 성장 곡선을 정의하는 ScriptableObject
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        // 빠른 접근을 위한 캐시(딕셔너리)
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        // 특정 캐릭터 클래스의 특정 스탯을 레벨 기반으로 반환
        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];

            if (levels.Length < level)
            {
                return 0;
            }

            // 배열은 0부터 시작, 레벨은 1부터 시작
            return levels[level - 1];
        }

        // 특정 스탯이 몇 레벨 까지 정의되어 있는지 반환
        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }
        
        // LookupTable 초기화 (처음 한 번만 빌드)
        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        // 내부 데이터 구조 정의
        [System.Serializable]
        class ProgressionCharacterClass
        {   
            // 캐릭터 타입(플레이어, 적)
            public CharacterClass characterClass;
            // 각 스탯별 성장 곡선
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {   
            // 스탯 종류(체력, 데미지, 경험치)
            public Stat stat;
            // 레벨별 수치
            public float[] levels;
        }
    }
}