using Godot;

public class HeroStateMachine : KinematicBody2D
{
    public HeroStateIdle StateIdle = new HeroStateIdle();

    public AnimatedSprite HeroAnimations;

    private IHeroState CurrentState;

    private bool IsInitialized = false;

    public override void _Ready()
    {
        IsInitialized = InitHeroStateMachine();

        if (!IsInitialized)
        {
            GD.Print("HeroStateMachine.cs – Error, could not initialize hero state machine.");
        }
    }

    private bool InitHeroStateMachine()
    {
        bool initOk;
        CurrentState = StateIdle;
        initOk = GetHeroAnimationsNode();
        return initOk;
    }

    private bool GetHeroAnimationsNode()
    {
        HeroAnimations = GetNode<AnimatedSprite>("./HeroAnimations");

        if (HeroAnimations is null)
        {
            GD.PrintErr("HeroStateMachine.cs – GetHeroAnimationsNode() – HeroAnimations is null.");
            return false;
        }
        return true;
    }

    private void UpdateHeroState(float delta)
    {
        CurrentState = CurrentState.DoState(this, delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (IsInitialized)
        {
            UpdateHeroState(delta);
        }
    }
}
