using System;

public class Map
{
    private readonly int id;
    private readonly int w;
    private readonly int h;
    private readonly int[] tiles;

    public Map(int index)
    {
        id= index;
        w = 1;
        h = 1;
        tiles = new int[1];
    }

    public Map (int index,int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException("宽度和高度必须为正数");
        }
        id = index;
        w = width;
        h = height;

        tiles = new int[width * height];
    }
    
    public int GetTile(int x, int y)
    {
        if (x < 0 || x >= w || y < 0 || y >= h)
        {
            throw new ArgumentOutOfRangeException(nameof(x) + "或" + nameof(y) + "坐标超出范围");
        }
        return tiles[x + y * w];
    }
    public void SetTile(int x, int y, int value)
    {
        if (x < 0 || x >= w || y < 0 || y >= h)
        {
            throw new ArgumentOutOfRangeException(nameof(x) + "或" + nameof(y) + "坐标超出范围");
        }
        tiles[x + y * w] = value;
    }

    public int GetId() => id;
    public int GetWidth() => w;
    public int GetHeight() => h;
}