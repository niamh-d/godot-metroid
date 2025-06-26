using Godot;

public class HeroStateAttack : Timer, IHeroState
{
    private bool Initialized;
    private bool AttackInProgress;
    private bool AttackTimerHasTimedOut;
    private HeroStateMachine Hero;

    public IHeroState DoState(HeroStateMachine hero, float deltatime)
    {
        InitState(hero);
        return Attack(deltatime);
    }

    public void InitState(HeroStateMachine hero)
    {
        if (!Initialized)
        {
            Hero = hero;
            ConnectAttackTimerSignal();
            Initialized = true;
        }
    }

    public string GetStateName()
    {
        return "StateAttack";
    }

    private void ConnectAttackTimerSignal()
    {
        Hero.HeroTimers.AttackTimer.Connect("timeout", this, nameof(OnAttackTimerTimeout));
    }

    public void OnAttackTimerTimeout()
    {
        Hero.HeroTimers.AttackTimer.Stop();
        AttackTimerHasTimedOut = true;
    }

    private IHeroState Attack(float deltatime)
    {
        if (Hero.IsOnFloor())
        {
            PlayAttackAnimation("HeroAttack");
        }

        else if (!Hero.IsOnFloor())

        {
            PlayAttackAnimation("HeroAttackInAir");
        }

        if (AttackTimerHasTimedOut)
        {
            AttackInProgress = false;

            if (Hero.IsOnFloor())
            {
                return Hero.StateIdle;
            }

            if (!Hero.IsOnFloor())
            {
                return Hero.StateFall;
            }
        }

        return Hero.StateAttack;
    }

    private void PlayAttackAnimation(string animation)
    {
        if (!AttackInProgress)
        {
            AttackTimerHasTimedOut = false;
            Hero.HeroTimers.AttackTimer.Start();
            AttackInProgress = true;
            Hero.HeroAnimations.Play(animation);
        }
    }
}
