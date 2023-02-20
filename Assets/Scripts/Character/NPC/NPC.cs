using System;
using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Serializable]
    public struct NPCInfo
    {
        public string Name;
        public string ID;
    }

    [field: Header("모델")]
    [field: SerializeField] public GameObject Model { get; private set; }
    [field: SerializeField] public Transform ModelParent { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public GameObject NameUI { get; private set; }
    [field: SerializeField] public GameObject QuestUI { get; private set; }
    [field: SerializeField] public Transform NameUIParent { get; private set; }
    [field: SerializeField] public Transform QuestUIParent { get; private set; }

    [field: Header("속성")]
    [field: SerializeField] public NPCInfo Info { get; private set; }

    [field: Header("퀘스트")]
    [field: SerializeField] public Quest[] QuestList { get; private set; }

    [field: Header("대화")]
    [field: SerializeField] public Talk CommonTalkData { get; private set; }
    [field: SerializeField] public QuestTalk[] QuestTalkData { get; private set; }

    private NPCNameUI enableNameUI;
    private QuestUI enableQuestUI;

    private void Awake()
    {
        InitializeModel();
        InitializeQuest();
    }

    [ContextMenu("InitializeModel")]
    public void InitializeModel()
    {
        // 모델 초기 설정
        Instantiate(Model, ModelParent);
    }

    public void InitializeQuest()
    {
        // 할당된 퀘스트 초기 설정
        foreach (Quest quest in QuestList)
        {
            quest.Initialize();
        }
    }

    public void Talk()
    {
        // 대화
        foreach(Quest quest in QuestList)
        {
            if(quest.State == Quest.QuestState.CanStart)
            {
                foreach(QuestTalk data in QuestTalkData)
                {
                    if(data.Quest.Info.Name.Equals(quest.Info.Name))
                    {
                        StartCoroutine(UIManager.Instance.TalkDialog.Talk(data, Info.Name, quest));
                        return;
                    }
                }
            }
        }
    }

    public void Enable()
    {
        // NPC 오브젝트 활성
        enableNameUI = ObjectPool.Instance.PopObject<NPCNameUI>(NameUI.name, Vector3.zero);
        enableNameUI.Initialize(UIManager.Instance.EnemyUIParantTrans, NameUIParent, Info.Name);

        if(QuestList.Length == 0)
        {
            return;
        }

        bool viewQuestUI = false;

        foreach(Quest quest in QuestList)
        {
            viewQuestUI = quest.State == Quest.QuestState.CanStart;
            if(viewQuestUI)
            {
                break;
            }
        }

        if(!viewQuestUI)
        {
            return;
        }

        enableQuestUI = ObjectPool.Instance.PopObject<QuestUI>(QuestUI.name, Vector3.zero);
        enableQuestUI.Initialize(UIManager.Instance.EnemyUIParantTrans, QuestUIParent);
    }

    public void Disable()
    {
        enableNameUI.gameObject.SetActive(false);
        enableQuestUI.gameObject.SetActive(false);
    }
}
