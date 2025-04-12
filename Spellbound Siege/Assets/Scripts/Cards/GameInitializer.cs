using UnityEngine;


public class GameInitializer : MonoBehaviour
{
    public GameObject effectPrefabToUse;

    void Start()
    {
        CardEffectProcessor.defaultEffectPrefab = effectPrefabToUse;
    }
}