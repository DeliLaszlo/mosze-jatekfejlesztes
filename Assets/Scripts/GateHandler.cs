using UnityEngine;

public class GateHandler : MonoBehaviour
{
    private string gateStateKey;

    private void Awake()
    {
        gateStateKey = SceneTransitionLevelStateManager.BuildStateKey(gameObject, "Gate");
        if (SceneTransitionLevelStateManager.IsGateDisabled(gateStateKey))
        {
            SceneTransitionLevelStateManager.DisableForSavedState(gameObject);
        }
    }

    public void disableSelf()
    {
        SceneTransitionLevelStateManager.MarkGateDisabled(gateStateKey);
        gameObject.SetActive(false);   
    }
}
