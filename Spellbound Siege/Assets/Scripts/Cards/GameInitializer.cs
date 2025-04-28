using UnityEngine;
using Spellbound;

public class GameInitializer : MonoBehaviour
{
    public GameObject fireEffect;
    public GameObject waterEffect;
    public GameObject iceEffect;

    void Start()
    {
        CardEffectProcessor.fireEffectPrefab = fireEffect;
        CardEffectProcessor.waterEffectPrefab = waterEffect;
        CardEffectProcessor.iceEffectPrefab = iceEffect;
    }
}