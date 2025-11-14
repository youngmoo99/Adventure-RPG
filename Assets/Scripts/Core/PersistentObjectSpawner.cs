using UnityEngine;

namespace RPG.Core
{   
    //씬 전환이 되어도 파괴되지 않고 계속 유지될 PersistentObject를 생성하는 클래스
    public class PersistentObjectSpawner : MonoBehaviour
    {   
        //유지할 프리팹
        [SerializeField] GameObject persistentObjectPrefab;

        // 이미 한 번 생성되었는지 여부를 추적하는 정적 변수
        static bool hasSpawned = false;

        void Awake()
        {
            // 이미 생성된 적이 있다면 다시 생성하지 않음
            if (hasSpawned) return;

            // 최초 1회만 PersistentObject 생성
            SpawnPersistentObjects();

            // 생성 완료 플래그 설정
            hasSpawned = true;
        }

        // Persistent Object를 실제로 생성
        void SpawnPersistentObjects()
        {   
            // 프리팹 인스턴스화
            GameObject persistentObject = Instantiate(persistentObjectPrefab);

            // 씬이 바뀌어도 삭제되지 않음
            DontDestroyOnLoad(persistentObject);

        }
    }
}
