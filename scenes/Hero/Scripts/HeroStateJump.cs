
public class HeroStateJump : IHeroState
{
    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return Jump(hero, delta);
    }

    private IHeroState Jump(HeroStateMachine hero, float delta)
    {
        hero.DisableSnap();

        hero.HeroAnimations.Play("HeroJump");

        if (!hero.IsOnFloor())
        {
            if (hero.HeroMoveLogic.Velocity.y < 0)
            {
                return hero.StateJump;
            }

            if (hero.HeroMoveLogic.Velocity.y > 0)
            {
                return hero.StateFall;
            }
        }
        else if (hero.IsOnFloor())
        {
            return hero.StateIdle;
        }
        return hero.StateJump;
    }
}
