using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Hunt", menuName = "Quest/CompleteRequire/Hunt")]
public class HuntGoal : Quest.QuestGoal
{
    public Enemy RequireEnemy;

    public override void Iniitalize()
    {
        base.Iniitalize();
        Description = $"{RequireEnemy.Data.EnemyName} {RequireAmount} 마리 처치";
        StartAction = () => QuestManager.Instance.StartCoroutine(Checking());
    }

    private IEnumerator Checking()
    {
        QuestManager.Instance.huntEnemyEntry.Add(RequireEnemy.name, 0);
        while (QuestManager.Instance.huntEnemyEntry[RequireEnemy.name] < RequireAmount)
        {
            CurrentAmount = QuestManager.Instance.huntEnemyEntry[RequireEnemy.name];
            yield return null;
        }
        CurrentAmount = RequireAmount;
        QuestManager.Instance.huntEnemyEntry.Remove(RequireEnemy.name);
        Evaluate();
    }
}
