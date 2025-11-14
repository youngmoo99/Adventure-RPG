using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    // 투사체(화살 / 파이어볼) 이동 및 피격 처리
    // 목표 추적(호밍) 옵션
    // 명중 시 이페트/이벤트/파츠 파괴/본체 지연 파괴
    public class Projectile : MonoBehaviour
    {
        // 투사체 속도
        [SerializeField] float speed = 1f;

        // 투사체 추적(호밍) 여부 
        [SerializeField] bool isHoming = true;

        // 타격 이펙트
        [SerializeField] GameObject hitEffect = null;

        // 투사체 최대 생존 시간
        [SerializeField] float maxLifeTime = 10f;

        // 명중시 파괴 파츠
        [SerializeField] GameObject[] destroyOnHit = null;

        // 명중 후 본체가 남아 있을 시간
        [SerializeField] float lifeAfterImpact = 2f;

        // 명중 시 콜백(사운드)
        [SerializeField] UnityEvent onHit;

        // 명중 목표
        Health target = null;

        // 공격자
        GameObject instigator = null;

        // 가할 데미지
        float damage = 0;

        void Start()
        {
            // 시작 시 목표 방향으로 정렬
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (target == null) return;

            // 호밍이면 매 프레임 목표를 향해 회전(목표가 살아있는 동안)
            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }

            // 전진 이동
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        // 외부에서 목표/공격자/피해량 설정
        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            // 수명 경과 후 자동 파괴
            Destroy(gameObject, maxLifeTime);
        }

        // 조준 위치 계산
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            // 캡슐 중앙으로 조준(몸통쪽 조준)
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        // 트리거 충돌
        void OnTriggerEnter(Collider other)
        {
            // 내 타겟과만 충돌 처리(다른 콜라이더는 무시)
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;

            // 데미지 적용
            target.TakeDamage(instigator, damage);

            // 이동 정지
            speed = 0;

            onHit.Invoke();

            // 명중 이펙트 생성
            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            // 지정된 파츠 제거
            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            // 약간 남겼다가 본체 파괴(이펙트 재생 시간 확보)
            Destroy(gameObject, lifeAfterImpact);

        }
    }
}
