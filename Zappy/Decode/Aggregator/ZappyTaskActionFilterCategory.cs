using System.ComponentModel;

namespace Zappy.Decode.Aggregator
{
    public enum ZappyTaskActionFilterCategory
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        Critical = 100,
        [EditorBrowsable(EditorBrowsableState.Never)]
        General = 400,
        PostCritical = 200,
        PostGeneral = 500,
        PostRedundantActionDeletion = 0x44c,
        PostSimpleToCompoundActionConversion = 800,
        PreGeneral = 300,
        PreRedundantActionDeletion = 900,
        PreSimpleToCompoundActionConversion = 600,
        [EditorBrowsable(EditorBrowsableState.Never)]
        RedundantActionDeletion = 0x3e8,
        [EditorBrowsable(EditorBrowsableState.Never)]
        SimpleToCompoundActionConversion = 700
    }
}