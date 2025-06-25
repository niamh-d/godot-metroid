using Godot;

public class HeroTimers
{
    private HeroStateMachine Hero;
    public Timer SlideTimer;
    public Timer SlideStandUpTimer;
    public Timer LedgeFallTimer;
    public Timer LedgeClimbTimer;
    public Timer AttackTimer;
    private bool TimersInitialized;

    public HeroTimers(HeroStateMachine hero, ref bool initOk)
    {
        Hero = hero;
        initOk = InitHeroTimers();
    }

    private bool InitHeroTimers()
    {
        TimersInitialized = true;

        SlideTimer = GetTimerNode("SlideTimer");
        if (!TimersInitialized) return false;

        SlideStandUpTimer = GetTimerNode("SlideStandUpTimer");
        if (!TimersInitialized) return false;

        LedgeFallTimer = GetTimerNode("LedgeFallTimer");
        if (!TimersInitialized) return false;

        LedgeClimbTimer = GetTimerNode("LedgeClimbTimer");
        if (!TimersInitialized) return false;

        AttackTimer = GetTimerNode("AttackTimer");
        if (!TimersInitialized) return false;

        return true;
    }

    private Timer GetTimerNode(string timerNode)
    {
        string timerPath = "./Timers/" + timerNode;
        var timer = Hero.GetNode<Timer>(timerPath);

        if (timer == null)
        {
            TimersInitialized = false;
            GD.PrintErr("Error in HeroTimers.cs – GetTimerNode() – Could not initialize timer, node: '" + timerNode + "' not found!");
        }

        return timer;
    }

}
