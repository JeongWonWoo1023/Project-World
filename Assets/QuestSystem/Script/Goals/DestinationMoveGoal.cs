using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "DestinationMove", menuName = "Quest/CompleteRequire/DestinationMove")]
public class DestinationMoveGoal : Quest.QuestGoal
{
    public float Range;
    public Vector3[] Destination;
    public Picking PickingObj;
    private Picking Picking;

    private void OnValidate()
    {
        RequireAmount = Destination.Length;
    }

    public override void Iniitalize()
    {
        base.Iniitalize();
        Description = $"목표 지점으로 이동";
        StartAction = () => QuestManager.Instance.StartCoroutine(ShowTarget());
    }

    public IEnumerator ShowTarget()
    {
        int count = 0;
        while(count < Destination.Length)
        {
            if(Picking == null)
            {
                Picking = ObjectPool.Instance.PopObject<Picking>(PickingObj.gameObject.name, Destination[count]);
                Picking.radius = Range;
            }
            if(Picking.isDetect)
            {
                count++;
                CurrentAmount++;
                Picking.isDetect = false;
                Picking.gameObject.SetActive(false);
                Picking = null;
            }
            yield return null;
        }
        Evaluate();
    }
}
