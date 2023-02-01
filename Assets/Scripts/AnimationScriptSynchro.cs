using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScriptSynchro : MonoBehaviour
{
    public PlayerAttack3D playerAttackScript;

    private void Awake()
    {
        playerAttackScript = GetComponent<PlayerAttack3D>();
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
