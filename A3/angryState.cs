using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class angryState : State
{
    public angryState(hahaState hahaState) : base("angry") { }

    public override State Update(FSMAgent agent)
    {
        //Handle Following Pacman
        Vector3 pacmanLocation = PacmanInfo.Instance.transform.position;
        if (agent.CloseEnough(pacmanLocation))
        {
            ScoreHandler.Instance.KillPacman();
        }
        //If we didn't return follow Pacman
        agent.SetTarget(pacmanLocation);
        if (PelletHandler.Instance.JustEatenPowerPellet)
        {
            return new ScareState(this);
        }
        //Stay in this state
        return this;
    }

    //Upon entering state, set timer to enter Scatter State
    public override void EnterState(FSMAgent agent)
    {
        //tried to make red animation but I failed
        //agent.SetAnimationStateEyes();
        agent.SetSpeedModifierDouble();
        agent.SetTimer(3);
    }

    public override void ExitState(FSMAgent agent) { }
}
