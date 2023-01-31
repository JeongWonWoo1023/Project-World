// Start Date : 2022. 11. 11.
// Update Date : 2022. 11. 11.
// Developer : Jeong Won Woo

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ObjectPool))]
public class ObjectPoolEditor : Editor
{
    const string INFO = "풀링한 오브젝트에 다음 메소드를 반드시 정의하세요 \nvoid OnDisable()\n{\n" +
        "    ObjectPool.Instance.PushPool(gameObject);    // 한 객체에 한번만 \n" +
        "    CancelInvoke();    // Monobehaviour에 Invoke가 있다면 \n}";

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox(INFO, MessageType.Info);
        base.OnInspectorGUI();
    }
}
#endif

// 메모리풀 컴포넌트
public class ObjectPool : Singleton<ObjectPool>
{
    [Serializable]
    public class Pool
    {
        public string key;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] private Pool[] pools;
    List<GameObject> spawnObject;
    Dictionary<string, Queue<GameObject>> poolDictionary;

    private void OnValidate()
    {
        foreach(Pool pool in pools)
        {
            if(pool.prefab != null)
            {
                pool.key = pool.prefab.name;
            }
        }
    }

    private void Start()
    {
        spawnObject = new List<GameObject>();
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            poolDictionary.Add(pool.key,new Queue<GameObject>());
            for(int i = 0; i < pool.size;i++)
            {
                GameObject obj = CreateNewObject(pool.key, pool.prefab);
                SortPool(obj);
            }
        }
    }

    public GameObject PopObject(string key, Vector3 position) => GetObjectFormPool(key, position, Quaternion.identity);

    public GameObject PopObject(string key, Vector3 position, Quaternion rotation) => GetObjectFormPool(key, position, rotation);

    public T PopObject<T>(string key, Vector3 position) where T : Component
    {
        GameObject clone = GetObjectFormPool(key, position, Quaternion.identity);
        if(clone.TryGetComponent(out T component))
        {
            return component;
        }
        else
        {
            clone.SetActive(false);
            throw new Exception($"Component is not found");
        }
    }

    public T PopObject<T>(string key, Vector3 position, Quaternion rotation) where T : Component
    {
        GameObject clone = GetObjectFormPool(key, position, rotation);
        if (clone.TryGetComponent(out T component))
        {
            return component;
        }
        else
        {
            clone.SetActive(false);
            throw new Exception($"Component is not found");
        }
    }

    public List<GameObject> PopAllObject(string key)
    {
        if(!poolDictionary.ContainsKey(key))
        {
            throw new Exception($"Pool with key {key} doesn't exist InputKey : {key}");
        }
        return spawnObject.FindAll(x => x.name == key);
    }

    public List<T> PopAllObject<T>(string key) where T : Component
    {
        List<GameObject> objects = PopAllObject(key);
        if (!objects[0].TryGetComponent(out T conponent))
        {
            throw new Exception($"Component is not found");
        }
        return objects.ConvertAll(x => x.GetComponent<T>());
    }

    public void PushPool(GameObject obj)
    {
        if(!poolDictionary.ContainsKey(obj.name))
        {
            throw new Exception($"Pool with key {obj.name} doesn't exist InputKey : {obj.name}");
        }
        if(obj.transform.parent != transform)
        {
            obj.transform.SetParent(transform);
        }
        poolDictionary[obj.name].Enqueue(obj);
    }

    [ContextMenu("GetSpawnObjectInfo")]
    private void GetSpawnObjectInfo()
    {
        foreach(Pool pool in pools)
        {
            int count = spawnObject.FindAll(x => x.name == pool.key).Count;
            Debug.Log($"Name : {pool.key} Count : {count}");
        }
    }

    private GameObject GetObjectFormPool(string key, Vector3 position, Quaternion rotation)
    {
        // 키가 없으면 예외 발생
        if (!poolDictionary.ContainsKey(key))
        {
            throw new Exception($"Pool with key {key} doesn't exist InputKey : {key}");
        }
        Queue<GameObject> queue = poolDictionary[key];
        // 내용물이 없다면
        if (queue.Count <= 0)
        {
            Pool pool = Array.Find(pools, x => x.key == key);
            GameObject obj = CreateNewObject(pool.key, pool.prefab);
            SortPool(obj);
        }

        GameObject result = queue.Dequeue();
        result.transform.position = position;
        result.transform.rotation = rotation;
        result.SetActive(true);

        return result;
    }

    // 새 오브젝트 생성
    private GameObject CreateNewObject(string key, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.name = key;
        obj.SetActive(false);
        return obj;
    }

    // 풀 내부 정렬
    private void SortPool(GameObject obj)
    {
        bool isFind = false;
        for(int i = 0; i < transform.childCount; i++)
        {
            if(i == transform.childCount - 1)
            {
                obj.transform.SetSiblingIndex(i);
                spawnObject.Insert(i, obj);
                break;
            }
            else if (transform.GetChild(i).name == obj.name)
            {
                isFind = true;
            }
            else if (isFind)
            {
                obj.transform.SetSiblingIndex(i);
                spawnObject.Insert(i, obj);
                break;
            }
        }
    }

}
