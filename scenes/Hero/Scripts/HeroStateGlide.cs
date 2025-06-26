using Godot;

public class HeroStateGlide : Node2D, IHeroState
{
    public float GliderGravity = 100;

    public IHeroState DoState(HeroStateMachine hero, float deltatime)
    {
        return Glide(hero);
    }

    public void InitState(HeroStateMachine hero)
    {
        // Nothing to do here
    }

    public string GetStateName()
    {
        return "StateGlide";
    }

    private IHeroState Glide(HeroStateMachine hero)
    {
        StopUpwardGliding(hero);
        CatchTheWind(hero);

        hero.HeroEquipment.Glider.FlipHorizontal = hero.HeroAnimations.FlipH;

        if (hero.IsOnFloor())
        {
            hero.HeroEquipment.Glider.CloseGlider();
            return hero.StateIdle;
        }

        if (hero.StateLedgeGrab.CanHeroLedgeGrab())
        {
            hero.HeroEquipment.Glider.CloseGlider();
            return hero.StateLedgeGrab;
        }


        return hero.StateGlide;
    }

    private void StopUpwardGliding(HeroStateMachine hero)
    {
        if (hero.HeroMoveLogic.Velocity.y <= 40)
        {
            hero.HeroMoveLogic.Velocity.y = Mathf.Lerp(hero.HeroMoveLogic.Velocity.y, 40, 0.1f);
        }
    }

    private void CatchTheWind(HeroStateMachine hero)
    {
        if (hero.HeroMoveLogic.Velocity.y > GliderGravity)
        {
            hero.HeroMoveLogic.Velocity.y = Mathf.Lerp(hero.HeroMoveLogic.Velocity.y, GliderGravity, 0.15f);
        }
    }
}
