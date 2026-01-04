using System.Collections.Generic;

namespace ObjectModel;

internal interface IItemModel
{
   List<NamedRef> Items { get; set; }
}