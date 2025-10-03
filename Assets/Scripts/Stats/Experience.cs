using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    // 경험치 보관 및 세이브/로드
    // 경험치 증가 시 onExperienceGained 이벤트 발행 (BaseStats가 구독)
    public class Experience : MonoBehaviour, ISaveable
    {   
        // 누적 경험치
        [SerializeField] float experiencePoints = 0;

        public event Action onExperienceGained;

        // 경험치 획득
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetPoints()
        {
            return experiencePoints;
        }

        // 세이브/로드
        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}