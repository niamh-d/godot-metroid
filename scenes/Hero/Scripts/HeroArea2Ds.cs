using Godot;

public class HeroArea2Ds : Node
{
    private HeroStateMachine Hero;
    public Area2D Area2DGrabRopeRight;
    public Area2D Area2DGrabRopeLeft;
    private bool Area2DsInitialized;

    public HeroArea2Ds(HeroStateMachine hero, ref bool initOk)
    {
        Hero = hero;
        initOk = InitHeroArea2Ds();
    }

    private bool InitHeroArea2Ds()
    {
        Area2DsInitialized = true;

        Area2DGrabRopeRight = GetArea2DNode("Area2DGrabRopeRight");
        if (!Area2DsInitialized) return false;

        Area2DGrabRopeLeft = GetArea2DNode("Area2DGrabRopeLeft");
        if (!Area2DsInitialized) return false;

        return true;
    }

    private Area2D GetArea2DNode(string Area2DNode)
    {
        string Area2DsPath = "./Area2Ds/" + Area2DNode;
        var area2D = Hero.GetNode<Area2D>(Area2DsPath);
        if (area2D == null)
        {
            Area2DsInitialized = false;
            GD.PrintErr("Error in HeroArea2Ds.cs – GetArea2dNode() – Could not initialize Area2D, node: '" + Area2DNode + "' was not found.");
        }
        return area2D;
    }

    public void DisableRopeSwingArea2DBehindHero()
    {
        if (Hero.HeroAnimations.FlipH)
        {
            Hero.HeroArea2Ds.Area2DGrabRopeRight.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
            Hero.HeroArea2Ds.Area2DGrabRopeLeft.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
        }
        if (!Hero.HeroAnimations.FlipH)
        {
            Hero.HeroArea2Ds.Area2DGrabRopeRight.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
            Hero.HeroArea2Ds.Area2DGrabRopeLeft.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        }
    }

}
