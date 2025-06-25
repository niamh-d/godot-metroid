using Godot;

public class HeroStateRun : IHeroState
{
    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return Idle(hero, delta);
    }
    private IHeroState Idle(HeroStateMachine hero, float delta)
    {
        hero.HeroAnimations.Play("HeroRun");

        if (Input.IsActionJustPressed("Jump"))
        {
            return hero.StateInitJump;
        }

        if (!hero.IsOnFloor())
        {
            return hero.StateFall;
        }

        else if (hero.IsOnFloor())
        {
            if (!hero.HeroMoveLogic.IsMoving)
            {
                return hero.StateIdle;
            }
        }
        return hero.StateRun;
    }
}