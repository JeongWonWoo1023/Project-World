// Start Date : 2022. 11. 11.
// Update Date : 2022. 11. 11.
// Developer : Jeong Won Woo

using UnityEngine;

// 싱글톤 디자인패턴 ( 상속으로 사용 )
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null) // 인스턴스가 비어있을 경우
            {
                _instance = FindObjectOfType(typeof(T)) as T; // 인스턴스 탐색
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>(); // 인스턴스 생성
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this.transform.GetComponent<T>(); // 인스턴스 등록
        Debug.Log($"생성 : {_instance}");
    }

    public virtual void Initialize() { } // 등록 시점 동작
    public virtual void Destroy() { Destroy(this); } // 인스턴스 제거시 동작
}
