
using System.Collections.Generic;
using UnityEngine;

public class RoomUtils
{
    public static GameObject GetRoomPrefabForNode(BSPTreeNode node, in RoomInfo[] rooms)
    {
        //Get all rooms under node size
        List<RoomInfo> underSizeRooms = new List<RoomInfo>();

        foreach (RoomInfo room in rooms)
        {
            if (room.size.x > node.container.width || room.size.y > node.container.height)
            {
                continue;
            }
            underSizeRooms.Add(room);
        }
        
        //Check we have some rooms
        if (underSizeRooms.Count == 0)
        {
            //Return a random room if none
            int roomIndex = Random.Range(0, rooms.Length);
            return rooms[roomIndex].roomPrefab;
        }

        //Choose a random room to put in this slot
        int randIndex = Random.Range(0, underSizeRooms.Count);
        return underSizeRooms[randIndex].roomPrefab;
    }
}
