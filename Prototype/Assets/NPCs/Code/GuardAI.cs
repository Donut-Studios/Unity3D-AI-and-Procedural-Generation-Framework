using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardAI : CoreAI
{
    protected void Start()
    {
        CoreAIStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (inFOV(player))
            moveTo(player);
        else
            my_nav.ResetPath();
    }
}
