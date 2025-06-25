using Godot;

public class HeroStateIdle : IHeroState
{
    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return Idle(hero, delta);
    }
    private IHeroState Idle(HeroStateMachine hero, float delta)
    {
        if (hero.IsOnFloor())
        {
            hero.HeroMoveLogic.EnableSnap();

            if (Input.IsActionJustPressed("Jump") && Input.IsActionPressed("Down"))
            {
                return hero.StateSlide;
            }

            if (Input.IsActionJustPressed("Jump"))
            {
                return hero.StateInitJump;
            }

            if (Input.IsActionJustPressed("Attack"))
            {
                return hero.StateAttack;
            }

            hero.HeroAnimations.Play("HeroIdle");

            if (hero.HeroMoveLogic.IsMoving)
            {
                return hero.StateRun;
            }

            return hero.StateIdle;
        }
        else if (!hero.IsOnFloor())
        {
            return hero.StateFall;
        }
        return hero.StateIdle;
    }
}