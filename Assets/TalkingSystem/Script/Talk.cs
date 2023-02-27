using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Talk", menuName = "NPC/Talk")]
public class Talk : ScriptableObject
{
    [field: SerializeField][field: TextArea] public string[] TalkText { get; private set; }
    public bool IsEnd { get; set; }
}
