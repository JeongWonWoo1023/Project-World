using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommonTalk", menuName = "NPC/CommonTalk")]
public class Talk : ScriptableObject
{
    [field: SerializeField][field: TextArea] public string[] TalkText { get; private set; }
}
