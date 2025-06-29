using System.Collections.Generic;
using Godot;

public enum MiniMapBorderE
{
    Empty = 0,
    Box = 1,
    Bottom = 2,
    BottomEnd = 3,
    BottomLeft = 4,
    BottomRight = 5,
    HorizontalCorridor = 6,
    Left = 7,
    LeftEnd = 8,
    Right = 9,
    RightEnd = 10,
    Top = 11,
    TopEnd = 12,
    TopLeft = 13,
    TopRight = 14,
    VerticalCorridor = 15,
}

public enum BorderScanE
{
    Empty = 1,
    Partial = 2,
    Solid = 3,
}

public class RoomScanResult
{
    public BorderScanE Top;
    public BorderScanE Bottom;
    public BorderScanE Left;
    public BorderScanE Right;
}

public class MiniMap : CanvasLayer
{

    private Sprite Location;
    private Node2D MiniMapRooms;
    private LevelArea LevelArea = null;
    private int? TileCollisionLayer = null;
    private Vector2 Resolution = Vector2.Zero;
    public Vector2 MiniMapOffset = new Vector2(77, 39);
    private List<Texture> MiniMapBorderTextures;
    private Texture MiniMapRoomBg;
    public Rect2 Camera2DLimits = new Rect2();

    [Export]
    private int BorderEdgePartialThreshold = 10;
    [Export]
    public Vector2 MiniMapTileSize = new Vector2(19, 19);
    [Export]
    public Vector2 NumTilesXY = new Vector2(40, 23);
    [Export]
    public Rect2 ScanRoomAreaSize = new Rect2(-50, -50, 50, 50);

    public override void _Ready()
    {
        GetResolution();
        TileCollisionLayer = GetWorldTileCollisionLayer();
        Location = GetNode<Sprite>("%Location");
        MiniMapRooms = GetNode<Node2D>("%MiniMapRooms");
        LoadMiniMapTextures();
        GetAreaNode();
        ScanRooms();
    }

    private void GetResolution()
    {
        Resolution = OS.WindowSize;
    }

    public Rect2 GetCameraAreaLimits()
    {
        Vector2 size = Camera2DLimits.Size += new Vector2(Resolution.x, Resolution.y);
        Vector2 pos = Camera2DLimits.Position;
        return new Rect2(pos, size);
    }

    public void UpdateMinMapPosition(Vector2 heroPosition)
    {
        int heroXPositionOnMap;
        int heroYPositionOnMap;

        if (heroPosition.y < 0)
            heroYPositionOnMap = (int)Mathf.Abs(heroPosition.y / Resolution.y) + 1;
        else
            heroYPositionOnMap = -(int)(heroPosition.y / Resolution.y);

        if (heroPosition.x < 0)
            heroXPositionOnMap = (int)Mathf.Abs(heroPosition.x / Resolution.x) + 1;
        else
            heroXPositionOnMap = -(int)(heroPosition.x / Resolution.x);

        MiniMapRooms.Position = new Vector2((heroXPositionOnMap * MiniMapTileSize.x), (heroYPositionOnMap * MiniMapTileSize.y));
    }

    private void LoadMiniMapTextures()
    {
        MiniMapRoomBg = GD.Load<Texture>("res://Scenes/MiniMap/Gfx/MiniMapRoomBg.png");
        MiniMapBorderTextures = new List<Texture>
        {
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderEmpty.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderBox.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderBottom.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderBottomEnd.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderBottomLeft.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderBottomRight.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderHorizontalCorridor.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderLeft.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderLeftEnd.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderRight.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderRightEnd.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderTop.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderTopEnd.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderTopLeft.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderTopRight.png"),
            GD.Load<Texture>("res://Scenes/MiniMap/Gfx/Borders/BorderVerticalCorridor.png")
        };
    }

    private void GetAreaNode()
    {
        var parent = GetParent();

        foreach (Node child in parent.GetChildren())
        {
            if (child.GetType().ToString().Equals("LevelArea"))
            {
                LevelArea = (LevelArea)child;
            }
        }
    }

    private void OnLocationBlinkTimerTimeout()
    {
        Location.Visible = !Location.Visible;
    }

    private int? GetWorldTileCollisionLayer()
    {
        for (int i = 1; i <= 32; ++i)
        {
            var layer = ProjectSettings.GetSetting("layer_names/2d_physics/layer_" + i).ToString();

            if (layer.Equals("Tiles"))
            {
                GD.Print("Tiles collision layer found on layer " + i);
                return i;
            }
        }
        GD.PrintErr("Could not find 'Tiles' collision layer!");
        GD.PrintErr("Make sure to set name 'Tiles' under 'project settinngs > layer names for collision layer.");
        return null;
    }

