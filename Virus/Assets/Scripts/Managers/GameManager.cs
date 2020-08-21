using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    
    #region variables

        #region private variables

        private List<Door> _doors;

        #endregion

        #region static variables

        private static Dictionary<string, Door> levelDoors = new Dictionary<string, Door>();
        private static Dictionary<string,SkinnedMeshRenderer> levelDoorsMaterials = new Dictionary<string, SkinnedMeshRenderer>();
        private static Player _player;
        
        public static GameManager instance;
        public static int appDataDroneNumbers = 4;
        public static Transform _playerTransform;
        public static float _playerHealth;
        
        #endregion
    
        #region public variables

        public Material doorLockMaterial;
        public Material doorOpenMaterial;
        
        #endregion
    
    #endregion

    #region buildin methods

    private void Start()
    {
        instance = this;
        _doors = FindObjectsOfType<Door>().ToList();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _playerTransform = _player.transform;
        _playerHealth = _player.healthBar.maximumHealth;
        foreach (var door in _doors)
        {
            GetDoor(door,"task manager door");
            GetDoor(door,"System32 door");
            GetDoor(door,"AppData door");
            GetDoor(door,"Registry door");
        }
    }

    private void Update()
    {
        LockCursor(true);
    }

    #endregion
    
    #region custom methods

    public static void LockCursor(bool isLocked)
    {
        if (!isLocked) return;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void GetDoor(Door door, string doorTag)
    {
        if (!door.CompareTag(doorTag)) return;
        string doorName = doorTag.Remove(doorTag.Length - 5);
        levelDoors.Add(doorName, door);
        levelDoorsMaterials.Add(doorName, door.GetComponentInChildren<SkinnedMeshRenderer>());
    }
    
    public static void OpenDoor(string doorName)
    {
        levelDoors[doorName].enabled = true;
        levelDoorsMaterials[doorName].material = instance.doorOpenMaterial;
    }

    public static void FillPlayerHealth(int fullHealth)
    {
        _player.healthBar.maximumHealth = fullHealth;
        _playerHealth = fullHealth;
    }

    public static void AddPlayerHealth(int toAdd)
    {
        _playerHealth += toAdd;
        if (_playerHealth > _player.healthBar.maximumHealth)
        {
            _playerHealth = _player.healthBar.maximumHealth;
            
        }
        _player.healthBar.health = _playerHealth;
            
    }
    
    #endregion
    
}
