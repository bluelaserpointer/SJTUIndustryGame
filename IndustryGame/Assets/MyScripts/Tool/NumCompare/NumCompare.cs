using System.ComponentModel;

public static class NumCompare
{
    public enum Type {
        [Description(">")]
        GT,
        [Description(">=")]
        GE,
        [Description("<")]
        LT,
        [Description("<=")]
        LE,
        [Description("=")]
        EQ
    };
    public static bool Judge(Type type, float value1, float value2)
    {
        switch(type)
        {
            case Type.GT:
                return value1 > value2;
            case Type.GE:
                return value1 >= value2;
            case Type.LT:
                return value1 < value2;
            case Type.LE:
                return value1 <= value2;
            case Type.EQ:
                return value1 == value2;
        }
        return false;
    }
}