    private void ScanRooms()
    {
        if (LevelArea == null)
        {
            GD.PrintErr("Cannot scan rooms. Level node is null!");
            return;
        }

        foreach (Node2D node in LevelArea.GetChildren())
        {
            if (node.GetType().ToString().Equals("LevelRoom"))
            {
                LevelRoom room = (LevelRoom)node;
                ScanRoom(room, room.Name, room.Position);
            }
        }
    }

    private void ScanRoom(Node2D room, string roomName, Vector2 roomPosition)
    {
        foreach (Node2D node in room.GetChildren())
        {
            if (node.GetClass().Equals("TileMap"))
            {
                TileMap tileMap = (TileMap)node;

                if (tileMap.CollisionLayer == TileCollisionLayer)
                {
                    ScanMiniMapRooms(roomPosition, tileMap);
                }
            }
        }
    }

    private Sprite AddRoomBackgroundSprite(Vector2 miniMapRoomPos)
    {
        Sprite roomBackground = new Sprite
        {
            Texture = MiniMapRoomBg,
            Centered = false,
            Position = miniMapRoomPos
        };
        return roomBackground;
    }

    private Sprite AddMiniMapRoomBorderToMiniMap(Vector2 miniMapRoomPos, MiniMapBorderE border)
    {
        Sprite miniMapRoomBorder = new Sprite
        {
            Texture = MiniMapBorderTextures[(int)border],
            Centered = false,
            Position = miniMapRoomPos,
        };
        return miniMapRoomBorder;
    }

    private void UpdateCameraAreaLimits(Vector2 roomScanPosition)
    {
        Vector2 size = Camera2DLimits.Size;
        Vector2 position = Camera2DLimits.Position;

        if (Camera2DLimits.Position.x > roomScanPosition.x)
        {
            position.x = roomScanPosition.x;
        }

        if (Camera2DLimits.Position.y > roomScanPosition.y)
        {
            position.y = roomScanPosition.y;
        }

        if (Camera2DLimits.Size.x < roomScanPosition.x)
        {
            size.x = roomScanPosition.x;
        }

        if (Camera2DLimits.Size.y < roomScanPosition.y)
        {
            size.y = roomScanPosition.y;
        }
        Camera2DLimits.Size = size;
        Camera2DLimits.Position = position;
    }

    private void ScanMiniMapRooms(Vector2 roomPosition, TileMap tileMap)
    {
        Vector2 roomAreaMiniMapPosition = new Vector2((roomPosition.x / Resolution.x) * MiniMapTileSize.x, (roomPosition.y / Resolution.y) * MiniMapTileSize.y);
        Vector2 roomLevelPos = Vector2.Zero;

        for (int y = (int)ScanRoomAreaSize.Position.y; y < (int)ScanRoomAreaSize.Size.y; ++y)
        {
            for (int x = (int)ScanRoomAreaSize.Position.x; x < (int)ScanRoomAreaSize.Size.x; ++x)
            {
                roomLevelPos.x = x * NumTilesXY.x;
                roomLevelPos.y = y * NumTilesXY.y;
                RoomScanResult borders = ScanRoomBorders(roomLevelPos, tileMap);
                MiniMapBorderE border = GetMiniMapBorder(borders);
                Vector2 roomPosOnMiniMap = new Vector2((roomAreaMiniMapPosition.x + x * MiniMapTileSize.x) + MiniMapOffset.x, (roomAreaMiniMapPosition.y + y * MiniMapTileSize.y) + MiniMapOffset.y);

                if (border == MiniMapBorderE.Empty)
                {
                    if (BorderlessRoomHasTiles(roomLevelPos, tileMap))
                    {
                        MiniMapRooms.AddChild(AddRoomBackgroundSprite(roomPosOnMiniMap));
                        Vector2 roomScanPosition = new Vector2(roomPosition.x + (x * Resolution.x), roomPosition.y + (y * Resolution.y));
                        UpdateCameraAreaLimits(roomScanPosition);
                    }
                }
                else
                {
                    MiniMapRooms.AddChild(AddRoomBackgroundSprite(roomPosOnMiniMap));
                    MiniMapRooms.AddChild(AddMiniMapRoomBorderToMiniMap(roomPosOnMiniMap, border));
                    Vector2 roomScanPosition = new Vector2(roomPosition.x + (x * Resolution.x), roomPosition.y + (y * Resolution.y));
                    UpdateCameraAreaLimits(roomScanPosition);
                }
            }
        }
    }

