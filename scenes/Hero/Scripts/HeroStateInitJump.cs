using Godot;

public class HeroStateInitJump : IHeroState
{
    private float JumpForce = -700.0f;
    private bool JumpInitiated = false;

    public IHeroState DoState(HeroStateMachine hero, float delta)
    {
        return InitiateJump(hero, delta);
    }

    private IHeroState InitiateJump(HeroStateMachine hero, float delta)
    {
        hero.DisableSnap();

        if (!JumpInitiated)
        {
            JumpInitiated = true;
            hero.HeroMoveLogic.Velocity.y = JumpForce;
        }

        hero.HeroAnimations.Play("HeroInitJump");

        if (hero.LastPlayedHeroAnimation.Equals("HeroInitJump"))
        {
            this.JumpInitiated = false;
            return hero.StateJump;
        }

        return hero.StateInitJump;
    }
}