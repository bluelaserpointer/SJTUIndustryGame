using System.ComponentModel;

/*
 * 广义：陆生动物，水生动物，两栖动物
细分：鱼类，爬行类，鸟类，两栖类，哺乳类等五大类。无脊椎动物中包括：原生动物，软体动物，蠕虫,昆虫，甲壳动物等门类
+探索（包括了地质学）
 */
public enum Ability
{
    [Description("探索")]
    Outdoor,
    //
    [Description("陆生动物")]
    Terrestrial,
    [Description("水生动物")]
    Aquatic,
    [Description("两栖动物")]
    Amphibian,
    //
    [Description("鱼类")]
    Fishes,
    [Description("爬行类")]
    Reptiles,
    [Description("鸟类")]
    Birds,
    [Description("哺乳类")]
    Mammalia,
    //
    [Description("刺胞动物")]
    Cnidaria,
    [Description("棘皮动物")]
    Echinodermata,
    [Description("节肢动物")]
    Arthropod,
    [Description("软体动物")]
    Mollusc,
    //
    [Description("(终止符)")]
    End
}

public class AbilityDescription
{
    public static string GetAbilityDescription(Ability ability)
    {
        string value = ability.ToString();
        System.Reflection.FieldInfo field = ability.GetType().GetField(value);
        object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
        if (objs.Length == 0)    //当描述属性没有时，直接返回名称
            return value;
        DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
        return descriptionAttribute.Description;
    }
}