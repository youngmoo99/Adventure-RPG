using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Resources;

namespace RPG.Movement
{
    // 이동 관련 로직 클래스 
    // NavMeshAgent를 사용하여 목적지로 이동, 애니메이션 연동, 세이브/로드 지원
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        //최대 이동 속도
        [SerializeField] float maxSpeed = 6f;

        NavMeshAgent navMeshAgent;
        Health health;

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {   
            // 죽었으면 이동 차단
            navMeshAgent.enabled = !health.IsDead();

            // 애니메이터 파라미터 업데이트 (이동 속도 반영)
            UpdateAnimator();
        }

        // 액션 스케줄러와 연동 : 이동 명령 시작
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {   
            // 현재 행동을 이동으로 등록
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        // 실제 이동 명령
        public void MoveTo(Vector3 destination, float speedFraction)
        {   
            // 목적지 설정
            navMeshAgent.destination = destination;
            // 이동 속도
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            // 이동 시작
            navMeshAgent.isStopped = false;
        }

        // IAction 구현 : 이동 취소시 멈춤
        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }
        
        // 애니메이터에 속도 값 반영
        private void UpdateAnimator()
        {   
            // NavMeshAgent의 월드 속도를 로컬 좌표계 속도로 변환
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            // z축 전진 속도를 기준으로 forwardSpeed 파라미터 설정
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        // ISaveable: 현재 위치 저장
        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        // ISaveable: 위치 복원
        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            // NavMeshAgent를 잠시 껐다 켜서 순간이동 적용
            navMeshAgent.enabled = false;
            transform.position = position.ToVector();
            navMeshAgent.enabled = true;

            // 위치 복원 후 이전 행동(이동 등) 취소
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}