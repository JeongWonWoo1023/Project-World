using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    public Player player;
    public Inventory Inventory;
    public List<Quest> QuestList = new List<Quest>();

    public Dictionary<string, int> huntEnemyEntry = new Dictionary<string, int>();

    public void Hunt(string name) 
    {
        if(!huntEnemyEntry.ContainsKey(name))
        {
            return;
        }
        huntEnemyEntry[name]++;
    }

    public void UpdateQuestState()
    {
        foreach(Quest quest in QuestList)
        {
            if(quest.State == Quest.QuestState.Completed)
            {
                continue;
            }
            if(quest.Info.RequireQuest != null && quest.Info.RequireQuest.State == Quest.QuestState.Completed)
            {
                quest.ResetQuest();
            }
        }
    }
}
