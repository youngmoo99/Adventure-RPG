using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    // 캐릭터의 레벨과 스탯(체력, 공격력)을 관리하는 클래스
    // 1) Progression(성장 테이블)로부터 기본 스탯들 가져옴 
    // 2) Experience 변화에 따라 레벨 계산/업데이트
    // 3) 레벨업 이펙트 & 이벤트 발행
    // 4) 장비/버프 등의 보정까지 합산해 최종 스탯 산출
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        // 시작 레벨
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        // 장비/버프 보정 사용 여부
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;
        Experience experience;

        void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }
        private void Start()
        {
            currentLevel.ForceInit();
        }

        void OnEnable()
        {   
            // 경험치 획득 시 레벨업 업데이트
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        // 경험치가 변하면 현재 레벨을 재계산하고 상승 시 이벤트/이펙트 처리
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        // 레벨업 파티클 재생
        void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        // 최종 스탯 = (기본 + 가산 보정) * (1 + %보정 / 100)
        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        // 성장 테이블에서 기본 스탯 조회
        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        // 가산 보정 합
        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifier(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        // % 보정 합
        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        // 현재 경험치로 레벨 계산
        // - Progression의 'ExperienceToLevelUp' 곡선을 사용
        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float currentXP = experience.GetPoints();
            // 마지막 레벨 직전(최고 레벨 바로 전)의 인덱스
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            // 요구 경험치를 순차 비교하여 현재 레벨 판정
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }
            // 요구치를 모두 넘겼다면 최종 레벨
            return penultimateLevel + 1;
        }
    }
}