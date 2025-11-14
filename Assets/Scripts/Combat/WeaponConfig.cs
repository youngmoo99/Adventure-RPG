using RPG.Core;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{   
    // 무기 데이터(데미지/사거리/애니메이터/투사체)를 보관하는 ScriptableObject
    // - Spawn: 손 소켓에 프리팹 장착 + 애니메이터 오버라이드 처리
    // - LaunchProjectile: 투사체 발사
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {   
        // 무기 전용 애니메이션
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        // 손에 붙일 무기 프리팹
        [SerializeField] Weapon equippedPrefab = null;
        // 무기 기본 데미지
        [SerializeField] float weaponDamage = 5f;
        // 데미지 % 보너스
        [SerializeField] float percentageBonus = 0;
        // 무기 사거리
        [SerializeField] float weaponRange = 2f;
        // 오른손/왼손 장착 여부
        [SerializeField] bool isRightHanded = true;
        // 투사체 프리팹
        [SerializeField] Projectile projectile = null;

        // 손 소켓 아래에 붙을 이름(교체/삭제 시 탐색용)
        const string weaponName = "Weapon";

        // 손 소켓에 무기를 장착하고 애니메이터 오버라이드 적용
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            // 예전 무기 제거
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            // 무기 프리팹 장착
            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                // 추후 교체/삭제를 쉽게 하기 위해 이름 고정
                weapon.gameObject.name = weaponName;
            }

            // 애니메이터 오버라이드 적용(없으면 기본 컨트롤러 원복)
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                // 기존에 다른 무기가 덮어쓴 오버라이드가 남아있다면 원본으로 되돌림
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        // 이전에 장착된 무기 오브젝트 제거
        void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            // 즉시 같은 이름으로 다시 탐색되지 않도록 변경
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }
        
        // 어느 손에 장착할지 결정
        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        // 투사체 무기인지 여부
        public bool HasProjectile()
        {
            return projectile != null;
        }

        // 투사체 발사(무기 손 위치에서 생성 후 목표/공격자/데미지 세팅)
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        // 스탯 인터페이스 제공
        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetRange()
        {
            return weaponRange;
        }
    }
}