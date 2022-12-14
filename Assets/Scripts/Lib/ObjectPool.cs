// Start Date : 2022. 11. 11.
// Update Date : 2022. 11. 11.
// Developer : Jeong Won Woo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �޸�Ǯ ���������� ( Ŭ���� �ν��Ͻ��� ���� )
public class ObjectPool : Singleton<ObjectPool>
{
    Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>(); // ���� Ǯ
    
    // ������Ʈ ��������
    public T Pop<T>(GameObject original, Vector3 pos, Quaternion rot)
    {
        string keyName = typeof(T).ToString();
        if (pool.ContainsKey(keyName))
        {
            if (pool[keyName].Count > 0)
            {
                GameObject target = pool[keyName].Dequeue();
                target.SetActive(true);
                target.transform.SetParent(null);
                target.transform.position = pos;
                target.transform.rotation = rot;
                return target.GetComponent<T>();
            }
        }
        else
        {
            pool[keyName] = new Queue<GameObject>();
        }
        return Instantiate(original, pos, rot).GetComponent<T>();
    }

    // ������Ʈ �ֱ�
    public void Push<T>(GameObject target)
    {
        target.transform.SetParent(transform);
        target.SetActive(false);
        pool[typeof(T).ToString()].Enqueue(target);
    }
}
