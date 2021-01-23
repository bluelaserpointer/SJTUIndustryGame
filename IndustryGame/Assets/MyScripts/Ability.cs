using System;
using System.ComponentModel;
using UnityEngine;

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
    Mollusc
}

public static class AbilityDescription
{
    public static string GetAbilityDescription(Ability ability)
    {
        return EnumHelper.GetDescription(ability);
    }
}
public static class AbilityIconProvider
{
    private static Sprite[] icons = new Sprite[Enum.GetValues(typeof(Ability)).Length];
    static AbilityIconProvider()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Ability)).Length; ++i)
        {
            icons[i] = Resources.Load<Sprite>("Ability/" + ((Ability)i).ToString());
        }
    }
    public static Sprite GetAbilityIcon(Ability ability)
    {
        return icons[(int)ability];
    }
}