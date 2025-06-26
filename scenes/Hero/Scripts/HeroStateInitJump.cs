using Godot;

public class HeroStateInitJump : IHeroState
{
    private float JumpForce = -700.0f;

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return InitiateJump(hero, delta);
    }

    public void InitState(HeroStateMachine hero)
    {
        // Nothing to do here
    }

    public string GetStateName()
    {
        return "StateInitJump";
    }

    private IHeroState InitiateJump(HeroStateMachine hero, float delta)
    {
        hero.HeroMoveLogic.DisableSnap();

        hero.HeroMoveLogic.Velocity.y = JumpForce;


        hero.HeroAnimations.Play("HeroInitJump");

        return hero.StateJump;
    }
}