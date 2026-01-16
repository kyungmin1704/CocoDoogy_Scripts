using CocoDoogy.GameFlow.InGame.Command.Content;
using CocoDoogy.Tile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command
{
    public static partial class CommandManager
    {
        public static string Save()
        {
            List<CommandData> data = new();
            foreach (var command in Executed)
            {
                data.Add(new(command.Type, JsonUtility.ToJson(command)));
            }
            CommandSave result = new()
            {
                StartPos = HexTileMap.StartPos,
                Data = data
            };
            return JsonUtility.ToJson(result);
        }
        public static void Load(string json)
        {
            Clear();

            CommandSave save = JsonUtility.FromJson<CommandSave>(json);
            
            PlayerHandler.Deploy(save.StartPos);

            foreach(var data in save.Data)
            {
                CommandBase command = data.Type switch
                {
                    CommandType.Move => JsonUtility.FromJson<MoveCommand>(data.DataJson),
                    CommandType.Trigger => JsonUtility.FromJson<TriggerCommand>(data.DataJson),
                    
                    CommandType.Slide => JsonUtility.FromJson<SlideCommand>(data.DataJson),
                    CommandType.Teleport => JsonUtility.FromJson<TeleportCommand>(data.DataJson),
                    CommandType.Sail => JsonUtility.FromJson<SailCommand>(data.DataJson),
                    
                    CommandType.Deploy => JsonUtility.FromJson<DeployCommand>(data.DataJson),
                    CommandType.Refill => JsonUtility.FromJson<RefillCommand>(data.DataJson),
                    CommandType.SandCount => JsonUtility.FromJson<SandCountCommand>(data.DataJson),
                    CommandType.Weather => JsonUtility.FromJson<WeatherCommand>(data.DataJson),
                    CommandType.Gimmick => JsonUtility.FromJson<GimmickCommand>(data.DataJson),
                    CommandType.Increase => JsonUtility.FromJson<IncreaseCommand>(data.DataJson),
                    CommandType.DeckReset => JsonUtility.FromJson<DeckResetCommand>(data.DataJson),
                    
                    CommandType.MaxUp => JsonUtility.FromJson<MaxUpItemCommand>(data.DataJson),
                    CommandType.Recover => JsonUtility.FromJson<RecoverItemCommand>(data.DataJson),
                    CommandType.Undo => JsonUtility.FromJson<UndoItemCommand>(data.DataJson),
                    _ => null
                };

                if (command == null) continue;
                Undid.Push(command);
            }
        }


        [System.Serializable]
        private class CommandSave
        {
            public Vector2Int StartPos = Vector2Int.zero;
            public List<CommandData> Data = null;
        }
        [System.Serializable]
        private class CommandData
        {
            [SerializeField] private CommandType t = CommandType.None;
            [SerializeField] private string jn = string.Empty;


            public CommandType Type { get => t; private set => t = value; }
            public string DataJson { get => jn; private set => jn = value; }


            public CommandData(CommandType type, string json)
            {
                Type = type;
                DataJson = json;
            }
        }
    }
}