public class HeroStateFall : IHeroState
{

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return Fall(hero, delta);
    }

    private IHeroState Fall(HeroStateMachine hero, float delta)
    {
        hero.HeroMoveLogic.EnableSnap();

        hero.HeroAnimations.Play("HeroFall");

        if (hero.IsOnFloor())
        {
            if (hero.IsMoving)
            {
                return hero.StateRun;
            }
            return hero.StateIdle;
        }
        return hero.StateFall;
    }
}
