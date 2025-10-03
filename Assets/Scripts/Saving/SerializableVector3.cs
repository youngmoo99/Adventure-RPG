using UnityEngine;

namespace RPG.Saving
{   
    // BinaryFormatter로 Vector3를 안전하게 직렬화하기 위한 래퍼 클래스
    // (Unity의 Vector3는 기본적으로 [Serializable]이지만,
    //  포맷터/버전 등에 따라 커스텀 래퍼를 쓰기도 함)
    [System.Serializable]
    public class SerializableVector3
    {
        float x, y, z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
        
        // 저장된 값으로부터 Unity Vector3 생성
        public Vector3 ToVector()
        {
            return new Vector3(x, y, z);
        }
    }
}