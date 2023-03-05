using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Reflection;
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
            { 220, new object[] { typeof(Ladder),      StructureType.Ladder     ,"Ladder"                ,2      ,30         ,0        ,true, 6} },
            { 500, new object[] { typeof(Axle),        StructureType.Axle       ,"Axle"                  ,2      ,30         ,0        ,true, 6} },
            { 510, new object[] { typeof(Cog),         StructureType.Cog        ,"Cogwheel"              ,2      ,30         ,0        ,true, 6} },
            { 520, new object[] { typeof(Motor),       StructureType.Motor      ,"Motor"                 ,2      ,30         ,0        ,true, 6} },
            { 530, new object[] { typeof(Drillhead),   StructureType.Drillhead  ,"Drillhead"             ,2      ,30         ,0        ,true, 6} }

        };

        public static bool building = false;
        public static StructureType buildingMode;
        public static int buildingId;

        public static Structure CreateStructure(Vector2i indexLoc, Bore br, int id)
        {
            var args = new object[] { br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y, id };
            var structure = Activator.CreateInstance((Type)info[id][0], args) as Structure;

            Console.WriteLine($"Created {structure.GetType().Name} with id: {id}");

            return structure;
        }

        public static GameObject UpdateBuilding(Vector2f mousePos, Map map, Bore bore)
        {
            MethodInfo methodInfo = InfoType(buildingId).GetMethod("UpdateBuilding", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var args = new object[] { mousePos, map, bore, buildingId };
            GameObject newObject = (GameObject)methodInfo.Invoke(null, args); //this runs Structure.Place(x,y,id) or one of its derived classes methods
            if(newObject != null)
                Program.player.inventory.ConsumeItem(buildingId, 1);
            return newObject;
        }

        public static void DrawBuildingSprite(RenderWindow window)
        {
            if (SpriteManager.buildingSprite != null)
            {
                SpriteManager.buildingSprite.Draw(window, RenderStates.Default);
            }
        }

        public static MethodInfo GetMethodInfo(int id, string method)
        {
            Type currentType = Build.InfoType(id);
            MethodInfo methodInfo = null;
            while (currentType != null)
            {
                methodInfo = currentType.GetMethod(method, BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
                if (methodInfo != null)
                {
                    break;
                }
                currentType = currentType.BaseType;
            }

            return methodInfo;

        }
        public static string InfoName(int id)
        {
            if(info.ContainsKey(id))
                return (string)info[id][2];
            return null;
        }

        public static StructureType InfoStructureType(int id)
        {
            if (info.ContainsKey(id))
                return (StructureType)info[id][1];
            return StructureType.Structure;
        }

        public static Type InfoType(int id)
        {
            if (info.ContainsKey(id))
                return (Type)info[id][0];
            return null;
        }

        public static bool InfoContains(int id)
        {
            return info.ContainsKey(id);
        }
    }
}
