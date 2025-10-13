using System;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    // 캐릭터의 HP 관리 클래스 
    // 1) 체력 계산, 데미지 처리, 죽음/부활 로직
    // 2) 경험치 보상, 레벨업 시 체력 회복, 세이브/로드 지원
    public class Health : MonoBehaviour, ISaveable
    {   
        // 레벨업 시 HP 회복 비율
        [SerializeField] float regenerationPercentage = 70f;
        [SerializeField] TakeDamageEvent takeDamage;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }
        // Lazy 초기화: 시작 시 최대 체력으로 세팅
        LazyValue<float> healthPoints;

        // 사망 여부 플래그
        bool isDead = false;

        void Awake()
        {   
            // BaseStats에서 초기 체력을 가져오도록 LazyValue 세팅
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);

        }
        private void Start()
        {
            // LazyValue 강제 초기화 → 체력을 실제로 채움
            healthPoints.ForceInit();
        }

        void OnEnable()
        {   
            // 레벨업 이벤트 발생 시 체력 회복 등록
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        void OnDisable()
        {   
            // 이벤트 해제 (메모리 누수 방지)
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        // 현재 사망 여부 확인
        public bool IsDead()
        {
            return isDead;
        }

        // 데미지 처리
        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + " took damage : " + damage);
            // 체력을 0 이상으로 유지
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);

            // 사망 처리
            if (healthPoints.value == 0)
            {
                Die();
                // 가해자에게 경험치 보상
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        // 현재 체력 반환
        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        // 최대 체력 반환
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        // 체력 % 반환(UI 표시)
        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }


        // 사망 처리
        private void Die()
        {
            // 이미 죽었으면 무시
            if (isDead) return;

            isDead = true;
            // 죽는 애니메이션 실행
            GetComponent<Animator>().SetTrigger("die");
            // 현재 행동 취소
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        // 경험치 보상
        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            
            // 공격자에게 경험치 지급
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        // 레벨업 시 체력 회복
        void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            // 현재 체력이 regen 값도자 낮으면 regen 값으로 회복
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        // 세이브/로드 지원
        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;

            // 복원했는데 체력이 0 이하라면 사망 처리
            if (healthPoints.value <= 0)
            {
                Die();
            }
        }
    }
}