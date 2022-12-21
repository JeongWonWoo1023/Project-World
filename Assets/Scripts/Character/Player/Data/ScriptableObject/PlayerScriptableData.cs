// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using UnityEngine;

[CreateAssetMenu(fileName = "Player",menuName ="Charactors/Player")]
public class PlayerScriptableData : ScriptableObject
{
    [field: SerializeField] public PlayerGroundedData GroundedData { get; private set; }
}
