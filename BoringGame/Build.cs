using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace BoringGame
{
    public static class Build
    {
        public static Dictionary<int, object[]> info = new Dictionary<int, object[]>
        {
            //ID                       type            StructureType              name                    size    weight      power     example bonus info if needed
            { 100, new object[] { typeof(Structure),   StructureType.Furnace    ,"Burner Furnace"        ,1      ,100        ,0        ,1 } },
            { 101, new object[] { typeof(Structure),   StructureType.Furnace    ,"Electric Furnace"      ,1      ,120        ,1000     ,1 } },
            { 200, new object[] { typeof(Cart),        StructureType.Cart       ,"Cart"                  ,2      ,0          ,200      ,2, 6f} },
            { 210, new object[] { typeof(Platform),    StructureType.Platform   ,"Platform"              ,2      ,100        ,0        ,20, "test"} },
            { 220, new object[] { typeof(Ladder),      StructureType.Ladder     ,"Ladder"                ,2      ,30         ,0        ,true, 6} }

        };


        public static Structure CreateStructure(Vector2i indexLoc, Bore br, int id)
        {
            var args = new object[] { br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y, id};
            var structure = Activator.CreateInstance((System.Type)info[id][0], args) as Structure;

            Console.WriteLine($"Created {structure.GetType().Name} with id: {id}");

            return structure;
        }

        //public static StructureType GetStructureType(int id)  //transforms the id to enum StructureType using the info dictionary
        //{
        //    System.Type type = (System.Type)info[id][0];
        //    return (StructureType)Enum.Parse(typeof(StructureType), type.Name);

        //}



        public static string InfoName(int id)
        {
            return (string)info[id][2];
        }

        public static StructureType InfoStructureType(int id)
        {
            return (StructureType)info[id][1];
        }
    }
}
