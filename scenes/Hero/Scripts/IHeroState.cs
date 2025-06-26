public interface IHeroState
{
    IHeroState DoState(HeroStateMachine hero, float deltatime);

    void InitState(HeroStateMachine hero);

    string GetStateName();
}