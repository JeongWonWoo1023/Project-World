using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Talk", menuName = "Quest/CompleteRequire/Talk")]
public class TalkGoal : Quest.QuestGoal
{
    public Talk data;
    public NPC targetNPC;

    public override void Iniitalize()
    {
        base.Iniitalize();
        Description = $"{targetNPC.Info.Name}와(과) 대화";
        StartAction = () =>
        {
            targetNPC.QuestEndTalk = data;
        };
    }

    private IEnumerator WaitTalk()
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while(!data.IsEnd)
        {
            yield return delay;
        }
        Evaluate();
    }
}
