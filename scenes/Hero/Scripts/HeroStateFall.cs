using Godot;

public class HeroStateFall : Timer, IHeroState
{
    private bool Initialized;
    public bool CanCoyoteTimeJump = false;
    private HeroStateMachine Hero;

    public IHeroState DoState(HeroStateMachine Hero, float delta)
    {
        InitState(Hero);
        return Fall(delta);
    }

    private void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Initialized = true;
            Hero = hero;

            ConnectCoyoteTimeTimerSignal();
        }
    }

    void ConnectCoyoteTimeTimerSignal()
    {
        Hero.HeroTimers.CoyoteTimeTimer.Connect("timeout", this, nameof(OnCoyoteTimeTimerTimeout));
    }

    public void OnCoyoteTimeTimerTimeout()
    {
        CanCoyoteTimeJump = false;
    }

    private IHeroState Fall(float delta)
    {
        Hero.HeroMoveLogic.EnableSnap();

        Hero.HeroAnimations.Play("HeroFall");

        if (Input.IsActionJustPressed("Attack"))
        {
            return Hero.StateAttack;
        }

        if (Hero.StateLedgeGrab.CanHeroLedgeGrab(Hero))
        {
            return Hero.StateLedgeGrab;
        }

        if (Input.IsActionPressed("Glide"))
        {
            Hero.HeroEquipment.Glider.OpenGlider();
            return Hero.StateGlide;
        }

        if (Hero.IsOnFloor())
        {
            Hero.StateJump.ResetJumpCounter();

            if (Hero.HeroMoveLogic.IsMoving)
            {
                return Hero.StateRun;
            }
            return Hero.StateIdle;
        }

        if (Input.IsActionJustPressed("Jump"))
        {

            if (CanCoyoteTimeJump
            || Hero.StateJump.CanHeroPerformBufferJump(Hero)
            || Hero.StateJump.CanWallJump(Hero)
            || Hero.StateJump.CanJumpAgainInAir())
            {
                CanCoyoteTimeJump = false;
                return Hero.StateInitJump;
            }
        }

        return Hero.StateFall;
    }

    public void HeroPassedOverAnEdgeStartCoyoteTimeTimer(HeroStateMachine hero)
    {
        hero.StateFall.CanCoyoteTimeJump = true;
        Hero.HeroTimers.CoyoteTimeTimer.Start();
    }
}
