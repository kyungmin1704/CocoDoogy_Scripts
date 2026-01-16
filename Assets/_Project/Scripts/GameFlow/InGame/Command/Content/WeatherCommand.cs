using CocoDoogy.GameFlow.InGame.Weather;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class WeatherCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        [UnityEngine.SerializeField] private WeatherType pw = WeatherType.Sunny;
        [UnityEngine.SerializeField] private WeatherType nw = WeatherType.Sunny;
        
        
        /// <summary>
        /// 기존 위치
        /// </summary>
        public WeatherType PreWeather { get => pw; private set => pw = value; }
        /// <summary>
        /// 이동한 위치
        /// </summary>
        public WeatherType NextWeather { get => nw; private set => nw = value; }


        public WeatherCommand(object param): base(CommandType.Weather)
        {
            (WeatherType, WeatherType) data = ((WeatherType, WeatherType))param;

            PreWeather = data.Item1;
            NextWeather = data.Item2;
        }


        public override void Execute()
        {
            WeatherManager.StartGlobalWeather(NextWeather);
        }

        public override void Undo()
        {
            WeatherManager.StartGlobalWeather(PreWeather);
        }
    }
}