using UnityEngine;

namespace RPG.Core
{
    // 행동 스케줄러
    // 동시에 하나의 IAction만 활성화되도록 보장 
    // 새로운 행동이 시작되면 이전 행동 취소
    public class ActionScheduler : MonoBehaviour
    {   
        // 현재 실행 중인 행동
        IAction currentAction;

        //새로운 행동 시작
        public void StartAction(IAction action)
        {
            //같은 행동이 이미 실행중이면 아무 것도 하지 않음(중복 방지)
            if (currentAction == action) return;

            //기존에 실행중인 행동이 있으면 취소
            if (currentAction != null)
            {
                currentAction.Cancel();
            }

            //현재 행동을 새 행동으로 교체
            currentAction = action;
        }

        //현재 행동을 취소
        public void CancelCurrentAction()
        {
            StartAction(null);
        }

    }
}