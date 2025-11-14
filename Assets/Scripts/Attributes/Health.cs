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
    // 1) 체력 계산 및 데미지/회복 처리
    // 2) 사망/부활 로직 및 애니메이션 트리거
    // 3) 레벨업 시 체력 회복  처리
    // 4) 경험치 보상
    // 5) 세이브/로드 지원(ISaveable)
    public class Health : MonoBehaviour, ISaveable
    {
        // 레벨업 시 HP 회복 비율
        [SerializeField] float regenerationPercentage = 70f;
        // 데미지를 입었을 때 이벤트
        [SerializeField] TakeDamageEvent takeDamage;
        // 사망 시 호출할 Event
        [SerializeField] UnityEvent onDie;

        // 데미지 이벤트용 사용자 정의 Event
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        // LazyValue로 체력을 지연 초기화 (게임 시작 시 BaseStats에서 읽음)
        LazyValue<float> healthPoints;

        // 캐릭터 사망 여부
        bool isDead = false;

        void Awake()
        {
            // BaseStats에서 초기 체력 값을 가져옴
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {   
            // 현재 레벨에 따른 최대 체력 반환
            return GetComponent<BaseStats>().GetStat(Stat.Health);

        }
        private void Start()
        {
            // LazyValue 강제 초기화 → 실제 체력 값 확정
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

        // 데미지를 입었을 때 호출
        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + " took damage : " + damage);

            // 체력을 0 이상으로 유지
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);

            // 사망 처리
            if (healthPoints.value == 0)
            {   
                // 사망 이벤트 실행
                onDie.Invoke();
                Die();

                // 가해자에게 경험치 보상
                AwardExperience(instigator);
            }
            else
            {   
                // 아직 살아 있다면 데미지 이벤트 실행
                takeDamage.Invoke(damage);
            }
        }

        // 회복 함수
        public void Heal(float healthToRestore)
        {   
            // 현재 체력 + 회복량 (최대 체력 초과 방지)
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
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

        // 체력 비율 (0~1)
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

            // 죽는 애니메이션 트리거
            GetComponent<Animator>().SetTrigger("die");

            // 현재 행동 중단
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
            // 현재 체력이 회복 기준치 보다 낮을 경우, 그 값으로 갱신
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        // 세이브
        public object CaptureState()
        {
            return healthPoints.value;
        }

        // 로드
        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;

            // 로드시 체력이 0 이하라면 사망 처리
            if (healthPoints.value <= 0)
            {
                Die();
            }
        }


    }
}