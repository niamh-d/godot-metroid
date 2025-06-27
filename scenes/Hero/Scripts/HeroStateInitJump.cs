using Godot;

public class HeroStateInitJump : IHeroState
{
    private bool Initialized = false;
    private HeroStateMachine Hero;

    private float JumpForce = -700.0f;

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        InitState(hero);
        return InitiateJump(hero, delta);
    }

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Hero = hero;
            Initialized = true;
        }
    }

    public string GetStateName()
    {
        return "StateInitJump";
    }

    public void SetJumpForce(float jumpForce, float xVelocity = 0)
    {
        Hero.HeroMoveLogic.Velocity.x = xVelocity;
        JumpForce = jumpForce;
    }

    private void ResetJumpForceToNormal()
    {
        JumpForce = -700.0f;
    }

    private IHeroState InitiateJump(HeroStateMachine hero, float delta)
    {
        hero.HeroMoveLogic.DisableSnap();

        hero.HeroMoveLogic.Velocity.y = JumpForce;

        hero.HeroAnimations.Play("HeroInitJump");

        ResetJumpForceToNormal();

        return hero.StateJump;
    }
}