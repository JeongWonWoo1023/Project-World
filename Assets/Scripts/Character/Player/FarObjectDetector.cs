using System.Collections.Generic;
using UnityEngine;

public class FarObjectDetector : DetectorSensorRequire
{
    private void OnTriggerEnter(Collider other)
    {
        GetComponenetFromLayer<EnemySpawnPoint>(other, "Spawner")?.Enable();
        GetComponenetFromLayer<FieldItemPoint>(other, "FieldItem")?.Enable();
        GetComponenetFromLayer<NPC>(other, "NPC")?.Enable();
    }

    private void OnTriggerExit(Collider other)
    {
        GetComponenetFromLayer<EnemySpawnPoint>(other, "Spawner")?.Disable();
        GetComponenetFromLayer<FieldItemPoint>(other, "FieldItem")?.Disable();
        GetComponenetFromLayer<NPC>(other, "NPC")?.Disable();
    }
}
