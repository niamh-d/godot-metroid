public class HeroStateIdle : IHeroState
{
    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return Idle(hero, delta);
    }
    private IHeroState Idle(HeroStateMachine hero, float delta)
    {
        hero.HeroAnimations.Play("HeroIdle");
        return hero.StateIdle;
    }
}