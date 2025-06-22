using Godot;

public class HeroEquipment
{
    private HeroStateMachine Hero;
    private bool HeroEquipmentInitialized;
    public Glider Glider;

    public HeroEquipment(HeroStateMachine hero, ref bool initOk)
    {
        Hero = hero;
        initOk = InitHeroEquipment();
    }

    private bool InitHeroEquipment()
    {
        HeroEquipmentInitialized = true;

        InitGlider();
        if (!HeroEquipmentInitialized) return false;

        return true;
    }

    private void InitGlider()
    {
        Glider = Hero.GetNode<Glider>("./Equipment/Glider");

        if (Glider == null)
        {
            HeroEquipmentInitialized = false;
            GD.PrintErr("HeroEquipment.cs – InitGlider() –  Glider node not found!");
            HeroEquipmentInitialized = false;
            return;
        }

        HeroEquipmentInitialized = true;
    }
}
