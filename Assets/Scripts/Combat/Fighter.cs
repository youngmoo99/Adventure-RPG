using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Resources;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{   
    // 전투 로직 : 타겟 추적, 사거리 체크, 공격 애니메이션, 데미지 적용, 무기 장착/저장
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {   
        // 공격 간격(쿨다운)
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform handTransform = null;
        // 오른손 소켓
        [SerializeField] Transform rightHandTransform = null;
        // 왼손 소켓
        [SerializeField] Transform leftHandTransform = null;
        // 기본 무기
        [SerializeField] Weapon defaultWeapon = null;
        // 기본 무기 이름
        [SerializeField] string defaultWeaponName = "Unarmed";

        Health target;
        // 마지막 공격 이후 경과 시간
        float timeSinceLastAttack = Mathf.Infinity;
        // 지연 초기화되는 현재 무기
        LazyValue<Weapon> currentWeapon;

        void Awake()
        {   
            // 시작 시 기본 무기 장착을 Lazy로 설정
            currentWeapon = new LazyValue<Weapon>(GetSetupDefaultWeapon);
        }

        private Weapon GetSetupDefaultWeapon()
        {   
            // 기본 무기 장착 처리
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {   
            // Lazy 강제 초기화(실제 장착 실행)
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            // 타겟이 없거나 이미 죽었으면 아무것도 안 함
            if (target == null) return;
            if (target.IsDead()) return;

            // 사거리 밖이면 이동, 사거리 안이면 공격
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {   
                // 이동 취소
                GetComponent<Mover>().Cancel();
                // 공격 루틴
                AttackBehaviour();
            }
        }

        // 외부에서 무기 장착
        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        // 실제로 손 소켓에 무기 프리팹을 붙이고 애니메이터 교체
        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        // 현재 타겟 반환
        public Health GetTarget()
        {
            return target;
        }
        
        // 공격 동작
        private void AttackBehaviour()
        {   
            // 타겟을 바라보게 회전
            transform.LookAt(target.transform);

            // 공격 쿨다운이 지났다면 애니메이션 트리거
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // 애니메이션 Hit() 이벤트를 발동시킬 공격 트리거
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        // 공격 애니메이션 트리거 설정
        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        // Animation Event
        void Hit()
        {
            if (target == null) { return; }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {

                target.TakeDamage(gameObject, damage);
            }
        }

        // Animation Event (Bow)
        void Shoot()
        {
            Hit();
        }

        // 사거리 체크
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.value.GetRange();
        }

        // 공격 가능한지 판단(Health가 있고 죽지 않았는가)
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        // 공격 시작
        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        // IAction 구현 : 현재 공격 취소
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

        // IModifierProvider: 가산(절대) 보정 제공(데미지)
        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetDamage();
            }
        }
        // IModifierProvider: 퍼센트 보정 제공(데미지 %)
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }
        // ISaveable: 현재 무기 이름 저장
        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        // ISaveable: 저장된 이름으로 무기 로드 및 장착
        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }


    }
}