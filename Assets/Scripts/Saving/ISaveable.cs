namespace RPG.Saving
{   
    // 세이브/로드 대상이 되는 컴포넌트가 반드시 구현해야 하는 인터페이스
    public interface ISaveable
    {   
        // 현재 상태를 직렬화 가능한 객체로 캡처해서 반환
        object CaptureState();
        // 저장된 상태를 받아와서 복원
        void RestoreState(object state);
    }
}