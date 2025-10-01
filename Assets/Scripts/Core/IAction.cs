namespace RPG.Core
{   
    // 게임 내에서 실행 할수 있는 액션을 정의하는 인터페이스
    public interface IAction
    {   
        //현재 진행중인 행동을 취소
        void Cancel();

    }
}