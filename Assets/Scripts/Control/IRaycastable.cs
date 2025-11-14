namespace RPG.Control
{   
    // 레이캐스트 상호작용 대상이 구현해야 할 인터페이스
    // - 커서 아이콘 지정
    // - 클릭/호버 시 동작 수행 및 상호작용 여부 반환
    public interface IRaycastable
    {   
        // 이 대상 위 커서 타입
        CursorType GetCursorType();
        // 상호작용 처리(성공 시 true)
        bool HandleRaycast(PlayerController callingController);
    }
}

