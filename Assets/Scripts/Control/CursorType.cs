namespace RPG.Control
{   
    // 마우스 커서 상태 정의
    public enum CursorType
    {   
        // 기본(비활성)
        None,
        // 이동 가능
        Movement,
        // 전투/공격 가능
        Combat,
        // UI 위
        UI,
        // 줍기/상호작용
        Pickup
    }
}