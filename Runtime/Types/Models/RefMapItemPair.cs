using System;
using AlephVault.Unity.Binary;
using GameMeanMachine.Unity.WindRose.RefMapChars.Authoring.ScriptableObjects;

namespace GameMeanMachine.Unity.NetRose.RefMapChars
{
    namespace Types
    {
        namespace Models
        {
            public class ItemPair : ISerializable
            {
                // This is just a pair of elements meant to wrap a RefMap item (with
                // color variations).
                public RefMapItem.ColorCode ColorCode;
                public ushort ItemIndex;
            
                public void Serialize(Serializer serializer)
                {
                    serializer.Serialize(ref ColorCode);
                    serializer.Serialize(ref ItemIndex);
                }

                public static explicit operator ItemPair(Tuple<ushort, RefMapItem.ColorCode> value)
                {
                    return value == null ? null : new ItemPair { ColorCode = value.Item2, ItemIndex = value.Item1 };
                }
            
                public static implicit operator Tuple<ushort, RefMapItem.ColorCode>(ItemPair pair)
                {
                    return pair == null ? null : new Tuple<ushort, RefMapItem.ColorCode>(pair.ItemIndex, pair.ColorCode);
                }
            }
        }
    }
}
