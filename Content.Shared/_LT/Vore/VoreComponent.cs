namespace Content.Shared._LT;

[RegisterComponent]
public sealed partial class VoreComponent : Component
{
    [DataField("predCanditate")]
    public bool PredCanditate = false;
    [DataField("preyCanditate")]
    public bool PreyCanditate = true;
    [DataField]
    public bool PredPref = false;
    [DataField]
    public bool PreyPref = false;

    public float DoAfterDelay = 5f;
}
