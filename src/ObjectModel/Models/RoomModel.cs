using System;
using System.Collections.Generic;
using NetAF.Assets;
using NetAF.Assets.Locations;

namespace ObjectModel.Models
{
    public class RoomModel : GameObject
    {
        public override IExaminable Instanciate()
        {
            return new Room(Name, Description);
        }
    }
}