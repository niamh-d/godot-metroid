using Godot;

public class HeroStateRun : IHeroState
{
    private bool Initialized = false;
    private HeroStateMachine Hero;

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        InitState(hero);
        return Run(delta);
    }

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Initialized = true;
            Hero = hero;
        }
    }

    public string GetStateName()
    {
        return "StateRun";
    }

    private IHeroState Run(float delta)
    {
        Hero.HeroAnimations.Play("HeroRun");

        if (Input.IsActionJustPressed("Jump"))
        {
            return Hero.StateInitJump;
        }

        if (!Hero.IsOnFloor())
        {
            Hero.StateFall.HeroPassedOverAnEdgeStartCoyoteTimeTimer();

            return Hero.StateFall;
        }

        else if (Hero.IsOnFloor())
        {
            if (!Hero.HeroMoveLogic.IsMoving)
            {
                return Hero.StateIdle;
            }
        }
        return Hero.StateRun;
    }
}