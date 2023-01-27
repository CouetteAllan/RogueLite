using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScriptSynchro : MonoBehaviour
{
    public PlayerAttack playerAttackScript;
    
    public void BeginAttack()
    {
        playerAttackScript.ActivateHitBox();
    }

    public void EndAttack()
    {
        playerAttackScript.DeActivateHitBox();

    }

}
