// Start Date : 2022. 11. 11.
// Update Date : 2022. 11. 11.
// Developer : Jeong Won Woo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �̱��� ���������� ( ������� ��� )
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null) // �ν��Ͻ��� ������� ���
            {
                _instance = FindObjectOfType(typeof(T)) as T; // �ν��Ͻ� Ž��
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>(); // �ν��Ͻ� ����
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this.transform.GetComponent<T>(); // �ν��Ͻ� ���
    }

    public virtual void Init() { } // ��� ���� ����
    public virtual void Destroy() { Destroy(this); } // �ν��Ͻ� ���Ž� ����
}
