using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using RPG.Resources;
using GameDevTV.Utils;

namespace RPG.Control
{
    // 적 AI 컨트롤러 
    // 1) 플레이어가 사정거리안 -> 추격/공격
    // 2) 마지막으로 플레이어를 본 뒤 일정 시간 내 -> 의심(정지/탐색)
    // 3) 순찰 경로를 따라 순찰
    public class AIController : MonoBehaviour
    {   
        // 플레이어 추격 시작 거리
        [SerializeField] float chaseDistance = 5f;
        // 의심 시간
        [SerializeField] float suspicionTime = 5f;
        // 순찰 경로(없으면 가드 지점에 머무름)
        [SerializeField] PatrolPath patrolPath;
        // 웨이포인트 도달로 간주하는 거리
        [SerializeField] float waypointTolerance = 1f;
        // 웨이포인트에서 머무는 시간
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0, 1)]
        // 순찰 시 이동속도
        [SerializeField] float patrolSpeedFraction = 0.2f;
        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;
        // 시작 위치
        LazyValue<Vector3> guardPosition;
        // 마지막으로 플레이어를 본후 경과 시간
        float timeSinceLastSawPlayer = Mathf.Infinity;
        // 현재 웨이포인트 도착 후 경과 시간
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        // 현재 웨이포인트 인덱스
        int currentWaypointIndex = 0;

        void Awake()
        {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();

            // 지연 초기화 : 현재 위치를 가드 지점으로 사용
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        // 가드 지점 계산(현재 배치된 위치)
        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }
        void Start()
        {   
            //Lazy 강제 초기화
            guardPosition.ForceInit();
        }
        void Update()
        {
            //사망 시 더 이상 처리하지 않음
            if (health.IsDead()) return;
            // 공격/추격 상태
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            // 의심 상태
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            // 순찰 상태
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        // 프레임마다 타이머 증가
        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        // 의심 상태 : 현재 행동을 취소하고 주변을 살피는 느낌
        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        // 순찰 상태 : 웨이포인트를 순환하며 이동
        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null)
            {   
                // 웨이포인트에 도달했으면 대기 시간 초기화 후 다음 웨이포인트로 이동
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }

                // 현재 웨이포인트를 다음 목적지로 설정
                nextPosition = GetCurrentWaypoint();
            }
            // 웨이포인트에서 충분히 머문 뒤 이동 시작
            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }

        }
        
        // 현재 웨이포인트에 도달했는지 검사
        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        // 웨이포인트 인덱스 순환
        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        // 현재 웨이포인트 좌표
        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        // 공격/추격 동작
        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        // 플레이어가 추격/공격 범위 안에 있는지 검사
        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;
        }

        // 에디터에서 선택 시 추격 반경 시각화
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}

