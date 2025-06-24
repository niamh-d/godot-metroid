using Godot;

public class Hero2DRayCasts
{
    private HeroStateMachine Hero;
    private bool RaycastsInitialized;
    public RayCast2D LedgeGrabRayCastTileAbove;
    public RayCast2D LedgeGrabRayCastTileHead;
    public RayCast2D LeftWallRayCast;
    public RayCast2D RightWallRayCast;

    public Hero2DRayCasts(HeroStateMachine hero, ref bool initOk)
    {
        Hero = hero;
        initOk = InitHeroRayCasts();
    }

    private bool InitHeroRayCasts()
    {
        RaycastsInitialized = true;

        LedgeGrabRayCastTileAbove = GetRayCastNode("LedgeGrabRayCastTileAbove");
        if (!RaycastsInitialized) return false;

        LedgeGrabRayCastTileHead = GetRayCastNode("LedgeGrabRayCastTileHead");
        if (!RaycastsInitialized) return false;

        LeftWallRayCast = GetRayCastNode("LeftWallRayCast");
        if (!RaycastsInitialized) return false;

        RightWallRayCast = GetRayCastNode("RightWallRayCast");
        if (!RaycastsInitialized) return false;

        return true;
    }

    private RayCast2D GetRayCastNode(string rayCastNodeName)
    {
        string rayCastPath = "./RayCasts/" + rayCastNodeName;
        var rayCast = Hero.GetNode<RayCast2D>(rayCastPath);

        if (rayCast == null)
        {
            RaycastsInitialized = false;
            GD.PrintErr("Error in Hero2DRayCasts.cs – GetRayCastNode() – Could not initialize raycast, node: '" + rayCastNodeName + "' not found!");
        }

        return rayCast;
    }

}
