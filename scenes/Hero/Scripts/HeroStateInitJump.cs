using Godot;

public class HeroStateInitJump : IHeroState
{
    private float JumpForce = -700.0f;

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return InitiateJump(hero, delta);
    }

    private IHeroState InitiateJump(HeroStateMachine hero, float delta)
    {
        hero.DisableSnap();

        hero.HeroMoveLogic.Velocity.y = JumpForce;


        hero.HeroAnimations.Play("HeroInitJump");

        return hero.StateJump;
    }
}