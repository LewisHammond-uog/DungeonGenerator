using System;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

namespace DefaultNamespace
{
    public class RoomTileMap : TileMap3D
    {
        private const string FloorTag = "Floor";

        [SerializeField] private GameObject topLeftGO;
        [SerializeField] private GameObject bottomRightGO;
        

        /// <summary>
        /// Fill the Room TileMap with the floor objects in the room
        /// </summary>
        public void FillMapWithObjects()
        {
            //Get all of the child floor objects
            List<GameObject> floorObjects = new List<GameObject>();
            foreach (Transform child in transform)
            {
                if (child.CompareTag(FloorTag))
                {
                    floorObjects.Add(child.gameObject);
                }
            }
            
            //Calculate array size from tl and br game objects
            Vector3 bottomRightPos = bottomRightGO.transform.localPosition;
            Vector3 topLeftPos = topLeftGO.transform.localPosition;

            Vector2Int mapDiff = new Vector2Int
            {
                x = Math.Abs(Mathf.RoundToInt(bottomRightPos.x - topLeftPos.x)) + 1,
                y = Math.Abs(Mathf.RoundToInt(bottomRightPos.z - topLeftPos.z)) + 1
            };

            //Init map with dimentions
            Init(mapDiff);
            
            //Fill Map up with all of the positions
            Vector2Int originPos = new Vector2Int(Mathf.RoundToInt(topLeftGO.transform.localPosition.x), Mathf.RoundToInt(topLeftGO.transform.localPosition.z));
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    map[x, y] = FindObjectAtPos(originPos, x, y, floorObjects);
                }
            }
        }

        public Vector2Int GetRoomCenterOnGrid()
        {
            return mapSize / 2;
        }

        private GameObject FindObjectAtPos(Vector2Int origin, int x, int y, List<GameObject> objects)
        {
            return FindObjectAtPos(origin, new Vector2Int(x, y), objects);
        }
        
        private GameObject FindObjectAtPos(Vector2Int origin, Vector2Int pos, List<GameObject> objects)
        {
            //Loop objects check positions
            foreach (GameObject obj in objects)
            {
                int objX = Mathf.RoundToInt(obj.transform.localPosition.x);
                int objY = Mathf.RoundToInt(obj.transform.localPosition.z);
                Vector2Int objPosV2 = new Vector2Int(objX, objY);
                
                if (objPosV2 == pos)
                {
                    return obj;
                }   
            }

            return null;
        }
    }
}