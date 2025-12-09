using System.ComponentModel.DataAnnotations;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._LT;
[Serializable, NetSerializable]
public sealed partial class Belly
{
    [DataField]
    public Guid Id { get; set; } = Guid.NewGuid();
    [DataField]
    public string Name = "belly";
    [DataField]
    public string InnerDescription = "It's a belly! You are in it.";
    [DataField]
    public BellyDigestMode Mode = BellyDigestMode.Hold;
    [DataField]
    public string IngestDesc = "Ingests";
    [DataField]
    public string ExpellDesc = "Expells";
    [DataField]
    public string DigestDescPred = "You churned someone!";
    [DataField]
    public string DigestDescPrey = "You have been churned!";

    public float DigestDamageCaustic = 5f;
    public Belly()
    {

    }

    public Belly(string name, string innerDescription, BellyDigestMode mode, string  ingestDesc, string expellDesc, string digestDescPred,string digestDescPrey)
    {
        Name = name;
        InnerDescription = innerDescription;
        Mode = mode;
        IngestDesc = ingestDesc;
        ExpellDesc = expellDesc;
        DigestDescPred = digestDescPred;
        DigestDescPrey = digestDescPrey;
    }
}

