using UnityEngine;

namespace RPG.Core
{
    //행동 스케줄링 
    //새로운 행동이 시작되면 이전 행동 취소
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;

        //새로운 행동 시작
        public void StartAction(IAction action)
        {   
            //같은 행동이 이미 실행중이면 무시
            if (currentAction == action) return; 

            //기존에 실행중인 행동이 있으면 취소ㅓ
            if (currentAction != null)
            {
                currentAction.Cancel();
            }

            //새로운 행동 교체
            currentAction = action;
        }

        //현재 행동을 취소
        public void CancelCurrentAction()
        {
            StartAction(null);
        }

    }
}