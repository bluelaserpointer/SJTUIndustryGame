public abstract class SpecialistAction
{
    public SpecialistAction(Specialist specialist)
    {
        this.specialist = specialist;
    }
    public readonly Specialist specialist;
    public abstract string Name { get; }
    public virtual float StartMoneyCost => 0;
    public virtual float DayMoneyCost => 0;
    /// <summary>
    /// 措施进度 没有进度上限时默认1
    /// </summary>
    public virtual float ProgressRate => 1;
    public abstract void Stop();
    public abstract void DayIdle();
}

public class FindHabitats : SpecialistAction
{
    public FindHabitats(Specialist specialist) : base(specialist)
    {
    }
    public override string Name => "寻找栖息地";
    public override void Stop()
    {
    }
    public override void DayIdle()
    {
    }
}
public class WatchHabitat : SpecialistAction
{
    public WatchHabitat(Specialist specialist) : base(specialist)
    {
    }
    public override string Name => "观察栖息地";
    public override void Stop()
    {
    }
    public override void DayIdle()
    {
    }
}
public class Reserch : SpecialistAction //TODO: edit
{
    public Reserch(Specialist specialist) : base(specialist) { }
    private readonly string reserachName = "***";
    public override string Name => "研究" + reserachName;
    public override float ProgressRate => base.ProgressRate;
    public override void Stop()
    {
    }
    public override void DayIdle()
    {
    }
}
public class BoostBuildingEffects : SpecialistAction //TODO: edit
{
    public BoostBuildingEffects(Specialist specialist, Building building) : base(specialist)
    {
        this.building = building;
    }
    private readonly Building building;
    public override string Name => "指挥" + building.info.buildingName;

    public override void Stop()
    {
    }
    public override void DayIdle()
    {
    }
}
public class WorkEnvironmentProblem : SpecialistAction
{
    public WorkEnvironmentProblem(Specialist specialist, EnvironmentStatType environmentStatType) : base(specialist)
    {
        this.environmentStatType = environmentStatType;
    }
    private readonly EnvironmentStatType environmentStatType;
    public override string Name => "对策" + environmentStatType.statName;

    public override void Stop()
    {
    }

    public override void DayIdle()
    {
    }
}