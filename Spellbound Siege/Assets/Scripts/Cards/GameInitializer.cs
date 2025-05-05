using UnityEngine;
using Spellbound;

public class GameInitializer : MonoBehaviour
{
    public GameObject fireballEffect;
    public GameObject firewallEffect;
    public GameObject icespearEffect;
    public GameObject stoneEffect;
    public GameObject watershotEffect;
    
    void Start()
    {
        CardEffectProcessor.fireballEffect = fireballEffect;
        CardEffectProcessor.firewallEffect = firewallEffect;
        CardEffectProcessor.icespearEffect = icespearEffect;
        CardEffectProcessor.stoneEffect = stoneEffect;
        CardEffectProcessor.watershotEffect = watershotEffect;
    }
}