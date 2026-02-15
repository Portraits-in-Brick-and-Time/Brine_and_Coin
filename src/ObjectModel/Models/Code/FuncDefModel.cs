using System.Collections.Generic;
using MessagePack;
using ObjectModel.Evaluation;

namespace ObjectModel.Models.Code;

[MessagePackObject(AllowPrivate = true)]
internal class FuncDefModel : GameObjectModel
{
    [Key(4)]
    public string[] Parameters { get; set; } = [];

    [Key(5)]
    public List<IEvaluable> Action { get; set; } = [];
}