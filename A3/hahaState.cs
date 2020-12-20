using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hahaState : State
{
    public hahaState() : base("haha") { }

    public override void EnterState(FSMAgent agent)
    {
        agent.SetTimer(10f);
    }

    public override void ExitState(FSMAgent agent)
    {
        //base.ExitState(agent);
    }

    public override State Update(FSMAgent agent)
    {
        //Handle Following Pacman
        Vector3 pacmanLocation = PacmanInfo.Instance.transform.position;
        if (agent.CloseEnough(pacmanLocation))
        {
            ScoreHandler.Instance.KillPacman();
            //ScoreHandler.Instance.UpdateScore();
        }

        //If timer complete, go to Scatter State
        if (agent.TimerComplete())
        {
            return new ScatterState(new Vector3(ObstacleHandler.Instance.XBound, ObstacleHandler.Instance.YBound), this);
        }

        //If Pacman ate a power pellet, go to Frightened State
        if (PelletHandler.Instance.JustEatenPowerPellet)
        {
            return new FrightenedState(this);
        }
        //If we didn't return follow Pacman
        agent.SetTarget(pacmanLocation);

        if (ScoreHandler.Instance.Score % 30 == 0)
        {
            return new angryState(this);
        }

        //Stay in this state
        return this;
    }
}