    private bool BorderlessRoomHasTiles(Vector2 position, TileMap tileMap)
    {
        for (int y = (int)position.y; y < (int)(position.y + NumTilesXY.y); ++y)
        {
            for (int x = (int)position.x; x < (int)(position.x + NumTilesXY.x); ++x)
            {
                if (tileMap.GetCell(x, y) >= 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private RoomScanResult ScanRoomBorders(Vector2 position, TileMap tileMap)
    {
        RoomScanResult scan = new RoomScanResult
        {
            Top = ScanRoomBorderTop(position, tileMap),
            Bottom = ScanRoomBorderBottom(position, tileMap),
            Left = ScanRoomBorderLeft(position, tileMap),
            Right = ScanRoomBorderRight(position, tileMap)
        };
        return scan;
    }

    private BorderScanE ScanRoomBorderTop(Vector2 position, TileMap tileMap)
    {
        BorderScanE result = BorderScanE.Solid;
        int numTiles = 0;

        for (int x = (int)position.x; x < (int)position.x + NumTilesXY.x; ++x)
        {
            if (tileMap.GetCell(x, (int)position.y) < 0) { result = BorderScanE.Partial; }

            else if (tileMap.GetCell(x, (int)position.y) >= 0) { numTiles++; }
        }

        if (numTiles >= BorderEdgePartialThreshold) { return result; }

        return BorderScanE.Empty;
    }

    private BorderScanE ScanRoomBorderBottom(Vector2 position, TileMap tileMap)
    {
        BorderScanE result = BorderScanE.Solid;
        int numTiles = 0;

        for (int x = (int)position.x; x < (int)position.x + NumTilesXY.x; ++x)
        {
            if (tileMap.GetCell(x, (int)(position.y + NumTilesXY.y) - 1) < 0) { result = BorderScanE.Partial; }

            else if (tileMap.GetCell(x, (int)(position.y + NumTilesXY.y) - 1) >= 0) { numTiles++; }
        }

        if (numTiles >= BorderEdgePartialThreshold) { return result; }

        return BorderScanE.Empty;
    }

    private BorderScanE ScanRoomBorderLeft(Vector2 position, TileMap tileMap)
    {
        BorderScanE result = BorderScanE.Solid;
        int numTiles = 0;

        for (int y = (int)position.y; y < (int)position.y + NumTilesXY.y; ++y)
        {
            if (tileMap.GetCell((int)position.x, y) < 0) { result = BorderScanE.Partial; }

            else if (tileMap.GetCell((int)position.x, y) >= 0) { numTiles++; }
        }

        if (numTiles >= BorderEdgePartialThreshold) { return result; }

        return BorderScanE.Empty;
    }

    private BorderScanE ScanRoomBorderRight(Vector2 position, TileMap tileMap)
    {
        BorderScanE result = BorderScanE.Solid;
        int numTiles = 0;

        for (int y = (int)position.y; y < (int)position.y + NumTilesXY.y; ++y)
        {
            if (tileMap.GetCell((int)(position.x + NumTilesXY.x) - 1, y) < 0) { result = BorderScanE.Partial; }

            else if (tileMap.GetCell((int)(position.x + NumTilesXY.x) - 1, y) >= 0) { numTiles++; }
        }

        if (numTiles >= BorderEdgePartialThreshold) { return result; }

        return BorderScanE.Empty;
    }

    private MiniMapBorderE GetMiniMapBorder(RoomScanResult borders)
    {
        // Check which minmap border that was found, and return the result
        if (IsBorderBox(borders)) { return MiniMapBorderE.Box; }
        else if (IsBorderBottom(borders)) { return MiniMapBorderE.Bottom; }
        else if (IsBorderBotomEnd(borders)) { return MiniMapBorderE.BottomEnd; }
        else if (IsBorderBotomLeft(borders)) { return MiniMapBorderE.BottomLeft; }
        else if (IsBorderBotomRight(borders)) { return MiniMapBorderE.BottomRight; }
        else if (IsBorderHorizontalCorridor(borders)) { return MiniMapBorderE.HorizontalCorridor; }
        else if (IsBorderLeft(borders)) { return MiniMapBorderE.Left; }
        else if (IsBorderLeftEnd(borders)) { return MiniMapBorderE.LeftEnd; }
        else if (IsBorderRight(borders)) { return MiniMapBorderE.Right; }
        else if (IsBorderRightEnd(borders)) { return MiniMapBorderE.RightEnd; }
        else if (IsBorderTop(borders)) { return MiniMapBorderE.Top; }
        else if (IsBorderTopEnd(borders)) { return MiniMapBorderE.TopEnd; }
        else if (IsBorderTopLeft(borders)) { return MiniMapBorderE.TopLeft; }
        else if (IsBorderTopRight(borders)) { return MiniMapBorderE.TopRight; }
        else if (IsBorderVerticalCorridor(borders)) { return MiniMapBorderE.VerticalCorridor; }
        else { return MiniMapBorderE.Empty; }
    }
    private bool IsBorderEmptyOrPartial(BorderScanE borderScan)
    {
        if (borderScan == BorderScanE.Empty || borderScan == BorderScanE.Partial) { return true; }
        return false;
    }

    private bool IsBorderBox(RoomScanResult borders)
    {
        if (!IsBorderEmptyOrPartial(borders.Top) && !IsBorderEmptyOrPartial(borders.Bottom)
        && !IsBorderEmptyOrPartial(borders.Left) && !IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }

    private bool IsBorderBottom(RoomScanResult borders)
    {
        if (IsBorderEmptyOrPartial(borders.Top) && !IsBorderEmptyOrPartial(borders.Bottom)
        && IsBorderEmptyOrPartial(borders.Left) && IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
    private bool IsBorderBotomEnd(RoomScanResult borders)
    {
        if (IsBorderEmptyOrPartial(borders.Top) && !IsBorderEmptyOrPartial(borders.Bottom)
        && !IsBorderEmptyOrPartial(borders.Left) && !IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
    private bool IsBorderBotomLeft(RoomScanResult borders)
    {
        if (IsBorderEmptyOrPartial(borders.Top) && !IsBorderEmptyOrPartial(borders.Bottom)
        && !IsBorderEmptyOrPartial(borders.Left) && IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }

    private bool IsBorderBotomRight(RoomScanResult borders)
    {
        if (IsBorderEmptyOrPartial(borders.Top) && !IsBorderEmptyOrPartial(borders.Bottom)
        && IsBorderEmptyOrPartial(borders.Left) && !IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
    private bool IsBorderHorizontalCorridor(RoomScanResult borders)
    {
        if (!IsBorderEmptyOrPartial(borders.Top) && !IsBorderEmptyOrPartial(borders.Bottom)
        && IsBorderEmptyOrPartial(borders.Left) && IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
    private bool IsBorderLeft(RoomScanResult borders)
    {
        if (IsBorderEmptyOrPartial(borders.Top) && IsBorderEmptyOrPartial(borders.Bottom)
        && !IsBorderEmptyOrPartial(borders.Left) && IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
    private bool IsBorderLeftEnd(RoomScanResult borders)
    {
        if (!IsBorderEmptyOrPartial(borders.Top) && !IsBorderEmptyOrPartial(borders.Bottom)
        && !IsBorderEmptyOrPartial(borders.Left) && IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
    private bool IsBorderRight(RoomScanResult borders)
    {
        if (IsBorderEmptyOrPartial(borders.Top) && IsBorderEmptyOrPartial(borders.Bottom)
        && IsBorderEmptyOrPartial(borders.Left) && !IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }

    private bool IsBorderRightEnd(RoomScanResult borders)
    {
        if (!IsBorderEmptyOrPartial(borders.Top) && !IsBorderEmptyOrPartial(borders.Bottom)
        && IsBorderEmptyOrPartial(borders.Left) && !IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }

    private bool IsBorderTop(RoomScanResult borders)
    {
        if (!IsBorderEmptyOrPartial(borders.Top) && IsBorderEmptyOrPartial(borders.Bottom)
        && IsBorderEmptyOrPartial(borders.Left) && IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
    private bool IsBorderTopEnd(RoomScanResult borders)
    {
        if (!IsBorderEmptyOrPartial(borders.Top) && IsBorderEmptyOrPartial(borders.Bottom)
        && !IsBorderEmptyOrPartial(borders.Left) && !IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
    private bool IsBorderTopLeft(RoomScanResult borders)
    {
        if (!IsBorderEmptyOrPartial(borders.Top) && IsBorderEmptyOrPartial(borders.Bottom)
        && !IsBorderEmptyOrPartial(borders.Left) && IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
    private bool IsBorderTopRight(RoomScanResult borders)
    {
        if (!IsBorderEmptyOrPartial(borders.Top) && IsBorderEmptyOrPartial(borders.Bottom)
        && IsBorderEmptyOrPartial(borders.Left) && !IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
    private bool IsBorderVerticalCorridor(RoomScanResult borders)
    {
        if (IsBorderEmptyOrPartial(borders.Top) && IsBorderEmptyOrPartial(borders.Bottom)
        && !IsBorderEmptyOrPartial(borders.Left) && !IsBorderEmptyOrPartial(borders.Right))
        {
            return true;
        }
        return false;
    }
}
