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
    [Description("陆生动物")]
    Terrestrial,
    [Description("水生动物")]
    Aquatic,
    [Description("两栖动物")]
    Amphibian,
    [Description("鱼类")]
    Fishes,
    [Description("爬行类")]
    Reptiles,
    [Description("鸟类")]
    Birds,
    [Description("哺乳类")]
    Mammalia,
    [Description("原生动物")]
    Protozoons,
    [Description("软体动物")]
    Molluscs,
    [Description("蠕虫")]
    Worms,
    [Description("昆虫")]
    Insects,
    [Description("甲壳动物")]
    Crustaceans,
    [Description("(终止符)")]
    End
}
