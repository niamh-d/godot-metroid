public interface IHeroState
{
    IHeroState DoState(HeroStateMachine hero, float deltatime);
}