using System.Collections.Generic;
using Hocon;
using MessagePack;
using ObjectModel.Evaluation;

namespace ObjectModel.Models.Code;

[MessagePackObject(AllowPrivate = true)]
internal class ValueModel(object value) : IEvaluable
{
    [Key(0)]
    public object Value { get; set; } = value;

    public ValueModel() : this(null)
    {
    }

    public object Evaluate(Evaluator evaluator, Scope scope)
    {
        return Value;
    }

    public static IEvaluable FromObject(KeyValuePair<string, HoconField> rootObj)
    {
        object value = null;

        if (rootObj.Value.Type == HoconType.String)
        {
            value = rootObj.Value.GetString();
        }
        else if (rootObj.Value.Type == HoconType.Number)
        {
            value = rootObj.Value.GetNumber();
        }
        else if (rootObj.Value.Type == HoconType.Boolean)
        {
            value = rootObj.Value.GetBoolean();
        }

        return new ValueModel() { Value = value };
    }
}
