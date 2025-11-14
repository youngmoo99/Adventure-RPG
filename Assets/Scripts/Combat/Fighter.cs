using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{   
    // 전투 로직(타겟 추적/ 사거리 체크/ 공격/ 무기 장착/ 세이브) 담당
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {   
        // 공격 간격
        [SerializeField] float timeBetweenAttacks = 1f;

        // 오른손 소켓
        [SerializeField] Transform rightHandTransform = null;

        // 왼손 켓
        [SerializeField] Transform leftHandTransform = null;

        // 기본 무기 설정(SO)
        [SerializeField] WeaponConfig defaultWeapon = null;

        //현재 공격 대상
        Health target;

        // 마지막 공격 후 경과 시간
        float timeSinceLastAttack = Mathf.Infinity;

        // 현재 무기 설정
        WeaponConfig currentWeaponConfig;

        // 씬에 붙는 무기 인스턴스(지연 초기화)
        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {   
            // 시작 무기 설정
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        // 기본 무기를 손에 붙이고 그 인스턴스를 반환
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Start()
        {   
            // Lazy 강제 초기화(실제 무기 인스턴스 생성)
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            // 타겟이 없거나 이미 죽었으면 아무것도 하지 않음
            if (target == null) return;
            if (target.IsDead()) return;

            // 사거리 밖이면 추적 이동, 안이면 공격
            if (!GetIsInRange(target.transform))
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        // 외부에서 무기 장착(교체)
        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        // 실제로 손 소켓에 무기를 스폰하고 애니메이터 오버라이드 적용
        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        // 현재 타겟 반환
        public Health GetTarget()
        {
            return target;
        }

        // 사거리 내에서의 공격 동작(회전 + 쿨다은 검사 + 트리거)
        private void AttackBehaviour()
        {   
            // 타겟을 바라보게 회전
            transform.LookAt(target.transform);

            // 쿨다운 종료 시 공격 애니메이션 트리거
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // 애니메이션 이벤트 Hit() 유발
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        // 애니메이션 트리거 설정
        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        // Animation Event
        void Hit()
        {
            if (target == null) { return; }

            // 스탯 시스템에서 최종 데미지(기본 + 보정 등) 가져오기
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            // 무기가 추가 효과를 갖는 경우(슬래시 이펙트 등)
            if (currentWeapon.value != null)
            {   
                currentWeapon.value.OnHit();
            }

            // 투사체 무기면 발사, 아니면 즉시 피격  처리
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        // 활, 완드 애니메이션
        void Shoot()
        {
            Hit();
        }

        // 사거리 체크
        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetRange();
        }

        // 공격 가능 여부(도달 가능 OR 이미 사거리 내, 그리고 대상이 살아있음)
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !GetIsInRange(combatTarget.transform)) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        // 공격 시작: 스케줄러에 자신을 등록하고 타겟 설정
        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        // IAction 구현: 현재 공격  취소
        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        // 애니메이션 트리거 정리
        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        // ----- IModifierProvider: 스탯 보정 제공 -----
        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                // 무기가 주는 고정(가산) 데미지
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {   
                // 무기가 주는 % 보너스 데미지
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        // ----- ISaveable -----
        public object CaptureState()
        {   
            // 현재 무기 이름을 저장
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}