using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScriptSynchro : MonoBehaviour
{
    public PlayerAttack playerAttackScript;

    private void Awake()
    {
        playerAttackScript = GetComponent<PlayerAttack>();
    }

    public void BeginAttack()
    {
        playerAttackScript.ActivateHitBox();
    }

    public void EndAttack()
    {
        playerAttackScript.DeActivateHitBox();

    }

}
