using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Quest",menuName = "Quest/Quest")]
public class Quest : ScriptableObject
{
    public enum QuestState
    {
        CanStart, CantStart, Doing, Completed
    }

    [Serializable]
    public struct QuestInfo
    {
        public string Name;
        public Sprite Icon;
        public string Description;
        public int RequireLevel;
        public Quest RequireQuest;
    }

    [Header("퀘스트 정보")] public QuestInfo Info;

    [Serializable]
    public struct QuestReward
    {
        public Item[] Item;
        public int[] ItemCount;
        public int EXP;
        public int Gold;
    }

    private void OnValidate()
    {
        Reward.ItemCount = new int[Reward.Item.Length];
    }

    [Header("퀘스트 보상")] public QuestReward Reward;

    public abstract class QuestGoal : ScriptableObject
    {
        // 퀘스트 완료 조건 추상 클래스
        protected string Description;
        public int CurrentAmount { get; protected set; }
        public int RequireAmount = 1;
        public QuestGoal requireGoal;

        public bool Completed { get; protected set; }
        [HideInInspector] public UnityEvent GoalCompleted;
        [HideInInspector] public UnityAction StartAction;

        [ContextMenu("ResetAmount")]
        public void ResetAmount()
        {
            CurrentAmount = 0;
        }

        public virtual string GetDescription()
        {
            // 퀘스트 설명 호출
            return Description;
        }

        public virtual void Iniitalize()
        {
            // 초기화
            Completed = false;
            GoalCompleted = new UnityEvent();
        }

        public virtual IEnumerator WaitRequire()
        {
            WaitForEndOfFrame delay = new WaitForEndOfFrame();
            if(requireGoal == null)
            {
                StartAction?.Invoke();
                yield break;
            }
            while(!requireGoal.Completed)
            {
                yield return delay;
            }
            StartAction?.Invoke();
        }

        protected void Evaluate()
        {
            // 완료여부 체크
            if(CurrentAmount >= RequireAmount)
            {
                Complete();
            }
        }

        private void Complete()
        {
            // 완료 조건 만족 시 처리
            Completed = true;
            GoalCompleted.Invoke();
            GoalCompleted.RemoveAllListeners();
        }
    }

    public Talk startTalk;
    public List<QuestGoal> Goals;
    [field: SerializeField] public QuestState State { get; protected set; }
    [HideInInspector] public UnityEvent QuestCompleted;

    public void Initialize()
    {
        // 퀘스트 초기 설정

        CheckRequire();

        // 완료 조건 초기화 및 보상 설정
        QuestCompleted = new UnityEvent();
        QuestCompleted.AddListener(
            () =>
            {
                QuestManager.Instance.player.CurrentEXP += Reward.EXP;
                QuestManager.Instance.Inventory.Gold += Reward.Gold;
                if(Reward.Item.Length > 0)
                {
                    for(int i =0; i < Reward.Item.Length;++i)
                    {
                        QuestManager.Instance.Inventory.AddItem(Reward.Item[i], Reward.ItemCount[i]);
                    }
                }
                QuestManager.Instance.UpdateQuestState();
            });

        foreach (QuestGoal goal in Goals)
        {
            goal.Iniitalize();
            goal.GoalCompleted.AddListener(delegate { CheckGoals(); });
        }
        QuestManager.Instance.QuestList.Add(this);
    }

    public void Start()
    {
        if (CheckRequire())
        {
            State = QuestState.Doing;
        }
        foreach (QuestGoal goal in Goals)
        {
            QuestManager.Instance.StartCoroutine(goal.WaitRequire());
        }
        Debug.Log("퀘스트 시작");
    }

    [ContextMenu("ResetQuest")]
    public void ResetQuest()
    {
        State = QuestState.CanStart;
    }

    public bool CheckRequire()
    {
        // 퀘스트 시작 요구사항 체크
        if(State == QuestState.Completed)
        {
            return false;
        }

        if (Info.RequireLevel > QuestManager.Instance.player.status.Experience.Level || (Info.RequireQuest != null && Info.RequireQuest.State != QuestState.Completed))
        {
            State = QuestState.CantStart;
            return false;
        }
        else
        {
            State = QuestState.CanStart;
            return true;
        }
    }

    private void CheckGoals()
    {
        // 퀘스트 완료 여부 체크
        if(Goals.All(g => g.Completed))
        {
            State = QuestState.Completed;
            QuestCompleted.Invoke();
            QuestCompleted.RemoveAllListeners();
        }
    }
}
