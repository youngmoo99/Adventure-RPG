using System.Collections;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Control
{
    // 적이 죽으면 일정 시간 후 원래 위치에서 부활하는 컴포넌트
    public class EnemyRespawner : MonoBehaviour
    {
        [SerializeField] float respawnDelay = 10f; // 리스폰 딜레이 (초)

        Health health;
        AIController aiController;
        Vector3 spawnPosition;
        bool isRespawning = false;
        bool wasDead = false;

        void Awake()
        {
            health = GetComponent<Health>();
            aiController = GetComponent<AIController>();
        }

        void Start()
        {
            // 초기 스폰 위치 저장
            spawnPosition = transform.position;
        }

        void Update()
        {
            // 이미 리스폰 중이면 무시
            if (isRespawning) return;

            // 죽은 상태가 되었고, 이전 프레임에서는 살아있었으면 리스폰 코루틴 시작
            if (health.IsDead() && !wasDead)
            {
                wasDead = true;
                StartCoroutine(RespawnAfterDelay());
            }

            // 살아있는 상태면 wasDead 플래그 리셋
            if (!health.IsDead())
            {
                wasDead = false;
            }
        }

        IEnumerator RespawnAfterDelay()
        {
            isRespawning = true;

            // 리스폰 딜레이 대기
            yield return new WaitForSeconds(respawnDelay);

            // 시체 렌더러 비활성화 (시체 숨기기)
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            Collider[] colliders = GetComponentsInChildren<Collider>();
            
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }

            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            // 원래 위치로 이동 (AIController의 guardPosition 사용)
            Vector3 respawnPosition = spawnPosition;
            if (aiController != null)
            {
                respawnPosition = aiController.GetGuardPositionValue();
            }
            transform.position = respawnPosition;

            // 부활 처리 (체력 회복 및 애니메이션 리셋)
            health.Resurrect();

            // AI 상태 리셋 (어그로 상태 초기화하여 플레이어가 사정거리 안에 들어와야 공격하도록)
            if (aiController != null)
            {
                aiController.ResetAIState();
            }

            // 렌더러 및 콜라이더 재활성화
            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
            }

            foreach (var collider in colliders)
            {
                collider.enabled = true;
            }

            isRespawning = false;
            wasDead = false;
        }
    }
}

