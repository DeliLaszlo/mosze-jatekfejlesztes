using UnityEngine;

public class SceneTransitionSpawnPoint : MonoBehaviour
{
    public const string DefaultEntryPointId = "Default";

    [SerializeField] private string entryPointId = DefaultEntryPointId;

    public string EntryPointId => string.IsNullOrWhiteSpace(entryPointId) ? DefaultEntryPointId : entryPointId;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(entryPointId))
        {
            entryPointId = DefaultEntryPointId;
        }
    }
#endif
}
