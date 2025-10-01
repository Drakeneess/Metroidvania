// SettingsNavUtil.cs
using UnityEngine;

public static class SettingsNavUtil
{
    public static int MoveVertical(int current, int length, Vector2 dir)
    {
        if (dir.y > 0f) current--;
        else if (dir.y < 0f) current++;
        return Wrap(current, length);
    }

    public static int MoveHorizontal(int current, int length, Vector2 dir)
    {
        if (dir.x > 0f) current++;
        else if (dir.x < 0f) current--;
        return Wrap(current, length);
    }

    public static int MoveMatrix(int current, int length, Vector2 dir, int columns)
    {
        if (dir.y > 0f) current -= columns;
        else if (dir.y < 0f) current += columns;
        else if (dir.x > 0f) current++;
        else if (dir.x < 0f) current--;
        return Wrap(current, length);
    }

    private static int Wrap(int index, int length)
    {
        if (length <= 0) return 0;
        if (index < 0) return length - 1;
        if (index >= length) return 0;
        return index;
    }
}
